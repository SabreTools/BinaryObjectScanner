using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.LinearExecutable
{
    /// <summary>
    /// The Verify Record Directive Table is an optional table. It maintains a record
    /// of the pages in the EXE file that have been fixed up and written back to the
    /// original linear EXE module, along with the module dependencies used to perform
    /// these fixups. This table provides an efficient means for verifying the virtual
    /// addresses required for the fixed up pages when the module is loaded.
    /// </summary>
    /// <see href="https://faydoc.tripod.com/formats/exe-LE.htm"/>
    /// <see href="http://www.edm2.com/index.php/LX_-_Linear_eXecutable_Module_Format_Description"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class VerifyRecordDirectiveTableEntry
    {
        /// <summary>
        /// Number of module dependencies.
        /// </summary>
        /// <remarks>
        /// This field specifies how many entries there are in the verify record
        /// directive table. This is equal to the number of modules referenced by
        /// this module.
        /// </remarks>
        public ushort EntryCount;

        /// <summary>
        /// Ordinal index into the Import Module Name Table.
        /// </summary>
        /// <remarks>
        /// This value is an ordered index in to the Import Module Name Table for
        /// the referenced module.
        /// </remarks>
        public ushort OrdinalIndex;

        /// <summary>
        /// Module Version.
        /// </summary>
        /// <remarks>
        /// This is the version of the referenced module that the fixups were
        /// originally performed.This is used to insure the same version of the
        /// referenced module is loaded that was fixed up in this module and
        /// therefore the fixups are still correct. This requires the version
        /// number in a module to be incremented anytime the entry point offsets
        /// change.
        /// </remarks>
        public ushort Version;

        /// <summary>
        /// Module # of Object Entries.
        /// </summary>
        /// <remarks>
        /// This field is used to identify the number of object verify entries
        /// that follow for the referenced module.
        /// </remarks>
        public ushort ObjectEntriesCount;

        /// <summary>
        /// Object # in Module.
        /// </summary>
        /// <remarks>
        /// This field specifies the object number in the referenced module that
        /// is being verified.
        /// </remarks>
        public ushort ObjectNumberInModule;

        /// <summary>
        /// Object load base address.
        /// </summary>
        /// <remarks>
        /// This is the address that the object was loaded at when the fixups were
        /// performed.
        /// </remarks>
        public ushort ObjectLoadBaseAddress;

        /// <summary>
        /// Object virtual address size.
        /// </summary>
        /// <remarks>
        /// This field specifies the total amount of virtual memory required for
        /// this object.
        /// </remarks>
        public ushort ObjectVirtualAddressSize;
    }
}
