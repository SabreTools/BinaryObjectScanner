using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Data
{
    internal static class StaticChecks
    {
        #region Public Collections

        /// <summary>
        /// Cache for all IContentCheck types
        /// </summary>
        public static List<IContentCheck> ContentCheckClasses
        {
            get
            {
                contentCheckClasses ??= Factory.InitCheckClasses<IContentCheck>();
                return contentCheckClasses ?? [];
            }
        }

        /// <summary>
        /// Cache for all IExecutableCheck<LinearExecutable> types
        /// </summary>
        public static List<IExecutableCheck<LinearExecutable>> LinearExecutableCheckClasses
        {
            get
            {
                linearExecutableCheckClasses ??= Factory.InitCheckClasses<IExecutableCheck<LinearExecutable>>();
                return linearExecutableCheckClasses ?? [];
            }
        }

        /// <summary>
        /// Cache for all IExecutableCheck<MSDOS> types
        /// </summary>
        public static List<IExecutableCheck<MSDOS>> MSDOSExecutableCheckClasses
        {
            get
            {
                msdosExecutableCheckClasses ??= Factory.InitCheckClasses<IExecutableCheck<MSDOS>>();
                return msdosExecutableCheckClasses ?? [];
            }
        }

        /// <summary>
        /// Cache for all IExecutableCheck<NewExecutable> types
        /// </summary>
        public static List<IExecutableCheck<NewExecutable>> NewExecutableCheckClasses
        {
            get
            {
                newExecutableCheckClasses ??= Factory.InitCheckClasses<IExecutableCheck<NewExecutable>>();
                return newExecutableCheckClasses ?? [];
            }
        }

        /// <summary>
        /// Cache for all IPathCheck types
        /// </summary>
        public static List<IPathCheck> PathCheckClasses
        {
            get
            {
                pathCheckClasses ??= Factory.InitCheckClasses<IPathCheck>();
                return pathCheckClasses ?? [];
            }
        }

        /// <summary>
        /// Cache for all IExecutableCheck<PortableExecutable> types
        /// </summary>
        public static List<IExecutableCheck<PortableExecutable>> PortableExecutableCheckClasses
        {
            get
            {
                portableExecutableCheckClasses ??= Factory.InitCheckClasses<IExecutableCheck<PortableExecutable>>();
                return portableExecutableCheckClasses ?? [];
            }
        }

        #endregion

        #region Internal Instances

        /// <summary>
        /// Cache for all IContentCheck types
        /// </summary>
        private static List<IContentCheck>? contentCheckClasses;

        /// <summary>
        /// Cache for all IExecutableCheck<LinearExecutable> types
        /// </summary>
        private static List<IExecutableCheck<LinearExecutable>>? linearExecutableCheckClasses;

        /// <summary>
        /// Cache for all IExecutableCheck<MSDOS> types
        /// </summary>
        private static List<IExecutableCheck<MSDOS>>? msdosExecutableCheckClasses;

        /// <summary>
        /// Cache for all IExecutableCheck<NewExecutable> types
        /// </summary>
        private static List<IExecutableCheck<NewExecutable>>? newExecutableCheckClasses;

        /// <summary>
        /// Cache for all IPathCheck types
        /// </summary>
        private static List<IPathCheck>? pathCheckClasses;

        /// <summary>
        /// Cache for all IExecutableCheck<PortableExecutable> types
        /// </summary>
        private static List<IExecutableCheck<PortableExecutable>>? portableExecutableCheckClasses;

        #endregion
    }
}