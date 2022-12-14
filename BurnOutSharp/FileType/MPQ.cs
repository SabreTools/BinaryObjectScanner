using System;
using System.Collections.Concurrent;
using System.IO;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Tools;
using StormLibSharp;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// MPQ game data archive
    /// </summary>
    public class MPQ : IScannable
    {
        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
            {
                return Scan(scanner, fs, file);
            }
        }

        // TODO: Add stream opening support
        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            // If the mpq file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                using (MpqArchive mpqArchive = new MpqArchive(file, FileAccess.Read))
                {
                    // Try to open the listfile
                    string listfile = null;
                    MpqFileStream listStream = mpqArchive.OpenFile("(listfile)");
                   
                    // If we can't read the listfile, we just return
                    if (!listStream.CanRead)
                        return null;

                    // Read the listfile in for processing
                    using (StreamReader sr = new StreamReader(listStream))
                    {
                        listfile = sr.ReadToEnd();
                    }

                    // Split the listfile by newlines
                    string[] listfileLines = listfile.Replace("\r\n", "\n").Split('\n');

                    // Loop over each entry
                    foreach (string sub in listfileLines)
                    {
                        // If an individual entry fails
                        try
                        {
                            string tempFile = Path.Combine(tempPath, sub);
                            Directory.CreateDirectory(Path.GetDirectoryName(tempFile));
                            mpqArchive.ExtractFile(sub, tempFile);
                        }
                        catch (Exception ex)
                        {
                            if (scanner.IncludeDebug) Console.WriteLine(ex);
                        }
                    }
                }

                // Collect and format all found protections
                var protections = scanner.GetProtections(tempPath);

                // If temp directory cleanup fails
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch (Exception ex)
                {
                    if (scanner.IncludeDebug) Console.WriteLine(ex);
                }

                // Remove temporary path references
                Utilities.StripFromKeys(protections, tempPath);

                return protections;
            }
            catch (Exception ex)
            {
                if (scanner.IncludeDebug) Console.WriteLine(ex);
            }

            return null;
        }

        // http://zezula.net/en/mpq/mpqformat.html
        #region TEMPORARY AREA FOR MPQ FORMAT

        internal class MoPaQArchive
        {
            //#region Serialization

            ///// <summary>
            ///// Deserialize <paramref name="data"/> at <paramref name="dataPtr"/> into a new MoPaQArchive object
            ///// </summary>
            //public static MoPaQArchive Deserialize(byte[] data, ref int dataPtr)
            //{
            //    if (data == null || dataPtr < 0)
            //        return null;

            //    MoPaQArchive archive = new MoPaQArchive();

            //    // Check for User Data
            //    uint possibleSignature = BitConverter.ToUInt32(data, dataPtr);
            //    if (possibleSignature == MoPaQUserData.SignatureValue)
            //    {
            //        // Save the current position for offset correction
            //        int basePtr = dataPtr;

            //        // Deserialize the user data, returning null if invalid
            //        archive.UserData = MoPaQUserData.Deserialize(data, ref dataPtr);
            //        if (archive.UserData == null)
            //            return null;

            //        // Set the starting position according to the header offset
            //        dataPtr = basePtr + (int)archive.UserData.HeaderOffset;
            //    }

            //    // Check for the Header
            //    possibleSignature = BitConverter.ToUInt32(data, dataPtr);
            //    if (possibleSignature == MoPaQArchiveHeader.SignatureValue)
            //    {
            //        // Deserialize the header, returning null if invalid
            //        archive.ArchiveHeader = MoPaQArchiveHeader.Deserialize(data, ref dataPtr);
            //        if (archive.ArchiveHeader == null)
            //            return null;
            //    }

            //    // If we don't have a header, return null
            //    if (archive.ArchiveHeader == null)
            //        return null;

            //    // TODO: Read in Hash Table
            //    // TODO: Read in Block Table
            //    // TODO: Read in Hi-Block Table
            //    // TODO: Read in BET Table
            //    // TODO: Read in HET Table

            //    return archive;
            //}

            //#endregion
        }

        internal class MoPaQUserData
        {
            //#region Constants

            //public const int Size = 0x10;

            ///// <summary>
            ///// Human-readable signature
            ///// </summary>
            //public static readonly string SignatureString = $"MPQ{(char)0x1B}";

            ///// <summary>
            ///// Signature as an unsigned Int32 value
            ///// </summary>
            //public const uint SignatureValue = 0x1B51504D;

            ///// <summary>
            ///// Signature as a byte array
            ///// </summary>
            //public static readonly byte[] SignatureBytes = new byte[] { 0x4D, 0x50, 0x51, 0x1B };

            //#endregion

            //#region Serialization

            ///// <summary>
            ///// Deserialize <paramref name="data"/> at <paramref name="dataPtr"/> into a new MoPaQUserData object
            ///// </summary>
            //public static MoPaQUserData Deserialize(byte[] data, ref int dataPtr)
            //{
            //    if (data == null || dataPtr < 0)
            //        return null;

            //    MoPaQUserData userData = new MoPaQUserData();

            //    userData.Signature = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    if (userData.Signature != SignatureValue)
            //        return null;

            //    userData.UserDataSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    userData.HeaderOffset = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    userData.UserDataHeaderSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;

            //    return userData;
            //}

            //#endregion
        }

        internal class MoPaQArchiveHeader
        {
            //#region Constants

            //#region Signatures

            ///// <summary>
            ///// Human-readable signature
            ///// </summary>
            //public static readonly string SignatureString = $"MPQ{(char)0x1A}";

            ///// <summary>
            ///// Signature as an unsigned Int32 value
            ///// </summary>
            //public const uint SignatureValue = 0x1A51504D;

            ///// <summary>
            ///// Signature as a byte array
            ///// </summary>
            //public static readonly byte[] SignatureBytes = new byte[] { 0x4D, 0x50, 0x51, 0x1A };

            //#endregion

            //#region Header Sizes

            //public const int HeaderVersion1Size = 0x20;

            //public const int HeaderVersion2Size = 0x2C;

            //public const int HeaderVersion3Size = 0x44;

            //public const int HeaderVersion4Size = 0xD0;

            //#endregion

            //#endregion

            //#region Serialization

            ///// <summary>
            ///// Deserialize <paramref name="data"/> at <paramref name="dataPtr"/> into a new MoPaQArchiveHeader object
            ///// </summary>
            //public static MoPaQArchiveHeader Deserialize(byte[] data, ref int dataPtr)
            //{
            //    if (data == null || dataPtr < 0)
            //        return null;

            //    MoPaQArchiveHeader archiveHeader = new MoPaQArchiveHeader();

            //    // V1 - Common
            //    archiveHeader.Signature = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    if (archiveHeader.Signature != SignatureValue)
            //        return null;

            //    archiveHeader.HeaderSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    archiveHeader.ArchiveSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    archiveHeader.FormatVersion = BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;
            //    archiveHeader.BlockSize = BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;
            //    archiveHeader.HashTablePosition = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    archiveHeader.BlockTablePosition = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    archiveHeader.HashTableSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    archiveHeader.BlockTableSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;

            //    // V2
            //    if (archiveHeader.FormatVersion >= 2 && archiveHeader.HeaderSize >= HeaderVersion2Size)
            //    {
            //        archiveHeader.HiBlockTablePosition = BitConverter.ToUInt64(data, dataPtr); dataPtr += 8;
            //        archiveHeader.HashTablePositionHi = BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;
            //        archiveHeader.BlockTablePositionHi = BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;
            //    }

            //    // V3
            //    if (archiveHeader.FormatVersion >= 3 && archiveHeader.HeaderSize >= HeaderVersion3Size)
            //    {
            //        archiveHeader.ArchiveSizeLong = BitConverter.ToUInt64(data, dataPtr); dataPtr += 8;
            //        archiveHeader.BetTablePosition = BitConverter.ToUInt64(data, dataPtr); dataPtr += 8;
            //        archiveHeader.HetTablePosition = BitConverter.ToUInt64(data, dataPtr); dataPtr += 8;
            //    }

            //    // V4
            //    if (archiveHeader.FormatVersion >= 4 && archiveHeader.HeaderSize >= HeaderVersion4Size)
            //    {
            //        archiveHeader.HashTableSizeLong = BitConverter.ToUInt64(data, dataPtr); dataPtr += 8;
            //        archiveHeader.BlockTableSizeLong = BitConverter.ToUInt64(data, dataPtr); dataPtr += 8;
            //        archiveHeader.HiBlockTableSize = BitConverter.ToUInt64(data, dataPtr); dataPtr += 8;
            //        archiveHeader.HetTableSize = BitConverter.ToUInt64(data, dataPtr); dataPtr += 8;
            //        archiveHeader.BetTablesize = BitConverter.ToUInt64(data, dataPtr); dataPtr += 8;
            //        archiveHeader.RawChunkSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;

            //        Array.Copy(data, dataPtr, archiveHeader.BlockTableMD5, 0, 0x10); dataPtr += 0x10;
            //        Array.Copy(data, dataPtr, archiveHeader.HashTableMD5, 0, 0x10); dataPtr += 0x10;
            //        Array.Copy(data, dataPtr, archiveHeader.HiBlockTableMD5, 0, 0x10); dataPtr += 0x10;
            //        Array.Copy(data, dataPtr, archiveHeader.BetTableMD5, 0, 0x10); dataPtr += 0x10;
            //        Array.Copy(data, dataPtr, archiveHeader.HetTableMD5, 0, 0x10); dataPtr += 0x10;
            //        Array.Copy(data, dataPtr, archiveHeader.MpqHeaderMD5, 0, 0x10); dataPtr += 0x10;
            //    }

            //    return archiveHeader;
            //}

            //#endregion
        }

        internal class MoPaQHetTable
        {
            //#region Constants

            //public const int Size = 0x44;

            ///// <summary>
            ///// Human-readable signature
            ///// </summary>
            //public static readonly string SignatureString = $"HET{(char)0x1A}";

            ///// <summary>
            ///// Signature as an unsigned Int32 value
            ///// </summary>
            //public const uint SignatureValue = 0x1A544548;

            ///// <summary>
            ///// Signature as a byte array
            ///// </summary>
            //public static readonly byte[] SignatureBytes = new byte[] { 0x48, 0x45, 0x54, 0x1A };

            //#endregion

            //#region Serialization

            ///// <summary>
            ///// Deserialize <paramref name="data"/> at <paramref name="dataPtr"/> into a new MoPaQHetTable object
            ///// </summary>
            //public static MoPaQHetTable Deserialize(byte[] data, ref int dataPtr)
            //{
            //    if (data == null || dataPtr < 0)
            //        return null;

            //    MoPaQHetTable hetTable = new MoPaQHetTable();

            //    // Common Headers
            //    hetTable.Signature = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    if (hetTable.Signature != SignatureValue)
            //        return null;

            //    hetTable.Version = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    hetTable.DataSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;

            //    // HET-Specific
            //    hetTable.TableSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    hetTable.MaxFileCount = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    hetTable.HashTableSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    hetTable.TotalIndexSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    hetTable.IndexSizeExtra = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    hetTable.IndexSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    hetTable.BlockTableSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;

            //    hetTable.HashTable = new byte[hetTable.HashTableSize];
            //    Array.Copy(data, dataPtr, hetTable.HashTable, 0, hetTable.HashTableSize);
            //    dataPtr += (int)hetTable.HashTableSize;

            //    // TODO: Populate the file indexes array
            //    hetTable.FileIndexes = new byte[(int)hetTable.HashTableSize][];

            //    return hetTable;
            //}

            //#endregion
        }

        internal class MoPaQBetTable
        {
            //#region Constants

            //public const int Size = 0x88;

            ///// <summary>
            ///// Human-readable signature
            ///// </summary>
            //public static readonly string SignatureString = $"BET{(char)0x1A}";

            ///// <summary>
            ///// Signature as an unsigned Int32 value
            ///// </summary>
            //public const uint SignatureValue = 0x1A544542;

            ///// <summary>
            ///// Signature as a byte array
            ///// </summary>
            //public static readonly byte[] SignatureBytes = new byte[] { 0x42, 0x45, 0x54, 0x1A };

            //#endregion

            //#region Serialization

            ///// <summary>
            ///// Deserialize <paramref name="data"/> at <paramref name="dataPtr"/> into a new MoPaQBetTable object
            ///// </summary>
            //public static MoPaQBetTable Deserialize(byte[] data, ref int dataPtr)
            //{
            //    if (data == null || dataPtr < 0)
            //        return null;

            //    MoPaQBetTable betTable = new MoPaQBetTable();

            //    // Common Headers
            //    betTable.Signature = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    if (betTable.Signature != SignatureValue)
            //        return null;

            //    betTable.Version = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    betTable.DataSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;

            //    // BET-Specific
            //    betTable.TableSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    betTable.FileCount = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    betTable.Unknown = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    betTable.TableEntrySize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;

            //    betTable.FilePositionBitIndex = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    betTable.FileSizeBitIndex = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    betTable.CompressedSizeBitIndex = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    betTable.FlagIndexBitIndex = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    betTable.UnknownBitIndex = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;

            //    betTable.FilePositionBitCount = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    betTable.FileSizeBitCount = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    betTable.CompressedSizeBitCount = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    betTable.FlagIndexBitCount = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    betTable.UnknownBitCount = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;

            //    betTable.TotalBetHashSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    betTable.BetHashSizeExtra = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    betTable.BetHashSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    betTable.BetHashArraySize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    betTable.FlagCount = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;

            //    betTable.FlagsArray = new uint[betTable.FlagCount];
            //    Buffer.BlockCopy(data, dataPtr, betTable.FlagsArray, 0, (int)betTable.FlagCount * 4);
            //    dataPtr += (int)betTable.FlagCount * 4;

            //    // TODO: Populate the file table
            //    // TODO: Populate the hash table

            //    return betTable;
            //}

            //#endregion
        }

        internal class MoPaQHashEntry
        {
            //#region Constants

            //public const int Size = 0x10;

            //#endregion

            //#region Serialization

            ///// <summary>
            ///// Deserialize <paramref name="data"/> at <paramref name="dataPtr"/> into a new MoPaQHashEntry object
            ///// </summary>
            //public static MoPaQHashEntry Deserialize(byte[] data, ref int dataPtr)
            //{
            //    if (data == null || dataPtr < 0)
            //        return null;

            //    MoPaQHashEntry hashEntry = new MoPaQHashEntry();

            //    hashEntry.NameHashPartA = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    hashEntry.NameHashPartB = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    hashEntry.Locale = (MoPaQLocale)BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;
            //    hashEntry.Platform = BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;
            //    hashEntry.BlockIndex = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;

            //    return hashEntry;
            //}

            //#endregion
        }

        internal class MoPaQBlockEntry
        {
            //#region Constants

            //public const int Size = 0x10;

            //#endregion

            //#region Serialization

            ///// <summary>
            ///// Deserialize <paramref name="data"/> at <paramref name="dataPtr"/> into a new MoPaQBlockEntry object
            ///// </summary>
            //public static MoPaQBlockEntry Deserialize(byte[] data, ref int dataPtr)
            //{
            //    if (data == null || dataPtr < 0)
            //        return null;

            //    MoPaQBlockEntry blockEntry = new MoPaQBlockEntry();

            //    blockEntry.FilePosition = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    blockEntry.CompressedSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    blockEntry.UncompressedSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    blockEntry.Flags = (MoPaQFileFlags)BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;

            //    return blockEntry;
            //}

            //#endregion
        }

        internal class MoPaQPatchInfo
        {
            //#region Serialization

            ///// <summary>
            ///// Deserialize <paramref name="data"/> at <paramref name="dataPtr"/> into a new MoPaQPatchInfo object
            ///// </summary>
            //public static MoPaQPatchInfo Deserialize(byte[] data, ref int dataPtr)
            //{
            //    if (data == null || dataPtr < 0)
            //        return null;

            //    MoPaQPatchInfo patchInfo = new MoPaQPatchInfo();

            //    patchInfo.Length = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    patchInfo.Flags = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    patchInfo.DataSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
            //    Array.Copy(data, dataPtr, patchInfo.MD5, 0, 0x10); dataPtr += 0x10;

            //    // TODO: Fill the sector offset table

            //    return patchInfo;
            //}

            //#endregion
        }

        internal class MoPaQPatchHeader
        {
            //#region Constants

            //#region Signatures

            //#region Patch Header

            ///// <summary>
            ///// Human-readable signature
            ///// </summary>
            //public static readonly string PatchSignatureString = $"PTCH";

            ///// <summary>
            ///// Signature as an unsigned Int32 value
            ///// </summary>
            //public const uint PatchSignatureValue = 0x48435450;

            ///// <summary>
            ///// Signature as a byte array
            ///// </summary>
            //public static readonly byte[] PatchSignatureBytes = new byte[] { 0x50, 0x54, 0x43, 0x48 };

            //#endregion

            //#region MD5 Block

            ///// <summary>
            ///// Human-readable signature
            ///// </summary>
            //public static readonly string Md5SignatureString = $"MD5_";

            ///// <summary>
            ///// Signature as an unsigned Int32 value
            ///// </summary>
            //public const uint Md5SignatureValue = 0x5F35444D;

            ///// <summary>
            ///// Signature as a byte array
            ///// </summary>
            //public static readonly byte[] Md5SignatureBytes = new byte[] { 0x4D, 0x44, 0x35, 0x5F };

            //#endregion

            //#region XFRM Block

            ///// <summary>
            ///// Human-readable signature
            ///// </summary>
            //public static readonly string XFRMSignatureString = $"XFRM";

            ///// <summary>
            ///// Signature as an unsigned Int32 value
            ///// </summary>
            //public const uint XFRMSignatureValue = 0x4D524658;

            ///// <summary>
            ///// Signature as a byte array
            ///// </summary>
            //public static readonly byte[] XFRMSignatureBytes = new byte[] { 0x58, 0x46, 0x52, 0x4D };

            //#endregion

            //#region BSDIFF Patch Type

            ///// <summary>
            ///// Human-readable signature
            ///// </summary>
            //public static readonly string BSDIFF40SignatureString = $"BSDIFF40";

            ///// <summary>
            ///// Signature as an unsigned Int64 value
            ///// </summary>
            //public const ulong BSDIFF40SignatureValue = 0x3034464649445342;

            ///// <summary>
            ///// Signature as a byte array
            ///// </summary>
            //public static readonly byte[] BSDIFF40SignatureBytes = new byte[] { 0x42, 0x53, 0x44, 0x49, 0x46, 0x46, 0x34, 0x30 };


            //#endregion

            //#endregion

            //#endregion
        }

        #endregion
    }
}
