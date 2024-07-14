using System.Collections.Generic;
using System.Net;
using Game.Structure.App;

namespace Base.Core
{
    public enum TransactionResult
    {
        Ok,
        Failed,
    }
    
    public interface IClientCallResult
    {
        TransactionResult TransactionResult { get; }
        HttpStatusCode StatusCode { get; }
        FailureInfo FailureInfo { get; }
        string RequestUri { get; }
        Dictionary<string, string> ResponseHeaders { get; }
    }
    
    public class ClientCallResult<T> : IClientCallResult
    {
        public FailureInfo FailureInfo { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public TransactionResult TransactionResult { get; private set; }
        public T ResponseData { get; private set; }
        public BaseResponseProto ResponseProto { get; private set; }
        public Dictionary<string, string> ResponseHeaders { get; private set; }
        public string RequestUri { get; }

        public ClientCallResult()
        {
        
        }
        public ClientCallResult(FailureInfo failureInfo, HttpStatusCode statusCode, TransactionResult transactionResult,
            T responseData, string requestUri, Dictionary<string, string> responseHeaders, BaseResponseProto baseResponseData)
        {
            FailureInfo = failureInfo;
            StatusCode = statusCode;
            TransactionResult = transactionResult;
            ResponseData = responseData;
            RequestUri = requestUri;
            ResponseHeaders = responseHeaders;
            ResponseProto = baseResponseData;
        }
    }
    
    public class FailureInfo
    {
        public FailureInfo()
        {
        }

        public FailureInfo(string _level, HttpStatusCode _httpStatus, string _message)
        {
            level = _level;
            httpStatus = _httpStatus;
            message = _message;
        }

        public override string ToString() =>
            $"FailureInfo  Status: {httpStatus.ToString()}, ErrorCode: {message} Message: {message}";

        public HttpStatusCode httpStatus;
        public string level; // INFO, WARN, ERROR
        public string errorCode;

        public string message;
    }
}