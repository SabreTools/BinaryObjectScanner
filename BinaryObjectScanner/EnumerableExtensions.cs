using System;
using System.Collections.Generic;

namespace BinaryObjectScanner
{
    internal static class EnumerableExtensions
    {
        /// <summary>
        /// Wrap iterating through an enumerable with an action
        /// </summary>
        /// <remarks>
        /// .NET Frameworks 2.0 and 3.5 process in series.
        /// .NET Frameworks 4.0 onward process in parallel.
        /// </remarks>
        public static void IterateWithAction<T>(this IEnumerable<T> source, Action<T> action)
        {
#if NET20 || NET35
            foreach (var item in source)
            {
                action(item);
            }
#else
            System.Threading.Tasks.Parallel.ForEach(source, action);
#endif
        }
    }
}