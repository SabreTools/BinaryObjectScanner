using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Utilities;

namespace BinaryObjectScanner
{
    public static class Factory
    {
        /// <summary>
        /// Create an instance of a detectable based on file type
        /// </summary>
        public static IDetectable? CreateDetectable(SupportedFileType fileType)
        {
            switch (fileType)
            {
                case SupportedFileType.AACSMediaKeyBlock: return new FileType.AACSMediaKeyBlock();
                case SupportedFileType.BDPlusSVM: return new FileType.BDPlusSVM();
                //case SupportedFileType.CIA: return new FileType.CIA();
                case SupportedFileType.Executable: return new FileType.Executable();
                case SupportedFileType.LDSCRYPT: return new FileType.LDSCRYPT();
                //case SupportedFileType.N3DS: return new FileType.N3DS();
                //case SupportedFileType.Nitro: return new FileType.Nitro();
                case SupportedFileType.PLJ: return new FileType.PLJ();
                case SupportedFileType.RealArcadeInstaller: return new FileType.RealArcadeInstaller();
                case SupportedFileType.RealArcadeMezzanine: return new FileType.RealArcadeMezzanine();
                case SupportedFileType.SFFS: return new FileType.SFFS();
                case SupportedFileType.Textfile: return new FileType.Textfile();
                default: return null;
            }
        }

        /// <summary>
        /// Create an instance of an extractable based on file type
        /// </summary>
        public static IExtractable? CreateExtractable(SupportedFileType fileType)
        {
            switch (fileType)
            {
                case SupportedFileType.BFPK: return new FileType.BFPK();
                case SupportedFileType.BSP: return new FileType.BSP();
                case SupportedFileType.BZip2: return new FileType.BZip2();
                case SupportedFileType.CFB: return new FileType.CFB();
                //case SupportedFileType.CIA: return new FileType.CIA();
                case SupportedFileType.GCF: return new FileType.GCF();
                case SupportedFileType.GZIP: return new FileType.GZIP();
                case SupportedFileType.InstallShieldArchiveV3: return new FileType.InstallShieldArchiveV3();
                case SupportedFileType.InstallShieldCAB: return new FileType.InstallShieldCAB();
                case SupportedFileType.MicrosoftCAB: return new FileType.MicrosoftCAB();
                case SupportedFileType.MicrosoftLZ: return new FileType.MicrosoftLZ();
                case SupportedFileType.MPQ: return new FileType.MPQ();
                //case SupportedFileType.N3DS: return new FileType.N3DS();
                //case SupportedFileType.NCF: return new FileType.NCF();
                //case SupportedFileType.Nitro: return new FileType.Nitro();
                case SupportedFileType.PAK: return new FileType.PAK();
                case SupportedFileType.PFF: return new FileType.PFF();
                case SupportedFileType.PKZIP: return new FileType.PKZIP();
                //case SupportedFileType.PLJ: return new FileType.PLJ();
                //case SupportedFileType.Quantum: return new FileType.Quantum();
                case SupportedFileType.RAR: return new FileType.RAR();
                case SupportedFileType.SevenZip: return new FileType.SevenZip();
                case SupportedFileType.SFFS: return new FileType.SFFS();
                case SupportedFileType.SGA: return new FileType.SGA();
                case SupportedFileType.TapeArchive: return new FileType.TapeArchive();
                case SupportedFileType.VBSP: return new FileType.VBSP();
                case SupportedFileType.VPK: return new FileType.VPK();
                case SupportedFileType.WAD: return new FileType.WAD();
                case SupportedFileType.XZ: return new FileType.XZ();
                case SupportedFileType.XZP: return new FileType.XZP();
                default: return null;
            }
        }
    }
}
