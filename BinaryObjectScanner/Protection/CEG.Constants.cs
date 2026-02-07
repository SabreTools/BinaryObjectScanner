using System.Collections.Generic;

namespace BinaryObjectScanner.Protection
{
    public partial class CEG
    {
        /// <summary>
        /// Dictionary of known CEG executables with compilation time as the key, and the string value as a combination
        /// of depot ID, manifest version, and filename.
        /// </summary>
        /// <remarks>Contained in a separate file since dictionary size will be very large.</remarks>
        private static readonly Dictionary<uint, string> CEGDictionary = new()
        {
            //{ 1298033762, "Shogun total war 2 demo test" },
            { 1264170802, "10681 (v0-1) - AvP_DX11.exe" },
            { 1264171199, "10681 (v0-1) - AvP.exe" },
            { 1264759825, "10681 (v2) - AvP.exe" },
            { 1264760178, "10681 (v2) - AvP_DX11.exe" },
            { 1265104029, "10681 (v3) - AvP.exe" },
            { 1265104767, "10681 (v3) - AvP_DX11.exe" },
            { 1265196934, "10681 (v4) - AvP.exe" },
            { 1265197111, "10681 (v4) - AvP_DX11.exe" },
            { 1265200884, "10681 (v5) - AvP_DX11.exe" },
            { 1265201120, "10681 (v5) - AvP.exe" },
            { 1265218568, "10681 (v6) - AvP.exe" },
            { 1265218748, "10681 (v6) - AvP_DX11.exe" },
            { 1265626467, "10681 (v7) - AvP_DX11.exe" },
            { 1265626733, "10681 (v7) - AvP.exe" },
            { 1265639234, "10681 (v8) - AvP.exe" },
            { 1265639459, "10681 (v8) - AvP_DX11.exe" },
            { 1265711093, "10681 (v9) - AvP_DX11.exe" },
            { 1265711343, "10681 (v9) - AvP.exe" },
            { 1265729320, "10681 (v10-11) - AvP.exe" },
            { 1265729583, "10681 (v10-11) - AvP_DX11.exe" },
            { 1265898579, "10681 (v12) - AvP.exe" },
            { 1265898814, "10681 (v12) - AvP_DX11.exe" },
            { 1265910002, "10681 (v13) - AvP_DX11.exe" },
            { 1265910166, "10681 (v13) - AvP.exe" },
            { 1265973550, "10681 (v14) - AvP.exe" },
            { 1265973915, "10681 (v14) - AvP_DX11.exe" },
            { 1265992000, "10681 (v15) - AvP_DX11.exe" },
            { 1265992237, "10681 (v15) - AvP.exe" },
            { 1266001507, "10681 (v16) - AvP_DX11.exe" },
            { 1266001711, "10681 (v16) - AvP.exe" },
            { 1266254226, "10681 (v17) - AvP_DX11.exe" },
            { 1266254418, "10681 (v17) - AvP.exe" },
            { 1266280922, "10681 (v18-19) - AvP.exe" },
            { 1266283570, "10681 (v18-19) - AvP_DX11.exe" },
            { 1266338581, "10681 (v20) - AvP_DX11.exe" },
            { 1266339302, "10681 (v20) - AvP.exe" },
            { 1266493077, "10681 (v21) - AvP.exe" },
            { 1266493275, "10681 (v21) - AvP_DX11.exe" },
            { 1266593945, "10681 (v22) - AvP.exe" },
            { 1266594188, "10681 (v22) - AvP_DX11.exe" },
            { 1266799462, "10681 (v23) - AvP.exe" },
            { 1266802085, "10681 (v23) - AvP_DX11.exe" },
            { 1266863479, "10681 (v24) - AvP.exe" },
            { 1266863657, "10681 (v24) - AvP_DX11.exe" },
            { 1266927862, "10681 (v25) - AvP_DX11.exe" },
            { 1266928166, "10681 (v25) - AvP.exe" },
        };
    }
}
