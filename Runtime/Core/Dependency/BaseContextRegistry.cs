using System;
using System.Collections.Generic;

namespace Base.Core
{
    public class BaseContextRegistry
    {
        private IDictionary<Enum, DependencyContext> m_contextRegistry = new Dictionary<Enum, DependencyContext>();
    }
}