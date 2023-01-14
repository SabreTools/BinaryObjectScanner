using System;
using System.Collections.Concurrent;
using System.IO;
using BurnOutSharp.Interfaces;
using static BurnOutSharp.Utilities.Dictionary;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// PlayJ audio file
    /// </summary>
    public class PLJ : IScannable
    {
        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
            try
            {
                byte[] magic = new byte[16];
                stream.Read(magic, 0, 16);

                if (Tools.Utilities.GetFileType(magic) == SupportedFileType.PLJ)
                {
                    AppendToDictionary(protections, file, "PlayJ Audio File");
                    return protections;
                }

                // Header Layout
                // ------------------------------------------------------------------------------
                // 0x00                                 Signature                       UInt32
                // 0x04                                 Version?                        UInt32
                // 0x08                                 UNKNOWN                         byte[34]
                // 0x2A                                 Track Title Length              UInt16
                // 0x2C                                 Track Title                     String
                // 0x2C+TTL                             Track Artist Length             UInt16
                // 0x2E+TTL                             Track Artist                    String
                // 0x2E+TTL+TAL                         Album Length                    UInt16
                // 0x30+TTL+TAL                         Album                           String
                // 0x30+TTL+TAL+AL                      Songwriter Length               UInt16
                // 0x32+TTL+TAL+AL                      Songwriter                      String
                // 0x32+TTL+TAL+AL+AAL                  Copyright Owner Length          UInt16
                // 0x34+TTL+TAL+AL+AAL                  Copyright Owner                 String
                // 0x34+TTL+TAL+AL+AAL+COL              Distributor Length              UInt16
                // 0x36+TTL+TAL+AL+AAL+COL              Distributor                     String
                //
                // The following samples also have a "Credits" section after the distributor, same format
                // - golden_empire.plj
                // - i_want_to_take_you_higher.plj
                //
                // 0x36+TTL+TAL+AL+AAL+COL+DL           Metadata? Length                UInt32
                // 0x38+TTL+TAL+AL+AAL+COL+DL           Metadata?                       byte[DL]
                //      Most samples hae repeating or semi-repeating values here, roughly UInt32-sized
                // 0x38+TTL+TAL+AL+AAL+COL+DL+ML        Version?                        UInt32
                //      Most of the samples have 0x00000000
                //      lady.plj has 0x00000002 and references "ad006376_5.dat" after, version 2?
                //      nature_soundscape_v.plj has 0x00C88028
            }
            catch (Exception ex)
            {
                if (scanner.IncludeDebug) Console.WriteLine(ex);
            }

            return null;
        }
    }
}
