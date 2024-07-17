using System;
using System.Collections.Generic;

namespace Base.Core
{
    public static class CoreContext
    {
        public const int GLOBAL_CONTEXT = 0;
        public const int HOME_CONTEXT = 1;
        public const int GAME_CONTEXT = 2;
    }
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
            if (Instance == null)
            {
                return null;
            }
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

        public static void StopContext(int index)
        {
            if (Instance.ContextRegistry.TryGetValue(index, out DependencyContext context))
            {
                context.Dispose();
                Instance.ContextRegistry.Remove(index);
            }
        }
    }
}