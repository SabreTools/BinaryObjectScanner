using System.Collections.Generic;
using System.IO;
using System.Text;
using BurnOutSharp.PackerType;
using BurnOutSharp.ProtectionType;

namespace BurnOutSharp.FileType
{
    internal class Executable
    {
        public static bool ShouldScan(byte[] magic)
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

        public static Dictionary<string, List<string>> Scan(Scanner scanner, Stream stream, string file = null)
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
            var subProtections = new Dictionary<string, List<string>>();
            string protection;

            #region Protections

            // 3PLock
            protection = ThreePLock.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // 321Studios Online Activation
            protection = ThreeTwoOneStudios.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // ActiveMARK
            protection = ActiveMARK.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Alpha-ROM
            protection = AlphaROM.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Cactus Data Shield
            protection = CactusDataShield.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // CD-Cops
            protection = CDCops.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // CD-Lock
            protection = CDLock.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // CDSHiELD SE
            protection = CDSHiELDSE.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // CD Check
            protection = CDCheck.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Cenega ProtectDVD
            protection = CengaProtectDVD.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Code Lock
            protection = CodeLock.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // CopyKiller
            protection = CopyKiller.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // DVD-Cops
            protection = DVDCops.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // EA Protections
            protection = ElectronicArts.CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Games for Windows - Live
            protection = GFWL.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Impulse Reactor
            protection = ImpulseReactor.CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Inno Setup
            protection = InnoSetup.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // ITENIUM Trial & Buy Protection
            protection = Itenium.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // JoWooD X-Prot
            protection = JoWooDXProt.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Key-Lock (Dongle)
            protection = KeyLock.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // LaserLock
            protection = LaserLock.CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // ProtectDisc
            protection = ProtectDisc.CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Ring PROTECH
            protection = RingPROTECH.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // SafeDisc / SafeCast
            protection = SafeDisc.CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // SafeLock
            protection = SafeLock.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // SecuROM
            protection = SecuROM.CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // SmartE
            protection = SmartE.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // SolidShield
            protection = SolidShield.CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // StarForce
            protection = StarForce.CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // SVK Protector
            protection = SVKProtector.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Sysiphus / Sysiphus DVD
            protection = Sysiphus.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // TAGES
            protection = Tages.CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // VOB ProtectCD/DVD
            protection = VOBProtectCDDVD.CheckContents(file, fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Wise Installer
            subProtections = WiseInstaller.CheckContents(scanner, file, fileContent);
            if (subProtections != null && subProtections.Count > 0)
                Utilities.AppendToDictionary(protections, subProtections);

            // WTM CD Protect
            protection = WTMCDProtect.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // XCP 1/2
            protection = XCP.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            // Xtreme-Protector
            protection = XtremeProtector.CheckContents(fileContent, scanner.IncludePosition);
            if (!string.IsNullOrWhiteSpace(protection))
                Utilities.AppendToDictionary(protections, file, protection);

            #endregion

            #region Packers

            // If we're looking for packers too, run scans
            if (scanner.IncludePackers)
            {

                // Armadillo
                protection = Armadillo.CheckContents(fileContent, scanner.IncludePosition);
                if (!string.IsNullOrWhiteSpace(protection))
                    Utilities.AppendToDictionary(protections, file, protection);

                // dotFuscator
                protection = dotFuscator.CheckContents(fileContent, scanner.IncludePosition);
                if (!string.IsNullOrWhiteSpace(protection))
                    Utilities.AppendToDictionary(protections, file, protection);

                // EXE Stealth
                protection = EXEStealth.CheckContents(fileContent, scanner.IncludePosition);
                if (!string.IsNullOrWhiteSpace(protection))
                    Utilities.AppendToDictionary(protections, file, protection);

                // NSIS
                protection = NSIS.CheckContents(fileContent, scanner.IncludePosition);
                if (!string.IsNullOrWhiteSpace(protection))
                    Utilities.AppendToDictionary(protections, file, protection);

                // PE Compact
                protection = PECompact.CheckContents(fileContent, scanner.IncludePosition);
                if (!string.IsNullOrWhiteSpace(protection))
                    Utilities.AppendToDictionary(protections, file, protection);

                // UPX
                protection = UPX.CheckContents(fileContent, scanner.IncludePosition);
                if (!string.IsNullOrWhiteSpace(protection))
                    Utilities.AppendToDictionary(protections, file, protection);
            }

            #endregion

            return protections;
        }
    }
}
