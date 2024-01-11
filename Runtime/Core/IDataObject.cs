#region Header
// Date: 11/01/2024
// Created by: Huynh Phong Tran
// File name: IDataObject.cs
#endregion
namespace Base.Core
{
    public interface IDataObject
    {
        int    Index      { get; set; }
        string Type       { get; set; }
        string ObjectName { get; set; }
    }
}