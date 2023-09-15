using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class NCF : WrapperBase<SabreTools.Models.NCF.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Half-Life No Cache File (NCF)";

        #endregion

        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.NCF.Header.Dummy0"/>
#if NET48
        public uint Dummy0 => this.Model.Header.Dummy0;
#else
        public uint? Dummy0 => this.Model.Header?.Dummy0;
#endif

        /// <inheritdoc cref="Models.NCF.Header.MajorVersion"/>
#if NET48
        public uint MajorVersion => this.Model.Header.MajorVersion;
#else
        public uint? MajorVersion => this.Model.Header?.MajorVersion;
#endif

        /// <inheritdoc cref="Models.NCF.Header.MinorVersion"/>
#if NET48
        public uint MinorVersion => this.Model.Header.MinorVersion;
#else
        public uint? MinorVersion => this.Model.Header?.MinorVersion;
#endif

        /// <inheritdoc cref="Models.NCF.Header.CacheID"/>
#if NET48
        public uint CacheID => this.Model.Header.CacheID;
#else
        public uint? CacheID => this.Model.Header?.CacheID;
#endif

        /// <inheritdoc cref="Models.NCF.Header.LastVersionPlayed"/>
#if NET48
        public uint LastVersionPlayed => this.Model.Header.LastVersionPlayed;
#else
        public uint? LastVersionPlayed => this.Model.Header?.LastVersionPlayed;
#endif

        /// <inheritdoc cref="Models.NCF.Header.Dummy1"/>
#if NET48
        public uint Dummy1 => this.Model.Header.Dummy1;
#else
        public uint? Dummy1 => this.Model.Header?.Dummy1;
#endif

        /// <inheritdoc cref="Models.NCF.Header.Dummy2"/>
#if NET48
        public uint Dummy2 => this.Model.Header.Dummy2;
#else
        public uint? Dummy2 => this.Model.Header?.Dummy2;
#endif

        /// <inheritdoc cref="Models.NCF.Header.FileSize"/>
#if NET48
        public uint FileSize => this.Model.Header.FileSize;
#else
        public uint? FileSize => this.Model.Header?.FileSize;
#endif

        /// <inheritdoc cref="Models.NCF.Header.BlockSize"/>
#if NET48
        public uint BlockSize => this.Model.Header.BlockSize;
#else
        public uint? BlockSize => this.Model.Header?.BlockSize;
#endif

        /// <inheritdoc cref="Models.NCF.Header.BlockCount"/>
#if NET48
        public uint BlockCount => this.Model.Header.BlockCount;
#else
        public uint? BlockCount => this.Model.Header?.BlockCount;
#endif

        /// <inheritdoc cref="Models.NCF.Header.Dummy3"/>
#if NET48
        public uint Dummy3 => this.Model.Header.Dummy3;
#else
        public uint? Dummy3 => this.Model.Header?.Dummy3;
#endif

        #endregion

        #region Directory Header

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.Dummy0"/>
#if NET48
        public uint DH_Dummy0 => this.Model.DirectoryHeader.Dummy0;
#else
        public uint? DH_Dummy0 => this.Model.DirectoryHeader?.Dummy0;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.CacheID"/>
#if NET48
        public uint DH_CacheID => this.Model.DirectoryHeader.CacheID;
#else
        public uint? DH_CacheID => this.Model.DirectoryHeader?.CacheID;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.LastVersionPlayed"/>
#if NET48
        public uint DH_LastVersionPlayed => this.Model.DirectoryHeader.LastVersionPlayed;
#else
        public uint? DH_LastVersionPlayed => this.Model.DirectoryHeader?.LastVersionPlayed;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.ItemCount"/>
#if NET48
        public uint DH_ItemCount => this.Model.DirectoryHeader.ItemCount;
#else
        public uint? DH_ItemCount => this.Model.DirectoryHeader?.ItemCount;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.FileCount"/>
#if NET48
        public uint DH_FileCount => this.Model.DirectoryHeader.FileCount;
#else
        public uint? DH_FileCount => this.Model.DirectoryHeader?.FileCount;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.ChecksumDataLength"/>
#if NET48
        public uint DH_ChecksumDataLength => this.Model.DirectoryHeader.ChecksumDataLength;
#else
        public uint? DH_ChecksumDataLength => this.Model.DirectoryHeader?.ChecksumDataLength;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.DirectorySize"/>
#if NET48
        public uint DH_DirectorySize => this.Model.DirectoryHeader.DirectorySize;
#else
        public uint? DH_DirectorySize => this.Model.DirectoryHeader?.DirectorySize;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.NameSize"/>
