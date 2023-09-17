using System;
using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// BD+ SVM
    /// </summary>
    public class BDPlusSVM : IDetectable
    {
        /// <inheritdoc/>
#if NET48
        public string Detect(string file, bool includeDebug)
#else
        public string? Detect(string file, bool includeDebug)
#endif
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Detect(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
#if NET48
        public string Detect(Stream stream, string file, bool includeDebug)
#else
        public string? Detect(Stream stream, string file, bool includeDebug)
#endif
        {
            // If the BD+ file itself fails
            try
            {
                // Create the wrapper
                var svm = SabreTools.Serialization.Wrappers.BDPlusSVM.Create(stream);
                if (svm == null)
                    return null;

                // Format the date
                string date = $"{svm.Model.Year:0000}/{svm.Model.Month:00}/{svm.Model.Day:00}";

                // Return the formatted value
                return $"BD+ {date}";
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
            }

            return null;
        }
    }
}
