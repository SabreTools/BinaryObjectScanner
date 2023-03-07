using System.Linq;
using System.Text;

namespace BinaryObjectScanner.ASN1
{
#pragma warning disable IDE0011
    
    /// <summary>
    /// Methods related to Object Identifiers (OID) and OID-IRI formatting
    /// </summary>
    public static partial class ObjectIdentifier
    {
        /// <summary>
        /// Parse an OID in separated-value notation into OID-IRI notation
        /// </summary>
        /// <param name="values">List of values to check against</param>
        /// <param name="index">Current index into the list</param>
        /// <returns>OID-IRI formatted string, if possible</returns>
        /// <see href="http://www.oid-info.com/index.htm"/>
        public static string ParseOIDToOIDIRINotation(ulong[] values)
        {
            // If we have an invalid set of values, we can't do anything
            if (values == null || values.Length == 0)
                return null;

            // Set the initial index
            int index = 0;

            // Get a string builder for the path
            var nameBuilder = new StringBuilder();

            // Try to parse the standard value
            string standard = ParseOIDToOIDIRINotation(values, ref index);
            if (standard == null)
                return null;

            // Add the standard value to the output
            nameBuilder.Append(standard);

            // If we have no more items
            if (index == values.Length)
                return nameBuilder.ToString();

            // Add trailing items as just values
            nameBuilder.Append("/");
            nameBuilder.Append(string.Join("/", values.Skip(index)));

            // Create and return the string
            return nameBuilder.ToString();
        }

        /// <summary>
        /// Parse an OID in separated-value notation into OID-IRI notation
        /// </summary>
        /// <param name="values">List of values to check against</param>
        /// <param name="index">Current index into the list</param>
        /// <returns>OID-IRI formatted string, if possible</returns>
        /// <see href="http://www.oid-info.com/index.htm"/>
        private static string ParseOIDToOIDIRINotation(ulong[] values, ref int index)
        {
            // If we have an invalid set of values, we can't do anything
            if (values == null || values.Length == 0)
                return null;

            // If we have an invalid index, we can't do anything
            if (index < 0 || index >= values.Length)
                return null;

            #region Start

            switch (values[index++])
            {
                case 0: goto oid_0;
                case 1: goto oid_1;
                case 2: goto oid_2;
                default: return $"/{values[index - 1]}";
            }

        #endregion

        // itu-t, ccitt, itu-r
        #region 0.*

        oid_0:

            if (index == values.Length) return "/ITU-T";
            switch (values[index++])
            {
                case 0: goto oid_0_0;
                case 2: return "/ITU-T/Administration";
                case 3: return "/ITU-T/Network-Operator";
                case 4: return "/ITU-T/Identified-Organization";
                case 5: return "/ITU-R/R-Recommendation";
                case 9: return "/ITU-T/Data";
                default: return $"/ITU-T/{values[index - 1]}";
            };

        // recommendation
        #region 0.0.*

        oid_0_0:

            if (index == values.Length) return "/ITU-T/Recommendation";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/A";
                case 2: return "/ITU-T/Recommendation/B";
                case 3: return "/ITU-T/Recommendation/C";
                case 4: return "/ITU-T/Recommendation/D";
                case 5: return "/ITU-T/Recommendation/E";
                case 6: return "/ITU-T/Recommendation/F";
                case 7: return "/ITU-T/Recommendation/G";
                case 8: return "/ITU-T/Recommendation/H";
                case 9: return "/ITU-T/Recommendation/I";
                case 10: return "/ITU-T/Recommendation/J";
                case 11: return "/ITU-T/Recommendation/K";
                case 12: return "/ITU-T/Recommendation/L";
                case 13: return "/ITU-T/Recommendation/M";
                case 14: return "/ITU-T/Recommendation/N";
                case 15: return "/ITU-T/Recommendation/O";
                case 16: return "/ITU-T/Recommendation/P";
                case 17: return "/ITU-T/Recommendation/Q";
                case 18: return "/ITU-T/Recommendation/R";
                case 19: return "/ITU-T/Recommendation/S";
                case 20: return "/ITU-T/Recommendation/T";
                case 21: return "/ITU-T/Recommendation/U";
                case 22: return "/ITU-T/Recommendation/V";
                case 24: return "/ITU-T/Recommendation/X";
                case 25: return "/ITU-T/Recommendation/Y";
                case 26: return "/ITU-T/Recommendation/Z";
                default: return $"/ITU-T/Recommendation/{values[index - 1]}";
            }

