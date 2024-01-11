#region Header
// Date: 11/01/2024
// Created by: Huynh Phong Tran
// File name: ISettingAcessor.cs
#endregion

using Base.Services;

namespace Base.Core
{
    public interface ISettingAccessor
    {
        void AddSetting<T>(T setting);
        T    GetSetting<T>(string id);
    }
}