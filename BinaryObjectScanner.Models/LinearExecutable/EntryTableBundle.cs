using System.Runtime.InteropServices;

namespace BinaryObjectScanner.Models.LinearExecutable
{
    /// <summary>
    /// The entry table contains object and offset information that is used to resolve
    /// fixup references to the entry points within this module. Not all entry points
    /// in the entry table will be exported, some entry points will only be used
    /// within the module. An ordinal number is used to index into the entry table.
    /// The entry table entries are numbered starting from one. 
    /// 
    /// The list of entries are compressed into 'bundles', where possible. The entries
    /// within each bundle are all the same size. A bundle starts with a count field
    /// which indicates the number of entries in the bundle. The count is followed by
    /// a type field which identifies the bundle format. This provides both a means
    /// for saving space as well as a mechanism for extending the bundle types.
    /// </summary>
    /// <see href="https://faydoc.tripod.com/formats/exe-LE.htm"/>
    /// <see href="http://www.edm2.com/index.php/LX_-_Linear_eXecutable_Module_Format_Description"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class EntryTableBundle
    {
        /// <summary>
        /// Number of entries.
        /// </summary>
        /// <remarks>
        /// This is the number of entries in this bundle.
        /// 
        /// A zero value for the number of entries identifies the end of the
        /// entry table. There is no further bundle information when the number
        /// of entries is zero. In other words the entry table is terminated by
        /// a single zero byte.
        /// 
        /// For <see cref="BundleType.UnusedEntry"/>, this is the number of unused
        ///     entries to skip.
        /// For <see cref="BundleType.SixteenBitEntry"/>, this is the number of 16-bit
        ///     entries in this bundle. The flags and offset value are repeated this
        ///     number of times.
        /// For <see cref="BundleType.TwoEightySixCallGateEntry"/>, this is the number
        ///     of 286 call gate entries in this bundle. The flags, callgate, and offset
        ///     value are repeated this number of times.
        /// For <see cref="BundleType.ThirtyTwoBitEntry"/>, this is the number
        ///     of 32-bit entries in this bundle. The flags and offset value are repeated
        ///     this number of times.
        /// For <see cref="BundleType.ForwarderEntry"/>, this field is reserved for future use.
        /// </remarks>
        public byte Entries;

        /// <summary>
        /// This defines the bundle type which determines the contents of the BUNDLE INFO.
        /// </summary>
        public BundleType BundleType;

        /// <summary>
        /// Table entries in the bundle
        /// </summary>
        public EntryTableEntry[] TableEntries;
    }
}
