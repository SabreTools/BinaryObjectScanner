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
        public string? Detect(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Detect(fs, file, includeDebug);
        }

        /// <inheritdoc/>
        public string? Detect(Stream stream, string file, bool includeDebug)
        {
            // If the BD+ file itself fails
            try
            {
                // Create the wrapper
                var svm = SabreTools.Serialization.Wrappers.BDPlusSVM.Create(stream);
                if (svm == null)
                    return null;

                // Return the formatted value
                return $"BD+ {svm.Date}";
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
            }

            return null;
        }
    }
}
