using System;
using System.Text;
using SabreTools.Models.BDPlus;

namespace BinaryObjectScanner.Printing
{
    public static class BDPlusSVM
    {
        public static void Print(StringBuilder builder, SVM svm)
        {
            builder.AppendLine("BD+ SVM Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine($"Signature: {svm.Signature}");
            builder.AppendLine($"Unknown 1: {(svm.Unknown1 == null ? "[NULL]" : BitConverter.ToString(svm.Unknown1).Replace('-', ' '))}");
            builder.AppendLine($"Year: {svm.Year} (0x{svm.Year:X})");
            builder.AppendLine($"Month: {svm.Month} (0x{svm.Month:X})");
            builder.AppendLine($"Day: {svm.Day} (0x{svm.Day:X})");
            builder.AppendLine($"Unknown 2: {(svm.Unknown2 == null ? "[NULL]" : BitConverter.ToString(svm.Unknown2).Replace('-', ' '))}");
            builder.AppendLine($"Length: {svm.Length} (0x{svm.Length:X})");
            //builder.AppendLine($"  Data: {(svm.Data == null ? "[NULL]" : BitConverter.ToString(svm.Data).Replace('-', ' '))}");
            builder.AppendLine();
        }
    }
}