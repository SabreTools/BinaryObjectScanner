using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BurnOutSharp
{
    /// <summary>
    /// Internal static initializers using reflection
    /// </summary>
    /// <remarks>
    /// These can probably be consolidated if Type variables are used
    /// </remarks>
    internal static class Initializer
    {
        /// <summary>
        /// Initialize all IContentCheck implementations
        /// </summary>
        public static IEnumerable<IContentCheck> InitContentCheckClasses()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && t.GetInterface(nameof(IContentCheck)) != null)
                .Select(t => Activator.CreateInstance(t) as IContentCheck);
        }

        /// <summary>
        /// Initialize all INEContentCheck implementations
        /// </summary>
        public static IEnumerable<INEContentCheck> InitNEContentCheckClasses()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && t.GetInterface(nameof(INEContentCheck)) != null)
                .Select(t => Activator.CreateInstance(t) as INEContentCheck);
        }

        /// <summary>
        /// Initialize all IPathCheck implementations
        /// </summary>
        public static IEnumerable<IPathCheck> InitPathCheckClasses()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && t.GetInterface(nameof(IPathCheck)) != null)
                .Select(t => Activator.CreateInstance(t) as IPathCheck);
        }

        /// <summary>
        /// Initialize all IPEContentCheck implementations
        /// </summary>
        public static IEnumerable<IPEContentCheck> InitPEContentCheckClasses()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && t.GetInterface(nameof(IPEContentCheck)) != null)
                .Select(t => Activator.CreateInstance(t) as IPEContentCheck);
        }
    }
}
