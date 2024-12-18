using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Base.Helper;
using Base.Logging;
using Cysharp.Threading.Tasks;
using Game.Structure.App;
using Google.Protobuf;
using UnityEngine.Networking;

namespace Base.Core
{
    public enum RequestMethod
    {
        Create,
        Delete,
        Get,
        Head,
        Patch,
        Post,
        Put
    }

    public interface IAuthHeaderProvider
    {
        Dictionary<string, string> GetAuthHeaders();
    }

    public static class RequestMethodExtensions
    {
        /// <summary>
        /// this should be faster than call ToString or a dictionary lookup thanks to jump table
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string GetMethodString(this RequestMethod method)
        {
            switch (method)
            {
                case RequestMethod.Create:
                    return UnityWebRequest.kHttpVerbCREATE;
                case RequestMethod.Delete:
                    return UnityWebRequest.kHttpVerbDELETE;
                case RequestMethod.Get:
                    return UnityWebRequest.kHttpVerbGET;
                case RequestMethod.Head:
                    return UnityWebRequest.kHttpVerbHEAD;
                case RequestMethod.Patch:
                    return "PATCH";
                case RequestMethod.Post:
                    return UnityWebRequest.kHttpVerbPOST;
                case RequestMethod.Put:
                    return UnityWebRequest.kHttpVerbPUT;
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }
    }

    public class ClientCallRequest<T> where T : IMessage<T>
    {
        // there was a timeout
        public const string TIMEOUT_ERROR_CODE = "TIMEOUT_ERROR";

        // user cancelled-this will only show up in the life cycle handler, because otherwise we float it as an unhandled exception
        public const string CANCELLED_ERROR_CODE = "CANCELLATION_TOKEN_CANCELLED";

        // dev tool is forcing this to fail for testing reasons
        public const string DEBUG_FORCED_FAIL_ERROR_CODE = "DEBUG_FORCED_FAIL";

        // there was a failure deserializing the failure info
        public const string FAILURE_DESERIALIZATION_ERROR_CODE = "FAILURE_INFO_DESERIALIZATION_ERROR";

        // there was a failure deserializing the result from an otherwise successful looking request
        public const string SUCCESS_DESERIALIZATION_ERROR_CODE = "SUCCESSFUL_REQUEST_DESERIALIZATION_ERROR";

        private bool IgnoreAuthenticateProvider { get; set; } = false;
        private string SessionId { get; set; } = "";

        /// <summary>
        /// method to use when making request
        /// </summary>
        private readonly RequestMethod Method;

        /// <summary>
        /// uri we're requesting
        /// </summary>
        protected string Uri { get; set; }

        /// <summary>
        /// time out of inner request (Within a given retry)
        /// </summary>
        private int InnerTimeoutSeconds { get; set; }

        /// <summary>
        /// cancellation token which can be set by external source
        /// </summary>
        private CancellationToken CancellationToken;

        /// <summary>
        /// a quick lookup of whether we have an request params- useful for determining if we need a ? or a & before our next one
        /// </summary>
        private bool HasRequestParams { get; set; }

        private string ServiceName { get; set; }

        /// <summary>
        /// extra headers to add- just a list, so if you set the same thing twice, we'll call set headers twice for that thing on the actual request.
        /// at least it's ordered!
        /// </summary>
        private List<KeyValuePair<string, string>> Headers = null;

        /// <summary>
        /// An authentication provider we can use for auth info when sending request.
        /// this is passed through directly so that each retry can apply it in case auth creds expire / refresh between retries
        /// </summary>
        private IAuthHeaderProvider AuthHeaderProvider { get; set; }

        private bool IgnoreAuthenticationProvider { get; set; }

        /// <summary>
        /// data to upload in the body! set with UseBody
        /// </summary>
        private byte[] BodyData;

        /// <summary>
        /// body content type! set with UseBody
        /// </summary>
        private string BodyContentType;

        /// <summary>
        /// create a client service request.
        /// you'll get a lot of nice defaults if you use ClientServiceRequestManager.CreateRequest to do this for you
        /// (i.e. access token hearders, telemetry headers, default inner timeout, retry policy, etc)
        /// </summary>
        /// <param name="method"></param>
        /// <param name="uri"></param>
        /// <param name="innerTimeoutSeconds"></param>
        public ClientCallRequest(RequestMethod method, string uri, int innerTimeoutSeconds)
        {
            Method = method;
            Uri = uri;
            InnerTimeoutSeconds = innerTimeoutSeconds;
        }

        public ClientCallRequest(RequestMethod method, int innerTimeoutSeconds)
        {
            Method = method;
            InnerTimeoutSeconds = innerTimeoutSeconds;
        }

        public ClientCallRequest()
        {
            Method = RequestMethod.Post;
            InnerTimeoutSeconds = 20000;
        }

        public ClientCallRequest(string uri)
        {
            Uri = uri;
            Method = RequestMethod.Get;
            InnerTimeoutSeconds = 20000;
        }

