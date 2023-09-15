using System;
using System.Text;

namespace BinaryObjectScanner.Printing
{
    internal static class Extensions
    {
        /// <summary>
        /// Append a line containing a byte value to a StringBuilder
        /// </summary>
#if NET48
        public static StringBuilder AppendLine(this StringBuilder sb, byte[] value, string prefixString)
#else
        public static StringBuilder AppendLine(this StringBuilder sb, byte[]? value, string prefixString)
#endif
        {
            string valueString = (value == null ? "[NULL]" : BitConverter.ToString(value).Replace('-', ' '));
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a Int8 to a StringBuilder
        /// </summary>
#if NET48
        public static StringBuilder AppendLine(this StringBuilder sb, sbyte value, string prefixString)
#else
        public static StringBuilder AppendLine(this StringBuilder sb, sbyte? value, string prefixString)
#endif
        {
#if NET6_0_OR_GREATER
            value ??= 0;
#endif

            string valueString = $"{value} (0x{value:X})";
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a UInt8 to a StringBuilder
        /// </summary>
#if NET48
        public static StringBuilder AppendLine(this StringBuilder sb, byte value, string prefixString)
#else
        public static StringBuilder AppendLine(this StringBuilder sb, byte? value, string prefixString)
#endif
        {
#if NET6_0_OR_GREATER
            value ??= 0;
#endif

            string valueString = $"{value} (0x{value:X})";
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a Int16 to a StringBuilder
        /// </summary>
#if NET48
        public static StringBuilder AppendLine(this StringBuilder sb, short value, string prefixString)
#else
        public static StringBuilder AppendLine(this StringBuilder sb, short? value, string prefixString)
#endif
        {
#if NET6_0_OR_GREATER
            value ??= 0;
#endif

            string valueString = $"{value} (0x{value:X})";
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a UInt16 to a StringBuilder
        /// </summary>
#if NET48
        public static StringBuilder AppendLine(this StringBuilder sb, ushort value, string prefixString)
#else
        public static StringBuilder AppendLine(this StringBuilder sb, ushort? value, string prefixString)
#endif
        {
#if NET6_0_OR_GREATER
            value ??= 0;
#endif

            string valueString = $"{value} (0x{value:X})";
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a Int32 to a StringBuilder
        /// </summary>
#if NET48
        public static StringBuilder AppendLine(this StringBuilder sb, int value, string prefixString)
#else
        public static StringBuilder AppendLine(this StringBuilder sb, int? value, string prefixString)
#endif
        {
#if NET6_0_OR_GREATER
            value ??= 0;
#endif

            string valueString = $"{value} (0x{value:X})";
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a UInt32 to a StringBuilder
        /// </summary>
#if NET48
        public static StringBuilder AppendLine(this StringBuilder sb, uint value, string prefixString)
#else
        public static StringBuilder AppendLine(this StringBuilder sb, uint? value, string prefixString)
#endif
        {
#if NET6_0_OR_GREATER
            value ??= 0;
#endif

            string valueString = $"{value} (0x{value:X})";
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a Int64 to a StringBuilder
        /// </summary>
#if NET48
        public static StringBuilder AppendLine(this StringBuilder sb, long value, string prefixString)
#else
        public static StringBuilder AppendLine(this StringBuilder sb, long? value, string prefixString)
#endif
        {
#if NET6_0_OR_GREATER
            value ??= 0;
#endif

            string valueString = $"{value} (0x{value:X})";
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a UInt64 to a StringBuilder
        /// </summary>
#if NET48
        public static StringBuilder AppendLine(this StringBuilder sb, ulong value, string prefixString)
#else
        public static StringBuilder AppendLine(this StringBuilder sb, ulong? value, string prefixString)
#endif
        {
#if NET6_0_OR_GREATER
            value ??= 0;
#endif

            string valueString = $"{value} (0x{value:X})";
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a string to a StringBuilder
        /// </summary>
#if NET48
        public static StringBuilder AppendLine(this StringBuilder sb, string value, string prefixString)
#else
        public static StringBuilder AppendLine(this StringBuilder sb, string? value, string prefixString)
#endif
        {
#if NET6_0_OR_GREATER
            value ??= string.Empty;
#endif

            string valueString = value ?? "[NULL]";
            return sb.AppendLine($"{prefixString}: {valueString}");
        }
    }
}