#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.Linq;

namespace BinaryObjectScanner.Utilities
{
    public static class Extensions
    {
        #region ConcurrentQueue

        /// <summary>
        /// Add a range of values from one queue to another
        /// </summary>
        /// <param name="original">Queue to add data to</param>
        /// <param name="values">Array to get data from</param>
#if NET20 || NET35
        public static void AddRange(this Queue<string> original, string[] values)
#else
        public static void AddRange(this ConcurrentQueue<string> original, string[] values)
#endif
        {
            if (values == null || values.Length == 0)
                return;

            for (int i = 0; i < values.Length; i++)
            {
                original.Enqueue(values[i]);
            }
        }

        /// <summary>
        /// Add a range of values from one queue to another
        /// </summary>
        /// <param name="original">Queue to add data to</param>
        /// <param name="values">Queue to get data from</param>
#if NET20 || NET35
        public static void AddRange(this Queue<string> original, IEnumerable<string> values)
#else
        public static void AddRange(this ConcurrentQueue<string> original, IEnumerable<string> values)
#endif
        {
            if (values == null || !values.Any())
                return;

            foreach (string value in values)
            {
                original.Enqueue(value);
            }
        }

        #endregion
    }
}