        #endregion

        #endregion

        // iso
        #region 1.*

        oid_1:

            if (index == values.Length) return "/ISO";
            switch (values[index++])
            {
                case 0: return "/ISO/Standard";
                case 1: return "/ISO/Registration-Authority";
                case 2: goto oid_1_2;
                case 3: return "/ISO/Identified-Organization";
                default: return $"/ISO/{values[index - 1]}";
            }

        // member-body
        #region 1.2.*

        oid_1_2:

            if (index == values.Length) return "/ISO/Member-Body";
            switch (values[index++])
            {
                case 36: return "/ISO/Member-Body/AU";
                case 40: return "/ISO/Member-Body/AT";
                case 56: return "/ISO/Member-Body/BE";
                case 124: return "/ISO/Member-Body/CA";
                case 156: return "/ISO/Member-Body/CN";
                case 203: return "/ISO/Member-Body/CZ";
                case 208: return "/ISO/Member-Body/DK";
                case 246: return "/ISO/Member-Body/FI";
                case 250: return "/ISO/Member-Body/FR";
                case 276: return "/ISO/Member-Body/DE";
                case 300: return "/ISO/Member-Body/GR";
                case 344: return "/ISO/Member-Body/HK";
                case 372: return "/ISO/Member-Body/IE";
                case 392: return "/ISO/Member-Body/JP";
                case 398: return "/ISO/Member-Body/KZ";
                case 410: return "/ISO/Member-Body/KR";
                case 498: return "/ISO/Member-Body/MD";
                case 528: return "/ISO/Member-Body/NL";
                case 566: return "/ISO/Member-Body/NG";
                case 578: return "/ISO/Member-Body/NO";
                case 616: return "/ISO/Member-Body/PL";
                case 643: return "/ISO/Member-Body/RU";
                case 702: return "/ISO/Member-Body/SG";
                case 752: return "/ISO/Member-Body/SE";
                case 804: return "/ISO/Member-Body/UA";
                case 826: return "/ISO/Member-Body/GB";
                case 840: return "/ISO/Member-Body/US";
                default: return $"/ISO/Member-Body/{values[index - 1]}";
            }

        #endregion

        #endregion

        // joint-iso-itu-t, joint-iso-ccitt
        #region 2.*

        oid_2:

            if (index == values.Length) return "/Joint-ISO-ITU-T";
            switch (values[index++])
            {
                case 1: return "/ASN.1";
                case 16: goto oid_2_16;
                case 17: return "/Joint-ISO-ITU-T/Registration-Procedures";
                case 23: return "/Joint-ISO-ITU-T/International-Organizations";
                case 25: goto oid_2_25;
                case 27: return "/Tag-Based";
                case 28: return "/Joint-ISO-ITU-T/ITS";
                case 41: return "/BIP";
                case 42: goto oid_2_42;
                case 48: goto oid_2_48;
                case 49: goto oid_2_49;
                case 50: return "/OIDResolutionSystem";
                case 51: return "/GS1";
                case 52: return "/Joint-ISO-ITU-T/UAV";
                case 999: return "/Joint-ISO-ITU-T/Example";
                default: return $"/Joint-ISO-ITU-T/{values[index - 1]}";
            }

        // country
        #region 2.16.*

        oid_2_16:

