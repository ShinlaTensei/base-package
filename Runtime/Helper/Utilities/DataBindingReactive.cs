using System;
using System.Collections.Generic;
using System.Reflection;
using UniRx;

namespace Base.Helper
{
    public sealed class DataBindingReactive<T> : ReactiveProperty<T>
    {
        public DataBindingReactive() {}

        public DataBindingReactive(T initialValue) : base(initialValue)
        {
            
        }

        private DataBindingEqualityComparer<T> m_equalityComparer = new DataBindingEqualityComparer<T>();

        protected override IEqualityComparer<T> EqualityComparer => m_equalityComparer;
    }

    public sealed class DataBindingEqualityComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            Type type = typeof(T);
            if (x == null || y == null)
            {
                return false;
            }
            
            if (type.IsValueType)
            {
                FieldInfo[] fields = type.GetFields();
                
                foreach (FieldInfo field in fields)
                {
                    if (field.GetValue(x) != field.GetValue(y))
                    {
                        return false;
                    }
                }

                return true;
            }

            return x.Equals(y);
        }

        public int GetHashCode(T obj)
        {
            return obj?.GetHashCode() ?? 0;
        }
    }
}