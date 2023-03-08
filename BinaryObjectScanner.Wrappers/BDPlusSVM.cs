using System;
using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class BDPlusSVM : WrapperBase
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string Description => "BD+ SVM";

        #endregion

        #region Pass-Through Properties

        /// <inheritdoc cref="Models.BDPlus.SVM.Signature"/>
        public string Signature => _svm.Signature;

        /// <inheritdoc cref="Models.BDPlus.SVM.Unknown1"/>
        public byte[] Unknown1 => _svm.Unknown1;

        /// <inheritdoc cref="Models.BDPlus.SVM.Year"/>
        public ushort Year => _svm.Year;

        /// <inheritdoc cref="Models.BDPlus.SVM.Month"/>
        public byte Month => _svm.Month;

        /// <inheritdoc cref="Models.BDPlus.SVM.Day"/>
        public byte Day => _svm.Day;

        /// <inheritdoc cref="Models.BDPlus.SVM.Unknown2"/>
        public byte[] Unknown2 => _svm.Unknown2;

        /// <inheritdoc cref="Models.BDPlus.SVM.Length"/>
        public uint Length => _svm.Length;

        /// <inheritdoc cref="Models.BDPlus.SVM.Data"/>
        public byte[] Data => _svm.Data;

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the SVM
        /// </summary>
        private Models.BDPlus.SVM _svm;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private BDPlusSVM() { }

        /// <summary>
        /// Create a BD+ SVM from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A BD+ SVM wrapper on success, null on failure</returns>
        public static BDPlusSVM Create(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a BD+ SVM from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A BD+ SVM wrapper on success, null on failure</returns>
        public static BDPlusSVM Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var svm = Builders.BDPlus.ParseSVM(data);
            if (svm == null)
                return null;

            var wrapper = new BDPlusSVM
            {
                _svm = svm,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();
            
            builder.AppendLine("BD+ Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            PrintSVM(builder);

            return builder;
        }

        /// <summary>
        /// Print SVM information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintSVM(StringBuilder builder)
        {
            builder.AppendLine("  SVM Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Signature: {Signature}");
            builder.AppendLine($"  Unknown 1: {BitConverter.ToString(Unknown1).Replace('-', ' ')}");
            builder.AppendLine($"  Year: {Year} (0x{Year:X})");
            builder.AppendLine($"  Month: {Month} (0x{Month:X})");
            builder.AppendLine($"  Day: {Day} (0x{Day:X})");
            builder.AppendLine($"  Unknown 2: {BitConverter.ToString(Unknown2).Replace('-', ' ')}");
            builder.AppendLine($"  Length: {Length} (0x{Length:X})");
            //builder.AppendLine($"  Data: {BitConverter.ToString(Data ?? new byte[0]).Replace('-', ' ')}");
            builder.AppendLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_svm, _jsonSerializerOptions);

#endif

        #endregion
    }
}