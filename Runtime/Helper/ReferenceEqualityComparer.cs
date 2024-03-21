using System.Collections.Generic;

namespace Base.Helper
{
    public sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T> where T : class
    {
        public bool Equals(T x, T y)
        {
            return x == y;
        }

        public int GetHashCode(T obj)
        {
            return obj?.GetHashCode() ?? 0;
        }
    }
}
