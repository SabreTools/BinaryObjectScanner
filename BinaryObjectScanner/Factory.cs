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
            return fileType switch
            {
                WrapperType.AACSMediaKeyBlock => new FileType.AACSMediaKeyBlock(),
                WrapperType.BDPlusSVM => new FileType.BDPlusSVM(),
                //WrapperType.CIA => new FileType.CIA(),
                WrapperType.Executable => new FileType.Executable(),
                WrapperType.LDSCRYPT => new FileType.LDSCRYPT(),
                //WrapperType.N3DS => new FileType.N3DS(),
                //WrapperType.Nitro => new FileType.Nitro(),
                WrapperType.PlayJAudioFile => new FileType.PLJ(),
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
        public static IExtractable? CreateExtractable(WrapperType fileType)
        {
            return fileType switch
            {
                WrapperType.BFPK => new FileType.BFPK(),
                WrapperType.BSP => new FileType.BSP(),
                WrapperType.BZip2 => new FileType.BZip2(),
                WrapperType.CFB => new FileType.CFB(),
                //WrapperType.CIA => new FileType.CIA(),
                WrapperType.GCF => new FileType.GCF(),
                WrapperType.GZIP => new FileType.GZIP(),
                WrapperType.InstallShieldArchiveV3 => new FileType.InstallShieldArchiveV3(),
                WrapperType.InstallShieldCAB => new FileType.InstallShieldCAB(),
                WrapperType.MicrosoftCAB => new FileType.MicrosoftCAB(),
                WrapperType.MicrosoftLZ => new FileType.MicrosoftLZ(),
                WrapperType.MoPaQ => new FileType.MPQ(),
                //WrapperType.N3DS => new FileType.N3DS(),
                //WrapperType.NCF => new FileType.NCF(),
                //WrapperType.Nitro => new FileType.Nitro(),
                WrapperType.PAK => new FileType.PAK(),
                WrapperType.PFF => new FileType.PFF(),
                WrapperType.PKZIP => new FileType.PKZIP(),
                //WrapperType.PlayJAudioFile => new FileType.PLJ(),
                //WrapperType.Quantum => new FileType.Quantum(),
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
