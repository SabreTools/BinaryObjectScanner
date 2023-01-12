namespace BurnOutSharp.Models.BDPlus
{
    /// <see href="https://github.com/mwgoldsmith/bdplus/blob/master/src/libbdplus/bdsvm/loader.c"/>
    public sealed class SVM
    {
        /// <summary>
        /// "BDSVM_CC"
        /// </summary>
        public string Signature;

        /// <summary>
        /// 5 bytes of unknown data
        /// </summary>
        public byte[] Unknown1;

        /// <summary>
        /// Version year
        /// </summary>
        public ushort Year;

        /// <summary>
        /// Version month
        /// </summary>
        public byte Month;

        /// <summary>
        /// Version day
        /// </summary>
        public byte Day;

        /// <summary>
        /// 4 bytes of unknown data
        /// </summary>
        public byte[] Unknown2;

        /// <summary>
        /// Length
        /// </summary>
        public uint Length;

        /// <summary>
        /// Length bytes of data
        /// </summary>
        public byte[] Data;
    }
}