            if (index == values.Length) return "/Country";
            switch (values[index++])
            {
                case 4: return "/Country/AF";
                case 8: return "/Country/AL";
                case 12: return "/Country/DZ";
                case 20: return "/Country/AD";
                case 24: return "/Country/AO";
                case 28: return "/Country/AG";
                case 31: return "/Country/AZ";
                case 32: return "/Country/AR";
                case 36: return "/Country/AU";
                case 40: return "/Country/AT";
                case 44: return "/Country/BS";
                case 48: return "/Country/BH";
                case 50: return "/Country/BD";
                case 51: return "/Country/AM";
                case 52: return "/Country/BB";
                case 56: return "/Country/BE";
                case 60: return "/Country/BM";
                case 64: return "/Country/BT";
                case 68: return "/Country/BO";
                case 70: return "/Country/BA";
                case 72: return "/Country/BW";
                case 76: return "/Country/BR";
                case 84: return "/Country/BZ";
                case 90: return "/Country/SB";
                case 96: return "/Country/BN";
                case 100: return "/Country/BG";
                case 104: return "/Country/MM";
                case 108: return "/Country/BI";
                case 112: return "/Country/BY";
                case 116: return "/Country/KH";
                case 120: return "/Country/CM";
                case 124: return "/Country/CA";
                case 132: return "/Country/CV";
                case 140: return "/Country/CF";
                case 144: return "/Country/LK";
                case 148: return "/Country/TD";
                case 152: return "/Country/CL";
                case 156: return "/Country/CN";
                case 158: return "/Country/TW";
                case 170: return "/Country/CO";
                case 174: return "/Country/KM";
                case 178: return "/Country/CG";
                case 180: return "/Country/CD";
                case 188: return "/Country/CR";
                case 191: return "/Country/HR";
                case 192: return "/Country/CU";
                case 196: return "/Country/CY";
                case 203: return "/Country/CZ";
                case 204: return "/Country/BJ";
                case 208: return "/Country/DK";
                case 212: return "/Country/DM";
                case 214: return "/Country/DO";
                case 218: return "/Country/EC";
                case 222: return "/Country/SV";
                case 226: return "/Country/GQ";
                case 231: return "/Country/ET";
                case 232: return "/Country/ER";
                case 233: return "/Country/EE";
                case 242: return "/Country/FJ";
                case 246: return "/Country/FI";
                case 250: return "/Country/FR";
                case 262: return "/Country/DJ";
                case 266: return "/Country/GA";
                case 268: return "/Country/GE";
                case 270: return "/Country/GM";
                case 275: return "/Country/PS";
                case 276: return "/Country/DE";
                case 288: return "/Country/GH";
                case 296: return "/Country/KI";
                case 300: return "/Country/GR";
                case 308: return "/Country/GD";
                case 320: return "/Country/GT";
                case 324: return "/Country/GN";
                case 328: return "/Country/GY";
                case 332: return "/Country/HT";
                case 336: return "/Country/VA";
                case 340: return "/Country/HN";
                case 344: return "/Country/HK";
                case 348: return "/Country/HU";
                case 352: return "/Country/IS";
                case 356: return "/Country/IN";
                case 360: return "/Country/ID";
                case 364: return "/Country/IR";
                case 368: return "/Country/IQ";
                case 372: return "/Country/IE";
                case 376: return "/Country/IL";
                case 380: return "/Country/IT";
                case 384: return "/Country/CI";
                case 388: return "/Country/JM";
                case 392: return "/Country/JP";
                case 398: return "/Country/KZ";
                case 400: return "/Country/JO";
                case 404: return "/Country/KE";
                case 408: return "/Country/KP";
                case 410: return "/Country/KR";
                case 414: return "/Country/KW";
                case 417: return "/Country/KG";
                case 418: return "/Country/LA";
                case 422: return "/Country/LB";
                case 426: return "/Country/LS";
                case 428: return "/Country/LV";
                case 430: return "/Country/LR";
                case 434: return "/Country/LY";
                case 438: return "/Country/LI";
                case 440: return "/Country/LT";
                case 442: return "/Country/LU";
                case 450: return "/Country/MG";
                case 454: return "/Country/MW";
                case 458: return "/Country/MY";
                case 462: return "/Country/MV";
                case 466: return "/Country/ML";
                case 470: return "/Country/MT";
                case 478: return "/Country/MR";
                case 480: return "/Country/MU";
                case 484: return "/Country/MX";
                case 492: return "/Country/MC";
                case 496: return "/Country/MN";
                case 498: return "/Country/MD";
                case 499: return "/Country/ME";
                case 504: return "/Country/MA";
                case 508: return "/Country/MZ";
                case 512: return "/Country/OM";
                case 516: return "/Country/NA";
                case 520: return "/Country/NR";
                case 524: return "/Country/NP";
                case 528: return "/Country/NL";
                case 530: return "/Country/AN";
                case 548: return "/Country/VU";
                case 554: return "/Country/NZ";
                case 558: return "/Country/NI";
                case 562: return "/Country/NE";
                case 566: return "/Country/NG";
                case 578: return "/Country/NO";
                case 583: return "/Country/FM";
                case 584: return "/Country/MH";
                case 585: return "/Country/PW";
                case 586: return "/Country/PK";
                case 591: return "/Country/PA";
                case 598: return "/Country/PG";
                case 600: return "/Country/PY";
                case 604: return "/Country/PE";
                case 608: return "/Country/PH";
                case 616: return "/Country/PL";
                case 620: return "/Country/PT";
                case 624: return "/Country/GW";
                case 626: return "/Country/TL";
                case 634: return "/Country/QA";
                case 642: return "/Country/RO";
                case 643: return "/Country/RU";
                case 646: return "/Country/RW";
                case 659: return "/Country/KN";
                case 662: return "/Country/LC";
                case 670: return "/Country/VC";
                case 674: return "/Country/SM";
                case 678: return "/Country/ST";
                case 682: return "/Country/SA";
                case 686: return "/Country/SN";
                case 688: return "/Country/RS";
                case 690: return "/Country/SC";
                case 694: return "/Country/SL";
                case 702: return "/Country/SG";
                case 703: return "/Country/SK";
                case 704: return "/Country/VN";
                case 705: return "/Country/SI";
                case 706: return "/Country/SO";
                case 710: return "/Country/ZA";
                case 716: return "/Country/ZW";
                case 724: return "/Country/ES";
                case 728: return "/Country/SS";
                case 729: return "/Country/SD";
                case 740: return "/Country/SR";
                case 748: return "/Country/SZ";
                case 752: return "/Country/SE";
                case 756: return "/Country/CH";
                case 760: return "/Country/SY";
                case 762: return "/Country/TJ";
                case 764: return "/Country/TH";
                case 768: return "/Country/TG";
                case 776: return "/Country/TO";
                case 780: return "/Country/TT";
                case 784: return "/Country/AE";
                case 788: return "/Country/TN";
                case 792: return "/Country/TR";
                case 795: return "/Country/TM";
                case 798: return "/Country/TV";
                case 800: return "/Country/UG";
                case 804: return "/Country/UA";
                case 807: return "/Country/MK";
                case 818: return "/Country/EG";
                case 826: return "/Country/GB";
                case 834: return "/Country/TZ";
                case 840: return "/Country/US";
                case 854: return "/Country/BF";
                case 858: return "/Country/UY";
                case 860: return "/Country/UZ";
                case 862: return "/Country/VE";
                case 882: return "/Country/WS";
                case 887: return "/Country/YE";
                case 894: return "/Country/ZM";
                default: return $"/Country/{values[index - 1]}";
            }

