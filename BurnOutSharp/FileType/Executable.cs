using System.Collections.Generic;
using System.IO;
using System.Text;
using BurnOutSharp.PackerType;
using BurnOutSharp.ProtectionType;

namespace BurnOutSharp.FileType
{
    internal class Executable : IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic)
        {
            // DOS MZ executable file format (and descendants)
            if (magic.StartsWith(new byte[] { 0x4d, 0x5a }))
                return true;

            // Executable and Linkable Format
            if (magic.StartsWith(new byte[] { 0x7f, 0x45, 0x4c, 0x46 }))
                return true;

            // Mach-O binary (32-bit)
            if (magic.StartsWith(new byte[] { 0xfe, 0xed, 0xfa, 0xce }))
                return true;

            // Mach-O binary (32-bit, reverse byte ordering scheme)
            if (magic.StartsWith(new byte[] { 0xce, 0xfa, 0xed, 0xfe }))
                return true;

            // Mach-O binary (64-bit)
            if (magic.StartsWith(new byte[] { 0xfe, 0xed, 0xfa, 0xcf }))
                return true;

            // Mach-O binary (64-bit, reverse byte ordering scheme)
            if (magic.StartsWith(new byte[] { 0xcf, 0xfa, 0xed, 0xfe }))
                return true;

            // Prefrred Executable File Format
            if (magic.StartsWith(new byte[] { 0x4a, 0x6f, 0x79, 0x21, 0x70, 0x65, 0x66, 0x66 }))
                return true;

            return false;
        }

        /// <inheritdoc/>
        public Dictionary<string, List<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        public Dictionary<string, List<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            // Load the current file content
            byte[] fileContent = null;
            using (BinaryReader br = new BinaryReader(stream, Encoding.Default, true))
            {
                fileContent = br.ReadBytes((int)stream.Length);
            }

            // If we can, seek to the beginning of the stream
            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);

            // Files can be protected in multiple ways
            var protections = new Dictionary<string, List<string>>();
            string protection;

            #region Protections

            // 3PLock
            protection = new ThreePLock().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // 321Studios Online Activation
            protection = new ThreeTwoOneStudios().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // ActiveMARK
            protection = new ActiveMARK().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Alpha-ROM
            protection = new AlphaROM().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Cactus Data Shield
            protection = new CactusDataShield().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // CD-Cops
            protection = new CDCops().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // CD-Lock
            protection = new CDLock().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // CDSHiELD SE
            protection = new CDSHiELDSE().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // CD Check
            protection = new CDCheck().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Cenega ProtectDVD
            protection = new CengaProtectDVD().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Code Lock
            protection = new CodeLock().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // CopyKiller
            protection = new CopyKiller().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // DVD-Cops
            protection = new DVDCops().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // EA Protections
            protection = new ElectronicArts().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Games for Windows - Live
            protection = new GFWL().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Impulse Reactor
            protection = new ImpulseReactor().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Inno Setup
            protection = new InnoSetup().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // INTENIUM Trial & Buy Protection
            protection = new Intenium().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // JoWooD X-Prot
            protection = new JoWooDXProt().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Key-Lock (Dongle)
            protection = new KeyLock().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // LaserLock
            protection = new LaserLock().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // MediaMax CD-3
            protection = new MediaMaxCD3().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Origin
            protection = new Origin().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // ProtectDisc
            protection = new ProtectDisc().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Ring PROTECH
            protection = new RingPROTECH().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // SafeDisc / SafeCast
            protection = new SafeDisc().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // SafeLock
            protection = new SafeLock().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // SecuROM
            protection = new SecuROM().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // SmartE
            protection = new SmartE().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // SolidShield
            protection = new SolidShield().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // StarForce
            protection = new StarForce().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // SVK Protector
            protection = new SVKProtector().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Sysiphus / Sysiphus DVD
            protection = new Sysiphus().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // TAGES
            protection = new Tages().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // VOB ProtectCD/DVD
            protection = new VOBProtectCDDVD().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // WTM CD Protect
            protection = new WTMCDProtect().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // XCP 1/2
            protection = new XCP().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Xtreme-Protector
            protection = new XtremeProtector().CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            #endregion

            #region Archive-as-Executable Formats / Installers

            // If we're looking for archives too, run scans
            if (scanner.ScanArchives)
            {
                // Wise Installer
                if (file != null && !string.IsNullOrEmpty(new WiseInstaller().CheckContents(file, fileContent, scanner.IncludePosition)))
                {
                    var subProtections = new WiseInstaller().Scan(scanner, null, file);
                    Utilities.PrependToKeys(subProtections, file);
                    Utilities.AppendToDictionary(protections, subProtections);
                }
            }

            #endregion

            #region Packers

            // If we're looking for packers too, run scans
            if (scanner.ScanPackers)
            {
                // Armadillo
                protection = new Armadillo().CheckContents(file, fileContent, scanner.IncludePosition);
                if (!string.IsNullOrWhiteSpace(protection))
                    Utilities.AppendToDictionary(protections, file, protection);

                // dotFuscator
                protection = new dotFuscator().CheckContents(file, fileContent, scanner.IncludePosition);
                if (!string.IsNullOrWhiteSpace(protection))
                    Utilities.AppendToDictionary(protections, file, protection);

                // EXE Stealth
                protection = new EXEStealth().CheckContents(file, fileContent, scanner.IncludePosition);
                if (!string.IsNullOrWhiteSpace(protection))
                    Utilities.AppendToDictionary(protections, file, protection);

                // NSIS
                protection = new NSIS().CheckContents(file, fileContent, scanner.IncludePosition);
                if (!string.IsNullOrWhiteSpace(protection))
                    Utilities.AppendToDictionary(protections, file, protection);

                // PE Compact
                protection = new PECompact().CheckContents(file, fileContent, scanner.IncludePosition);
                if (!string.IsNullOrWhiteSpace(protection))
                    Utilities.AppendToDictionary(protections, file, protection);

                // UPX
                protection = new UPX().CheckContents(file, fileContent, scanner.IncludePosition);
                if (!string.IsNullOrWhiteSpace(protection))
                    Utilities.AppendToDictionary(protections, file, protection);

                // Wise Installer
                protection = new WiseInstaller().CheckContents(file, fileContent, scanner.IncludePosition);
                if (!string.IsNullOrWhiteSpace(protection))
                    Utilities.AppendToDictionary(protections, file, protection);
            }

            #endregion

            return protections;
        }
    }
}