#if NET48
        public uint DH_NameSize => this.Model.DirectoryHeader.NameSize;
#else
        public uint? DH_NameSize => this.Model.DirectoryHeader?.NameSize;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.Info1Count"/>
#if NET48
        public uint DH_Info1Count => this.Model.DirectoryHeader.Info1Count;
#else
        public uint? DH_Info1Count => this.Model.DirectoryHeader?.Info1Count;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.CopyCount"/>
#if NET48
        public uint DH_CopyCount => this.Model.DirectoryHeader.CopyCount;
#else
        public uint? DH_CopyCount => this.Model.DirectoryHeader?.CopyCount;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.LocalCount"/>
#if NET48
        public uint DH_LocalCount => this.Model.DirectoryHeader.LocalCount;
#else
        public uint? DH_LocalCount => this.Model.DirectoryHeader?.LocalCount;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.Dummy1"/>
#if NET48
        public uint DH_Dummy1 => this.Model.DirectoryHeader.Dummy1;
#else
        public uint? DH_Dummy1 => this.Model.DirectoryHeader?.Dummy1;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.Dummy2"/>
#if NET48
        public uint DH_Dummy2 => this.Model.DirectoryHeader.Dummy2;
#else
        public uint? DH_Dummy2 => this.Model.DirectoryHeader?.Dummy2;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.Checksum"/>
#if NET48
        public uint DH_Checksum => this.Model.DirectoryHeader.Checksum;
#else
        public uint? DH_Checksum => this.Model.DirectoryHeader?.Checksum;
#endif

        #endregion

        #region Directory Entries

        /// <inheritdoc cref="Models.NCF.File.DirectoryEntries"/>
#if NET48
        public SabreTools.Models.NCF.DirectoryEntry[] DirectoryEntries => this.Model.DirectoryEntries;
#else
        public SabreTools.Models.NCF.DirectoryEntry?[]? DirectoryEntries => this.Model.DirectoryEntries;
#endif

        #endregion

        #region Directory Names

        /// <inheritdoc cref="Models.NCF.File.DirectoryNames"/>
#if NET48
        public Dictionary<long, string> DirectoryNames => this.Model.DirectoryNames;
#else
        public Dictionary<long, string?>? DirectoryNames => this.Model.DirectoryNames;
#endif

        #endregion

        #region Directory Info 1 Entries

        /// <inheritdoc cref="Models.NCF.File.DirectoryInfo1Entries"/>
#if NET48
        public SabreTools.Models.NCF.DirectoryInfo1Entry[] DirectoryInfo1Entries => this.Model.DirectoryInfo1Entries;
#else
        public SabreTools.Models.NCF.DirectoryInfo1Entry?[]? DirectoryInfo1Entries => this.Model.DirectoryInfo1Entries;
#endif

        #endregion

        #region Directory Info 2 Entries

        /// <inheritdoc cref="Models.NCF.File.DirectoryInfo2Entries"/>
#if NET48
        public SabreTools.Models.NCF.DirectoryInfo2Entry[] DirectoryInfo2Entries => this.Model.DirectoryInfo2Entries;
#else
        public SabreTools.Models.NCF.DirectoryInfo2Entry?[]? DirectoryInfo2Entries => this.Model.DirectoryInfo2Entries;
#endif

        #endregion

        #region Directory Copy Entries

        /// <inheritdoc cref="Models.NCF.File.DirectoryCopyEntries"/>
#if NET48
        public SabreTools.Models.NCF.DirectoryCopyEntry[] DirectoryCopyEntries => this.Model.DirectoryCopyEntries;
#else
        public SabreTools.Models.NCF.DirectoryCopyEntry?[]? DirectoryCopyEntries => this.Model.DirectoryCopyEntries;
#endif

        #endregion

        #region Directory Local Entries

        /// <inheritdoc cref="Models.NCF.File.DirectoryLocalEntries"/>
#if NET48
        public SabreTools.Models.NCF.DirectoryLocalEntry[] DirectoryLocalEntries => this.Model.DirectoryLocalEntries;
#else
        public SabreTools.Models.NCF.DirectoryLocalEntry?[]? DirectoryLocalEntries => this.Model.DirectoryLocalEntries;
#endif

        #endregion

        #region Unknown Header

        /// <inheritdoc cref="Models.NCF.UnknownHeader.Dummy0"/>
#if NET48
        public uint UH_Dummy0 => this.Model.UnknownHeader.Dummy0;
#else
        public uint? UH_Dummy0 => this.Model.UnknownHeader?.Dummy0;
#endif

        /// <inheritdoc cref="Models.NCF.UnknownHeader.Dummy1"/>
