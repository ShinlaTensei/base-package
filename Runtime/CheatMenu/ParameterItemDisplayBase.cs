using Base.Helper;

namespace Base
{
    public abstract class ParameterItemDisplayBase : BaseUI
    {
        public abstract void Initialize(CheatParameterData parameterData);
        
        public abstract string GetValue();
    }
}