        /// <summary>
        /// set cancellation token which can cancel the request
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public ClientCallRequest<T> UseCancellationToken(CancellationToken ct)
        {
            CancellationToken = ct;

            return this;
        }

        /// <summary>
        /// set a header on the request.
        /// See https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.SetRequestHeader.html
        /// for a list of headers that are illegal to set 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ClientCallRequest<T> SetRequestHeader(string name, string value)
        {
            // might not need this, so don't allocate unless necessary...
            if (Headers == null)
            {
                Headers = new List<KeyValuePair<string, string>>();
            }

            Headers.Add(new KeyValuePair<string, string>(name, value));
            return this;
        }

        /// <summary>
        /// add a request param by just appending it to the uri.
        /// this is rare (who uses these today :p ) so we don't make a dictionary- we literally just append a ? or an &
        /// and the encoded name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ClientCallRequest<T> AddRequestParam(string name, string value)
        {
            string delim = HasRequestParams ? "&" : "?";
            Uri = Uri + delim + UnityWebRequest.EscapeURL(name) + "=" + UnityWebRequest.EscapeURL(value);
            HasRequestParams = true;

            return this;
        }

        /// <summary>
        /// use any data as the body of this request- specifying content type
        /// </summary>
        /// <param name="data"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public ClientCallRequest<T> UseBody(byte[] data, string contentType)
        {
            BodyData = data;
            BodyContentType = contentType;

            return this;
        }

        /// <summary>
        /// set the timeout after which this request will automatically return a transaction failure
        /// with a failure info code of TimeoutErrorCode
        /// </summary>
        public ClientCallRequest<T> UseTimeoutSeconds(int seconds)
        {
            InnerTimeoutSeconds = seconds;

            return this;
        }

        /// <summary>
        /// set an auth provider that will be queried for auth immediately before each request (even internal retries)
        /// </summary>
        /// <param name="authenticationProvider"></param>
        /// <returns></returns>
        public ClientCallRequest<T> UseAuthenticationProvider(IAuthHeaderProvider authenticationProvider)
        {
            AuthHeaderProvider = authenticationProvider;

            return this;
        }

        public ClientCallRequest<T> SetIgnoreAuthenticationProvider()
        {
            IgnoreAuthenticationProvider = true;
            return this;
        }

        public ClientCallRequest<T> SetSessionId(string sessionId)
        {
            this.SessionId = sessionId;
            return this;
        }

        public async UniTask<ClientCallResult<T>> SendAsync()
        {
            string methodString = Method.GetMethodString();
            try
            {
                IClientCallResult result;

                result = await SendAsyncInternal(CancellationToken);
                return (ClientCallResult<T>) result;
            }
            catch (Exception e)
            {
                ClientCallResult<T> result = new ClientCallResult<T>(
                    failureInfo: new FailureInfo {errorCode = CANCELLED_ERROR_CODE, message = e.ToString()},
                    statusCode: 0,
                    transactionResult: TransactionResult.Failed,
                    responseData: default,
                    requestUri: Uri, responseHeaders: null, null);

                throw;
            }
        }

        private async UniTask<IClientCallResult> SendAsyncInternal(CancellationToken ct)
        {
            PDebug.Debug("<color=white>SendRequest :" + Uri + " - ServiceName: " + ServiceName + " </color>\n" + "<color=white>Method:</color> " +
                         Method.GetMethodString());

            UnityWebRequest wr = new UnityWebRequest(Uri, Method.GetMethodString());
            wr.disposeDownloadHandlerOnDispose = true;
            wr.timeout = InnerTimeoutSeconds;

            if (BodyData != null)
            {
                wr.uploadHandler?.Dispose();
                wr.uploadHandler = new UploadHandlerRaw(BodyData);
                wr.disposeUploadHandlerOnDispose = true;
            }

            if (Headers != null)
            {
                for (int i = 0, c = Headers.Count; i < c; ++i)
                {
                    // hoping I avoid some copies doing things this way.
                    wr.SetRequestHeader(Headers[i].Key, Headers[i].Value);
                }
            }

            // update authentication here because we might be in the middle of a retry and 
            // our auth might have refreshed!
            Dictionary<string, string> headers = AuthHeaderProvider?.GetAuthHeaders();

            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    if (!string.IsNullOrEmpty(header.Value))
                    {
                        wr.SetRequestHeader(header.Key, header.Value);
                    }
                }
            }

            // make sure we can cancel!
            CancellationTokenRegistration registration = ct.Register(() =>
            {
                PDebug.InfoFormat("[ClientServiceRequest] SendAsyncInternal Cancellation Token cancelled for {0} {1}", Method, Uri);
                // I hope the uwr was not disposed...maybe unity is kind about letting us check?
                // well, we'll catch an exception in case it's not...
                try
                {
                    wr.Abort();
                }
                catch (ArgumentNullException)
                {
                    // this happens if the uwr was disposed somehow before abort was called- that seems okay since all we want to do is abort
                    // and it seems like this can happen when unity is backgrounded
                    PDebug.InfoFormat(
                        "[ClientServiceRequest] Attempt to abort {0} {1} but webrequest was already cancelled.  That's okay, but might be useful info",
                        Method, Uri);
                }
            });