#if NET48
        public uint UH_Dummy1 => this.Model.UnknownHeader.Dummy1;
#else
        public uint? UH_Dummy1 => this.Model.UnknownHeader?.Dummy1;
#endif

        #endregion

        #region Unknown Entries

        /// <inheritdoc cref="Models.NCF.File.UnknownEntries"/>
#if NET48
        public SabreTools.Models.NCF.UnknownEntry[] UnknownEntries => this.Model.UnknownEntries;
#else
        public SabreTools.Models.NCF.UnknownEntry?[]? UnknownEntries => this.Model.UnknownEntries;
#endif

        #endregion

        #region Checksum Header

        /// <inheritdoc cref="Models.NCF.ChecksumHeader.Dummy0"/>
#if NET48
        public uint CH_Dummy0 => this.Model.ChecksumHeader.Dummy0;
#else
        public uint? CH_Dummy0 => this.Model.ChecksumHeader?.Dummy0;
#endif

        /// <inheritdoc cref="Models.NCF.ChecksumHeader.ChecksumSize"/>
#if NET48
        public uint CH_ChecksumSize => this.Model.ChecksumHeader.ChecksumSize;
#else
        public uint? CH_ChecksumSize => this.Model.ChecksumHeader?.ChecksumSize;
#endif

        #endregion

        #region Checksum Map Header

        /// <inheritdoc cref="Models.NCF.ChecksumMapHeader.Dummy0"/>
#if NET48
        public uint CMH_Dummy0 => this.Model.ChecksumMapHeader.Dummy0;
#else
        public uint? CMH_Dummy0 => this.Model.ChecksumMapHeader?.Dummy0;
#endif

        /// <inheritdoc cref="Models.NCF.ChecksumMapHeader.Dummy1"/>
#if NET48
        public uint CMH_Dummy1 => this.Model.ChecksumMapHeader.Dummy1;
#else
        public uint? CMH_Dummy1 => this.Model.ChecksumMapHeader?.Dummy1;
#endif

        /// <inheritdoc cref="Models.NCF.ChecksumMapHeader.ItemCount"/>
#if NET48
        public uint CMH_ItemCount => this.Model.ChecksumMapHeader.ItemCount;
#else
        public uint? CMH_ItemCount => this.Model.ChecksumMapHeader?.ItemCount;
#endif

        /// <inheritdoc cref="Models.NCF.ChecksumMapHeader.ChecksumCount"/>
#if NET48
        public uint CMH_ChecksumCount => this.Model.ChecksumMapHeader.ChecksumCount;
#else
        public uint? CMH_ChecksumCount => this.Model.ChecksumMapHeader?.ChecksumCount;
#endif

        #endregion

        #region Checksum Map Entries

        /// <inheritdoc cref="Models.NCF.File.ChecksumMapEntries"/>
#if NET48
        public SabreTools.Models.NCF.ChecksumMapEntry[] ChecksumMapEntries => this.Model.ChecksumMapEntries;
#else
        public SabreTools.Models.NCF.ChecksumMapEntry?[]? ChecksumMapEntries => this.Model.ChecksumMapEntries;
#endif

        #endregion

        #region Checksum Entries

        /// <inheritdoc cref="Models.NCF.File.ChecksumEntries"/>
#if NET48
        public SabreTools.Models.NCF.ChecksumEntry[] ChecksumEntries => this.Model.ChecksumEntries;
#else
        public SabreTools.Models.NCF.ChecksumEntry?[]? ChecksumEntries => this.Model.ChecksumEntries;
#endif

        #endregion

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public NCF(SabreTools.Models.NCF.File model, byte[] data, int offset)
#else
        public NCF(SabreTools.Models.NCF.File? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public NCF(SabreTools.Models.NCF.File model, Stream data)
#else
        public NCF(SabreTools.Models.NCF.File? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an NCF from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the NCF</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An NCF wrapper on success, null on failure</returns>
#if NET48
        public static NCF Create(byte[] data, int offset)
#else
        public static NCF? Create(byte[]? data, int offset)
#endif
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a NCF from a Stream
        /// </summary>
        /// <param name="data">Stream representing the NCF</param>
        /// <returns>An NCF wrapper on success, null on failure</returns>
#if NET48
        public static NCF Create(Stream data)
#else
        public static NCF? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = new SabreTools.Serialization.Streams.NCF().Deserialize(data);
            if (file == null)
                return null;

            try
            {
                return new NCF(file, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();
            Printing.NCF.Print(builder, this.Model);
            return builder;
        }

        #endregion
    }
}