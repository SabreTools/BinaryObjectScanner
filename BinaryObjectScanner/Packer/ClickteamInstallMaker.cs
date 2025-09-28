namespace BinaryObjectScanner.Packer
{
    // TODO: Implement
    public class ClickteamInstallMaker
    {
        // All data stored in the overlay
        // Header is zlib-compressed. Samples seem to indicate that
        // there is a 19-byte "header" before the compressed header
        // data.
        // The decompressed header data has no easily-recognized
        // structures like offsets or sizes.

        // File entries are stored BZ2-compressed. Unable to find an
        // indicator of where the length of these compressed entries
        // can be found. This results in the entries all being read
        // at the same time and failing to extract as a result.

        // When manually extracted, each BZ2 block is a correct
        // set of compressed data. Without a notable filename map,
        // the extracted data is difficult to work with.
    }
}
