namespace BurnOutSharp.Models.AACS
{
    public enum RecordType : byte
    {
        EndOfMediaKeyBlock = 0x02,
        ExplicitSubsetDifference = 0x04,
        MediaKeyData = 0x05,
        SubsetDifferenceIndex = 0x07,
        TypeAndVersion = 0x10,
        VerifyMediaKey = 0x81,
    }
}