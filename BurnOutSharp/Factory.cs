using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Utilities;

namespace BurnOutSharp
{
    internal static class Factory
    {
        /// <summary>
        /// Create an instance of a detectable based on file type
        /// </summary>
        public static IDetectable CreateDetectable(SupportedFileType fileType)
        {
            switch (fileType)
            {
                case SupportedFileType.AACSMediaKeyBlock: return new BinaryObjectScanner.FileType.AACSMediaKeyBlock();
                case SupportedFileType.BDPlusSVM: return new BinaryObjectScanner.FileType.BDPlusSVM();
                //case SupportedFileType.CIA: return new BinaryObjectScanner.FileType.CIA();
                case SupportedFileType.Executable: return new BinaryObjectScanner.FileType.Executable();
                case SupportedFileType.LDSCRYPT: return new BinaryObjectScanner.FileType.LDSCRYPT();
                //case SupportedFileType.N3DS: return new BinaryObjectScanner.FileType.N3DS();
                //case SupportedFileType.Nitro: return new BinaryObjectScanner.FileType.Nitro();
                case SupportedFileType.PLJ: return new BinaryObjectScanner.FileType.PLJ();
                case SupportedFileType.SFFS: return new BinaryObjectScanner.FileType.SFFS();
                case SupportedFileType.Textfile: return new BinaryObjectScanner.FileType.Textfile();
                default: return null;
            }
        }

        /// <summary>
        /// Create an instance of an extractable based on file type
        /// </summary>
        public static IExtractable CreateExtractable(SupportedFileType fileType)
        {
            switch (fileType)
            {
                case SupportedFileType.BFPK: return new BinaryObjectScanner.FileType.BFPK();
                case SupportedFileType.BSP: return new BinaryObjectScanner.FileType.BSP();
                case SupportedFileType.BZip2: return new BinaryObjectScanner.FileType.BZip2();
                case SupportedFileType.CFB: return new BinaryObjectScanner.FileType.CFB();
                //case SupportedFileType.CIA: return new BinaryObjectScanner.FileType.CIA();
                case SupportedFileType.GCF: return new BinaryObjectScanner.FileType.GCF();
                case SupportedFileType.GZIP: return new BinaryObjectScanner.FileType.GZIP();
                case SupportedFileType.InstallShieldArchiveV3: return new BinaryObjectScanner.FileType.InstallShieldArchiveV3();
                case SupportedFileType.InstallShieldCAB: return new BinaryObjectScanner.FileType.InstallShieldCAB();
                case SupportedFileType.MicrosoftCAB: return new BinaryObjectScanner.FileType.MicrosoftCAB();
                case SupportedFileType.MicrosoftLZ: return new BinaryObjectScanner.FileType.MicrosoftLZ();
                case SupportedFileType.MPQ: return new BinaryObjectScanner.FileType.MPQ();
                //case SupportedFileType.N3DS: return new BinaryObjectScanner.FileType.N3DS();
                //case SupportedFileType.NCF: return new BinaryObjectScanner.FileType.NCF();
                //case SupportedFileType.Nitro: return new BinaryObjectScanner.FileType.Nitro();
                case SupportedFileType.PAK: return new BinaryObjectScanner.FileType.PAK();
                case SupportedFileType.PFF: return new BinaryObjectScanner.FileType.PFF();
                case SupportedFileType.PKZIP: return new BinaryObjectScanner.FileType.PKZIP();
                //case SupportedFileType.PLJ: return new BinaryObjectScanner.FileType.PLJ();
                //case SupportedFileType.Quantum: return new BinaryObjectScanner.FileType.Quantum();
                case SupportedFileType.RAR: return new BinaryObjectScanner.FileType.RAR();
                case SupportedFileType.SevenZip: return new BinaryObjectScanner.FileType.SevenZip();
                case SupportedFileType.SFFS: return new BinaryObjectScanner.FileType.SFFS();
                case SupportedFileType.SGA: return new BinaryObjectScanner.FileType.SGA();
                case SupportedFileType.TapeArchive: return new BinaryObjectScanner.FileType.TapeArchive();
                case SupportedFileType.VBSP: return new BinaryObjectScanner.FileType.VBSP();
                case SupportedFileType.VPK: return new BinaryObjectScanner.FileType.VPK();
                case SupportedFileType.WAD: return new BinaryObjectScanner.FileType.WAD();
                case SupportedFileType.XZ: return new BinaryObjectScanner.FileType.XZ();
                case SupportedFileType.XZP: return new BinaryObjectScanner.FileType.XZP();
                default: return null;
            }
        }
    }
}
