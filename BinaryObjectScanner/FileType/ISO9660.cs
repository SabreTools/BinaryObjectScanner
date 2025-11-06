using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
                = RunISOChecks(file, StaticChecks.ISO9660CheckClasses, includeDebug);
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
            int index = 0;
            for (int i = 0; i < bytes.Length; ++i) {
                if (bytes[i] == containedZeroes[index])
                {
                    if (++index >= containedZeroes.Length)
                        return false; 
                }
                else
                    index = 0;
            }
            
            // Checks if there are strings in the data
            // TODO: is this too dangerous, or too faulty?
            // Currently-found worst cases:
            // "Y:1BY:1BC" in Redump ID 23339
            var strings = bytes.ReadStringsWithEncoding(charLimit: 7, Encoding.ASCII);
            Regex rgx = new Regex("[^a-zA-Z0-9 -'!?,.]");
            foreach (string str in strings)
            {
                if (rgx.Replace(str, "").Length > 7)
                    return false;
            }
            
            return true;
        }
        
        // TODO: can these 2 "noteworthy" functions be cached?
        // Checks whether the Application Use data is "noteworthy" enough to be worth checking for protection.
        public static bool NoteworthyApplicationUse(PrimaryVolumeDescriptor pvd)
        {
            int offset = 0;
            var applicationUse = pvd.ApplicationUse;
            var noteworthyApplicationUse = true;
            if (Array.TrueForAll(applicationUse, b => b == 0x00))
                noteworthyApplicationUse = false;
            string? potentialAppUseString = applicationUse.ReadNullTerminatedAnsiString(ref offset);
            if (potentialAppUseString != null && potentialAppUseString.Length > 0) // Some image authoring programs add a starting string to AU data
            {
                if (potentialAppUseString.StartsWith("ImgBurn"))
                    noteworthyApplicationUse = false;
                else if (potentialAppUseString.StartsWith("ULTRAISO"))
                    noteworthyApplicationUse = false;
                else if (potentialAppUseString.StartsWith("Rimage"))
                    noteworthyApplicationUse = false;
                else if (Array.TrueForAll(Encoding.ASCII.GetBytes(potentialAppUseString), b => b == 0x20))
                    noteworthyApplicationUse = false;
                // TODO: Unhandled "norb" mastering that puts stuff everywhere, inconsistently. See RID 103641
                // More things will have to go here as more disc authoring softwares are found that do this.
                // Redump ID 24478 has a bunch of 0x20 with norb in the middle, some discs have 0x20 that ends in a "/"
                // character. If these are found to be causing issues they can be added.
            }
            
            offset = 141;
            potentialAppUseString = applicationUse.ReadNullTerminatedAnsiString(ref offset);
            if (potentialAppUseString == "CD-XA001") 
                    noteworthyApplicationUse = false;
            
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