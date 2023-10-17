using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Base.Logging;
using Base.Module;
using Google.Protobuf;
using UnityEngine;

namespace Base.Helper
{
    public static class BlueprintHelper
    {
        #region Blueprint Proto

        public static T ProtoDeserialize<T>(byte[] rawData) where T : IMessage<T>
        {
            try
            {
                MessageParser<T> parser = (MessageParser<T>)typeof(T).GetProperty("Parser")?.GetValue(null, null);

                return parser.ParseFrom(rawData);
            }
            catch (Exception exception)
            {
                PDebug.GetLogger().Error("[ProtoDeserialized] Parse {0} Error {1}", typeof(T).Name, exception);

                throw;
            }
        }
        
        public static T ProtoDeserialize<T>(ByteString data) where T : IMessage<T>
        {
            try
            {
                MessageParser<T> parser = (MessageParser<T>)typeof(T).GetProperty("Parser").GetValue(null, null);
                return parser.ParseFrom(data);
            }
            catch (Exception e)
            {
                PDebug.GetLogger().Error("[ProtoDeserialized] Parse {0} Error {1}", typeof(T).Name, e);
                throw e;
            }
        }
        /// <summary>
        /// Deserialize a json string to proto class
        /// </summary>
        /// <param name="rawData">json string</param>
        /// <typeparam name="T">proto message</typeparam>
        /// <returns>proto message</returns>
        public static T ProtoDeserialize<T>(string rawData) where T : IMessage<T>
        {
            try
            {
                MessageParser<T> parser = (MessageParser<T>)typeof(T).GetProperty("Parser").GetValue(null, null);
                return parser.ParseJson(rawData);
            }
            catch (Exception e)
            {
                PDebug.ErrorFormat("[ProtoDeserialized] Parse {0} Error {1}", typeof(T).Name, e);
                throw;
            }
        }

        public static T ProtoDeserialize<T>(Stream stream) where T : IMessage<T>
        {
            try
            {
                MessageParser<T> parser = (MessageParser<T>)typeof(T).GetProperty("Parser").GetValue(null, null);
                return parser.ParseFrom(stream);
            }
            catch (Exception e)
            {
                PDebug.ErrorFormat("[ProtoDeserialized] Parse {0} Error {1}", typeof(T).Name, e);
                throw;
            }
        }

        #endregion
        
        public static void ReadBlueprint(this IBlueprint blueprint, byte[] rawData)
        {
            BlueprintReaderAttribute att = blueprint.GetType().GetCustomAttribute(typeof(BlueprintReaderAttribute)) 
                    as BlueprintReaderAttribute;

            if (att is null || att.IsIgnore) return;
            blueprint.LoadDummy = att.IsLocal;
            if (att.IsLocal)
            {
                string dataPath = $"{Application.streamingAssetsPath}/{att.DataPath}";
                if (att.DataFormat is DataFormat.Json)
                {
                    string json = File.ReadAllText(dataPath);
                    (blueprint as IJsonDataDeserialize)?.DeserializeJson(json);
                }
                else if (att.DataFormat is DataFormat.Proto)
                {
                    byte[] data = File.ReadAllBytes(dataPath);
                    (blueprint as IProtoDataDeserialize)?.DeserializeProto(data);
                }
                else
                {
                    
                }
            }
            else
            {
                if (att.DataFormat == DataFormat.Json)
                {
                    string json = Encoding.UTF8.GetString(rawData);
                    (blueprint as IJsonDataDeserialize)?.DeserializeJson(json);
                }
                else if (att.DataFormat is DataFormat.Proto)
                {
                    (blueprint as IProtoDataDeserialize)?.DeserializeProto(rawData);
                }
                else
                {
                    string csv = Encoding.UTF8.GetString(rawData);
                    (blueprint as ICsvDataDeserialize)?.DeserializeCsv(csv);
                }
            }
        }
    }
}

