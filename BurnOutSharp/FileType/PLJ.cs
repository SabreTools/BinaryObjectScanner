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

                // Header Layout (V1)
                // ------------------------------------------------------------------------------
                // 0x00                                 Signature                       UInt32
                // 0x04                                 Version                         UInt32 [0x00000000]
                // 0x08                                 Track ID                        UInt32 [Always 0xFFFFFFFF in CD titles]
                // 0x0C                                 Unknown Offset 1                UInt32
                // 0x10                                 Unknown Offset 2                UInt32
                // 0x14                                 Unknown Offset 3                UInt32
                // 0x18                                 Unknown (Initial play count?)   UInt32 [Always 0x00000001]
                // 0x1C                                 Unknown (Play count?)           UInt32 [Always 0x00000001 in download titles]
                // 0x20                                 Year                            UInt32 [0xFFFFFFFF if unset]
                // 0x24                                 Track Number                    Byte
                // 0x25                                 Subgenre                        Byte
                // 0x26                                 Duration in Seconds             UInt32
                // 0x2A                                 Track Length                    UInt16
                // 0x2C                                 Track                           String
                // 0x2C+TL                              Artist Length                   UInt16
                // 0x2E+TL                              Artist                          String
                // 0x2E+TL+TAL                          Album Length                    UInt16
                // 0x30+TL+TAL                          Album                           String
                // 0x30+TL+TAL+AL                       Writer Length                   UInt16
                // 0x32+TL+TAL+AL                       Writer                          String
                // 0x32+TL+TAL+AL+WL                    Publisher Length                UInt16
                // 0x34+TL+TAL+AL+WL                    Publisher                       String
                // 0x34+TL+TAL+AL+WL+PL                 Label Length                    UInt16
                // 0x36+TL+TAL+AL+WL+PL                 Label                           String
                //
                // The following samples also have a "Comments" section after the label, same format
                // - golden_empire.plj
                // - i_want_to_take_you_higher.plj
                //
                // Unknown Offset 1
                // ------------------------------------------------------------------------------
                // UO1                                  Unknown Block 1 Length          UInt32
                // UO1+2                                Unknown Block 1                 byte[UB1L]
                // 
                // Unknown Offset 2
                // ------------------------------------------------------------------------------
                // UO2                                  Unknown Value                   UInt32
                //      Most of the samples have 0x00000000
                //      nature_soundscape_v.plj has 0x00C88028
                //
                // Unknown Offset 3
                // ------------------------------------------------------------------------------
                // UO3                                  Unknown Data                    byte[??]
                // 

                // Header Layout (V2) [WIP]
                // ------------------------------------------------------------------------------
                // 0x00                                 Signature                       UInt32
                // 0x04                                 Version                         UInt32 [0x0000000A]
                // 0x08                                 UNKNOWN                         byte[36]
                // 0x2A                                 Track Length                    UInt16
                // 0x2C                                 Track                           String
                // 0x2C+TL                              Artist Length                   UInt16
                // 0x2E+TL                              Artist                          String
                // 0x2E+TL+TAL                          Album Length                    UInt16
                // 0x30+TL+TAL                          Album                           String
                // 0x30+TL+TAL+AL                       Writer Length                   UInt16
                // 0x32+TL+TAL+AL                       Writer                          String
                // 0x32+TL+TAL+AL+WL                    Publisher Length                UInt16
                // 0x34+TL+TAL+AL+WL                    Publisher                       String
                // 0x34+TL+TAL+AL+WL+PL                 Label Length                    UInt16
                // 0x36+TL+TAL+AL+WL+PL                 Label                           String
                
                // In the third block:
                //      lady.plj has 0x00000002 and references "ad006376_5.dat" after

                // Known Subgenre IDs (http://playj.com/static/subgenre_XX.html)
                // -----------------------------------------------
                // 2 - Blues/Folk/Country > Blues (Modern/Electric)
                // 3 - Blues/Folk/Country > Blues (Modern/Acoustic)
                // 4 - Blues/Folk/Country > Blues (Traditional)
                // 5 - Blues/Folk/Country > Folk (Traditional)
                // 6 - Blues/Folk/Country > Folk (Contemporary)
                // 7 - Blues/Folk/Country > Folk (Jazz)
                // TODO: When converting to enum, fill in the rest
            }
            catch (Exception ex)
            {
                if (scanner.IncludeDebug) Console.WriteLine(ex);
            }

            return null;
        }
    }
}
