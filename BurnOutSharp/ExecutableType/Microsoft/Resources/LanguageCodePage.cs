using System;
using System.IO;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Resources
{
    /// <summary>
    /// If you use the Var structure to list the languages your application or DLL supports instead of using multiple version resources,
    /// use the Value member to contain an array of DWORD values indicating the language and code page combinations supported by this file.
    /// The low-order word of each DWORD must contain a Microsoft language identifier, and the high-order word must contain the IBM code page number.
    /// Either high-order or low-order word can be zero, indicating that the file is language or code page independent.
    /// If the Var structure is omitted, the file will be interpreted as both language and code page independent.
    /// </summary>
    internal class LanguageCodePage
    {
        /// <summary>
        /// The low-order word of each DWORD must contain a Microsoft language identifier
        /// </summary>
        public ushort MicrosoftLanguageIdentifier;

        /// <summary>
        /// The high-order word must contain the IBM code page number
        /// </summary>
        public ushort IBMCodePageNumber;

        public static LanguageCodePage Deserialize(Stream stream)
        {
            LanguageCodePage lcp = new LanguageCodePage();

            lcp.MicrosoftLanguageIdentifier = stream.ReadUInt16();
            lcp.IBMCodePageNumber = stream.ReadUInt16();

            return lcp;
        }

        public static LanguageCodePage Deserialize(byte[] content, ref int offset)
        {
            LanguageCodePage lcp = new LanguageCodePage();

            lcp.MicrosoftLanguageIdentifier = BitConverter.ToUInt16(content, offset); offset += 2;
            lcp.IBMCodePageNumber = BitConverter.ToUInt16(content, offset); offset += 2;

            return lcp;
        }
    }
}