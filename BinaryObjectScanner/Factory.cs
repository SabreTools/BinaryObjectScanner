using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner
{
    public static class Factory
    {
        /// <summary>
        /// Create an instance of a detectable based on file type
        /// </summary>
        public static Interfaces.IDetectable? CreateDetectable(WrapperType fileType)
            => CreateDetectable(fileType, null);

        /// <summary>
        /// Create an instance of a detectable based on file type
        /// </summary>
        public static Interfaces.IDetectable? CreateDetectable(WrapperType fileType, IWrapper? wrapper)
        {
            // Use the wrapper before the type
            switch (wrapper)
            {
                case AACSMediaKeyBlock obj: return new FileType.AACSMediaKeyBlock(obj);
                case BDPlusSVM obj: return new FileType.BDPlusSVM(obj);
                // case CIA obj => new FileType.CIA(obj),
                case LinearExecutable obj: return new FileType.LinearExecutable(obj);
                case MSDOS obj: return new FileType.MSDOS(obj);
                // case N3DS obj: return new FileType.N3DS(obj);
                case NewExecutable obj: return new FileType.NewExecutable(obj);
                case PlayJAudioFile obj: return new FileType.PLJ(obj);
                case PortableExecutable obj: return new FileType.PortableExecutable(obj);
            }

            // Fall back on the file type for types not implemented in Serialization
            return fileType switch
            {
                // WrapperType.CIA => new FileType.CIA(),
                WrapperType.LDSCRYPT => new FileType.LDSCRYPT(),
                // WrapperType.N3DS => new FileType.N3DS(),
                WrapperType.RealArcadeInstaller => new FileType.RealArcadeInstaller(),
                WrapperType.RealArcadeMezzanine => new FileType.RealArcadeMezzanine(),
                WrapperType.SFFS => new FileType.SFFS(),
                WrapperType.Textfile => new FileType.Textfile(),
                _ => null,
            };
        }

        /// <summary>
        /// Create an instance of an extractable based on file type
        /// </summary>
        public static Interfaces.IExtractable? CreateExtractable(WrapperType fileType)
            => CreateExtractable(fileType, null);

        /// <summary>
        /// Create an instance of an extractable based on file type
        /// </summary>
        public static Interfaces.IExtractable? CreateExtractable(WrapperType fileType, IWrapper? wrapper)
        {
            // Use the wrapper before the type
            switch (wrapper)
            {
                case BFPK obj: return new FileType.BFPK(obj);
                case BSP obj: return new FileType.BSP(obj);
                case BZip2 obj: return new FileType.BZip2(obj);
                case CFB obj: return new FileType.CFB(obj);
                // case CIA => new FileType.CIA(),
                case GCF obj: return new FileType.GCF(obj);
                case GZip obj: return new FileType.GZip(obj);
                case InstallShieldArchiveV3 obj: return new FileType.InstallShieldArchiveV3(obj);
                case InstallShieldCabinet obj: return new FileType.InstallShieldCAB(obj);
                case LZKWAJ obj: return new FileType.LZKWAJ(obj);
                case LZQBasic obj: return new FileType.LZQBasic(obj);
                case LZSZDD obj: return new FileType.LZSZDD(obj);
                case MicrosoftCabinet obj: return new FileType.MicrosoftCAB(obj);
                case MoPaQ obj: return new FileType.MPQ(obj);
                // case N3DS: return new FileType.N3DS();
                // case NCF: return new FileType.NCF();
                case NewExecutable obj: return new FileType.NewExecutable(obj);
                // case Nitro: return new FileType.Nitro();
                case PAK obj: return new FileType.PAK(obj);
                case PFF obj: return new FileType.PFF(obj);
                case PKZIP obj: return new FileType.PKZIP(obj);
                // case PlayJAudioFile: return new FileType.PLJ();
                case PortableExecutable obj: return new FileType.PortableExecutable(obj);
                case Quantum obj: return new FileType.Quantum(obj);
                case RAR obj: return new FileType.RAR(obj);
                case SevenZip obj: return new FileType.SevenZip(obj);
                case SGA obj: return new FileType.SGA(obj);
                case TapeArchive obj: return new FileType.TapeArchive(obj);
                case VBSP obj: return new FileType.VBSP(obj);
                case VPK obj: return new FileType.VPK(obj);
                case WAD3 obj: return new FileType.WAD3(obj);
                case XZ obj: return new FileType.XZ(obj);
                case XZP obj: return new FileType.XZP(obj);
            }

            // Fall back on the file type for types not implemented in Serialization
            return fileType switch
            {
                // WrapperType.CIA => new FileType.CIA(),
                // WrapperType.N3DS => new FileType.N3DS(),
                // WrapperType.NCF => new FileType.NCF(),
                // WrapperType.Nitro => new FileType.Nitro(),
                // WrapperType.PlayJAudioFile => new FileType.PLJ(),
                WrapperType.SFFS => new FileType.SFFS(),
                _ => null,
            };
        }
    }
}
