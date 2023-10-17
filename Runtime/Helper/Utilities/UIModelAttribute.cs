using System;

[AttributeUsage(AttributeTargets.Class)]
public class UIModelAttribute : Attribute
{
    private string _modelName;
    private string _sceneName;

    public string ModelName => _modelName;
    public string SceneName => _sceneName;

    public UIModelAttribute(string modelName, string sceneName)
    {
        this._modelName = modelName;
        _sceneName = sceneName;
    }
}
