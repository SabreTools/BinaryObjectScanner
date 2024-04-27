using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner
{
    public static class Factory
    {
        /// <summary>
        /// Create an instance of a detectable based on file type
        /// </summary>
        public static IDetectable? CreateDetectable(WrapperType fileType)
        {
            switch (fileType)
            {
                case WrapperType.AACSMediaKeyBlock: return new FileType.AACSMediaKeyBlock();
                case WrapperType.BDPlusSVM: return new FileType.BDPlusSVM();
                //case WrapperType.CIA: return new FileType.CIA();
                case WrapperType.Executable: return new FileType.Executable();
                case WrapperType.LDSCRYPT: return new FileType.LDSCRYPT();
                //case WrapperType.N3DS: return new FileType.N3DS();
                //case WrapperType.Nitro: return new FileType.Nitro();
                case WrapperType.PlayJAudioFile: return new FileType.PLJ();
                case WrapperType.RealArcadeInstaller: return new FileType.RealArcadeInstaller();
                case WrapperType.RealArcadeMezzanine: return new FileType.RealArcadeMezzanine();
                case WrapperType.SFFS: return new FileType.SFFS();
                case WrapperType.Textfile: return new FileType.Textfile();
                default: return null;
            }
        }

        /// <summary>
        /// Create an instance of an extractable based on file type
        /// </summary>
        public static IExtractable? CreateExtractable(WrapperType fileType)
        {
            switch (fileType)
            {
                case WrapperType.BFPK: return new FileType.BFPK();
                case WrapperType.BSP: return new FileType.BSP();
                case WrapperType.BZip2: return new FileType.BZip2();
                case WrapperType.CFB: return new FileType.CFB();
                //case WrapperType.CIA: return new FileType.CIA();
                case WrapperType.GCF: return new FileType.GCF();
                case WrapperType.GZIP: return new FileType.GZIP();
                case WrapperType.InstallShieldArchiveV3: return new FileType.InstallShieldArchiveV3();
                case WrapperType.InstallShieldCAB: return new FileType.InstallShieldCAB();
                case WrapperType.MicrosoftCAB: return new FileType.MicrosoftCAB();
                case WrapperType.MicrosoftLZ: return new FileType.MicrosoftLZ();
                case WrapperType.MoPaQ: return new FileType.MPQ();
                //case WrapperType.N3DS: return new FileType.N3DS();
                //case WrapperType.NCF: return new FileType.NCF();
                //case WrapperType.Nitro: return new FileType.Nitro();
                case WrapperType.PAK: return new FileType.PAK();
                case WrapperType.PFF: return new FileType.PFF();
                case WrapperType.PKZIP: return new FileType.PKZIP();
                //case WrapperType.PLJ: return new FileType.PLJ();
                //case WrapperType.Quantum: return new FileType.Quantum();
                case WrapperType.RAR: return new FileType.RAR();
                case WrapperType.SevenZip: return new FileType.SevenZip();
                case WrapperType.SFFS: return new FileType.SFFS();
                case WrapperType.SGA: return new FileType.SGA();
                case WrapperType.TapeArchive: return new FileType.TapeArchive();
                case WrapperType.VBSP: return new FileType.VBSP();
                case WrapperType.VPK: return new FileType.VPK();
                case WrapperType.WAD: return new FileType.WAD();
                case WrapperType.XZ: return new FileType.XZ();
                case WrapperType.XZP: return new FileType.XZP();
                default: return null;
            }
        }
    }
}
