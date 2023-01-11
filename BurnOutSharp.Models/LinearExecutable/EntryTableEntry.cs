using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.LinearExecutable
{
    /// <see href="https://faydoc.tripod.com/formats/exe-LE.htm"/>
    /// <see href="http://www.edm2.com/index.php/LX_-_Linear_eXecutable_Module_Format_Description"/>
    [StructLayout(LayoutKind.Explicit)]
    public sealed class EntryTableEntry
    {
        #region 16-bit Entry

        /// <summary>
        /// Object number.
        /// </summary>
        /// <remarks>
        /// This is the object number for the entries in this bundle.
        /// </remarks>
        [FieldOffset(0)] public ushort SixteenBitObjectNumber;

        /// <summary>
        /// Entry flags.
        /// </summary>
        /// <remarks>
        /// These are the flags for this entry point.
        /// </remarks>
        [FieldOffset(2)] public EntryFlags SixteenBitEntryFlags;

        /// <summary>
        /// Offset in object.
        /// </summary>
        /// <remarks>
        /// This is the offset in the object for the entry point defined at this ordinal number.
        /// </remarks>
        [FieldOffset(3)] public ushort SixteenBitOffset;

        #endregion

        #region 286 Call Gate Entry

        /// <summary>
        /// Object number.
        /// </summary>
        /// <remarks>
        /// This is the object number for the entries in this bundle.
        /// </remarks>
        [FieldOffset(0)] public ushort TwoEightySixObjectNumber;

        /// <summary>
        /// Entry flags.
        /// </summary>
        /// <remarks>
        /// These are the flags for this entry point.
        /// </remarks>
        [FieldOffset(2)] public EntryFlags TwoEightySixEntryFlags;

        /// <summary>
        /// Offset in object.
        /// </summary>
        /// <remarks>
        /// This is the offset in the object for the entry point defined at this ordinal number.
        /// </remarks>
        [FieldOffset(3)] public ushort TwoEightySixOffset;

        /// <summary>
        /// Callgate selector.
        /// </summary>
        /// <remarks>
        /// The callgate selector is a reserved field used by the loader to store a call
        /// gate selector value for references to ring 2 entry points. When a ring 3
        /// reference to a ring 2 entry point is made, the callgate selector with a zero
        /// offset is place in the relocation fixup address. The segment number and offset
        /// in segment is placed in the LDT callgate.
        /// </remarks>
        [FieldOffset(5)] public ushort TwoEightySixCallgate;

        #endregion

        #region 32-bit Entry

        /// <summary>
        /// Object number.
        /// </summary>
        /// <remarks>
        /// This is the object number for the entries in this bundle.
        /// </remarks>
        [FieldOffset(0)] public ushort ThirtyTwoBitObjectNumber;

        /// <summary>
        /// Entry flags.
        /// </summary>
        /// <remarks>
        /// These are the flags for this entry point.
        /// </remarks>
        [FieldOffset(2)] public EntryFlags ThirtyTwoBitEntryFlags;

        /// <summary>
        /// Offset in object.
        /// </summary>
        /// <remarks>
        /// This is the offset in the object for the entry point defined at this ordinal number.
        /// </remarks>
        [FieldOffset(3)] public uint ThirtyTwoBitOffset;

        #endregion

        #region Forwarder Entry

        /// <summary>
        /// 0
        /// </summary>
        /// <remarks>
        /// This field is reserved for future use.
        /// </remarks>
        [FieldOffset(0)] public ushort ForwarderReserved;

        /// <summary>
        /// Forwarder flags.
        /// </summary>
        /// <remarks>
        /// These are the flags for this entry point.
        /// </remarks>
        [FieldOffset(2)] public ForwarderFlags ForwarderFlags;

        /// <summary>
        /// Module Ordinal Number
        /// </summary>
        /// <remarks>
        /// This is the index into the Import Module Name Table for this forwarder.
        /// </remarks>
        [FieldOffset(3)] public ushort ForwarderModuleOrdinalNumber;

        /// <summary>
        /// Procedure Name Offset
        /// </summary>
        /// <remarks>
        /// If the FLAGS field indicates import by ordinal, then this field is the
        /// ordinal number into the Entry Table of the target module, otherwise this
        /// field is the offset into the Procedure Names Table of the target module.
        /// 
        /// A Forwarder entry (type = 4) is an entry point whose value is an imported
        /// reference. When a load time fixup occurs whose target is a forwarder, the
        /// loader obtains the address imported by the forwarder and uses that imported
        /// address to resolve the fixup.
        /// 
        /// A forwarder may refer to an entry point in another module which is itself a
        /// forwarder, so there can be a chain of forwarders. The loader will traverse
        /// the chain until it finds a non-forwarded entry point which terminates the
        /// chain, and use this to resolve the original fixup. Circular chains are
        /// detected by the loader and result in a load time error. A maximum of 1024
        /// forwarders is allowed in a chain; more than this results in a load time error.
        /// 
        /// Forwarders are useful for merging and recombining API calls into different
        /// sets of libraries, while maintaining compatibility with applications. For
        /// example, if one wanted to combine MONCALLS, MOUCALLS, and VIOCALLS into a
        /// single libraries, one could provide entry points for the three libraries
        /// that are forwarders pointing to the common implementation.
        /// </remarks>
        [FieldOffset(5)] public uint ProcedureNameOffset;

        /// <summary>
        /// Import Ordinal Number
        /// </summary>
        /// <remarks>
        /// If the FLAGS field indicates import by ordinal, then this field is the
        /// ordinal number into the Entry Table of the target module, otherwise this
        /// field is the offset into the Procedure Names Table of the target module.
        /// 
        /// A Forwarder entry (type = 4) is an entry point whose value is an imported
        /// reference. When a load time fixup occurs whose target is a forwarder, the
        /// loader obtains the address imported by the forwarder and uses that imported
        /// address to resolve the fixup.
        /// 
        /// A forwarder may refer to an entry point in another module which is itself a
        /// forwarder, so there can be a chain of forwarders. The loader will traverse
        /// the chain until it finds a non-forwarded entry point which terminates the
        /// chain, and use this to resolve the original fixup. Circular chains are
        /// detected by the loader and result in a load time error. A maximum of 1024
        /// forwarders is allowed in a chain; more than this results in a load time error.
        /// 
        /// Forwarders are useful for merging and recombining API calls into different
        /// sets of libraries, while maintaining compatibility with applications. For
        /// example, if one wanted to combine MONCALLS, MOUCALLS, and VIOCALLS into a
        /// single libraries, one could provide entry points for the three libraries
        /// that are forwarders pointing to the common implementation.
        /// </remarks>
        [FieldOffset(5)] public uint ImportOrdinalNumber;

        #endregion
    }
}