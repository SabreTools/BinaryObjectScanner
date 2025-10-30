using System;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Data;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// .iso file
    /// </summary>
    public abstract class ISO<T> : DetectableBase<T>
        where T : WrapperBase
    {
        /// <inheritdoc/>
        public ISO(T wrapper) : base(wrapper) { }

        #region Check Runners

        /// <summary>
        /// Handle a single file based on all content check implementations
        /// </summary>
        /// <param name="file">Name of the source file of the stream, for tracking</param>
        /// <param name="stream">Stream to scan the contents of</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Set of protections in file, empty on error</returns>
        protected IDictionary<IContentCheck, string> RunContentChecks(string? file, Stream stream, bool includeDebug)
        {
            // Create the output dictionary
            var protections = new CheckDictionary<IContentCheck>();

            // If we have an invalid file
            if (string.IsNullOrEmpty(file))
                return protections;
            else if (!File.Exists(file))
                return protections;

            // Read the file contents
            byte[] fileContent = [];
            try
            {
                // If the stream isn't seekable
                if (!stream.CanSeek)
                    return protections;

                stream.Seek(0, SeekOrigin.Begin);
                fileContent = stream.ReadBytes((int)stream.Length);
                if (fileContent == null)
                    return protections;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return protections;
            }

            // Iterate through all checks
            StaticChecks.ContentCheckClasses.IterateWithAction(checkClass =>
            {
                // Get the protection for the class, if possible
                var protection = checkClass.CheckContents(file!, fileContent, includeDebug);
                if (string.IsNullOrEmpty(protection))
                    return;

                protections.Append(checkClass, protection);
            });

            return protections;
        }

        /// <summary>
        /// Handle a single file based on all ISO check implementations
        /// </summary>
        /// <param name="file">Name of the source file of the ISO, for tracking</param>
        /// <param name="iso">ISO to scan</param>
        /// <param name="checks">Set of checks to use</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Set of protections in file, empty on error</returns>
        protected IDictionary<U, string> RunISOChecks<U>(string file, T iso, U[] checks, bool includeDebug)
            where U : IISOCheck<T>
        {
            // Create the output dictionary
            var protections = new CheckDictionary<U>();

            // Iterate through all checks
            checks.IterateWithAction(checkClass =>
            {
                // Get the protection for the class, if possible
                var protection = checkClass.CheckISO(file, iso, includeDebug);
                if (string.IsNullOrEmpty(protection))
                    return;

                protections.Append(checkClass, protection);
            });

            return protections;
        }

        #endregion
    }
}
