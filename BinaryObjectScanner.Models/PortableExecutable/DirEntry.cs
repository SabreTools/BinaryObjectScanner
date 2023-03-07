namespace BinaryObjectScanner.Models.PortableExecutable
{
    /// <summary>
    /// Contains the information necessary for an application to access a specific font. The structure
    /// definition provided here is for explanation only; it is not present in any standard header file.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/direntry"/>
    public sealed class DirEntry
    {
        /// <summary>
        /// A unique ordinal identifier for an individual font in a font resource group.
        /// </summary>
        public ushort FontOrdinal;

        /// <summary>
        /// The FONTDIRENTRY structure for the specified font directly follows the DIRENTRY structure
        /// for that font.
        /// </summary>
        public FontDirEntry Entry;
    }
}
