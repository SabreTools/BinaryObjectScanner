namespace BurnOutSharp.Models.CFB
{
    /// <summary>
    /// Microsoft Compound File Binary (CFB) file format, also known as the
    /// Object Linking and Embedding (OLE) or Component Object Model (COM)
    /// structured storage compound file implementation binary file format.
    /// This structure name can be shortened to compound file.
    /// </summary>
    /// <see href="https://winprotocoldoc.blob.core.windows.net/productionwindowsarchives/MS-CFB/%5bMS-CFB%5d.pdf"/>
    public sealed class Binary
    {
        /// <summary>
        /// Compound file header
        /// </summary>
        public FileHeader Header { get; set; }

        /// <summary>
        /// The FAT is the main allocator for space within a compound file.
        /// Every sector in the file is represented within the FAT in some
        /// fashion, including those sectors that are unallocated (free).
        /// The FAT is a sector chain that is made up of one or more FAT sectors.
        /// </summary>
        /// <remarks>
        /// If Header Major Version is 3, there MUST be 128 fields specified to fill a 512-byte sector.
        /// 
        /// If Header Major Version is 4, there MUST be 1,024 fields specified to fill a 4,096-byte sector
        /// </remarks>
        public SectorNumber[] FATSectorNumbers { get; set; }

        /// <summary>
        /// The mini FAT is used to allocate space in the mini stream.
        /// The mini stream is divided intosmaller, equal-length sectors,
        /// and the sector size that is used for the mini stream is specified
        /// from the Compound File Header (64 bytes).
        /// </summary>
        /// <remarks>
        /// If Header Major Version is 3, there MUST be 128 fields specified to fill a 512-byte sector.
        /// 
        /// If Header Major Version is 4, there MUST be 1,024 fields specified to fill a 4,096-byte sector
        /// </remarks>
        public SectorNumber[] MiniFATSectorNumbers { get; set; }

        /// <summary>
        /// The DIFAT array is used to represent storage of the FAT sectors.
        /// The DIFAT is represented by an array of 32-bit sector numbers.
        /// The DIFAT array is stored both in the header and in DIFAT sectors.
        /// In the header, the DIFAT array occupies 109 entries, and in each
        /// DIFAT sector, the DIFAT array occupies the entire sector minus
        /// 4 bytes. (The last field is for chaining the DIFAT sector chain.)
        /// </summary>
        /// <remarks>
        /// If Header Major Version is 3, there MUST be 127 fields specified to
        /// fill a 512-byte sector minus the "Next DIFAT Sector Location" field.
        /// 
        /// If Header Major Version is 4, there MUST be 1,023 fields specified
        /// to fill a 4,096-byte sector minus the "Next DIFAT Sector Location" field.
        /// </remarks>
        public SectorNumber[] DIFATSectorNumbers { get; set; }

        /// <summary>
        /// The directory entry array is an array of directory entries that
        /// are grouped into a directory sector. Each storage object or stream
        /// object within a compound file is represented by a single directory
        /// entry. The space for the directory sectors that are holding the
        /// array is allocated from the FAT.
        /// </summary>
        /// <remarks>
        /// The first entry in the first sector of the directory chain (also
        /// referred to as the first element of the directory array, or stream
        /// ID #0) is known as the root directory entry, and it is reserved for
        /// two purposes. First, it provides a root parent for all objects that
        /// are stationed at the root of the compound file. Second, its function
        /// is overloaded to store the size and starting sector for the mini stream.
        /// 
        /// The root directory entry behaves as both a stream and a storage object.
        /// The root directory entry's Name field MUST contain the null-terminated
        /// string "Root Entry" in Unicode UTF-16.
        /// 
        /// The object class GUID (CLSID) that is stored in the root directory
        /// entry can be used for COM activation of the document's application.
        /// 
        /// The time stamps for the root storage are not maintained in the root
        /// directory entry. Rather, the root storage's creation and modification
        /// time stamps are normally stored on the file itself in the file system.
        /// 
        /// The Creation Time field in the root storage directory entry MUST be
        /// all zeroes. The Modified Time field in the root storage directory
        /// entry MAY be all zeroes.
        /// <remarks>
        public DirectoryEntry[] DirectoryEntries { get; set; }
    }
}