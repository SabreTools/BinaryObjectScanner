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
                = RunDiskImageChecks(file, StaticChecks.ISO9660CheckClasses, includeDebug);
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

        /// <summary>
        /// Checks whether the sequence of bytes is pure data (as in, not empty,
        /// not text, just high-entropy data)
        /// </summary>
        public static bool IsPureData(byte[] bytes)
        {
            // Check if there are three 0x00s in a row. Two seems like pushing it
            int index = 0;
            for (int i = 0; i < bytes.Length; ++i)
            {
                if (bytes[i] == 0x00)
                {
                    if (++index >= 3)
                        return false;
                }
                else
                {
                    index = 0;
                }
            }

            // Checks if there are strings in the data
            // TODO: is this too dangerous, or too faulty?
            // Currently-found worst cases:
            // "Y:1BY:1BC" in Redump ID 23339
            var strings = bytes.ReadStringsWithEncoding(charLimit: 7, Encoding.ASCII);
            var rgx = new Regex("[^a-zA-Z0-9 -'!,.]");
            foreach (string str in strings)
            {
                if (rgx.Replace(str, "").Length > 7)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks whether the Application Use data is "noteworthy" enough to be worth checking for protection.
        /// </summary>
        /// TODO: can these 2 "noteworthy" functions be cached?
        public static bool NoteworthyApplicationUse(PrimaryVolumeDescriptor pvd)
        {
            var applicationUse = pvd.ApplicationUse;
            if (Array.TrueForAll(applicationUse, b => b == 0x00))
                return false;

            int offset = 0;
            string? potentialAppUseString = applicationUse.ReadNullTerminatedAnsiString(ref offset);
            if (potentialAppUseString != null && potentialAppUseString.Length > 0) // Some image authoring programs add a starting string to AU data
            {
                if (potentialAppUseString.StartsWith("ImgBurn"))
                    return false;
                else if (potentialAppUseString.StartsWith("ULTRAISO"))
                    return false;
                else if (potentialAppUseString.StartsWith("Rimage"))
                    return false;
                else if (Array.TrueForAll(Encoding.ASCII.GetBytes(potentialAppUseString), b => b == 0x20))
                    return false;

                // TODO: Unhandled "norb" mastering that puts stuff everywhere, inconsistently. See RID 103641
                // More things will have to go here as more disc authoring softwares are found that do this.
                // Redump ID 24478 has a bunch of 0x20 with norb in the middle, some discs have 0x20 that ends in a "/"
                // character. If these are found to be causing issues they can be added.
            }

            // Seems to come from "FERGUS_MCNEILL - ISOCD 1.00 by Pantaray, Inc. USA -"
            offset = 1;
            potentialAppUseString = applicationUse.ReadNullTerminatedAnsiString(ref offset);
            if (potentialAppUseString == "FS")
                return false;
            
            offset = 141;
            potentialAppUseString = applicationUse.ReadNullTerminatedAnsiString(ref offset);
            if (potentialAppUseString == "CD-XA001")
                return false;

            return true;
        }

        /// <summary>
        /// Checks whether the Reserved 653 Bytes are "noteworthy" enough to be worth checking for protection.
        /// </summary>
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
