namespace BurnOutSharp.Models.N3DS
{
    /// <see href="https://www.3dbrew.org/wiki/NCSD#Development_Card_Info_Header_Extension"/>
    public sealed class DevelopmentCardInfoHeader
    {
        /// <summary>
        /// InitialData
        /// </summary>
        public InitialData InitialData;

        /// <summary>
        /// CardDeviceReserved1
        /// </summary>
        public byte[] CardDeviceReserved1;

        /// <summary>
        /// TitleKey
        /// </summary>
        public byte[] TitleKey;

        /// <summary>
        /// CardDeviceReserved2
        /// </summary>
        public byte[] CardDeviceReserved2;

        /// <summary>
        /// TestData
        /// </summary>
        public TestData TestData;
    }
}