        #endregion

        // uuid [TODO: Requires 128-bit values]
        #region 2.25.*

        oid_2_25:

            if (index == values.Length) return "/Joint-ISO-ITU-T/UUID";
            switch (values[index++])
            {
                case 0: return "/Joint-ISO-ITU-T/UUID/00000000-0000-0000-0000-000000000000";
                //case 288786655511405443130567505384701230: return "/Joint-ISO-ITU-T/UUID/00379e48-0a2b-1085-b288-0002a5d5fd2e";
                //case 987895962269883002155146617097157934: return "/Joint-ISO-ITU-T/UUID/00be4308-0c89-1085-8ea0-0002a5d5fd2e";
                //case 1858228783942312576083372383319475483: return "/Joint-ISO-ITU-T/UUID/0165e1c0-a655-11e0-95b8-0002a5d5c51b";
                //case 2474299330026746002885628159579243803: return "/Joint-ISO-ITU-T/UUID/01dc8860-25fb-11da-82b2-0002a5d5c51b";
                //case 3263645701162998421821186056373271854: return "/Joint-ISO-ITU-T/UUID/02748e28-08c4-1085-b21d-0002a5d5fd2e";
                //case 3325839809379844461264382260940242222: return "/Joint-ISO-ITU-T/UUID/02808890-0ad8-1085-9bdf-0002a5d5fd2e";
                // TODO: Left off at http://www.oid-info.com/get/2.25.3664154270495270126161055518190585115
                default: return $"/Joint-ISO-ITU-T/UUID/{values[index - 1]}";
            }

