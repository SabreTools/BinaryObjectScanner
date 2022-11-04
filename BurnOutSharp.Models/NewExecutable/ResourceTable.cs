using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.NewExecutable
{
    /// <summary>
    /// The resource table follows the segment table and contains entries for
    /// each resource in the executable file. The resource table consists of
    /// an alignment shift count, followed by a table of resource records. The
    /// resource records define the type ID for a set of resources. Each
    /// resource record contains a table of resource entries of the defined
    /// type. The resource entry defines the resource ID or name ID for the
    /// resource. It also defines the location and size of the resource. The
    /// following describes the contents of each of these structures:
    /// </summary>
    /// <see href="http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm"/>
    [StructLayout(LayoutKind.Sequential)]
    public class ResourceTable
    {
        /// <summary>
        /// Alignment shift count for resource data.
        /// </summary>
        public ushort AlignmentShiftCount;

        /// <summary>
        /// A table of resource type information blocks follows.
        /// </summary>
        public ResourceTypeInformationEntry[] ResourceTypes;

        /// <summary>
        /// Resource type and name strings are stored at the end of the
        /// resource table.
        /// </summary>
        public ResourceTypeAndNameString[] TypeAndNameStrings;
    }
}
