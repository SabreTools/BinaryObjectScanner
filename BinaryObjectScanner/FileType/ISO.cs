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