            try
            {
                UnityWebRequest request = await wr.SendWebRequest();
                return HandleWebRequestCompleted(wr, registration);
            }
            catch (Exception e)
            {
                PDebug.ErrorFormat($"Send Request Error {e.Message}");
                return HandleWebRequestCompleted(wr, registration);
            }
        }

        private IClientCallResult HandleWebRequestCompleted(UnityWebRequest wr, CancellationTokenRegistration cancellationTokenRegistration)
        {
            string url = wr.url;
            string methodString = wr.method;
            
            try
            {
                if (wr.result != UnityWebRequest.Result.Success)
                {
                    string networkError = wr.error;
                    PDebug.ErrorFormat("[ClientCallRequest] Network Error Code {0}", networkError);

                    switch (networkError)
                    {
                        case "Request aborted":
                        case "Request timeout":
                            return MakeTimeoutResult(url, networkError);
                        default:
                            // what do we do for other kind of error (like network error?) just fall through and pass what we can?
                            // network error means we got bupkis back, so have to make our own failure info
                            ClientCallResult<T> networkErrorResult = new ClientCallResult<T>(
                                failureInfo: new FailureInfo("ERROR", (HttpStatusCode) wr.responseCode, networkError),
                                statusCode: (HttpStatusCode) wr.responseCode,
                                transactionResult: TransactionResult.Failed,
                                responseData: default,
                                requestUri: url, responseHeaders: null, null);
                            return networkErrorResult;
                    }
                }

                string text = string.Empty;
                byte[] rawData = wr?.downloadHandler.data;
                string encryptedMode = rawData != null ? wr.GetResponseHeader("Encrypt") : string.Empty;
                switch (encryptedMode)
                {
                    case "aes":
                        //rawData = Encryption.Decrypt(rawData);
                        break;
                    default:
                        break;
                }

                BaseResponseProto responseProto = null;
                if (rawData != null)
                {
                    responseProto = BaseResponseProto.Parser.ParseFrom(rawData);
                }

                long statusCode = wr.responseCode;

                if (statusCode >= 400)
                {
                    ClientCallResult<T> httpErrorResult = new ClientCallResult<T>(
                        failureInfo: CreateFailureInfo(statusCode, text, null),
                        statusCode: (HttpStatusCode) statusCode,
                        transactionResult: TransactionResult.Failed,
                        responseData: default,
                        requestUri: url, responseHeaders: null, null);
                    return httpErrorResult;
                }

                try
                {
                    T responseObject = default(T);

                    if (responseProto != null && responseProto.Data != null)
                    {
                        responseObject = BlueprintHelper.ProtoDeserialize<T>(responseProto.Data);
                        PDebug.DebugFormat("Request response: {0} - ServiceName: {1} - Data: {3}", url, ServiceName, responseObject.ToString());
                    }

                    ClientCallResult<T> successResult = new ClientCallResult<T>(null, (HttpStatusCode)statusCode, TransactionResult.Ok,
                        responseObject, url, wr.GetResponseHeaders(), responseProto);
                    return successResult;
                }
                catch (Exception e)
                {
                    PDebug.Error($"We got bad data {e.Message}");
                    ClientCallResult<T> successResult = new ClientCallResult<T>(
                        failureInfo: new FailureInfo { errorCode = SUCCESS_DESERIALIZATION_ERROR_CODE, message = text },
                        statusCode: (HttpStatusCode)statusCode,
                        transactionResult: TransactionResult.Failed,
                        responseData: default,
                        requestUri: url, responseHeaders: null, null);
                    return successResult;
                }
            }
            finally
            {
                wr.Dispose();
                cancellationTokenRegistration.Dispose();
            }
        }
        
        private static ClientCallResult<T> MakeTimeoutResult(string uri, string message)
        {
            return new ClientCallResult<T>(
                failureInfo: new FailureInfo {errorCode = TIMEOUT_ERROR_CODE, message = message},
                statusCode: 0,
                transactionResult: TransactionResult.Failed,
                responseData: default,
                requestUri: uri, responseHeaders: null, null);
        }
        
        private static FailureInfo CreateFailureInfo(long statusCode, string text, string traceId)
        {
            FailureInfo failureInfo = null;

            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    // failureInfo = JsonConvert.DeserializeObject<FailureInfo>(text);
                }
                catch
                {
                    // so sometimes we get failure info deserialization failing because of nginx errors and such.
                    // how much do we want to spam the world? not a lot. let's just put it in the message?
                    failureInfo = new FailureInfo
                    {
                        httpStatus = (HttpStatusCode) statusCode, errorCode = FAILURE_DESERIALIZATION_ERROR_CODE,
                        message = text
                    };
                }
            }
            else
            {
                failureInfo = new FailureInfo {httpStatus = (HttpStatusCode) statusCode};
            }

            return failureInfo;
        }
    }
}