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
    }
}