        #endregion

        // telebiometrics
        #region 2.42.*

        oid_2_42:

            if (index == values.Length) return "/Telebiometrics";
            switch (values[index++])
            {
                case 0: goto oid_2_42_0;
                case 1: goto oid_2_42_1;
                case 2: goto oid_2_42_2;
                case 3: goto oid_2_42_3;
                default: return $"/Telebiometrics/{values[index - 1]}";
            }

        // modules
        #region 2.42.0.*

        oid_2_42_0:

            if (index == values.Length) return "/Telebiometrics/Modules";
            switch (values[index++])
            {
                case 0: goto oid_2_42_0_0;
                default: return $"/Telebiometrics/Modules/{values[index - 1]}";
            }

        // main
        #region 2.42.0.0.*

        oid_2_42_0_0:

            if (index == values.Length) return "/Telebiometrics/Modules/Main_Module";
            switch (values[index++])
            {
                case 1: return "/Telebiometrics/Modules/Main_Module/Version1";
                default: return $"/Telebiometrics/Modules/Main_Module/{values[index - 1]}";
            }

        #endregion

        #endregion

        // tmm
        #region 2.42.1.*

        oid_2_42_1:

            if (index == values.Length) return "/Telebiometrics/TMM";
            switch (values[index++])
            {
                case 0: goto oid_2_42_1_0;
                case 1: goto oid_2_42_1_1;
                case 2: goto oid_2_42_1_2;
                case 3: goto oid_2_42_1_3;
                case 4: return "/Telebiometrics/TMM/Practitioners";
                default: return $"/Telebiometrics/TMM/{values[index - 1]}";
            }

        // modules
        #region 2.42.1.0.*

        oid_2_42_1_0:

            if (index == values.Length) return "/Telebiometrics/TMM/Modules";
            switch (values[index++])
            {
                case 0: goto oid_2_42_1_0_0;
                default: return $"/Telebiometrics/TMM/Modules/{values[index - 1]}";
            }

        // main
        #region 2.42.1.0.0.*

        oid_2_42_1_0_0:

            if (index == values.Length) return "/Telebiometrics/TMM/Modules/Main";
            switch (values[index++])
            {
                case 0: return "/Telebiometrics/TMM/Modules/Main/First_Version";
                default: return $"/Telebiometrics/TMM/Modules/Main/{values[index - 1]}";
            }

        #endregion

        #endregion

        // measures, metric
        #region 2.42.1.1.*

        oid_2_42_1_1:

            if (index == values.Length) return "/Telebiometrics/TMM/Measures";
            switch (values[index++])
            {
                case 1: goto oid_2_42_1_1_1;
                case 2: return "/Telebiometrics/TMM/Measures/Units";
                case 3: return "/Telebiometrics/TMM/Measures/Symbols";
                case 4: return "/Telebiometrics/TMM/Measures/Conditions";
                case 5: goto oid_2_42_1_1_5;
                default: return $"/Telebiometrics/TMM/Measures/{values[index - 1]}";
            }

