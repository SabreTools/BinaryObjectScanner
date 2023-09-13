using System;
using System.IO;
using System.Text;
using SabreTools.Models.AACS;

namespace BinaryObjectScanner.Wrappers
{
    public class AACSMediaKeyBlock : WrapperBase<MediaKeyBlock>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "AACS Media Key Block";

        #endregion

        #region Pass-Through Properties

        #region Records

        /// <inheritdoc cref="Models.AACS.MediaKeyBlock.Records"/>
#if NET48
        public Record[] Records => _model.Records;
#else
        public Record?[]? Records => _model.Records;
#endif

        #endregion

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public AACSMediaKeyBlock(MediaKeyBlock model, byte[] data, int offset)
#else
        public AACSMediaKeyBlock(MediaKeyBlock? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public AACSMediaKeyBlock(MediaKeyBlock model, Stream data)
#else
        public AACSMediaKeyBlock(MediaKeyBlock? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an AACS media key block from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An AACS media key block wrapper on success, null on failure</returns>
#if NET48
        public static AACSMediaKeyBlock Create(byte[] data, int offset)
#else
        public static AACSMediaKeyBlock? Create(byte[]? data, int offset)
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
        /// Create an AACS media key block from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>An AACS media key block wrapper on success, null on failure</returns>
#if NET48
        public static AACSMediaKeyBlock Create(Stream data)
#else
        public static AACSMediaKeyBlock? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var mediaKeyBlock = new SabreTools.Serialization.Streams.AACS().Deserialize(data);
            if (mediaKeyBlock == null)
                return null;

            try
            {
                return new AACSMediaKeyBlock(mediaKeyBlock, data);
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

            builder.AppendLine("AACS Media Key Block Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            PrintRecords(builder, Records);

            return builder;
        }

#if NET48
        private static void PrintRecords(StringBuilder builder, Record[] records)
#else
        private static void PrintRecords(StringBuilder builder, Record?[]? records)
#endif
        {
            builder.AppendLine("  Records Information:");
            builder.AppendLine("  -------------------------");
            if (records == null || records.Length == 0)
            {
                builder.AppendLine("  No records");
            }
            else
            {
                for (int i = 0; i < records.Length; i++)
                {
                    var record = records[i];
                    PrintRecord(builder, record, i);
                }
            }
            builder.AppendLine();
        }

#if NET48
        private static void PrintRecord(StringBuilder builder, Record record, int index)
#else
        private static void PrintRecord(StringBuilder builder, Record? record, int index)
#endif
        {
            builder.AppendLine($"  Record Entry {index}");
            if (record == null)
            {
                builder.AppendLine("    [NULL]");
                return;
            }

            builder.AppendLine($"    Record type: {record.RecordType} (0x{record.RecordType:X})");
            builder.AppendLine($"    Record length: {record.RecordLength} (0x{record.RecordLength:X})");

            switch (record)
            {
                case EndOfMediaKeyBlockRecord eomkb:
                    PrintRecord(builder, eomkb);
                    break;
                case ExplicitSubsetDifferenceRecord esd:
                    PrintRecord(builder, esd);
                    break;
                case MediaKeyDataRecord mkd:
                    PrintRecord(builder, mkd);
                    break;
                case SubsetDifferenceIndexRecord sdi:
                    PrintRecord(builder, sdi);
                    break;
                case TypeAndVersionRecord tav:
                    PrintRecord(builder, tav);
                    break;
                case DriveRevocationListRecord drl:
                    PrintRecord(builder, drl);
                    break;
                case HostRevocationListRecord hrl:
                    PrintRecord(builder, hrl);
                    break;
                case VerifyMediaKeyRecord vmk:
                    PrintRecord(builder, vmk);
                    break;
                case CopyrightRecord c:
                    PrintRecord(builder, c);
                    break;
            }
        }

#if NET48
        private static void PrintRecord(StringBuilder builder, EndOfMediaKeyBlockRecord record)
#else
        private static void PrintRecord(StringBuilder builder, EndOfMediaKeyBlockRecord record)
#endif
        {
            if (record == null)
                return;

            builder.AppendLine($"    Signature data: {(record.SignatureData == null ? "[NULL]" : BitConverter.ToString(record.SignatureData).Replace('-', ' '))}");
        }

#if NET48
        private static void PrintRecord(StringBuilder builder, ExplicitSubsetDifferenceRecord record)
#else
        private static void PrintRecord(StringBuilder builder, ExplicitSubsetDifferenceRecord? record)
#endif
        {
            if (record == null)
                return;

            builder.AppendLine($"    Subset Differences:");
            builder.AppendLine("    -------------------------");
            if (record?.SubsetDifferences == null || record.SubsetDifferences.Length == 0)
            {
                builder.AppendLine($"    No subset differences");
                return;
            }

            for (int j = 0; j < record.SubsetDifferences.Length; j++)
            {
                var sd = record.SubsetDifferences[j];
                builder.AppendLine($"    Subset Difference {j}");
                if (sd == null)
                {
                    builder.AppendLine("      [NULL]");
                }
                else
                {
                    builder.AppendLine($"      Mask: {sd.Mask} (0x{sd.Mask:X})");
                    builder.AppendLine($"      Number: {sd.Number} (0x{sd.Number:X})");
                }
            }
        }

#if NET48
        private static void PrintRecord(StringBuilder builder, MediaKeyDataRecord record)
#else
        private static void PrintRecord(StringBuilder builder, MediaKeyDataRecord? record)
#endif
        {
            if (record == null)
                return;

            builder.AppendLine($"    Media Keys:");
            builder.AppendLine("    -------------------------");
            if (record?.MediaKeyData == null || record.MediaKeyData.Length == 0)
            {
                builder.AppendLine($"    No media keys");
                return;
            }

            for (int j = 0; j < record.MediaKeyData.Length; j++)
            {
                var mk = record.MediaKeyData[j];
                builder.AppendLine($"      Media key {j}: {(mk == null ? "[NULL]" : BitConverter.ToString(mk).Replace('-', ' '))}");
            }
        }

#if NET48
        private static void PrintRecord(StringBuilder builder, SubsetDifferenceIndexRecord record)
#else
        private static void PrintRecord(StringBuilder builder, SubsetDifferenceIndexRecord? record)
#endif
        {
            if (record == null)
                return;

            builder.AppendLine($"    Span: {record.Span} (0x{record.Span:X})");
            builder.AppendLine($"    Offsets:");
            builder.AppendLine("    -------------------------");
            if (record.Offsets == null || record.Offsets.Length == 0)
            {
                builder.AppendLine($"    No offsets");
                return;
            }

            for (int j = 0; j < record.Offsets.Length; j++)
            {
                var offset = record.Offsets[j];
                builder.AppendLine($"      Offset {j}: {offset} (0x{offset:X})");
            }
        }

#if NET48
        private static void PrintRecord(StringBuilder builder, TypeAndVersionRecord record)
#else
        private static void PrintRecord(StringBuilder builder, TypeAndVersionRecord? record)
#endif
        {
            if (record == null)
                return;

            builder.AppendLine($"    Media key block type: {record.MediaKeyBlockType} (0x{record.MediaKeyBlockType:X})");
            builder.AppendLine($"    Version number: {record.VersionNumber} (0x{record.VersionNumber:X})");
        }

#if NET48
        private static void PrintRecord(StringBuilder builder, DriveRevocationListRecord record)
#else
        private static void PrintRecord(StringBuilder builder, DriveRevocationListRecord? record)
#endif
        {
            if (record == null)
                return;

            builder.AppendLine($"    Total number of entries: {record.TotalNumberOfEntries} (0x{record.TotalNumberOfEntries:X})");
            builder.AppendLine($"    Signature Blocks:");
            builder.AppendLine("    -------------------------");
            if (record.SignatureBlocks == null || record.SignatureBlocks.Length == 0)
            {
                builder.AppendLine($"    No signature blocks");
                return;
            }

            for (int j = 0; j < record.SignatureBlocks.Length; j++)
            {
                var block = record.SignatureBlocks[j];
                builder.AppendLine($"    Signature Block {j}");
                if (block == null)
                {
                    builder.AppendLine("      [NULL]");
                    continue;
                }

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
                        if (ef == null)
                        {
                            builder.AppendLine("        [NULL]");
                        }
                        else
                        {
                            builder.AppendLine($"        Range: {ef.Range} (0x{ef.Range:X})");
                            builder.AppendLine($"        Drive ID: {(ef.DriveID == null ? "[NULL]" : BitConverter.ToString(ef.DriveID).Replace('-', ' '))}");
                        }
                    }
                }
            }
        }

#if NET48
        private static void PrintRecord(StringBuilder builder, HostRevocationListRecord record)
#else
        private static void PrintRecord(StringBuilder builder, HostRevocationListRecord? record)
#endif
        {
            if (record == null)
                return;

            builder.AppendLine($"    Total number of entries: {record.TotalNumberOfEntries} (0x{record.TotalNumberOfEntries:X})");
            builder.AppendLine($"    Signature Blocks:");
            builder.AppendLine("    -------------------------");
            if (record.SignatureBlocks == null || record.SignatureBlocks.Length == 0)
            {
                builder.AppendLine($"    No signature blocks");
                return;
            }

            for (int j = 0; j < record.SignatureBlocks.Length; j++)
            {
                builder.AppendLine($"    Signature Block {j}");
                var block = record.SignatureBlocks[j];
                if (block == null)
                {
                    builder.AppendLine("      [NULL]");
                    continue;
                }

                builder.AppendLine($"      Number of entries: {block.NumberOfEntries}");
                builder.AppendLine($"      Entry Fields:");
                builder.AppendLine("      -------------------------");
                if (block.EntryFields == null || block.EntryFields.Length == 0)
                {
                    builder.AppendLine($"      No entry fields");
                    continue;
                }

                for (int k = 0; k < block.EntryFields.Length; k++)
                {
                    var ef = block.EntryFields[k];
                    builder.AppendLine($"      Entry {k}");
                    if (ef == null)
                    {
                        builder.AppendLine("        [NULL]");
                    }
                    else
                    {

                        builder.AppendLine($"        Range: {ef.Range} (0x{ef.Range:X})");
                        builder.AppendLine($"        Host ID: {(ef.HostID == null ? "[NULL]" : BitConverter.ToString(ef.HostID).Replace('-', ' '))}");
                    }
                }
            }
        }

#if NET48
        private static void PrintRecord(StringBuilder builder, VerifyMediaKeyRecord record)
#else
        private static void PrintRecord(StringBuilder builder, VerifyMediaKeyRecord? record)
#endif
        {
            if (record == null)
                return;

            builder.AppendLine($"    Ciphertext value: {(record.CiphertextValue == null ? "[NULL]" : BitConverter.ToString(record.CiphertextValue).Replace('-', ' '))}");
        }

#if NET48
        private static void PrintRecord(StringBuilder builder, CopyrightRecord record)
#else
        private static void PrintRecord(StringBuilder builder, CopyrightRecord? record)
#endif
        {
            if (record == null)
                return;

            builder.AppendLine($"    Copyright: {record.Copyright ?? "[NULL]"}");
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

#endif

        #endregion
    }
}