using System.Collections.Concurrent;

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
        public static void AddRange(this ConcurrentQueue<string> original, string[] values)
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
        public static void AddRange(this ConcurrentQueue<string> original, ConcurrentQueue<string> values)
        {
            while (!values.IsEmpty)
            {
                if (!values.TryDequeue(out var value))
                    return;

                original.Enqueue(value);
            }
        }

        #endregion
    }
}