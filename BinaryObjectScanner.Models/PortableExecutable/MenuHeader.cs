using System.Runtime.InteropServices;

namespace BinaryObjectScanner.Models.PortableExecutable
{
    /// <summary>
    /// Contains version information for the menu resource. The structure definition provided
    /// here is for explanation only; it is not present in any standard header file.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/menuheader"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class MenuHeader
    {
        /// <summary>
        /// The version number of the menu template. This member must be equal to zero to indicate
        /// that this is an RT_MENU created with a standard menu template.
        /// </summary>
        public ushort Version;

        /// <summary>
        /// The size of the menu template header. This value is zero for menus you create with a
        /// standard menu template.
        /// </summary>
        public ushort HeaderSize;
    }
}