        // quantities
        #region 2.42.1.1.1.*

        oid_2_42_1_1_1:

            if (index == values.Length) return "/Telebiometrics/TMM/Measures/Quantities";
            switch (values[index++])
            {
                case 1: return "/Telebiometrics/TMM/Measures/Quantities/Physics";
                case 2: return "/Telebiometrics/TMM/Measures/Quantities/Chemistry";
                case 3: return "/Telebiometrics/TMM/Measures/Quantities/Biology";
                case 4: return "/Telebiometrics/TMM/Measures/Quantities/Culturology";
                case 5: return "/Telebiometrics/TMM/Measures/Quantities/Psychology";
                default: return $"/Telebiometrics/TMM/Measures/Quantities/{values[index - 1]}";
            }

        #endregion

        // methods
        #region 2.42.1.1.5.*

        oid_2_42_1_1_5:

            if (index == values.Length) return "/Telebiometrics/TMM/Measures/Methods";
            switch (values[index++])
            {
                case 1: return "/Telebiometrics/TMM/Measures/Methods/Physics";
                case 2: return "/Telebiometrics/TMM/Measures/Methods/Chemistry";
                case 3: return "/Telebiometrics/TMM/Measures/Methods/Biology";
                case 4: return "/Telebiometrics/TMM/Measures/Methods/Culturology";
                case 5: return "/Telebiometrics/TMM/Measures/Methods/Psychology";
                default: return $"/Telebiometrics/TMM/Measures/Methods/{values[index - 1]}";
            }

        #endregion

        #endregion

        // fields-of-study, scientific
        #region 2.42.1.2.*

        oid_2_42_1_2:

            if (index == values.Length) return "/Telebiometrics/TMM/Fields_of_Study";
            switch (values[index++])
            {
                case 1: return "/Telebiometrics/TMM/Fields_of_Study/Physics";
                case 2: return "/Telebiometrics/TMM/Fields_of_Study/Chemistry";
                case 3: return "/Telebiometrics/TMM/Fields_of_Study/Biology";
                case 4: return "/Telebiometrics/TMM/Fields_of_Study/Culturology";
                case 5: return "/Telebiometrics/TMM/Fields_of_Study/Psychology";
                default: return $"/Telebiometrics/TMM/Fields_of_Study/{values[index - 1]}";
            }

        #endregion

        // modalities, sensory
        #region 2.42.1.3.*

        oid_2_42_1_3:

            if (index == values.Length) return "/Telebiometrics/TMM/Modalities";
            switch (values[index++])
            {
                case 1: return "/Telebiometrics/TMM/Modalities/Tango";
                case 2: return "/Telebiometrics/TMM/Modalities/Video";
                case 3: return "/Telebiometrics/TMM/Modalities/Audio";
                case 4: return "/Telebiometrics/TMM/Modalities/Chemo";
                case 5: return "/Telebiometrics/TMM/Modalities/Radio";
                case 6: return "/Telebiometrics/TMM/Modalities/Calor";
                case 7: return "/Telebiometrics/TMM/Modalities/Electro";
                default: return $"/Telebiometrics/TMM/Modalities/{values[index - 1]}";
            }

        #endregion

        #endregion

        // human-physiology
        #region 2.42.2.*

        oid_2_42_2:

            if (index == values.Length) return "/Telebiometrics/Human_Physiology";
            switch (values[index++])
            {
                case 0: goto oid_2_42_2_0;
                case 1: goto oid_2_42_2_1;
                case 2: return "/Telebiometrics/Human_Physiology/Symbol_Combinations";
                default: return $"/Telebiometrics/Human_Physiology/{values[index - 1]}";
            }

        // modules
        #region 2.42.2.0.*

        oid_2_42_2_0:

