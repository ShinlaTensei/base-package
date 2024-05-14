using System;
using System.Collections.Generic;

namespace Base.Core
{
    public class BaseContextRegistry : SingletonMono<BaseContextRegistry>
    {
        /// <summary>
        /// Backing field of <see cref="ContextRegistry"/>
        /// </summary>
        private readonly IDictionary<int, DependencyContext> m_contextRegistry = new Dictionary<int, DependencyContext>();
        /// <summary>
        /// Hold the dependency context
        /// </summary>
        private IDictionary<int, DependencyContext> ContextRegistry => m_contextRegistry;


        public static DependencyContext TryGetOrCreateContext(int index)
        {
            if (!Instance.ContextRegistry.TryGetValue(index, out DependencyContext context))
            {
                context = new DependencyContext();
                Instance.ContextRegistry[index] = context;
            }

            return context;
        }

        public static DependencyContext TryGetOrCreateContext(Enum enumValue)
        {
            return TryGetOrCreateContext(Convert.ToInt32(enumValue));
        }
    }
}