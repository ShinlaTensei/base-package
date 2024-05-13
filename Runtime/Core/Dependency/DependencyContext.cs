using System;
using System.Collections.Generic;

namespace Base.Core
{
    public class DependencyContext
    {
        /// <summary>
        /// Backing field of <see cref="DependencyRegistry"/>
        /// </summary>
        private readonly IDictionary<Type, DependencyObject> m_dependencyRegistry = new Dictionary<Type, DependencyObject>();
        
        /// <summary>
        /// Hold the reference to the register <see cref="UnityEngine.Object"/>
        /// </summary>
        private IDictionary<Type, DependencyObject> DependencyRegistry => m_dependencyRegistry;
    }
}