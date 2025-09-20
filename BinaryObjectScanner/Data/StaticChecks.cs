using System;
using System.Collections.Generic;
using System.Reflection;
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
                contentCheckClasses ??= InitCheckClasses<IContentCheck>();
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
                linearExecutableCheckClasses ??= InitCheckClasses<IExecutableCheck<LinearExecutable>>();
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
                msdosExecutableCheckClasses ??= InitCheckClasses<IExecutableCheck<MSDOS>>();
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
                newExecutableCheckClasses ??= InitCheckClasses<IExecutableCheck<NewExecutable>>();
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
                pathCheckClasses ??= InitCheckClasses<IPathCheck>();
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
                portableExecutableCheckClasses ??= InitCheckClasses<IExecutableCheck<PortableExecutable>>();
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

        /// <summary>
        /// Initialize all implementations of a type
        /// </summary>
        private static List<T>? InitCheckClasses<T>() =>
            InitCheckClasses<T>(Assembly.GetExecutingAssembly()) ?? [];

        /// <summary>
        /// Initialize all implementations of a type
        /// </summary>
        private static List<T>? InitCheckClasses<T>(Assembly assembly)
        {
            List<T> classTypes = [];

            // If not all types can be loaded, use the ones that could be
            Type?[] assemblyTypes = [];
            try
            {
                assemblyTypes = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException rtle)
            {
                assemblyTypes = [.. rtle!.Types!];
            }

            // Get information from the type param
            string interfaceName = typeof(T)!.FullName!;

            // Loop through all types 
            foreach (Type? type in assemblyTypes)
            {
                // Skip invalid types
                if (type == null)
                    continue;

                // If the type isn't a class
                if (!type.IsClass)
                    continue;

                // If the type isn't a class or doesn't implement the interface
                bool interfaceFound = false;
                foreach (var ii in type.GetInterfaces())
                {
                    if (ii.FullName != interfaceName)
                        continue;

                    interfaceFound = true;
                    break;
                }
                if (!interfaceFound)
                    continue;

                // Try to create a concrete instance of the type
                var instance = (T?)Activator.CreateInstance(type);
                if (instance != null)
                    classTypes.Add(instance);
            }

            return classTypes;
        }    
    }
}