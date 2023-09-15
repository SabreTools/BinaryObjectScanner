using System.Text;
using SabreTools.Models.AACS;

namespace BinaryObjectScanner.Printing
{
    public static class AACSMediaKeyBlock
    {
        public static void Print(StringBuilder builder, MediaKeyBlock mediaKeyBlock)
        {
            builder.AppendLine("AACS Media Key Block Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, mediaKeyBlock.Records);
        }

#if NET48
        private static void Print(StringBuilder builder, Record[] records)
#else
        private static void Print(StringBuilder builder, Record?[]? records)
#endif
        {
            builder.AppendLine("  Records Information:");
            builder.AppendLine("  -------------------------");
            if (records == null || records.Length == 0)
            {
                builder.AppendLine("  No records");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < records.Length; i++)
            {
                var record = records[i];
                Print(builder, record, i);
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, Record record, int index)
#else
        private static void Print(StringBuilder builder, Record? record, int index)
#endif
        {
            builder.AppendLine($"  Record Entry {index}");
            if (record == null)
            {
                builder.AppendLine("    [NULL]");
                return;
            }

            builder.AppendLine($"    Record type: {record.RecordType} (0x{record.RecordType:X})");
            builder.AppendLine(record.RecordLength, "    Record length");

            switch (record)
            {
                case EndOfMediaKeyBlockRecord eomkb:
                    Print(builder, eomkb);
                    break;
                case ExplicitSubsetDifferenceRecord esd:
                    Print(builder, esd);
                    break;
                case MediaKeyDataRecord mkd:
                    Print(builder, mkd);
                    break;
                case SubsetDifferenceIndexRecord sdi:
                    Print(builder, sdi);
                    break;
                case TypeAndVersionRecord tav:
                    Print(builder, tav);
                    break;
                case DriveRevocationListRecord drl:
                    Print(builder, drl);
                    break;
                case HostRevocationListRecord hrl:
                    Print(builder, hrl);
                    break;
                case VerifyMediaKeyRecord vmk:
                    Print(builder, vmk);
                    break;
                case CopyrightRecord c:
                    Print(builder, c);
                    break;
            }
        }

#if NET48
        private static void Print(StringBuilder builder, EndOfMediaKeyBlockRecord record)
#else
        private static void Print(StringBuilder builder, EndOfMediaKeyBlockRecord record)
#endif
        {
            if (record == null)
                return;

            builder.AppendLine(record.SignatureData, "    Signature data");
        }

#if NET48
        private static void Print(StringBuilder builder, ExplicitSubsetDifferenceRecord record)
#else
        private static void Print(StringBuilder builder, ExplicitSubsetDifferenceRecord? record)
#endif
        {
            if (record == null)
                return;

            builder.AppendLine("    Subset Differences:");
            builder.AppendLine("    -------------------------");
            if (record?.SubsetDifferences == null || record.SubsetDifferences.Length == 0)
            {
                builder.AppendLine("    No subset differences");
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
                    builder.AppendLine(sd.Mask, "      Mask");
                    builder.AppendLine(sd.Number, "      Number");
                }
            }
        }

#if NET48
        private static void Print(StringBuilder builder, MediaKeyDataRecord record)
#else
        private static void Print(StringBuilder builder, MediaKeyDataRecord? record)
#endif
        {
            if (record == null)
                return;

            builder.AppendLine("    Media Keys:");
            builder.AppendLine("    -------------------------");
            if (record?.MediaKeyData == null || record.MediaKeyData.Length == 0)
            {
                builder.AppendLine("    No media keys");
                return;
            }

            for (int j = 0; j < record.MediaKeyData.Length; j++)
            {
                var mk = record.MediaKeyData[j];
                builder.AppendLine(mk, $"      Media key {j}");
            }
        }

#if NET48
        private static void Print(StringBuilder builder, SubsetDifferenceIndexRecord record)
#else
        private static void Print(StringBuilder builder, SubsetDifferenceIndexRecord? record)
#endif
        {
            if (record == null)
                return;

            builder.AppendLine($"    Span: {record.Span} (0x{record.Span:X})");
            builder.AppendLine("    Offsets:");
            builder.AppendLine("    -------------------------");
            if (record.Offsets == null || record.Offsets.Length == 0)
            {
                builder.AppendLine("    No offsets");
                return;
            }

            for (int j = 0; j < record.Offsets.Length; j++)
            {
                var offset = record.Offsets[j];
                builder.AppendLine(offset, $"      Offset {j}");
            }
        }

#if NET48
        private static void Print(StringBuilder builder, TypeAndVersionRecord record)
#else
        private static void Print(StringBuilder builder, TypeAndVersionRecord? record)
#endif
        {
            if (record == null)
                return;

            builder.AppendLine($"    Media key block type: {record.MediaKeyBlockType} (0x{record.MediaKeyBlockType:X})");
            builder.AppendLine(record.VersionNumber, "    Version number");
        }

#if NET48
        private static void Print(StringBuilder builder, DriveRevocationListRecord record)
#else
        private static void Print(StringBuilder builder, DriveRevocationListRecord? record)
#endif
        {
            if (record == null)
                return;

            builder.AppendLine(record.TotalNumberOfEntries, "    Total number of entries");
            builder.AppendLine("    Signature Blocks:");
            builder.AppendLine("    -------------------------");
            if (record.SignatureBlocks == null || record.SignatureBlocks.Length == 0)
            {
                builder.AppendLine("    No signature blocks");
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

                builder.AppendLine(block.NumberOfEntries, "      Number of entries");
                builder.AppendLine("      Entry Fields:");
                builder.AppendLine("      -------------------------");
                if (block.EntryFields == null || block.EntryFields.Length == 0)
                {
                    builder.AppendLine("      No entry fields");
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
                            builder.AppendLine(ef.Range, "        Range");
                            builder.AppendLine(ef.DriveID, "        Drive ID");
                        }
                    }
                }
            }
        }

#if NET48
        private static void Print(StringBuilder builder, HostRevocationListRecord record)
#else
        private static void Print(StringBuilder builder, HostRevocationListRecord? record)
#endif
        {
            if (record == null)
                return;

            builder.AppendLine($"    Total number of entries: {record.TotalNumberOfEntries} (0x{record.TotalNumberOfEntries:X})");
            builder.AppendLine("    Signature Blocks:");
            builder.AppendLine("    -------------------------");
            if (record.SignatureBlocks == null || record.SignatureBlocks.Length == 0)
            {
                builder.AppendLine("    No signature blocks");
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

                builder.AppendLine(block.NumberOfEntries, "      Number of entries");
                builder.AppendLine("      Entry Fields:");
                builder.AppendLine("      -------------------------");
                if (block.EntryFields == null || block.EntryFields.Length == 0)
                {
                    builder.AppendLine("      No entry fields");
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

                        builder.AppendLine(ef.Range, "        Range");
                        builder.AppendLine(ef.HostID, "        Host ID");
                    }
                }
            }
        }

#if NET48
        private static void Print(StringBuilder builder, VerifyMediaKeyRecord record)
#else
        private static void Print(StringBuilder builder, VerifyMediaKeyRecord? record)
#endif
        {
            if (record == null)
                return;

            builder.AppendLine(record.CiphertextValue, "    Ciphertext value");
        }

#if NET48
        private static void Print(StringBuilder builder, CopyrightRecord record)
#else
        private static void Print(StringBuilder builder, CopyrightRecord? record)
#endif
        {
            if (record == null)
                return;

            builder.AppendLine(record.Copyright, "    Copyright");
        }
    }
}