            if (index == values.Length) return "/Telebiometrics/Human_Physiology/Modules";
            switch (values[index++])
            {
                case 0: goto oid_2_42_2_0_0;
                default: return $"/Telebiometrics/Human_Physiology/Modules/{values[index - 1]}";
            }

        // main
        #region 2.42.2.0.0.*

        oid_2_42_2_0_0:

            if (index == values.Length) return "/Telebiometrics/Human_Physiology/Modules/Main_Module";
            switch (values[index++])
            {
                case 0: return "/Telebiometrics/Human_Physiology/Modules/Main_Module/First_Version";
                default: return $"/Telebiometrics/Human_Physiology/Modules/Main_Module/{values[index - 1]}";
            }

        #endregion

        #endregion

        // symbols
        #region 2.42.2.1.*

        oid_2_42_2_1:

            if (index == values.Length) return "/Telebiometrics/Human_Physiology/Symbols";
            switch (values[index++])
            {
                case 1: return "/Telebiometrics/Human_Physiology/Symbols/Tango_in";
                case 2: return "/Telebiometrics/Human_Physiology/Symbols/Video_in";
                case 3: return "/Telebiometrics/Human_Physiology/Symbols/Audio_in";
                case 4: return "/Telebiometrics/Human_Physiology/Symbols/Chemo_in";
                case 5: return "/Telebiometrics/Human_Physiology/Symbols/Radio_in";
                case 6: return "/Telebiometrics/Human_Physiology/Symbols/Calor_in";
                case 7: return "/Telebiometrics/Human_Physiology/Symbols/Tango_out";
                case 8: return "/Telebiometrics/Human_Physiology/Symbols/Video_out";
                case 9: return "/Telebiometrics/Human_Physiology/Symbols/Audio_out";
                case 10: return "/Telebiometrics/Human_Physiology/Symbols/Chemo_out";
                case 11: return "/Telebiometrics/Human_Physiology/Symbols/Radio_out";
                case 12: return "/Telebiometrics/Human_Physiology/Symbols/Calor_out";
                case 13: return "/Telebiometrics/Human_Physiology/Symbols/Safe";
                case 14: return "/Telebiometrics/Human_Physiology/Symbols/Threshold";
                default: return $"/Telebiometrics/Human_Physiology/Symbols/{values[index - 1]}";
            }

        #endregion

        #endregion

        // obj-cat, telehealth, e-health-protocol, th
        #region 2.42.3.*

        oid_2_42_3:

            if (index == values.Length) return "/Telebiometrics/E_Health_Protocol";
            switch (values[index++])
            {
                case 0: goto oid_2_42_3_0;
                case 1: return "/Telebiometrics/E_Health_Protocol/[Patient schemes]";
                case 2: return "/Telebiometrics/E_Health_Protocol/[Medical staff schemes]";
                case 3: return "/Telebiometrics/E_Health_Protocol/[Observer schemes]";
                case 4: return "/Telebiometrics/E_Health_Protocol/[Pharmaceutical schemes]";
                case 5: return "/Telebiometrics/E_Health_Protocol/[Laboratory schemes]";
                case 6: return "/Telebiometrics/E_Health_Protocol/[Drug manufacturer schemes]";
                case 7: return "/Telebiometrics/E_Health_Protocol/[Medical device schemes]";
                case 8: return "/Telebiometrics/E_Health_Protocol/[Medical software schemes]";
                case 9: return "/Telebiometrics/E_Health_Protocol/[Medical insurance schemes]";
                case 10: return "/Telebiometrics/E_Health_Protocol/[Medical record schemes]";
                default: return $"/Telebiometrics/E_Health_Protocol/{values[index - 1]}";
            }

        // obj-cat, telehealth, e-health-protocol, th
        #region 2.42.3.0.*

        oid_2_42_3_0:

