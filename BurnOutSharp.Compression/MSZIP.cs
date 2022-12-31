using System;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace BurnOutSharp.Compression
{
    public class MSZIP
    {
        #region Instance Variables

        /// <summary>
        /// Inflater to be shared between blocks
        /// </summary>
        private readonly Inflater _inflater = new Inflater(noHeader: true);

        #endregion

        #region Decompressiom

        /// <summary>
        /// Decompress MSZIP data block
        /// </summary>
        public byte[] DecompressMSZIPData(byte[] data, byte[] previousBlock = null)
        {
            if (previousBlock != null)
                _inflater.Reset();

            _inflater.SetInput(buffer: data, 2, data.Length - 2);
            byte[] outputData = new byte[128 * 1024];
            int read = _inflater.Inflate(outputData);
            return outputData.AsSpan(0, read).ToArray();
        }

        #endregion
    }
}