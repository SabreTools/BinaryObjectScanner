using System.Runtime.InteropServices;

namespace BinaryObjectScanner.Models.LinearExecutable
{
    /// <summary>
    /// The resource table is an array of resource table entries. Each resource table
    /// entry contains a type ID and name ID. These entries are used to locate resource
    /// objects contained in the Object table. The number of entries in the resource
    /// table is defined by the Resource Table Count located in the linear EXE header.
    /// More than one resource may be contained within a single object. Resource table
    /// entries are in a sorted order, (ascending, by Resource Name ID within the
    /// Resource Type ID). This allows the DosGetResource API function to use a binary
    /// search when looking up a resource in a 32-bit module instead of the linear search
    /// being used in the current 16-bit module.
    /// </summary>
    /// <see href="https://faydoc.tripod.com/formats/exe-LE.htm"/>
    /// <see href="http://www.edm2.com/index.php/LX_-_Linear_eXecutable_Module_Format_Description"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ResourceTableEntry
    {
        /// <summary>
        /// Resource type ID.
        /// </summary>
        public ResourceTableEntryType TypeID;

        /// <summary>
        /// An ID used as a name for the resource when referred to.
        /// </summary>
        public ushort NameID;

        /// <summary>
        /// The number of bytes the resource consists of.
        /// </summary>
        public uint ResourceSize;

        /// <summary>
        /// The number of the object which contains the resource.
        /// </summary>
        public ushort ObjectNumber;

        /// <summary>
        /// The offset within the specified object where the resource begins.
        /// </summary>
        public uint Offset;
    }
}
