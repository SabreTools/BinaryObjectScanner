namespace BurnOutSharp.Models.N3DS
{
    /// <see href="https://www.3dbrew.org/wiki/NCCH#NCCH_Flags"/>
    public sealed class NCCHHeaderFlags
    {
        /// <summary>
        /// Reserved
        /// </summary>
        public byte Reserved0;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte Reserved1;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte Reserved2;

        /// <summary>
        /// Crypto Method: When this is non-zero, a NCCH crypto method using two keyslots is used.
        /// </summary>
        public CryptoMethod CryptoMethod;

        /// <summary>
        /// Content Platform: 1 = CTR, 2 = snake (New 3DS).
        /// </summary>
        public ContentPlatform ContentPlatform;

        /// <summary>
        /// Content Type Bit-masks: Data = 0x1, Executable = 0x2, SystemUpdate = 0x4, Manual = 0x8,
        /// Child = (0x4|0x8), Trial = 0x10. When 'Data' is set, but not 'Executable', NCCH is a CFA.
        /// Otherwise when 'Executable' is set, NCCH is a CXI.
        /// </summary>
        public ContentType MediaPlatformIndex;

        /// <summary>
        /// Content Unit Size i.e. u32 ContentUnitSize = 0x200*2^flags[6];
        /// </summary>
        public byte ContentUnitSize;

        /// <summary>
        /// Bit-masks: FixedCryptoKey = 0x1, NoMountRomFs = 0x2, NoCrypto = 0x4, using a new keyY
        /// generator = 0x20(starting with FIRM 9.6.0-X).
        /// </summary>
        public BitMasks BitMasks;
    }
}
