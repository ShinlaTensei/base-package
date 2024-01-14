using System;

public enum DataFormat {Proto, Json, Csv}

[AttributeUsage(AttributeTargets.Class)]
public class BlueprintReaderAttribute : Attribute
{
    public string DataPath;
    public bool IsLocal;
    public DataFormat DataFormat;
    public bool IsIgnore;
    public string RemotePath;

    public BlueprintReaderAttribute(string dataPath, DataFormat format, bool isLocal = false, bool isIgnore = false)
    {
        DataPath = dataPath;
        DataFormat = format;
        IsLocal = isLocal;
        IsIgnore = isIgnore;
        string[] remotePath = dataPath.Split('/');
        RemotePath = remotePath[^1].Split('.')[0];
    }
}
