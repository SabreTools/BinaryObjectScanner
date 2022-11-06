using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// The resource directory string area consists of Unicode strings, which
    /// are word-aligned. These strings are stored together after the last
    /// Resource Directory entry and before the first Resource Data entry.
    /// This minimizes the impact of these variable-length strings on the
    /// alignment of the fixed-size directory entries.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    [StructLayout(LayoutKind.Sequential)]
    public class ResourceDataEntry
    {
        /// <summary>
        /// The address of a unit of resource data in the Resource Data area.
        /// </summary>
        public uint DataRVA;

        /// <summary>
        /// The size, in bytes, of the resource data that is pointed to by the
        /// Data RVA field.
        /// </summary>
        public uint Size;

        /// <summary>
        /// The code page that is used to decode code point values within the
        /// resource data. Typically, the code page would be the Unicode code page.
        /// </summary>
        public uint Codepage;

        /// <summary>
        /// Reserved, must be 0.
        /// </summary>
        public uint Reserved;
    }
}
