using System;
using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class BDPlusSVM : WrapperBase<SabreTools.Models.BDPlus.SVM>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "BD+ SVM";

        #endregion

        #region Pass-Through Properties

        /// <inheritdoc cref="Models.BDPlus.SVM.Signature"/>
        public string Signature => _model.Signature;

        /// <inheritdoc cref="Models.BDPlus.SVM.Unknown1"/>
        public byte[] Unknown1 => _model.Unknown1;

        /// <inheritdoc cref="Models.BDPlus.SVM.Year"/>
        public ushort Year => _model.Year;

        /// <inheritdoc cref="Models.BDPlus.SVM.Month"/>
        public byte Month => _model.Month;

        /// <inheritdoc cref="Models.BDPlus.SVM.Day"/>
        public byte Day => _model.Day;

        /// <inheritdoc cref="Models.BDPlus.SVM.Unknown2"/>
        public byte[] Unknown2 => _model.Unknown2;

        /// <inheritdoc cref="Models.BDPlus.SVM.Length"/>
        public uint Length => _model.Length;

        /// <inheritdoc cref="Models.BDPlus.SVM.Data"/>
        public byte[] Data => _model.Data;

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public BDPlusSVM(SabreTools.Models.BDPlus.SVM model, byte[] data, int offset)
#else
        public BDPlusSVM(SabreTools.Models.BDPlus.SVM? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public BDPlusSVM(SabreTools.Models.BDPlus.SVM model, Stream data)
#else
        public BDPlusSVM(SabreTools.Models.BDPlus.SVM? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

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

            var svm = new SabreTools.Serialization.Streams.BDPlus().Deserialize(data);
            if (svm == null)
                return null;

                try
            {
                return new BDPlusSVM(svm, data);
            }
            catch
            {
                return null;
            }
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
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

#endif

        #endregion
    }
}