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
        void AddSetting<T>(string id, T setting) where T : IBaseSetting;
        T    GetSetting<T>(string id) where T : IBaseSetting;
    }

    public interface IBaseSetting
    {
        
    }
}