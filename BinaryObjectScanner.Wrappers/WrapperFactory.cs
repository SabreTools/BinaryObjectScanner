using System;
using System.IO;
using BinaryObjectScanner.Matching;
using BinaryObjectScanner.Utilities;
using SabreTools.IO;

namespace BinaryObjectScanner.Wrappers
{
    public static class WrapperFactory
    {
        /// <summary>
        /// Create an instance of a wrapper based on file type
        /// </summary>
#if NET48
        public static IWrapper CreateWrapper(SupportedFileType fileType, Stream data)
#else
        public static IWrapper? CreateWrapper(SupportedFileType fileType, Stream? data)
#endif
        {
            switch (fileType)
            {
                case SupportedFileType.AACSMediaKeyBlock: return AACSMediaKeyBlock.Create(data);
                case SupportedFileType.BDPlusSVM: return BDPlusSVM.Create(data);
                case SupportedFileType.BFPK: return BFPK.Create(data);
                case SupportedFileType.BSP: return BSP.Create(data);
                //case SupportedFileType.BZip2: return BZip2.Create(data);
                case SupportedFileType.CFB: return CFB.Create(data);
                case SupportedFileType.CIA: return CIA.Create(data);
                case SupportedFileType.Executable: return CreateExecutableWrapper(data);
                case SupportedFileType.GCF: return GCF.Create(data);
                //case SupportedFileType.GZIP: return GZIP.Create(data);
                //case SupportedFileType.IniFile: return IniFile.Create(data);
                //case SupportedFileType.InstallShieldArchiveV3: return InstallShieldArchiveV3.Create(data);
                case SupportedFileType.InstallShieldCAB: return InstallShieldCabinet.Create(data);
                //case SupportedFileType.LDSCRYPT: return BinaryObjectScanner.Wrappers.LDSCRYPT.Create(data);
                case SupportedFileType.MicrosoftCAB: return MicrosoftCabinet.Create(data);
                //case SupportedFileType.MicrosoftLZ: return MicrosoftLZ.Create(data);
                //case SupportedFileType.MPQ: return MoPaQ.Create(data);
                case SupportedFileType.N3DS: return N3DS.Create(data);
                case SupportedFileType.NCF: return NCF.Create(data);
                case SupportedFileType.Nitro: return Nitro.Create(data);
                case SupportedFileType.PAK: return PAK.Create(data);
                case SupportedFileType.PFF: return PFF.Create(data);
                //case SupportedFileType.PKZIP: return PKZIP.Create(data);
                case SupportedFileType.PLJ: return PlayJAudioFile.Create(data);
                case SupportedFileType.Quantum: return Quantum.Create(data);
                //case SupportedFileType.RAR: return RAR.Create(data);
                //case SupportedFileType.SevenZip: return SevenZip.Create(data);
                //case SupportedFileType.SFFS: return SFFS.Create(data);
                case SupportedFileType.SGA: return SGA.Create(data);
                //case SupportedFileType.TapeArchive: return TapeArchive.Create(data);
                //case SupportedFileType.Textfile: return Textfile.Create(data);
                case SupportedFileType.VBSP: return VBSP.Create(data);
                case SupportedFileType.VPK: return VPK.Create(data);
                case SupportedFileType.WAD: return WAD.Create(data);
                //case SupportedFileType.XZ: return XZ.Create(data);
                case SupportedFileType.XZP: return XZP.Create(data);
                default: return null;
            }
        }

        /// <summary>
        /// Create an instance of a wrapper based on the executable type
        /// </summary>
        /// <param name="stream">Stream data to parse</param>
        /// <returns>IWrapper representing the executable, null on error</returns>
#if NET48
        public static IWrapper CreateExecutableWrapper(Stream stream)
#else
        public static IWrapper? CreateExecutableWrapper(Stream? stream)
#endif
        {
            // If we have no stream
            if (stream == null)
                return null;

            // Try to get an MS-DOS wrapper first
#if NET48
            IWrapper wrapper = MSDOS.Create(stream);
#else
            IWrapper? wrapper = MSDOS.Create(stream);
#endif
            if (wrapper == null || !(wrapper is MSDOS msdos))
                return null;

            // Check for a valid new executable address
#if NET48
            if (msdos.NewExeHeaderAddr >= stream.Length)
#else
            if (msdos.NewExeHeaderAddr == null || msdos.NewExeHeaderAddr >= stream.Length)
#endif
                return wrapper;

            // Try to read the executable info
#if NET48
            stream.Seek(msdos.NewExeHeaderAddr, SeekOrigin.Begin);
            byte[] magic = stream.ReadBytes(4);
#else
            stream.Seek(msdos.NewExeHeaderAddr.Value, SeekOrigin.Begin);
            byte[]? magic = stream.ReadBytes(4);
#endif

            // If we didn't get valid data at the offset
            if (magic == null)
            {
                return wrapper;
            }

            // New Executable
            else if (magic.StartsWith(SabreTools.Models.NewExecutable.Constants.SignatureBytes))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return NewExecutable.Create(stream);
            }

            // Linear Executable
            else if (magic.StartsWith(SabreTools.Models.LinearExecutable.Constants.LESignatureBytes)
                || magic.StartsWith(SabreTools.Models.LinearExecutable.Constants.LXSignatureBytes))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return LinearExecutable.Create(stream);
            }

            // Portable Executable
            else if (magic.StartsWith(SabreTools.Models.PortableExecutable.Constants.SignatureBytes))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return PortableExecutable.Create(stream);
            }

            // Everything else fails
            return null;
        }
    }
}
