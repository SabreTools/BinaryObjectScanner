using System;
using System.IO;

namespace BurnOutSharp.Wrappers
{
    public class AACSMediaKeyBlock : WrapperBase
    {
        #region Pass-Through Properties

        #region Records

        /// <inheritdoc cref="Models.AACS.MediaKeyBlock.Records"/>
        public Models.AACS.Record[] Records => _mediaKeyBlock.Records;

        #endregion

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the AACS media key block
        /// </summary>
        private Models.AACS.MediaKeyBlock _mediaKeyBlock;

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
        public override void Print()
        {
            Console.WriteLine("AACS Media Key Block Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            PrintRecords();
        }

        /// <summary>
        /// Print records information
        /// </summary>
        private void PrintRecords()
        {
            Console.WriteLine("  Records Information:");
            Console.WriteLine("  -------------------------");
            if (Records == null || Records.Length == 0)
            {
                Console.WriteLine("  No records");
            }
            else
            {
                for (int i = 0; i < Records.Length; i++)
                {
                    var record = Records[i];
                    Console.WriteLine($"  Record Entry {i}");
                    Console.WriteLine($"    Record type: {record.RecordType} (0x{record.RecordType:X})");
                    Console.WriteLine($"    Record length: {record.RecordLength} (0x{record.RecordLength:X})");

                    switch (record.RecordType)
                    {
                        case Models.AACS.RecordType.EndOfMediaKeyBlock:
                            var eomkb = record as Models.AACS.EndOfMediaKeyBlockRecord;
                            Console.WriteLine($"    Signature data: {BitConverter.ToString(eomkb.SignatureData ?? new byte[0]).Replace('-', ' ')}");
                            break;

                        case Models.AACS.RecordType.ExplicitSubsetDifference:
                            var esd = record as Models.AACS.ExplicitSubsetDifferenceRecord;
                            Console.WriteLine($"    Subset Differences:");
                            Console.WriteLine("    -------------------------");
                            if (esd.SubsetDifferences == null || esd.SubsetDifferences.Length == 0)
                            {
                                Console.WriteLine($"    No subset differences");
                            }
                            else
                            {
                                for (int j = 0; j < esd.SubsetDifferences.Length; j++)
                                {
                                    var sd = esd.SubsetDifferences[j];
                                    Console.WriteLine($"    Subset Difference {j}");
                                    Console.WriteLine($"      Mask: {sd.Mask} (0x{sd.Mask:X})");
                                    Console.WriteLine($"      Number: {sd.Number} (0x{sd.Number:X})");
                                }
                            }
                            break;

                        case Models.AACS.RecordType.MediaKeyData:
                            var mkd = record as Models.AACS.MediaKeyDataRecord;
                            Console.WriteLine($"    Media Keys:");
                            Console.WriteLine("    -------------------------");
                            if (mkd.MediaKeyData == null || mkd.MediaKeyData.Length == 0)
                            {
                                Console.WriteLine($"    No media keys");
                            }
                            else
                            {
                                for (int j = 0; j < mkd.MediaKeyData.Length; j++)
                                {
                                    var mk = mkd.MediaKeyData[j];
                                    Console.WriteLine($"      Media key {j}: {BitConverter.ToString(mk ?? new byte[0]).Replace('-', ' ')}");
                                }
                            }
                            break;

                        case Models.AACS.RecordType.SubsetDifferenceIndex:
                            var sdi = record as Models.AACS.SubsetDifferenceIndexRecord;
                            Console.WriteLine($"    Span: {sdi.Span} (0x{sdi.Span:X})");
                            Console.WriteLine($"    Offsets:");
                            Console.WriteLine("    -------------------------");
                            if (sdi.Offsets == null || sdi.Offsets.Length == 0)
                            {
                                Console.WriteLine($"    No offsets");
                            }
                            else
                            {
                                for (int j = 0; j < sdi.Offsets.Length; j++)
                                {
                                    var offset = sdi.Offsets[j];
                                    Console.WriteLine($"      Offset {j}: {offset} (0x{offset:X})");
                                }
                            }
                            break;

                        case Models.AACS.RecordType.TypeAndVersion:
                            var tav = record as Models.AACS.TypeAndVersionRecord;
                            Console.WriteLine($"    Media key block type: {tav.MediaKeyBlockType} (0x{tav.MediaKeyBlockType:X})");
                            Console.WriteLine($"    Version number: {tav.VersionNumber} (0x{tav.VersionNumber:X})");
                            break;

                        case Models.AACS.RecordType.DriveRevocationList:
                            var drl = record as Models.AACS.DriveRevocationListRecord;
                            Console.WriteLine($"    Total number of entries: {drl.TotalNumberOfEntries} (0x{drl.TotalNumberOfEntries:X})");
                            Console.WriteLine($"    Signature Blocks:");
                            Console.WriteLine("    -------------------------");
                            if (drl.SignatureBlocks == null || drl.SignatureBlocks.Length == 0)
                            {
                                Console.WriteLine($"    No signature blocks");
                            }
                            else
                            {
                                for (int j = 0; j < drl.SignatureBlocks.Length; j++)
                                {
                                    var block = drl.SignatureBlocks[j];
                                    Console.WriteLine($"    Signature Block {j}");
                                    Console.WriteLine($"      Number of entries: {block.NumberOfEntries}");
                                    Console.WriteLine($"      Entry Fields:");
                                    Console.WriteLine("      -------------------------");
                                    if (block.EntryFields == null || block.EntryFields.Length == 0)
                                    {
                                        Console.WriteLine($"      No entry fields");
                                    }
                                    else
                                    {
                                        for (int k = 0; k < block.EntryFields.Length; k++)
                                        {
                                            var ef = block.EntryFields[k];
                                            Console.WriteLine($"      Entry {k}");
                                            Console.WriteLine($"        Range: {ef.Range} (0x{ef.Range:X})");
                                            Console.WriteLine($"        Drive ID: {BitConverter.ToString(ef.DriveID ?? new byte[0]).Replace('-', ' ')}");
                                        }
                                    }
                                }
                            }
                            break;

                        case Models.AACS.RecordType.HostRevocationList:
                            var hrl = record as Models.AACS.HostRevocationListRecord;
                            Console.WriteLine($"    Total number of entries: {hrl.TotalNumberOfEntries} (0x{hrl.TotalNumberOfEntries:X})");
                            Console.WriteLine($"    Signature Blocks:");
                            Console.WriteLine("    -------------------------");
                            if (hrl.SignatureBlocks == null || hrl.SignatureBlocks.Length == 0)
                            {
                                Console.WriteLine($"    No signature blocks");
                            }
                            else
                            {
                                for (int j = 0; j < hrl.SignatureBlocks.Length; j++)
                                {
                                    var block = hrl.SignatureBlocks[j];
                                    Console.WriteLine($"    Signature Block {j}");
                                    Console.WriteLine($"      Number of entries: {block.NumberOfEntries}");
                                    Console.WriteLine($"      Entry Fields:");
                                    Console.WriteLine("      -------------------------");
                                    if (block.EntryFields == null || block.EntryFields.Length == 0)
                                    {
                                        Console.WriteLine($"      No entry fields");
                                    }
                                    else
                                    {
                                        for (int k = 0; k < block.EntryFields.Length; k++)
                                        {
                                            var ef = block.EntryFields[k];
                                            Console.WriteLine($"      Entry {k}");
                                            Console.WriteLine($"        Range: {ef.Range} (0x{ef.Range:X})");
                                            Console.WriteLine($"        Host ID: {BitConverter.ToString(ef.HostID ?? new byte[0]).Replace('-', ' ')}");
                                        }
                                    }
                                }
                            }
                            break;

                        case Models.AACS.RecordType.VerifyMediaKey:
                            var vmk = record as Models.AACS.VerifyMediaKeyRecord;
                            Console.WriteLine($"    Ciphertext value: {BitConverter.ToString(vmk.CiphertextValue ?? new byte[0]).Replace('-', ' ')}");
                            break;

                        case Models.AACS.RecordType.Copyright:
                            var c = record as Models.AACS.CopyrightRecord;
                            Console.WriteLine($"    Copyright: {c.Copyright ?? "[NULL]"}");
                            break;
                    }
                }
            }
            Console.WriteLine();
        }

        #endregion
    }
}