namespace BinaryObjectScanner.Packer
{
    // TODO: Implement
    public class ClickteamInstallMaker
    {
        // All data stored in the overlay

        // Header is zlib-compressed. Samples seem to indicate that
        // there is a 19-byte "header" before the compressed header
        // data. 5 bytes before the compressed data appears to be a
        // 32-bit value that represents the decompressed header size.
        // It is followed by a single byte

        // Possible layout of pre-compressed header:
        // 0x00-0x03 - Unknown [77 77 67 54] (1416066935)
        // 0x04-0x07 - Unknown [29 48 35 14] (339036201)
        // 0x08-0x09 - Unknown [01 00] (1)
        // 0x0A-0x0D - Size of compressed data? [54 02 00 00] (596)
        //      This seems shorter than the expected 604
        // 0x0E-0x11 - Size of decompressed data [26 08 00 00] (2086)
        // 0x12      - Unknown [01] (1)

        // The decompressed header data has no easily-recognized
        // structures like offsets or sizes.

        // File entries are stored BZ2-compressed. Unable to find an
        // indicator of where the length of these compressed entries
        // can be found. This results in the entries all being read
        // at the same time and failing to extract as a result.

        // It is possible there is an 8-byte "prefix" before the entries
        // in the data itself, but no values have been correllated
        // to either the compressed or extracted sizes yet.
        // This prefix could also be a data trailer, as multiple values
        // have been found seemingly-appended to data extracted
        // from these bz2 entries.

        // Possible layout of pre-compressed entry:
        // 0x00-0x03 - Unknown [10 9D 01 00] (105744)
        // 0x04-0x07 - Size of compressed data? [DA 12 02 00] (135898)
        //      This seems shorter than the expected 137949
        //      What is in the 2051 bytes remaining?
        // 0x08      - Unknown [02] (2)

        // When manually extracted, each BZ2 block is a correct
        // set of compressed data. Without a notable filename map,
        // the extracted data is difficult to work with.
    }
}
