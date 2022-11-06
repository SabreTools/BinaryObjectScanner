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
    public class ResourceDirectoryString
    {
        /// <summary>
        /// The size of the string, not including length field itself.
        /// </summary>
        public ushort Length;

        /// <summary>
        /// The variable-length Unicode string data, word-aligned.
        /// </summary>
        public byte[] UnicodeString;
    }
}
