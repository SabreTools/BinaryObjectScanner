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
                case BFPK: return new FileType.BFPK();
                case BSP: return new FileType.BSP();
                case BZip2: return new FileType.BZip2();
                case CFB: return new FileType.CFB();
                // case CIA => new FileType.CIA(),
                case GCF: return new FileType.GCF();
                case GZip: return new FileType.GZip();
                case InstallShieldArchiveV3: return new FileType.InstallShieldArchiveV3();
                case InstallShieldCabinet: return new FileType.InstallShieldCAB();
                case LinearExecutable obj: return new FileType.LinearExecutable(obj);
                case LZKWAJ: return new FileType.LZKWAJ();
                case LZQBasic: return new FileType.LZQBasic();
                case LZSZDD: return new FileType.LZSZDD();
                case MicrosoftCabinet: return new FileType.MicrosoftCAB();
                case MoPaQ: return new FileType.MPQ();
                case MSDOS obj: return new FileType.MSDOS(obj);
                // case N3DS: return new FileType.N3DS();
                // case NCF: return new FileType.NCF();
                case NewExecutable obj: return new FileType.NewExecutable(obj);
                // case Nitro: return new FileType.Nitro();
                case PAK: return new FileType.PAK();
                case PFF: return new FileType.PFF();
                case PKZIP: return new FileType.PKZIP();
                // case PlayJAudioFile: return new FileType.PLJ();
                case PortableExecutable obj: return new FileType.PortableExecutable(obj);
                case Quantum: return new FileType.Quantum();
                case RAR: return new FileType.RAR();
                case SevenZip: return new FileType.SevenZip();
                case SGA: return new FileType.SGA();
                case TapeArchive: return new FileType.TapeArchive();
                case VBSP: return new FileType.VBSP();
                case VPK: return new FileType.VPK();
                case WAD3: return new FileType.WAD3();
                case XZ: return new FileType.XZ();
                case XZP: return new FileType.XZP();
            }

            // Fall back on the file type for types not implemented in Serialization
            return fileType switch
            {
                WrapperType.BFPK => new FileType.BFPK(),
                WrapperType.BSP => new FileType.BSP(),
                WrapperType.BZip2 => new FileType.BZip2(),
                WrapperType.CFB => new FileType.CFB(),
                // WrapperType.CIA => new FileType.CIA(),
                // WrapperType.Executable => new FileType.Executable(),
                WrapperType.GCF => new FileType.GCF(),
                WrapperType.GZip => new FileType.GZip(),
                WrapperType.InstallShieldArchiveV3 => new FileType.InstallShieldArchiveV3(),
                WrapperType.InstallShieldCAB => new FileType.InstallShieldCAB(),
                // WrapperType.LinearExecutable => new FileType.LinearExecutable(),
                WrapperType.LZKWAJ => new FileType.LZKWAJ(),
                WrapperType.LZQBasic => new FileType.LZQBasic(),
                WrapperType.LZSZDD => new FileType.LZSZDD(),
                WrapperType.MicrosoftCAB => new FileType.MicrosoftCAB(),
                WrapperType.MoPaQ => new FileType.MPQ(),
                // WrapperType.MSDOS => new FileType.MSDOS(),
                // WrapperType.N3DS => new FileType.N3DS(),
                // WrapperType.NCF => new FileType.NCF(),
                // WrapperType.NewExecutable => new FileType.NewExecutable(),
                // WrapperType.Nitro => new FileType.Nitro(),
                WrapperType.PAK => new FileType.PAK(),
                WrapperType.PFF => new FileType.PFF(),
                WrapperType.PKZIP => new FileType.PKZIP(),
                // WrapperType.PlayJAudioFile => new FileType.PLJ(),
                // WrapperType.PortableExecutable => new FileType.PortableExecutable(),
                // WrapperType.Quantum => new FileType.Quantum(),
                WrapperType.RAR => new FileType.RAR(),
                WrapperType.SevenZip => new FileType.SevenZip(),
                WrapperType.SFFS => new FileType.SFFS(),
                WrapperType.SGA => new FileType.SGA(),
                WrapperType.TapeArchive => new FileType.TapeArchive(),
                WrapperType.VBSP => new FileType.VBSP(),
                WrapperType.VPK => new FileType.VPK(),
                WrapperType.WAD => new FileType.WAD3(),
                WrapperType.XZ => new FileType.XZ(),
                WrapperType.XZP => new FileType.XZP(),
                _ => null,
            };
        }
    }
}
