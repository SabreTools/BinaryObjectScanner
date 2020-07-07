using System.Collections.Generic;
using System.IO;
using System.Text;
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

        public static List<string> Scan(Stream stream, string file)
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
            List<string> protections = new List<string>();
            List<string> subProtections = new List<string>();
            string protection;

            // 3PLock
            protection = ThreePLock.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // ActiveMARK
            protection = ActiveMARK.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Alpha-ROM
            protection = AlphaROM.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Armadillo
            protection = Armadillo.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // CD-Cops
            protection = CDCops.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // CD-Lock
            protection = CDLock.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // CDSHiELD SE
            protection = CDSHiELDSE.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // CD Check
            protection = CDCheck.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Cenega ProtectDVD
            protection = CengaProtectDVD.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Code Lock
            protection = CodeLock.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // CopyKiller
            protection = CopyKiller.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Cucko (EA Custom)
            protection = Cucko.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // dotFuscator
            protection = dotFuscator.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // DVD-Cops
            protection = DVDCops.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // EA CdKey Registration Module
            protection = EACdKey.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // EXE Stealth
            protection = EXEStealth.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Games for Windows - Live
            protection = GFWL.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Impulse Reactor
            protection = ImpulseReactor.CheckContents(file, fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Inno Setup
            protection = InnoSetup.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // JoWooD X-Prot
            protection = JoWooDXProt.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Key-Lock (Dongle)
            protection = KeyLock.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // LaserLock
            protection = LaserLock.CheckContents(file, fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // PE Compact
            protection = PECompact.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // ProtectDisc
            protection = ProtectDisc.CheckContents(file, fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Ring PROTECH
            protection = RingPROTECH.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SafeDisc / SafeCast
            protection = SafeDisc.CheckContents(file, fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SafeLock
            protection = SafeLock.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SecuROM
            protection = SecuROM.CheckContents(file, fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SmartE
            protection = SmartE.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SolidShield
            protection = SolidShield.CheckContents(file, fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // StarForce
            protection = StarForce.CheckContents(file, fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SVK Protector
            protection = SVKProtector.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Sysiphus / Sysiphus DVD
            protection = Sysiphus.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // TAGES
            protection = Tages.CheckContents(file, fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // VOB ProtectCD/DVD
            protection = VOBProtectCDDVD.CheckContents(file, fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Wise Installer
            subProtections = WiseInstaller.CheckContents(file, fileContent);
            if (subProtections != null && subProtections.Count > 0)
                protections.AddRange(subProtections);

            // WTM CD Protect
            protection = WTMCDProtect.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Xtreme-Protector
            protection = XtremeProtector.CheckContents(fileContent);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            return protections;
        }
    }
}
