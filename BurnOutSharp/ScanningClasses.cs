using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BurnOutSharp
{
    /// <summary>
    /// Statically-generated lists of scanning classes
    /// </summary>
    internal static class ScanningClasses
    {
        #region Public Collections

        /// <summary>
        /// Cache for all IContentCheck types
        /// </summary>
        public static IEnumerable<IContentCheck> ContentCheckClasses
        {
            get
            {
                if (contentCheckClasses == null)
                    contentCheckClasses = InitCheckClasses<IContentCheck>();

                return contentCheckClasses;
            }
        }

        /// <summary>
        /// Cache for all INEContentCheck types
        /// </summary>
        public static IEnumerable<INEContentCheck> NEContentCheckClasses
        {
            get
            {
                if (neContentCheckClasses == null)
                    neContentCheckClasses = InitCheckClasses<INEContentCheck>();

                return neContentCheckClasses;
            }
        }

        /// <summary>
        /// Cache for all IPathCheck types
        /// </summary>
        public static IEnumerable<IPathCheck> PathCheckClasses
        {
            get
            {
                if (pathCheckClasses == null)
                    pathCheckClasses = InitCheckClasses<IPathCheck>();

                return pathCheckClasses;
            }
        }

        /// <summary>
        /// Cache for all IPEContentCheck types
        /// </summary>
        public static IEnumerable<IPEContentCheck> PEContentCheckClasses
        {
            get
            {
                if (peContentCheckClasses == null)
                    peContentCheckClasses = InitCheckClasses<IPEContentCheck>();

                return peContentCheckClasses;
            }
        }

        #endregion

        #region Internal Instances

        /// <summary>
        /// Cache for all IContentCheck types
        /// </summary>
        private static IEnumerable<IContentCheck> contentCheckClasses;

        /// <summary>
        /// Cache for all INEContentCheck types
        /// </summary>
        private static IEnumerable<INEContentCheck> neContentCheckClasses;

        /// <summary>
        /// Cache for all IPathCheck types
        /// </summary>
        private static IEnumerable<IPathCheck> pathCheckClasses;

        /// <summary>
        /// Cache for all IPEContentCheck types
        /// </summary>
        private static IEnumerable<IPEContentCheck> peContentCheckClasses;

        #endregion

        #region Initializers

        /// <summary>
        /// Initialize all implementations of a type
        /// </summary>
        private static IEnumerable<T> InitCheckClasses<T>()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && t.GetInterface(typeof(T).Name) != null)
                .Select(t => (T)Activator.CreateInstance(t));
        }

        #endregion
    }
}
