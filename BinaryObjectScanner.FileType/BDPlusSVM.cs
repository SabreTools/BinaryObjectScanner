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
        public string Detect(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Detect(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
        public string Detect(Stream stream, string file, bool includeDebug)
        {
            // If the BD+ file itself fails
            try
            {
                // Create the wrapper
                Wrappers.BDPlusSVM svm = Wrappers.BDPlusSVM.Create(stream);
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
