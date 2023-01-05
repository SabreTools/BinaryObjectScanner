﻿using System;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.Tools
{
    public static class Utilities
    {
        #region File Types

        /// <summary>
        /// Get the supported file type for a magic string
        /// </summary>
        /// <remarks>Recommend sending in 16 bytes to check</remarks>
        public static SupportedFileType GetFileType(byte[] magic)
        {
            // If we have an invalid magic byte array
            if (magic == null || magic.Length == 0)
                return SupportedFileType.UNKNOWN;

            // TODO: For all modelled types, use the constants instead of hardcoded values here
            #region BFPK

            if (magic.StartsWith(new byte?[] { 0x42, 0x46, 0x50, 0x4b }))
                return SupportedFileType.BFPK;

            #endregion

            #region BSP

            if (magic.StartsWith(new byte?[] { 0x1e, 0x00, 0x00, 0x00 }))
                return SupportedFileType.BSP;

            #endregion

            #region BZip2

            if (magic.StartsWith(new byte?[] { 0x42, 0x52, 0x68 }))
                return SupportedFileType.BZip2;

            #endregion

            #region Executable

            // DOS MZ executable file format (and descendants)
            if (magic.StartsWith(new byte?[] { 0x4d, 0x5a }))
                return SupportedFileType.Executable;

            /*
            // None of the following are supported in scans yet

            // Executable and Linkable Format
            if (magic.StartsWith(new byte?[] { 0x7f, 0x45, 0x4c, 0x46 }))
                return FileTypes.Executable;

            // Mach-O binary (32-bit)
            if (magic.StartsWith(new byte?[] { 0xfe, 0xed, 0xfa, 0xce }))
                return FileTypes.Executable;

            // Mach-O binary (32-bit, reverse byte ordering scheme)
            if (magic.StartsWith(new byte?[] { 0xce, 0xfa, 0xed, 0xfe }))
                return FileTypes.Executable;

            // Mach-O binary (64-bit)
            if (magic.StartsWith(new byte?[] { 0xfe, 0xed, 0xfa, 0xcf }))
                return FileTypes.Executable;

            // Mach-O binary (64-bit, reverse byte ordering scheme)
            if (magic.StartsWith(new byte?[] { 0xcf, 0xfa, 0xed, 0xfe }))
                return FileTypes.Executable;

            // Prefrred Executable File Format
            if (magic.StartsWith(new byte?[] { 0x4a, 0x6f, 0x79, 0x21, 0x70, 0x65, 0x66, 0x66 }))
                return FileTypes.Executable;
            */

            #endregion

            #region GCF

            if (magic.StartsWith(new byte?[] { 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 }))
                return SupportedFileType.GCF;

            #endregion

            #region GZIP

            if (magic.StartsWith(new byte?[] { 0x1f, 0x8b }))
                return SupportedFileType.GZIP;

            #endregion

            #region IniFile

            // No magic checks for IniFile

            #endregion

            #region InstallShieldArchiveV3

            if (magic.StartsWith(new byte?[] { 0x13, 0x5D, 0x65, 0x8C }))
                return SupportedFileType.InstallShieldArchiveV3;

            #endregion

            #region InstallShieldCAB

            if (magic.StartsWith(new byte?[] { 0x49, 0x53, 0x63 }))
                return SupportedFileType.InstallShieldCAB;

            #endregion

            #region LDSCRYPT

            if (magic.StartsWith(new byte?[] { 0x4C, 0x44, 0x53, 0x43, 0x52, 0x59, 0x50, 0x54 }))
                return SupportedFileType.LDSCRYPT;

            #endregion

            #region MicrosoftCAB

            if (magic.StartsWith(new byte?[] { 0x4d, 0x53, 0x43, 0x46 }))
                return SupportedFileType.MicrosoftCAB;

            #endregion

            #region MicrosoftLZ

            if (magic.StartsWith(new byte?[] { 0x53, 0x5a, 0x44, 0x44, 0x88, 0xf0, 0x27, 0x33 }))
                return SupportedFileType.MicrosoftLZ;

            #endregion

            #region MPQ

            if (magic.StartsWith(new byte?[] { 0x4d, 0x50, 0x51, 0x1a }))
                return SupportedFileType.MPQ;

            if (magic.StartsWith(new byte?[] { 0x4d, 0x50, 0x51, 0x1b }))
                return SupportedFileType.MPQ;

            #endregion

            #region MSI

            if (magic.StartsWith(new byte?[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }))
                return SupportedFileType.MSI;

            #endregion

            #region NCF

            if (magic.StartsWith(new byte?[] { 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00 }))
                return SupportedFileType.NCF;

            #endregion

            #region PAK

            if (magic.StartsWith(new byte?[] { 0x50, 0x41, 0x43, 0x4B }))
                return SupportedFileType.PAK;

            #endregion

            #region PKZIP

            // PKZIP (Unknown)
            if (magic.StartsWith(new byte?[] { 0x50, 0x4b, 0x00, 0x00 }))
                return SupportedFileType.PKZIP;

            // PKZIP
            if (magic.StartsWith(new byte?[] { 0x50, 0x4b, 0x03, 0x04 }))
                return SupportedFileType.PKZIP;

            // PKZIP (Empty Archive)
            if (magic.StartsWith(new byte?[] { 0x50, 0x4b, 0x05, 0x06 }))
                return SupportedFileType.PKZIP;

            // PKZIP (Spanned Archive)
            if (magic.StartsWith(new byte?[] { 0x50, 0x4b, 0x07, 0x08 }))
                return SupportedFileType.PKZIP;

            #endregion

            #region PLJ

            // https://www.iana.org/assignments/media-types/audio/vnd.everad.plj
            if (magic.StartsWith(new byte?[] { 0xFF, 0x9D, 0x53, 0x4B }))
                return SupportedFileType.PLJ;

            #endregion

            #region Quantum

            if (magic.StartsWith(new byte?[] { 0x44, 0x53 }))
                return SupportedFileType.Quantum;

            #endregion
            
            #region RAR

            // RAR archive version 1.50 onwards
            if (magic.StartsWith(new byte?[] { 0x52, 0x61, 0x72, 0x21, 0x1a, 0x07, 0x00 }))
                return SupportedFileType.RAR;

            // RAR archive version 5.0 onwards
            if (magic.StartsWith(new byte?[] { 0x52, 0x61, 0x72, 0x21, 0x1a, 0x07, 0x01, 0x00 }))
                return SupportedFileType.RAR;

            #endregion

            #region SevenZip

            if (magic.StartsWith(new byte?[] { 0x37, 0x7a, 0xbc, 0xaf, 0x27, 0x1c }))
                return SupportedFileType.SevenZip;

            #endregion

            #region SFFS

            // Found in Redump entry 81756, confirmed to be "StarForce Filesystem" by PiD.
            if (magic.StartsWith(new byte?[] { 0x53, 0x46, 0x46, 0x53 }))
                return SupportedFileType.SFFS;

            #endregion 

            #region SGA

            if (magic.StartsWith(new byte?[] { 0x5F, 0x41, 0x52, 0x43, 0x48, 0x49, 0x56, 0x45 }))
                return SupportedFileType.SGA;

            #endregion

            #region TapeArchive

            if (magic.StartsWith(new byte?[] { 0x75, 0x73, 0x74, 0x61, 0x72, 0x00, 0x30, 0x30 }))
                return SupportedFileType.TapeArchive;

            if (magic.StartsWith(new byte?[] { 0x75, 0x73, 0x74, 0x61, 0x72, 0x20, 0x20, 0x00 }))
                return SupportedFileType.TapeArchive;

            #endregion

            #region Textfile

            // Not all textfiles can be determined through magic number

            // HTML
            if (magic.StartsWith(new byte?[] { 0x3c, 0x68, 0x74, 0x6d, 0x6c }))
                return SupportedFileType.Textfile;

            // HTML and XML
            if (magic.StartsWith(new byte?[] { 0x3c, 0x21, 0x44, 0x4f, 0x43, 0x54, 0x59, 0x50, 0x45 }))
                return SupportedFileType.Textfile;

            // InstallShield Compiled Rules
            if (magic.StartsWith(new byte?[] { 0x61, 0x4C, 0x75, 0x5A }))
                return SupportedFileType.Textfile;

            // Microsoft Office File (old)
            if (magic.StartsWith(new byte?[] { 0xd0, 0xcf, 0x11, 0xe0, 0xa1, 0xb1, 0x1a, 0xe1 }))
                return SupportedFileType.Textfile;

            // Rich Text File
            if (magic.StartsWith(new byte?[] { 0x7b, 0x5c, 0x72, 0x74, 0x66, 0x31 }))
                return SupportedFileType.Textfile;

            // Windows Help File
            if (magic.StartsWith(new byte?[] { 0x3F, 0x5F, 0x03, 0x00 }))
                return SupportedFileType.Textfile;

            #endregion

            #region VBSP

            if (magic.StartsWith(new byte?[] { 0x56, 0x42, 0x53, 0x50 }))
                return SupportedFileType.VBSP;

            #endregion

            #region VPK

            if (magic.StartsWith(new byte?[] { 0x34, 0x12, 0xaa, 0x55 }))
                return SupportedFileType.VPK;

            #endregion

            #region WAD

            if (magic.StartsWith(new byte?[] { 0x57, 0x41, 0x44, 0x33 }))
                return SupportedFileType.WAD;

            #endregion

            #region XZ

            if (magic.StartsWith(new byte?[] { 0xfd, 0x37, 0x7a, 0x58, 0x5a, 0x00 }))
                return SupportedFileType.XZ;

            #endregion

            #region XZP

            if (magic.StartsWith(new byte?[] { 0x70, 0x69, 0x5A, 0x78 }))
                return SupportedFileType.XZP;

            #endregion

            // We couldn't find a supported match
            return SupportedFileType.UNKNOWN;
        }

        /// <summary>
        /// Get the supported file type for an extension
        /// </summary>
        /// <remarks>This is less accurate than a magic string match</remarks>
        public static SupportedFileType GetFileType(string extension)
        {
            // If we have an invalid extension
            if (string.IsNullOrWhiteSpace(extension))
                return SupportedFileType.UNKNOWN;

            // Normalize the extension
            extension = extension.TrimStart('.').Trim();

            #region BFPK

            // No extensions registered for BFPK

            #endregion

            #region BSP

            // Shares an extension with VBSP so it can't be used accurately
            // if (extension.Equals("bsp", StringComparison.OrdinalIgnoreCase))
            //     return SupportedFileType.BSP;

            #endregion

            #region BZip2

            if (extension.Equals("bz2", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.BZip2;

            #endregion

            #region Executable

            // DOS MZ executable file format (and descendants)
            if (extension.Equals("exe", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.Executable;

            // DOS MZ library file format (and descendants)
            if (extension.Equals("dll", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.Executable;

            #endregion

            #region GCF

            if (extension.Equals("gcf", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.GCF;

            #endregion

            #region GZIP

            if (extension.Equals("gz", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.GZIP;

            #endregion

            #region IniFile

            if (extension.Equals("ini", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.IniFile;

            #endregion

            #region InstallShieldArchiveV3

            if (extension.Equals("z", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.InstallShieldArchiveV3;

            #endregion

            #region InstallShieldCAB

            // No extensions registered for InstallShieldCAB
            // Both InstallShieldCAB and MicrosoftCAB share the same extension

            #endregion

            #region MicrosoftCAB

            // No extensions registered for InstallShieldCAB
            // Both InstallShieldCAB and MicrosoftCAB share the same extension

            #endregion

            #region MPQ

            if (extension.Equals("mpq", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.MPQ;

            #endregion

            #region MSI

            if (extension.Equals("msi", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.MSI;

            #endregion

            #region NCF

            if (extension.Equals("ncf", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.NCF;

            #endregion

            #region PAK

            // No extensions registered for PAK
            // Both PAK and Quantum share one extension
            // if (extension.Equals("pak", StringComparison.OrdinalIgnoreCase))
            //     return SupportedFileType.PAK;

            #endregion

            #region PKZIP

            // PKZIP
            if (extension.Equals("zip", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PKZIP;

            // Android package
            if (extension.Equals("apk", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PKZIP;

            // Java archive
            if (extension.Equals("jar", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PKZIP;

            // Google Earth saved working session file
            if (extension.Equals("kmz", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PKZIP;

            // KWord document
            if (extension.Equals("kwd", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PKZIP;

            // Microsoft Office Open XML Format (OOXML) Document
            if (extension.Equals("docx", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PKZIP;

            // Microsoft Office Open XML Format (OOXML) Presentation
            if (extension.Equals("pptx", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PKZIP;

            // Microsoft Office Open XML Format (OOXML) Spreadsheet
            if (extension.Equals("xlsx", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PKZIP;

            // OpenDocument text document
            if (extension.Equals("odt", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PKZIP;

            // OpenDocument presentation
            if (extension.Equals("odp", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PKZIP;

            // OpenDocument text document template
            if (extension.Equals("ott", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PKZIP;

            // Microsoft Open XML paper specification file
            if (extension.Equals("oxps", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PKZIP;

            // OpenOffice spreadsheet
            if (extension.Equals("sxc", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PKZIP;

            // OpenOffice drawing
            if (extension.Equals("sxd", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PKZIP;

            // OpenOffice presentation
            if (extension.Equals("sxi", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PKZIP;

            // OpenOffice word processing
            if (extension.Equals("sxw", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PKZIP;

            // StarOffice spreadsheet
            if (extension.Equals("sxc", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PKZIP;

            // Windows Media compressed skin file
            if (extension.Equals("wmz", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PKZIP;

            // Mozilla Browser Archive
            if (extension.Equals("xpi", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PKZIP;

            // XML paper specification file
            if (extension.Equals("xps", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PKZIP;

            // eXact Packager Models
            if (extension.Equals("xpt", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PKZIP;

            #endregion

            #region PLJ

            // https://www.iana.org/assignments/media-types/audio/vnd.everad.plj
            if (extension.Equals("plj", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.PLJ;

            #endregion

            #region Quantum

            if (extension.Equals("q", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.Quantum;

            // Both PAK and Quantum share one extension
            // if (extension.Equals("pak", StringComparison.OrdinalIgnoreCase))
            //     return SupportedFileType.Quantum;

            #endregion
            
            #region RAR

            if (extension.Equals("rar", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.RAR;

            #endregion

            #region SevenZip

            if (extension.Equals("7z", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.SevenZip;

            #endregion

            #region SGA

            if (extension.Equals("sga", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.SGA;

            #endregion

            #region TapeArchive

            if (extension.Equals("tar", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.SevenZip;

            #endregion

            #region Textfile

            // "Description in Zip"
            if (extension.Equals("diz", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.Textfile;

            // Generic textfile (no header)
            if (extension.Equals("txt", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.Textfile;

            // HTML
            if (extension.Equals("htm", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.Textfile;
            if (extension.Equals("html", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.Textfile;

            // InstallShield Script
            if (extension.Equals("ins", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.Textfile;

            // Microsoft Office File (old)
            if (extension.Equals("doc", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.Textfile;

            // Rich Text File
            if (extension.Equals("rtf", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.Textfile;

            // Setup information
            if (extension.Equals("inf", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.Textfile;

            // Windows Help File
            if (extension.Equals("hlp", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.Textfile;

            // XML
            if (extension.Equals("xml", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.Textfile;

            #endregion

            #region VBSP

            // Shares an extension with BSP so it can't be used accurately
            // if (extension.Equals("bsp", StringComparison.OrdinalIgnoreCase))
            //     return SupportedFileType.VBSP;

            #endregion

            #region VPK

            // Common extension so this cannot be used accurately
            // if (extension.Equals("vpk", StringComparison.OrdinalIgnoreCase))
            //     return SupportedFileType.VPK;

            #endregion

            #region WAD

            // Common extension so this cannot be used accurately
            // if (extension.Equals("wad", StringComparison.OrdinalIgnoreCase))
            //     return SupportedFileType.WAD;

            #endregion

            #region XZ

            if (extension.Equals("xz", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.XZ;

            #endregion

            #region XZP

            if (extension.Equals("xzp", StringComparison.OrdinalIgnoreCase))
                return SupportedFileType.XZP;

            #endregion

            // We couldn't find a supported match
            return SupportedFileType.UNKNOWN;
        }

        /// <summary>
        /// Create an instance of a scannable based on file type
        /// </summary>
        public static IScannable CreateScannable(SupportedFileType fileType)
        {
            switch (fileType)
            {
                case SupportedFileType.BFPK: return new FileType.BFPK();
                case SupportedFileType.BSP: return new FileType.BSP();
                case SupportedFileType.BZip2: return new FileType.BZip2();
                case SupportedFileType.Executable: return new FileType.Executable();
                case SupportedFileType.GCF: return new FileType.GCF();
                case SupportedFileType.GZIP: return new FileType.GZIP();
                //case FileTypes.IniFile: return new FileType.IniFile();
                case SupportedFileType.InstallShieldArchiveV3: return new FileType.InstallShieldArchiveV3();
                case SupportedFileType.InstallShieldCAB: return new FileType.InstallShieldCAB();
                case SupportedFileType.LDSCRYPT: return new FileType.LDSCRYPT();
                case SupportedFileType.MicrosoftCAB: return new FileType.MicrosoftCAB();
                case SupportedFileType.MicrosoftLZ: return new FileType.MicrosoftLZ();
                case SupportedFileType.MPQ: return new FileType.MPQ();
                case SupportedFileType.MSI: return new FileType.MSI();
                case SupportedFileType.PAK: return new FileType.PAK();
                case SupportedFileType.PKZIP: return new FileType.PKZIP();
                case SupportedFileType.PLJ: return new FileType.PLJ();
                //case SupportedFileType.Quantum: return new FileType.Quantum();
                case SupportedFileType.RAR: return new FileType.RAR();
                case SupportedFileType.SevenZip: return new FileType.SevenZip();
                case SupportedFileType.SFFS: return new FileType.SFFS();
                case SupportedFileType.SGA: return new FileType.SGA();
                case SupportedFileType.TapeArchive: return new FileType.TapeArchive();
                case SupportedFileType.Textfile: return new FileType.Textfile();
                case SupportedFileType.VBSP: return new FileType.VBSP();
                case SupportedFileType.VPK: return new FileType.VPK();
                case SupportedFileType.WAD: return new FileType.WAD();
                case SupportedFileType.XZ: return new FileType.XZ();
                case SupportedFileType.XZP: return new FileType.XZP();
                default: return null;
            }
        }

        #endregion

        #region Processed Executable Information

        /// <summary>
        /// Get the internal version as reported by the filesystem
        /// </summary>
        /// <param name="file">File to check for version</param>
        /// <returns>Version string, null on error</returns>
        public static string GetInternalVersion(string file)
        {
            try
            {
                using (Stream fileStream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var pex = PortableExecutable.Create(fileStream);
                    return GetInternalVersion(pex);
                }
            }
            catch
            {
                return string.Empty;
            }
        }
        
        /// <summary>
        /// Get the internal version as reported by the resources
        /// </summary>
        /// <param name="pex">PortableExecutable representing the file contents</param>
        /// <returns>Version string, null on error</returns>
        public static string GetInternalVersion(PortableExecutable pex)
        {
            string version = pex.FileVersion;
            if (!string.IsNullOrWhiteSpace(version))
                return version.Replace(", ", ".");

            version = pex.ProductVersion;
            if (!string.IsNullOrWhiteSpace(version))
                return version.Replace(", ", ".");

            version = pex.AssemblyVersion;
            if (!string.IsNullOrWhiteSpace(version))
                return version;

            return null;
        }

        #endregion

        #region Wrappers for Matchers

        /// <summary>
        /// Wrapper for GetInternalVersion for use in path matching
        /// </summary>
        /// <param name="firstMatchedString">File to check for version</param>
        /// <param name="files">Full list of input paths</param>
        /// <returns>Version string, null on error</returns>
        public static string GetInternalVersion(string firstMatchedString, IEnumerable<string> files) => GetInternalVersion(firstMatchedString);

        #endregion
    }
}