            if (index == values.Length) return "/Telebiometrics/E_Health_Protocol/Modules";
            switch (values[index++])
            {
                case 0: goto oid_2_42_3_0_0;
                case 1: goto oid_2_42_3_0_1;
                case 2: goto oid_2_42_3_0_2;
                case 3: goto oid_2_42_3_0_3;
                case 4: goto oid_2_42_3_0_4;
                case 5: goto oid_2_42_3_0_5;
                default: return $"/Telebiometrics/E_Health_Protocol/Modules/{values[index - 1]}";
            }

        // identification
        #region 2.42.3.0.0.*

        oid_2_42_3_0_0:

            if (index == values.Length) return "/Telebiometrics/E_Health_Protocol/Modules/Identification";
            switch (values[index++])
            {
                case 1: return "/Telebiometrics/E_Health_Protocol/Modules/Identification/Version1";
                default: return $"/Telebiometrics/E_Health_Protocol/Modules/Identification/{values[index - 1]}";
            }

        #endregion

        // set-up
        #region 2.42.3.0.1.*

        oid_2_42_3_0_1:

            if (index == values.Length) return "/Telebiometrics/E_Health_Protocol/Modules/Setup";
            switch (values[index++])
            {
                case 1: return "/Telebiometrics/E_Health_Protocol/Modules/Setup/Version1";
                default: return $"/Telebiometrics/E_Health_Protocol/Modules/Setup/{values[index - 1]}";
            }

        #endregion

        // send-and-ack
        #region 2.42.3.0.2.*

        oid_2_42_3_0_2:

            if (index == values.Length) return "/Telebiometrics/E_Health_Protocol/Modules/Send-and-ack";
            switch (values[index++])
            {
                case 1: return "/Telebiometrics/E_Health_Protocol/Modules/Send-and-ack/Version1";
                default: return $"/Telebiometrics/E_Health_Protocol/Modules/Send-and-ack/{values[index - 1]}";
            }

        #endregion

        // command-response
        #region 2.42.3.0.3.*

        oid_2_42_3_0_3:

            if (index == values.Length) return "/Telebiometrics/E_Health_Protocol/Modules/Command-response";
            switch (values[index++])
            {
                case 1: return "/Telebiometrics/E_Health_Protocol/Modules/Command-response/Version1";
                default: return $"/Telebiometrics/E_Health_Protocol/Modules/Command-response/{values[index - 1]}";
            }

        #endregion

        // quantity-and-units
        #region 2.42.3.0.4.*

        oid_2_42_3_0_4:

            if (index == values.Length) return "/Telebiometrics/E_Health_Protocol/Modules/Quantities_And_Units";
            switch (values[index++])
            {
                case 1: return "/Telebiometrics/E_Health_Protocol/Modules/Quantities_And_Units/Version1";
                default: return $"/Telebiometrics/E_Health_Protocol/Modules/Quantities_And_Units/{values[index - 1]}";
            }

        #endregion

        // examples
        #region 2.42.3.0.5.*

        oid_2_42_3_0_5:

            if (index == values.Length) return "/Telebiometrics/E_Health_Protocol/Modules/Examples";
            switch (values[index++])
            {
                case 0: return "/Telebiometrics/E_Health_Protocol/Modules/Examples/Command_Response";
                case 1: return "/Telebiometrics/E_Health_Protocol/Modules/Examples/Data_Message";
                default: return $"/Telebiometrics/E_Health_Protocol/Modules/Examples/{values[index - 1]}";
            }

        #endregion

        #endregion

        #endregion

        #endregion

        // cybersecurity
        #region 2.48.*

        oid_2_48:

            if (index == values.Length) return "/Cybersecurity";
            switch (values[index++])
            {
                case 1: return "/Cybersecurity/Country";
                case 2: return "/Cybersecurity/International-Org";
                default: return $"/Cybersecurity/{values[index - 1]}";
            }

        #endregion

        // alerting
        #region 2.49.*

        oid_2_49:

            if (index == values.Length) return "/Alerting";
            switch (values[index++])
            {
                case 0: return "/Alerting/WMO";
                default: return $"/Alerting/{values[index - 1]}";
            }

            #endregion

            #endregion
        }
    }

#pragma warning restore IDE0011
}