namespace BinaryObjectScanner.Packer
{
    // TODO: Implement
    public class ApecSoft
    {
        // The overlay data starts with the string "CWS\7" [43 57 53 07]
        // Data is likely compressed with zlib based on some clues from
        // the executable content.

        // The executable has a ".CRT" section that may contain clues of
        // how to process. The section appears mostly empty. The sample
        // I have has a value of [80 95 46 00] (4625792). This does not
        // appear to be an offset, relative or otherwise.

        // On initial research, there is no obvious file table or directory
        // visible in any of the sections.
    }
}
