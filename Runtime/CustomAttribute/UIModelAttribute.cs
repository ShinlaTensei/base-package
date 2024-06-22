using System;

[AttributeUsage(AttributeTargets.Class)]
public class UIModelAttribute : Attribute
{
    private string _modelName;

    public string ModelName => _modelName;

    public UIModelAttribute(string modelName)
    {
        this._modelName = modelName;
    }
}
