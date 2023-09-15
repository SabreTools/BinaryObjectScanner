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
            builder.AppendLine(svm.Signature, "Signature");
            builder.AppendLine(svm.Unknown1, "Unknown 1");
            builder.AppendLine(svm.Year, "Year");
            builder.AppendLine(svm.Month, "Month");
            builder.AppendLine(svm.Day, "Day");
            builder.AppendLine(svm.Unknown2, "Unknown 2");
            builder.AppendLine(svm.Length, "Length");
            //builder.AppendLine(svm.Data, "Data");
            builder.AppendLine();
        }
    }
}