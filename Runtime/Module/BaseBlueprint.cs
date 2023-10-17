using System;
using System.Collections.Generic;
using Base.Helper;
using Base.Logging;
using Google.Protobuf;
using Newtonsoft.Json;

namespace Base.Module
{
    public interface IBlueprint
    {
        void InitBlueprint(bool usingLocal = false);
        void Load();
        void LoadDummyData();

        string TypeUrl     { get; set; }
        bool   IsDataReady { get; set; }
        bool   LoadDummy   { get; set; }
    }

    public interface ICsvDataDeserialize
    {
        void DeserializeCsv(string csv);
        void SerializeCsv();
    }
    
    public interface IJsonDataDeserialize
    {
        void DeserializeJson(string json);

        string SerializeJson();
    }

    public interface IProtoDataDeserialize
    {
        void DeserializeProto(byte[] rawData);

        byte[] SerializeProto();
    }

    public abstract class BaseBlueprintProto<T> : IBlueprint, IProtoDataDeserialize where T : IMessage<T>
    {
        public string TypeUrl { get; set; }
        public bool IsDataReady { get; set; }
        public bool LoadDummy { get; set; }

        public T Data;
        public virtual void InitBlueprint(bool usingLocal = false)
        {
            LoadDummy = usingLocal;
            if (usingLocal)
            {
                LoadDummyData();
            }
            else Load();
        }

        public abstract void Load();

        public abstract void LoadDummyData();

        public virtual void DeserializeProto(byte[] rawData)
        {
            Data = BlueprintHelper.ProtoDeserialize<T>(rawData);
            IsDataReady = Data != null;
        }

        public virtual byte[] SerializeProto()
        {
            return Data != null ? Data.ToByteArray() : null;
        }
    }

    public abstract class BaseBlueprintJson<T> : IBlueprint, IJsonDataDeserialize where T : class
    {
        public string TypeUrl     { get; set; }
        public bool   IsDataReady { get; set; }
        public bool   LoadDummy   { get; set; }

        public T Data;
        public virtual void InitBlueprint(bool usingLocal = false)
        {
            LoadDummy = usingLocal;
            if (LoadDummy)
            {
                LoadDummyData();
            }
            else
            {
                Load();
            }
        }

        public abstract void Load();

        public abstract void LoadDummyData();

        public void DeserializeJson(string json)
        {
            try
            {
                Data = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                PDebug.Error(e, e.Message);
            }

            IsDataReady = Data != null;
        }

        public string SerializeJson()
        {
            return JsonConvert.SerializeObject(Data);
        }
    }

    public abstract class BaseBlueprintCsv<T> : IBlueprint, ICsvDataDeserialize where T : class
    {
        public string TypeUrl     { get; set; }
        public bool   IsDataReady { get; set; }
        public bool   LoadDummy   { get; set; }

        public List<T>    Data;

        public virtual void InitBlueprint(bool usingLocal = false)
        {
            LoadDummy = usingLocal;
            if (LoadDummy)
            {
                LoadDummyData();
            }
            else
            {
                Load();
            }
        }

        public abstract void Load();

        public abstract void LoadDummyData();

        public virtual void DeserializeCsv(string csv)
        {
            try
            {
                Data = FileUtilities.ReadFromCsvData<T>(csv);
            }
            catch (Exception e)
            {
                PDebug.Error(e, e.Message);
            }

            IsDataReady = Data != null;
        }

        public virtual void SerializeCsv()
        {
            
        }
    }
}

