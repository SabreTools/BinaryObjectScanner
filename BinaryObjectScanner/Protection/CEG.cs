using System;
using System.IO;
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
    // TODO: DO .version CHECK TO AVOID steamclient.dll ISSUES
    // TODO: add version and string checks as debug-gated info?
    // TODO: add specific game output as debug-gated info?
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
            // The .version section should always be present. Combined with a different check for a 4 byte constant,
            // the string checks could be removed, although checking for strips still requires a string check anyways.

            string[] lookupValues = ["STEAM_GUIDD", "STEAM_MINSTANCE", "STEAM_SPLIT_GUID"];
            string[] returnedGuids = ["NOTHING", "NOTHING", "NOTHING"];
            int localOffset = 0;
            for (int i = 0; i < lookupValues.Length; i++)
            {
                var resourceDataList = exe.FindResourceByNamedType(lookupValues[i]);
                if (resourceDataList.Count <= 0)
                {
                    if (exe.ContainsSection(".version"))
                        return "CEG - Unable to parse resources";

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
            if (steamMInstance == zeroGUID)
            {
                // There's also one edge case with a manifest for depotID 246581 where they're all 0x00, but this one
                // might not active anyways.
                if (steamGuid == zeroGUID || steamSplitGuid == zeroGUID)
                {
                    if (steamSplitGuid != zeroGUID)
                    {
                        if (CEGDictionary.TryGetValue(steamSplitGuid, out string? name))
                        {
                            return $"CEG - Possibly tampered - {name}{ReportHelper(name)}";
                        }
                        return "CEG - Unknown manifest, please report to us on GitHub!";
                    }

                    if (CEGBackupDictionary.TryGetValue(timestamp, out string[]? lookupGuids))
                    {
                        string returnString = "CEG - Possibly tampered, potentially";
                        for (int i = 0; i < lookupGuids.Length; i++)
                        {
                            string lookupGuid = lookupGuids[i];
                            if (CEGDictionary.TryGetValue(lookupGuid, out string? lookedUpName))
                            {
                                returnString = $"{returnString} {lookedUpName}";
                                if (i + 1 == lookupGuids.Length)
                                    break;

                                returnString = $"{returnString} or";
                            }
                        }
                        return returnString;

                    }

                    return "CEG - Unknown manifest, please report to us on GitHub!";
                }
                else // Stripped
                {
                    if (CEGDictionary.TryGetValue(steamSplitGuid, out string? name))
                        return $"CEG - Stripped - {name}";
                    else
                        return "CEG - Unknown manifest, please report to us on GitHub!";
                }
            }
            else // Contains strips
            {
                if (CEGDictionary.TryGetValue(steamSplitGuid, out string? name))
                    return $"CEG - Contains Strips - {name}{ReportHelper(name)}";
                else
                    return "CEG - Unknown manifest, please report to us on GitHub!";
            }
        }

        public string? ReportHelper(string lookupGuid)
        {

#if NETSTANDARD2_0_OR_GREATER || NET21_OR_GREATER || NETCOREAPP
            if (!HaveStrips.Contains(lookupGuid))
                return "- Please report to us on GitHub!";

            return null;
#else
            return null;
#endif
        }
    }
}
