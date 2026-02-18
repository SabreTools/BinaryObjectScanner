using SabreTools.IO.Extensions;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Protection that used to be offered by Valve for games on Steam. By default, protected executables are "stripped"
    /// of varying 4KiB "strips", and these "strips" would only be downloaded when the user attempted to run the game on
    /// Steam.
    /// </summary>
    public partial class CEG : IExecutableCheck<PortableExecutable>
    {
        /// <summary>
        /// General info on the contents of a CEG executable, as pertaining to detection.
        /// CEG executables should have 3 resources in .rsrc: STEAM_GUIDD, STEAM_MINSTANCE, and STEAM_SPLIT_GUID.
        /// STEAM_GUIDD's meaning is unknown, as it remains largely consistent within a depot, but not the whole time,
        /// and the same one will often show up across a few different unrelated depots. It's required along with split
        /// GUID to request strips from Steam's servers, so it might be some kind of specific server identifier.
        /// STEAM_MINSTANCE identifies a personalized executable that contains strips (and is all 0x00 if the executable is stripped)
        /// STEAM_SPLIT_GUID identifies a specific build of that particular executable.
        /// Some cracked versions of games are known to 0x00 out the data in all 3, but still keep them there.
        /// There's also one edge case with a manifest for depotID 246581 where they're all 0x00, but this one might not
        /// be active anyways.
        ///
        /// All* CEG executables contain a .version section (virtual size 4, physical size 512) that begins with 4 bytes
        /// identifying some kind of version, and the rest is all 0x00. This is not parsed or output since it's
        /// specifically the version of the static library the executable was compiled against, which doesn't mean much,
        /// and since there are CEG v1/v2/v3 functions, which *do* have significance, this would likely just create confusion.
        /// Unfortunately, it is not feasible to parse v1/v2/v3 functions in BOS anyways, at least not at the moment.
        /// *The only exceptions to this are some (but not all) versions of Company of Heroes 2 (depot ID 231431), as
        /// well as Football Manager 2013 (depot IDs 207892, 216532, 216552, 216572, 216592, 216612, 220502, likely more)
        /// use StarForce (ProActive? or maybe just packing/encryption/antitamper?), and this seems to strip the .version
        /// section. Currently not checked if the version just wound up somewhere else.
        ///
        /// All CEG executables contain the strings "STEAMSTART" and "STEAM_DRM_IPC" in the .rdata section.
        /// Additionally, the .rdata section will either contain the string "This file has been stripped" if the executable
        /// is stripped, or contain the string "This file contains strips" if the executable contains strips.
        /// </summary>
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string[] lookupValues = ["STEAM_GUIDD", "STEAM_MINSTANCE", "STEAM_SPLIT_GUID"];
            string[] returnedGuids = ["NOTHING", "NOTHING", "NOTHING"];
            int localOffset = 0;
            for (int i = 0; i < lookupValues.Length; i++)
            {
                var resourceDataList = exe.FindResourceByNamedType(lookupValues[i]);
                if (resourceDataList.Count <= 0)
                {
                    // The .version section should always be present, besides the two aforementioned games that use
                    // StarForce on top of CEG. If an executable contains a .version section, it almost certainly has
                    // CEG.
                    if (exe.ContainsSection(".version"))
                        return "CEG - Unable to parse resources, please report to us on GitHub!";

                    return null;
                }

                var resourceData = resourceDataList[0];
                if (resourceData != null)
                    returnedGuids[i] = resourceData.ReadGuid(ref localOffset).ToString();

                localOffset = 0;
            }

            string steamGuid = returnedGuids[0].ToUpperInvariant();
            string steamMInstance = returnedGuids[1].ToUpperInvariant();
            string steamSplitGuid = returnedGuids[2].ToUpperInvariant();
            uint timestamp = exe.COFFFileHeader.TimeDateStamp;

            const string zeroGUID = "00000000-0000-0000-0000-000000000000";

            // If steam M instance is all 0x00, either the executable is stripped or it was tampered with.
            if (steamMInstance == zeroGUID)
            {
                // If the steam guid or the steam split guid are all 0x00, the executable is almost certainly
                // tampered. Tampered executables are also desired, though, which is why it's reported.
                // There's also one edge case with a manifest for depotID 246581 where they're all 0x00, but this one
                // might not active anyways, and is a pretty major edge case
                if (steamGuid == zeroGUID || steamSplitGuid == zeroGUID)
                {
                    // If for whatever reason only the steam guid is zeroed out, the Steam Split Guid can still be
                    // used to look up the executable.
                    if (steamSplitGuid != zeroGUID)
                    {
                        if (CEGDictionary.TryGetValue(steamSplitGuid, out string? tamperedName))
                        {
                            return $"CEG - Possibly tampered - {tamperedName}{ReportHelper(steamSplitGuid)}";
                        }

                        return "CEG - Unknown manifest, please report to us on GitHub!";
                    }

                    // If it's tampered and the steam split guid is zeroed out, the compilation timestamp needs to be
                    // used as a less precise fallback.
                    if (CEGBackupDictionary.TryGetValue(timestamp, out string[]? lookupGuids))
                    {
                        string returnString = "CEG - Possibly tampered, potentially";
                        for (int i = 0; i < lookupGuids.Length; i++)
                        {
                            string lookupGuid = lookupGuids[i];
                            if (CEGDictionary.TryGetValue(lookupGuid, out string? lookedUpName))
                            {
                                returnString = $"{returnString} {lookedUpName}{ReportHelper(lookupGuid)}";
                                if (i + 1 == lookupGuids.Length)
                                    break;

                                returnString = $"{returnString} or";
                            }
                        }

                        return returnString;
                    }

                    // If it still can't be matched to anything, it's an unknown manifest.
                    return "CEG - Unknown manifest, please report to us on GitHub!";
                }

                // If the steam M instance is all 0x00 but the other values are not, the executable is stripped.
                // Some tampered executables may still wind up here, but there's only so much that can be done to
                // compensate for tampering.
                if (CEGDictionary.TryGetValue(steamSplitGuid, out string? strippedName))
                    return $"CEG - Stripped - {strippedName}";
                else
                    return "CEG - Unknown manifest, please report to us on GitHub!";
            }

            // If the steam M instance is not all 0x00, it has strips.
            if (CEGDictionary.TryGetValue(steamSplitGuid, out string? name))
                return $"CEG - Contains Strips - {name}{ReportHelper(steamSplitGuid)}";
            else
                return "CEG - Unknown manifest, please report to us on GitHub!";
        }

        /// <summary>
        /// Checks if a given steam split guid is in the dictionary already to determine if reporting is desired.
        /// </summary>
        /// <param name="steamSplitGuid">Steam split GUID</param>
        /// <returns>A string about reporting on GitHub if not found, otherwise returns null.</returns>
        private string? ReportHelper(string steamSplitGuid)
        {
#if NETSTANDARD2_0_OR_GREATER || NET21_OR_GREATER || NETCOREAPP
            if (!HaveStrips.Contains(steamSplitGuid))
                return "- Please report to us on GitHub!";

            return null;
#else
            return null;
#endif
        }
    }
}
