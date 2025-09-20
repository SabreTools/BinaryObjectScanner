using System;
using System.Collections.Generic;
using System.IO;

namespace BinaryObjectScanner
{
    // TODO: Clean this up once IO is updated
    internal static class Extensions
    {
        /// <summary>
        /// Helper to get the filesize from a path
        /// </summary>
        /// <returns>Size of the file path, -1 on error</returns>
        public static long FileSize(this string? filename)
        {
            // Invalid filenames are ignored
            if (string.IsNullOrEmpty(filename))
                return -1;

            // Non-file paths are ignored
            if (!File.Exists(filename))
                return -1;

            try
            {
                return new FileInfo(filename).Length;
            }
            catch
            {
                // Ignore errors
                return -1;
            }
        }

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

        /// <inheritdoc cref="string.Contains(string)"/>
        public static bool OptionalContains(this string? self, string value)
            => OptionalContains(self, value, StringComparison.Ordinal);

        /// <inheritdoc cref="string.Contains(string, StringComparison)"/>
        public static bool OptionalContains(this string? self, string value, StringComparison comparisonType)
        {
            if (self == null)
                return false;

#if NETFRAMEWORK || NETSTANDARD2_0
            return self.Contains(value);
#else
            return self.Contains(value, comparisonType);
#endif
        }

        /// <inheritdoc cref="string.Equals(string)"/>
        public static bool OptionalEquals(this string? self, string value)
            => OptionalEquals(self, value, StringComparison.Ordinal);

        /// <inheritdoc cref="string.Equals(string, StringComparison)"/>
        public static bool OptionalEquals(this string? self, string value, StringComparison comparisonType)
        {
            if (self == null)
                return false;

            return self.Equals(value, comparisonType);
        }

        /// <inheritdoc cref="string.StartsWith(string)"/>
        public static bool OptionalStartsWith(this string? self, string value)
            => OptionalStartsWith(self, value, StringComparison.Ordinal);

        /// <inheritdoc cref="string.StartsWith(string, StringComparison)"/>
        public static bool OptionalStartsWith(this string? self, string value, StringComparison comparisonType)
        {
            if (self == null)
                return false;

            return self.StartsWith(value, comparisonType);
        }
    }
}