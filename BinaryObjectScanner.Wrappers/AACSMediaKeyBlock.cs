using System;
using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class AACSMediaKeyBlock : WrapperBase
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string Description => "AACS Media Key Block";

        #endregion

        #region Pass-Through Properties

        #region Records

        /// <inheritdoc cref="Models.AACS.MediaKeyBlock.Records"/>
        public BinaryObjectScanner.Models.AACS.Record[] Records => _mediaKeyBlock.Records;

        #endregion

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the AACS media key block
        /// </summary>
        private BinaryObjectScanner.Models.AACS.MediaKeyBlock _mediaKeyBlock;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private AACSMediaKeyBlock() { }

        /// <summary>
        /// Create an AACS media key block from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An AACS media key block wrapper on success, null on failure</returns>
        public static AACSMediaKeyBlock Create(byte[] data, int offset)
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
        /// Create an AACS media key block from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>An AACS media key block wrapper on success, null on failure</returns>
        public static AACSMediaKeyBlock Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var mediaKeyBlock = Builders.AACS.ParseMediaKeyBlock(data);
            if (mediaKeyBlock == null)
                return null;

            var wrapper = new AACSMediaKeyBlock
            {
                _mediaKeyBlock = mediaKeyBlock,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("AACS Media Key Block Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            PrintRecords(builder);

            return builder;
        }

        /// <summary>
        /// Print records information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintRecords(StringBuilder builder)
        {
            builder.AppendLine("  Records Information:");
            builder.AppendLine("  -------------------------");
            if (Records == null || Records.Length == 0)
            {
                builder.AppendLine("  No records");
            }
            else
            {
                for (int i = 0; i < Records.Length; i++)
                {
                    var record = Records[i];
                    builder.AppendLine($"  Record Entry {i}");
                    builder.AppendLine($"    Record type: {record.RecordType} (0x{record.RecordType:X})");
                    builder.AppendLine($"    Record length: {record.RecordLength} (0x{record.RecordLength:X})");

                    switch (record.RecordType)
                    {
                        case BinaryObjectScanner.Models.AACS.RecordType.EndOfMediaKeyBlock:
                            var eomkb = record as BinaryObjectScanner.Models.AACS.EndOfMediaKeyBlockRecord;
                            builder.AppendLine($"    Signature data: {BitConverter.ToString(eomkb.SignatureData ?? new byte[0]).Replace('-', ' ')}");
                            break;

                        case BinaryObjectScanner.Models.AACS.RecordType.ExplicitSubsetDifference:
                            var esd = record as BinaryObjectScanner.Models.AACS.ExplicitSubsetDifferenceRecord;
                            builder.AppendLine($"    Subset Differences:");
                            builder.AppendLine("    -------------------------");
                            if (esd.SubsetDifferences == null || esd.SubsetDifferences.Length == 0)
                            {
                                builder.AppendLine($"    No subset differences");
                            }
                            else
                            {
                                for (int j = 0; j < esd.SubsetDifferences.Length; j++)
                                {
                                    var sd = esd.SubsetDifferences[j];
                                    builder.AppendLine($"    Subset Difference {j}");
                                    builder.AppendLine($"      Mask: {sd.Mask} (0x{sd.Mask:X})");
                                    builder.AppendLine($"      Number: {sd.Number} (0x{sd.Number:X})");
                                }
                            }
                            break;

                        case BinaryObjectScanner.Models.AACS.RecordType.MediaKeyData:
                            var mkd = record as BinaryObjectScanner.Models.AACS.MediaKeyDataRecord;
                            builder.AppendLine($"    Media Keys:");
                            builder.AppendLine("    -------------------------");
                            if (mkd.MediaKeyData == null || mkd.MediaKeyData.Length == 0)
                            {
                                builder.AppendLine($"    No media keys");
                            }
                            else
                            {
                                for (int j = 0; j < mkd.MediaKeyData.Length; j++)
                                {
                                    var mk = mkd.MediaKeyData[j];
                                    builder.AppendLine($"      Media key {j}: {BitConverter.ToString(mk ?? new byte[0]).Replace('-', ' ')}");
                                }
                            }
                            break;

                        case BinaryObjectScanner.Models.AACS.RecordType.SubsetDifferenceIndex:
                            var sdi = record as BinaryObjectScanner.Models.AACS.SubsetDifferenceIndexRecord;
                            builder.AppendLine($"    Span: {sdi.Span} (0x{sdi.Span:X})");
                            builder.AppendLine($"    Offsets:");
                            builder.AppendLine("    -------------------------");
                            if (sdi.Offsets == null || sdi.Offsets.Length == 0)
                            {
                                builder.AppendLine($"    No offsets");
                            }
                            else
                            {
                                for (int j = 0; j < sdi.Offsets.Length; j++)
                                {
                                    var offset = sdi.Offsets[j];
                                    builder.AppendLine($"      Offset {j}: {offset} (0x{offset:X})");
                                }
                            }
                            break;

                        case BinaryObjectScanner.Models.AACS.RecordType.TypeAndVersion:
                            var tav = record as BinaryObjectScanner.Models.AACS.TypeAndVersionRecord;
                            builder.AppendLine($"    Media key block type: {tav.MediaKeyBlockType} (0x{tav.MediaKeyBlockType:X})");
                            builder.AppendLine($"    Version number: {tav.VersionNumber} (0x{tav.VersionNumber:X})");
                            break;

                        case BinaryObjectScanner.Models.AACS.RecordType.DriveRevocationList:
                            var drl = record as BinaryObjectScanner.Models.AACS.DriveRevocationListRecord;
                            builder.AppendLine($"    Total number of entries: {drl.TotalNumberOfEntries} (0x{drl.TotalNumberOfEntries:X})");
                            builder.AppendLine($"    Signature Blocks:");
                            builder.AppendLine("    -------------------------");
                            if (drl.SignatureBlocks == null || drl.SignatureBlocks.Length == 0)
                            {
                                builder.AppendLine($"    No signature blocks");
                            }
                            else
                            {
                                for (int j = 0; j < drl.SignatureBlocks.Length; j++)
                                {
                                    var block = drl.SignatureBlocks[j];
                                    builder.AppendLine($"    Signature Block {j}");
                                    builder.AppendLine($"      Number of entries: {block.NumberOfEntries}");
                                    builder.AppendLine($"      Entry Fields:");
                                    builder.AppendLine("      -------------------------");
                                    if (block.EntryFields == null || block.EntryFields.Length == 0)
                                    {
                                        builder.AppendLine($"      No entry fields");
                                    }
                                    else
                                    {
                                        for (int k = 0; k < block.EntryFields.Length; k++)
                                        {
                                            var ef = block.EntryFields[k];
                                            builder.AppendLine($"      Entry {k}");
                                            builder.AppendLine($"        Range: {ef.Range} (0x{ef.Range:X})");
                                            builder.AppendLine($"        Drive ID: {BitConverter.ToString(ef.DriveID ?? new byte[0]).Replace('-', ' ')}");
                                        }
                                    }
                                }
                            }
                            break;

                        case BinaryObjectScanner.Models.AACS.RecordType.HostRevocationList:
                            var hrl = record as BinaryObjectScanner.Models.AACS.HostRevocationListRecord;
                            builder.AppendLine($"    Total number of entries: {hrl.TotalNumberOfEntries} (0x{hrl.TotalNumberOfEntries:X})");
                            builder.AppendLine($"    Signature Blocks:");
                            builder.AppendLine("    -------------------------");
                            if (hrl.SignatureBlocks == null || hrl.SignatureBlocks.Length == 0)
                            {
                                builder.AppendLine($"    No signature blocks");
                            }
                            else
                            {
                                for (int j = 0; j < hrl.SignatureBlocks.Length; j++)
                                {
                                    var block = hrl.SignatureBlocks[j];
                                    builder.AppendLine($"    Signature Block {j}");
                                    builder.AppendLine($"      Number of entries: {block.NumberOfEntries}");
                                    builder.AppendLine($"      Entry Fields:");
                                    builder.AppendLine("      -------------------------");
                                    if (block.EntryFields == null || block.EntryFields.Length == 0)
                                    {
                                        builder.AppendLine($"      No entry fields");
                                    }
                                    else
                                    {
                                        for (int k = 0; k < block.EntryFields.Length; k++)
                                        {
                                            var ef = block.EntryFields[k];
                                            builder.AppendLine($"      Entry {k}");
                                            builder.AppendLine($"        Range: {ef.Range} (0x{ef.Range:X})");
                                            builder.AppendLine($"        Host ID: {BitConverter.ToString(ef.HostID ?? new byte[0]).Replace('-', ' ')}");
                                        }
                                    }
                                }
                            }
                            break;

                        case BinaryObjectScanner.Models.AACS.RecordType.VerifyMediaKey:
                            var vmk = record as BinaryObjectScanner.Models.AACS.VerifyMediaKeyRecord;
                            builder.AppendLine($"    Ciphertext value: {BitConverter.ToString(vmk.CiphertextValue ?? new byte[0]).Replace('-', ' ')}");
                            break;

                        case BinaryObjectScanner.Models.AACS.RecordType.Copyright:
                            var c = record as BinaryObjectScanner.Models.AACS.CopyrightRecord;
                            builder.AppendLine($"    Copyright: {c.Copyright ?? "[NULL]"}");
                            break;
                    }
                }
            }
            builder.AppendLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_mediaKeyBlock, _jsonSerializerOptions);

#endif

        #endregion
    }
}