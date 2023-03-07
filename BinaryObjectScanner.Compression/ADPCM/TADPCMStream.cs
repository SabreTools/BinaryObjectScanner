namespace BinaryObjectScanner.Compression.ADPCM
{
    /// <summary>
    /// Helper class for writing output ADPCM data
    /// </summary>
    /// <see href="https://github.com/ladislav-zezula/StormLib/blob/master/src/adpcm/adpcm.cpp"/>
    public unsafe class TADPCMStream
    {
        private byte* pbBufferEnd;
        private byte* pbBuffer;

        public TADPCMStream(void* pvBuffer, int cbBuffer)
        {
            pbBufferEnd = (byte*)pvBuffer + cbBuffer;
            pbBuffer = (byte*)pvBuffer;
        }

        public bool ReadByteSample(ref byte ByteSample)
        {
            // Check if there is enough space in the buffer
            if (pbBuffer >= pbBufferEnd)
                return false;

            ByteSample = *pbBuffer++;
            return true;
        }

        public bool WriteByteSample(byte ByteSample)
        {
            // Check if there is enough space in the buffer
            if (pbBuffer >= pbBufferEnd)
                return false;

            *pbBuffer++ = ByteSample;
            return true;
        }

        public bool ReadWordSample(ref short OneSample)
        {
            // Check if we have enough space in the output buffer
            if ((int)(pbBufferEnd - pbBuffer) < sizeof(short))
                return false;

            // Write the sample
            OneSample = (short)(pbBuffer[0] + ((pbBuffer[1]) << 0x08));
            pbBuffer += sizeof(short);
            return true;
        }

        public bool WriteWordSample(short OneSample)
        {
            // Check if we have enough space in the output buffer
            if ((int)(pbBufferEnd - pbBuffer) < sizeof(short))
                return false;

            // Write the sample
            *pbBuffer++ = (byte)(OneSample & 0xFF);
            *pbBuffer++ = (byte)(OneSample >> 0x08);
            return true;
        }

        public int LengthProcessed(void* pvOutBuffer)
        {
            return (int)((byte*)pbBuffer - (byte*)pvOutBuffer);
        }
    }
}