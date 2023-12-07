using System.Reflection;
using Base.Helper;

namespace Base.Cheat
{
    public abstract class ParameterItemDisplayBase : BaseUI
    {
        public bool IsActive { get; protected set; }
        
        public abstract void Initialize(ParameterInfo parameterData);
        
        public abstract string GetValue();
    }
}

