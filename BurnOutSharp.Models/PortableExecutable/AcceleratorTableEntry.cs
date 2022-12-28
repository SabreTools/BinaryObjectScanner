using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// Describes the data in an individual accelerator table resource. The structure definition
    /// provided here is for explanation only; it is not present in any standard header file.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/acceltableentry"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class AcceleratorTableEntry
    {
        /// <summary>
        /// Describes keyboard accelerator characteristics.
        /// </summary>
        public AcceleratorTableFlags Flags;

        /// <summary>
        /// An ANSI character value or a virtual-key code that identifies the accelerator key.
        /// </summary>
        public ushort Ansi;

        /// <summary>
        /// An identifier for the keyboard accelerator. This is the value passed to the window
        /// procedure when the user presses the specified key.
        /// </summary>
        public ushort Id;

        /// <summary>
        /// The number of bytes inserted to ensure that the structure is aligned on a DWORD boundary.
        /// </summary>
        public ushort Padding;
    }
}
