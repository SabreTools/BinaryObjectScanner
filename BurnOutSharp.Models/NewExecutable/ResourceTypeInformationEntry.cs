using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.NewExecutable
{
    /// <summary>
    /// A table of resource type information blocks follows. The following
    /// is the format of each type information block:
    /// </summary>
    /// <see href="http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm"/>
    [StructLayout(LayoutKind.Sequential)]
    public class ResourceTypeInformationEntry
    {
        /// <summary>
        /// Type ID. This is an integer type if the high-order bit is
        /// set (8000h); otherwise, it is an offset to the type string,
        /// the offset is relative to the beginning of the resource
        /// table. A zero type ID marks the end of the resource type
        /// information blocks.
        /// </summary>
        public ushort TypeID;

        /// <summary>
        /// Number of resources for this type.
        /// </summary>
        public ushort ResourceCount;

        /// <summary>
        /// Reserved.
        /// </summary>
        public uint Reserved;

        /// <summary>
        /// A table of resources for this type follows.
        /// </summary>
        public ResourceTypeResourceEntry[] Resources;
    }
}
