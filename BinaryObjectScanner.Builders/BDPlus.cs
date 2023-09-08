using System.IO;
using System.Text;
using SabreTools.IO;
using SabreTools.Models.BDPlus;
using static SabreTools.Models.BDPlus.Constants;

namespace BinaryObjectScanner.Builders
{
    public class BDPlus
    {
        #region Byte Data

        /// <summary>
        /// Parse a byte array into a BD+ SVM
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled BD+ SVM on success, null on error</returns>
        public static SVM ParseSVM(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return ParseSVM(dataStream);
        }

        #endregion

        #region Stream Data

        /// <summary>
        /// Parse a Stream into an BD+ SVM
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled BD+ SVM on success, null on error</returns>
        public static SVM ParseSVM(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Try to parse the SVM
            return ParseSVMData(data);
        }

        /// <summary>
        /// Parse a Stream into an SVM
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SVM on success, null on error</returns>
        private static SVM ParseSVMData(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var svm = new SVM();

            byte[] signature = data.ReadBytes(8);
            svm.Signature = Encoding.ASCII.GetString(signature);
            if (svm.Signature != SignatureString)
                return null;

            svm.Unknown1 = data.ReadBytes(5);
            svm.Year = data.ReadUInt16BigEndian();
            svm.Month = data.ReadByteValue();
            if (svm.Month < 1 || svm.Month > 12)
                return null;

            svm.Day = data.ReadByteValue();
            if (svm.Day < 1 || svm.Day > 31)
                return null;

            svm.Unknown2 = data.ReadBytes(4);
            svm.Length = data.ReadUInt32();
            // if (svm.Length > 0)
            //     svm.Data = data.ReadBytes((int)svm.Length);

            return svm;
        }

        #endregion
    }
}
