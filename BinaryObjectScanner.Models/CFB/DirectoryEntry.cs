using System;

namespace BinaryObjectScanner.Models.CFB
{
    /// <see href="https://winprotocoldoc.blob.core.windows.net/productionwindowsarchives/MS-CFB/%5bMS-CFB%5d.pdf"/>
    public sealed class DirectoryEntry
    {
        /// <summary>
        /// This field MUST contain a Unicode string for the storage or stream
        /// name encoded in UTF-16. The name MUST be terminated with a UTF-16
        /// terminating null character. Thus, storage and stream names are limited
        /// to 32 UTF-16 code points, including the terminating null character.
        /// When locating an object in the compound file except for the root
        /// storage, the directory entry name is compared by using a special
        /// case-insensitive uppercase mapping, described in Red-Black Tree.
        /// The following characters are illegal and MUST NOT be part of the
        /// name: '/', '\', ':', '!'.
        /// </summary>
        public string Name;

        /// <summary>
        /// This field MUST be 0x00, 0x01, 0x02, or 0x05, depending on the
        /// actual type of object. All other values are not valid.
        /// </summary>
        public ushort NameLength;

        /// <summary>
        /// This field MUST match the length of the Directory Entry Name Unicode
        /// string in bytes. The length MUST be a multiple of 2 and include the
        /// terminating null character in the count. This length MUST NOT exceed 64,
        /// the maximum size of the Directory Entry Name field.
        /// </summary>
        public ObjectType ObjectType;

        /// <summary>
        /// This field MUST be 0x00 (red) or 0x01 (black). All other values are not valid.
        /// </summary>
        public ColorFlag ColorFlag;

        /// <summary>
        /// This field contains the stream ID of the left sibling. If there
        /// is no left sibling, the field MUST be set to NOSTREAM (0xFFFFFFFF).
        /// </summary>
        public StreamID LeftSiblingID;

        /// <summary>
        /// This field contains the stream ID of the right sibling. If there
        /// is no right sibling, the field MUST be set to NOSTREAM (0xFFFFFFFF).
        /// </summary>
        public StreamID RightSiblingID;

        /// <summary>
        /// This field contains the stream ID of a child object. If there is no
        /// child object, including all entries for stream objects, the field
        /// MUST be set to NOSTREAM (0xFFFFFFFF).
        /// </summary>
        public StreamID ChildID;

        /// <summary>
        /// This field contains an object class GUID, if this entry is for a
        /// storage object or root storage object. For a stream object, this field
        /// MUST be set to all zeroes. A value containing all zeroes in a storage
        /// or root storage directory entry is valid, and indicates that no object
        /// class is associated with the storage. If an implementation of the file
        /// format enables applications to create storage objects without explicitly
        /// setting an object class GUID, it MUST write all zeroes by default. If
        /// this value is not all zeroes, the object class GUID can be used as a
        /// parameter to start applications.
        /// </summary>
        public Guid CLSID;

        /// <summary>
        /// This field contains the user-defined flags if this entry is for a storage
        /// object or root storage object. For a stream object, this field SHOULD be
        /// set to all zeroes because many implementations provide no way for
        /// applications to retrieve state bits from a stream object. If an
        /// implementation of the file format enables applications to create storage
        /// objects without explicitly setting state bits, it MUST write all zeroes
        /// by default.
        /// </summary>
        public uint StateBits;

        /// <summary>
        /// This field contains the creation time for a storage object, or all zeroes
        /// to indicate that the creation time of the storage object was not recorded.
        /// The Windows FILETIME structure is used to represent this field in UTC.
        /// For a stream object, this field MUST be all zeroes. For a root storage
        /// object, this field MUST be all zeroes, and the creation time is retrieved
        /// or set on the compound file itself.
        /// </summary>
        public ulong CreationTime;

        /// <summary>
        /// This field contains the modification time for a storage object, or all
        /// zeroes to indicate that the modified time of the storage object was not
        /// recorded. The Windows FILETIME structure is used to represent this field
        /// in UTC. For a stream object, this field MUST be all zeroes. For a root
        /// storage object, this field MAY<2> be set to all zeroes, and the modified
        /// time is retrieved or set on the compound file itself.
        /// </summary>
        public ulong ModifiedTime;

        /// <summary>
        /// This field contains the first sector location if this is a stream object.
        /// For a root storage object, this field MUST contain the first sector of the
        /// mini stream, if the mini stream exists. For a storage object, this field MUST
        /// be set to all zeroes.
        /// </summary>
        public uint StartingSectorLocation;

        /// <summary>
        /// This 64-bit integer field contains the size of the user-defined data if this
        /// is a stream object. For a root storage object, this field contains the size
        /// of the mini stream. For a storage object, this field MUST be set to all zeroes.
        /// </summary>
        /// <remarks>
        /// For a version 3 compound file 512-byte sector size, the value of this field MUST
        /// be less than or equal to 0x80000000. (Equivalently, this requirement can be stated:
        /// the size of a stream or of the mini stream in a version 3 compound file MUST be
        /// less than or equal to 2 gigabytes (GB).) Note that as a consequence of this
        /// requirement, the most significant 32 bits of this field MUST be zero in a version
        /// 3 compound file. However, implementers should be aware that some older
        /// implementations did not initialize the most significant 32 bits of this field,
        /// and these bits might therefore be nonzero in files that are otherwise valid
        /// version 3 compound files. Although this document does not normatively specify
        /// parser behavior, it is recommended that parsers ignore the most significant 32 bits
        /// of this field in version 3 compound files, treating it as if its value were zero,
        /// unless there is a specific reason to do otherwise (for example, a parser whose
        /// purpose is to verify the correctness of a compound file).
        /// </remarks>
        public ulong StreamSize;
    }
}