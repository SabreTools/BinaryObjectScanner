using System;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Data;
using SabreTools.Data.Models.ISO9660;
using SabreTools.IO.Extensions;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// ISO9660
    /// </summary>
    public class ISO9660 : DiskImage<SabreTools.Serialization.Wrappers.ISO9660>
    {
        /// <inheritdoc/>
        public ISO9660(SabreTools.Serialization.Wrappers.ISO9660 wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override string? Detect(Stream stream, string file, bool includeDebug)
        {
            // Create the output dictionary
            var protections = new ProtectionDictionary();

            // Standard checks
            var subProtections
                = RunISOChecks(file, _wrapper, StaticChecks.ISO9660CheckClasses, includeDebug);
            protections.Append(file, subProtections.Values);

            // If there are no protections
            if (protections.Count == 0)
                return null;

            // Create the internal list
            var protectionList = new List<string>();
            foreach (string key in protections.Keys)
            {
                protectionList.AddRange(protections[key]);
            }

            return string.Join(";", [.. protectionList]);
        }

        // Checks whether the sequence of bytes is pure data (as in, not empty, not text, just high-entropy data)
        public static bool IsPureData(byte[] bytes)
        {
            // Check if there are three 0x00s in a row. Two seems like pushing it
            byte[] containedZeroes = {0x00, 0x00, 0x00};
            for (int i = 0, index = 0; i < bytes.Length; ++i)
                
                if (bytes[i] == containedZeroes[index]) {
                    if (++index >= containedZeroes.Length) {
                        return false; 
                    }
                }

            // Checks if there are strings in the data
            // TODO: is this too dangerous?
            if (bytes.ReadStringsFrom(charLimit: 3) != null)
                return false;
            
            return true;
        }
        
        // Checks whether the Application Use data is "noteworthy" enough to be worth checking for protection.
        public static bool NoteworthyApplicationUse(PrimaryVolumeDescriptor pvd)
        {
            int offset = 0;
            var applicationUse = pvd.ApplicationUse;
            var noteworthyApplicationUse = true;
            if (Array.TrueForAll(applicationUse, b => b == 0x00))
                noteworthyApplicationUse = false;
            string? potentialAppUseString = applicationUse.ReadNullTerminatedAnsiString(ref offset);
            if (potentialAppUseString != null) // Some image authoring programs add a starting string to AU data
            {
                if (potentialAppUseString.StartsWith("ImgBurn"))
                    noteworthyApplicationUse = false;
                else if (potentialAppUseString.StartsWith("ULTRAISO"))
                    noteworthyApplicationUse = false;
                // More things will have to go here as more disc authoring softwares are found that do this.
            }
            return noteworthyApplicationUse;
        }
        
        // Checks whether the Reserved 653 Bytes are "noteworthy" enough to be worth checking for protection.
        public static bool NoteworthyReserved653Bytes(PrimaryVolumeDescriptor pvd)
        {
            var reserved653Bytes = pvd.Reserved653Bytes;
            var noteworthyReserved653Bytes = true;
            if (Array.TrueForAll(reserved653Bytes, b => b == 0x00))
                noteworthyReserved653Bytes = false;
            // Unsure if more will be needed
            return noteworthyReserved653Bytes;
        }
    }
}