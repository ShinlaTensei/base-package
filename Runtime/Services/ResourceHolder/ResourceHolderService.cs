#region Header
// Date: 11/10/2023
// Created by: Huynh Phong Tran
// File name: ResourceHolderService.cs
#endregion


using Base.Pattern;

namespace Base.Services
{
    public interface IResourceHolderService : IService
    {
        
    }
    
    
    public class ResourceHolderService : IResourceHolderService
    {
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
        public void Init()
        {
            throw new System.NotImplementedException();
        }
    }
}