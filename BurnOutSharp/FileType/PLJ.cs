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
                // 0x0C                                 Metadata? Offset                UInt32
                // 0x10                                 Unknown Offset                  UInt32
                // 0x14                                 Unknown Offset                  UInt32
                // 0x18                                 Unknown                         UInt32 [Always 0x00000001]
                // 0x1C                                 Unknown                         UInt32 [Always 0x00000001 in download titles]
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
                // The following samples also have a "Comments" section after the distributor, same format
                // - golden_empire.plj
                // - i_want_to_take_you_higher.plj
                //
                // 0x36+TL+TAL+AL+WL+PL+LL              Metadata? Length                UInt32
                // 0x38+TL+TAL+AL+WL+PL+LL              Metadata?                       byte[ML]
                //      Most samples hae repeating or semi-repeating values here, roughly UInt32-sized
                // 0x38+TL+TAL+AL+WL+PL+LL+ML           Extras?                         UInt32
                //      Most of the samples have 0x00000000
                //      lady.plj has 0x00000002 and references "ad006376_5.dat" after
                //      nature_soundscape_v.plj has 0x00C88028

                // Header Layout (V2)
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

                // Known Genre IDs (http://www.playj.com/static/genreindex_XX.html)
                // -----------------------------------------------
                // 1 - Blues/Folk/Country
                // 5 - Jazz
                // 10 - Reggae
                // 12 - Classical
                // 13 - Electronic
                // 15 - Pop/Rock
                // 16 - World
                // 17 - Urban
                // 18 - Latin
                // 20 - Soundtrack/Other
                // 21 - New Age
                // 22 - Spiritual
                // 23 - Sway & Tech
                // 24 - Jam Bands
                // 25 - Comedy
                // 26 - Brazilian

                // Known Subgenre IDs (http://playj.com/static/subgenre_XX.html)
                // -----------------------------------------------
                // 2 - Blues/Folk/Country > Blues (Modern/Electric)
                // 3 - Blues/Folk/Country > Blues (Modern/Acoustic)
                // 4 - Blues/Folk/Country > Blues (Traditional)
                // 5 - Blues/Folk/Country > Folk (Traditional)
                // 6 - Blues/Folk/Country > Folk (Contemporary)
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
