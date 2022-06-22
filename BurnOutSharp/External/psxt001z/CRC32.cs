namespace psxt001z
{
    internal class CRC32
    {
        #region Constants

        private const uint CRC_POLY = 0xEDB88320;

        private const uint CRC_MASK = 0xD202EF8D;

        #endregion

        #region Properties

        protected uint[] Table { get; private set; } = new uint[256];

        public uint m_crc32 { get; set; }

        #endregion

        #region Constructor

        public CRC32()
        {
            for (uint i = 0; i < 256; i++)
            {
                uint r, j;
                for (r = i, j = 8; j != 0; j--)
                {
                    r = ((r & 1) != 0) ? (r >> 1) ^ CRC_POLY : r >> 1;
                }

                Table[i] = r;
            }

            m_crc32 = 0;
        }

        #endregion

        #region Functions

        public void ProcessCRC(byte[] pData, int pDataPtr, int nLen)
        {
            uint crc = m_crc32;
            while (nLen-- != 0)
            {
                crc = Table[(byte)(crc ^ pData[pDataPtr++])] ^ crc >> 8;
                crc ^= CRC_MASK;
            }

            m_crc32 = crc;
        }

        #endregion
    }
}
