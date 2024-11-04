using System;
using System.IO;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Wrappers;

namespace Test
{
    internal static class Extractor
    {
        /// <summary>
        /// Wrapper to extract data for a single path
        /// </summary>
        /// <param name="path">File or directory path</param>
        /// <param name="outputDirectory">Output directory path</param>
        /// <param name="includeDebug">Enable including debug information</param>
        public static void ExtractPath(string path, string outputDirectory, bool includeDebug)
        {
            Console.WriteLine($"Checking possible path: {path}");

            // Check if the file or directory exists
            if (File.Exists(path))
            {
                ExtractFile(path, outputDirectory, includeDebug);
            }
            else if (Directory.Exists(path))
            {
                foreach (string file in IOExtensions.SafeEnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    ExtractFile(file, outputDirectory, includeDebug);
                }
            }
            else
            {
                Console.WriteLine($"{path} does not exist, skipping...");
            }
        }

        /// <summary>
        /// Print information for a single file, if possible
        /// </summary>
        private static void ExtractFile(string file, string outputDirectory, bool includeDebug)
        {
            Console.WriteLine($"Attempting to extract all files from {file}");
            using Stream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            // Get the extension for certain checks
            string extension = Path.GetExtension(file).ToLower().TrimStart('.');

            // Get the first 16 bytes for matching
            byte[] magic = new byte[16];
            try
            {
                stream.Read(magic, 0, 16);
                stream.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return;
            }

            // Get the file type
            WrapperType ft = WrapperFactory.GetFileType(magic, extension);

            // Executables technically can be "extracted", but let's ignore that
            // TODO: Support executables that include other stuff

            // 7-zip
            if (ft == WrapperType.SevenZip)
            {
                // Build the archive information
                Console.WriteLine("Extracting 7-zip contents");
                Console.WriteLine();

#if NET20 || NET35 || NET40 || NET452
                Console.WriteLine("Extraction is not supported for this framework!");
                Console.WriteLine();
#else
                // Extract using the FileType
                var sevenZip = new BinaryObjectScanner.FileType.SevenZip();
                sevenZip.Extract(stream, file, outputDirectory, includeDebug: true);
#endif
            }

            // BFPK archive
            else if (ft == WrapperType.BFPK)
            {
                // Build the BFPK information
                Console.WriteLine("Extracting BFPK contents");
                Console.WriteLine();

                // Extract using the FileType
                var bfpk = new BinaryObjectScanner.FileType.BFPK();
                bfpk.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // BSP
            else if (ft == WrapperType.BSP)
            {
                // Build the BSP information
                Console.WriteLine("Extracting BSP contents");
                Console.WriteLine();

                // Extract using the FileType
                var bsp = new BinaryObjectScanner.FileType.BSP();
                bsp.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // bzip2
            else if (ft == WrapperType.BZip2)
            {
                // Build the bzip2 information
                Console.WriteLine("Extracting bzip2 contents");
                Console.WriteLine();

#if NET20 || NET35 || NET40 || NET452
                Console.WriteLine("Extraction is not supported for this framework!");
                Console.WriteLine();
#else
                // Extract using the FileType
                var bzip2 = new BinaryObjectScanner.FileType.BZip2();
                bzip2.Extract(stream, file, outputDirectory, includeDebug: true);
#endif
            }

            // CFB
            else if (ft == WrapperType.CFB)
            {
                // Build the installer information
                Console.WriteLine("Extracting CFB contents");
                Console.WriteLine();

#if NET20 || NET35
                Console.WriteLine("Extraction is not supported for this framework!");
                Console.WriteLine();
#else
                // Extract using the FileType
                var cfb = new BinaryObjectScanner.FileType.CFB();
                cfb.Extract(stream, file, outputDirectory, includeDebug: true);
#endif
            }

            // GCF
            else if (ft == WrapperType.GCF)
            {
                // Build the GCF information
                Console.WriteLine("Extracting GCF contents");
                Console.WriteLine();

                // Extract using the FileType
                var gcf = new BinaryObjectScanner.FileType.GCF();
                gcf.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // gzip
            else if (ft == WrapperType.GZIP)
            {
                // Build the gzip information
                Console.WriteLine("Extracting gzip contents");
                Console.WriteLine();

#if NET20 || NET35 || NET40 || NET452
                Console.WriteLine("Extraction is not supported for this framework!");
                Console.WriteLine();
#else
                // Extract using the FileType
                var gzip = new BinaryObjectScanner.FileType.GZIP();
                gzip.Extract(stream, file, outputDirectory, includeDebug: true);
#endif
            }

            // InstallShield Archive V3 (Z)
            else if (ft == WrapperType.InstallShieldArchiveV3)
            {
                // Build the InstallShield Archive V3 information
                Console.WriteLine("Extracting InstallShield Archive V3 contents");
                Console.WriteLine();

                // Extract using the FileType
                var isav3 = new BinaryObjectScanner.FileType.InstallShieldArchiveV3();
                isav3.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // IS-CAB archive
            else if (ft == WrapperType.InstallShieldCAB)
            {
                // Build the archive information
                Console.WriteLine("Extracting IS-CAB contents");
                Console.WriteLine();

                // Extract using the FileType
                var iscab = new BinaryObjectScanner.FileType.InstallShieldCAB();
                iscab.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // Microsoft Cabinet archive
            else if (ft == WrapperType.MicrosoftCAB)
            {
                // Build the cabinet information
                Console.WriteLine("Extracting MS-CAB contents");
                Console.WriteLine();

#if NET20 || NET35 || !WIN
                Console.WriteLine("Extraction is not supported for this framework!");
                Console.WriteLine();
#else
                // Extract using the FileType
                var mscab = new BinaryObjectScanner.FileType.MicrosoftCAB();
                mscab.Extract(stream, file, outputDirectory, includeDebug: true);
#endif
            }

            // Microsoft LZ / LZ32
            else if (ft == WrapperType.MicrosoftLZ)
            {
                // Build the Microsoft LZ / LZ32 information
                Console.WriteLine("Extracting Microsoft LZ / LZ32 contents");
                Console.WriteLine();

                // Extract using the FileType
                var lz = new BinaryObjectScanner.FileType.MicrosoftLZ();
                lz.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // MoPaQ (MPQ) archive
            else if (ft == WrapperType.MoPaQ)
            {
                // Build the cabinet information
                Console.WriteLine("Extracting MoPaQ contents");
                Console.WriteLine();

#if NET20 || NET35 || NET40 || !WIN
                Console.WriteLine("Extraction is not supported for this framework!");
                Console.WriteLine();
#else
                // Extract using the FileType
                var mpq = new BinaryObjectScanner.FileType.MPQ();
                mpq.Extract(stream, file, outputDirectory, includeDebug: true);
#endif
            }

            // PAK
            else if (ft == WrapperType.PAK)
            {
                // Build the archive information
                Console.WriteLine("Extracting PAK contents");
                Console.WriteLine();

                // Extract using the FileType
                var pak = new BinaryObjectScanner.FileType.PAK();
                pak.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // PFF
            else if (ft == WrapperType.PFF)
            {
                // Build the archive information
                Console.WriteLine("Extracting PFF contents");
                Console.WriteLine();

                // Extract using the FileType
                var pff = new BinaryObjectScanner.FileType.PFF();
                pff.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // PKZIP
            else if (ft == WrapperType.PKZIP)
            {
                // Build the archive information
                Console.WriteLine("Extracting PKZIP contents");
                Console.WriteLine();

#if NET20 || NET35 || NET40 || NET452
                Console.WriteLine("Extraction is not supported for this framework!");
                Console.WriteLine();
#else
                // Extract using the FileType
                var pkzip = new BinaryObjectScanner.FileType.PKZIP();
                pkzip.Extract(stream, file, outputDirectory, includeDebug: true);
#endif
            }

            // Quantum
            else if (ft == WrapperType.Quantum)
            {
                // Build the archive information
                Console.WriteLine("Extracting Quantum contents");
                Console.WriteLine();

                // Extract using the FileType
                var quantum = new BinaryObjectScanner.FileType.Quantum();
                quantum.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // RAR
            else if (ft == WrapperType.RAR)
            {
                // Build the archive information
                Console.WriteLine("Extracting RAR contents");
                Console.WriteLine();

#if NET20 || NET35 || NET40 || NET452
                Console.WriteLine("Extraction is not supported for this framework!");
                Console.WriteLine();
#else
                // Extract using the FileType
                var rar = new BinaryObjectScanner.FileType.RAR();
                rar.Extract(stream, file, outputDirectory, includeDebug: true);
#endif
            }

            // SGA
            else if (ft == WrapperType.SGA)
            {
                // Build the archive information
                Console.WriteLine("Extracting SGA contents");
                Console.WriteLine();

                // Extract using the FileType
                var sga = new BinaryObjectScanner.FileType.SGA();
                sga.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // Tape Archive
            else if (ft == WrapperType.TapeArchive)
            {
                // Build the archive information
                Console.WriteLine("Extracting Tape Archive contents");
                Console.WriteLine();

#if NET20 || NET35 || NET40 || NET452
                Console.WriteLine("Extraction is not supported for this framework!");
                Console.WriteLine();
#else
                // Extract using the FileType
                var tar = new BinaryObjectScanner.FileType.TapeArchive();
                tar.Extract(stream, file, outputDirectory, includeDebug: true);
#endif
            }

            // VBSP
            else if (ft == WrapperType.VBSP)
            {
                // Build the archive information
                Console.WriteLine("Extracting VBSP contents");
                Console.WriteLine();

                // Extract using the FileType
                var vbsp = new BinaryObjectScanner.FileType.VBSP();
                vbsp.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // VPK
            else if (ft == WrapperType.VPK)
            {
                // Build the archive information
                Console.WriteLine("Extracting VPK contents");
                Console.WriteLine();

                // Extract using the FileType
                var vpk = new BinaryObjectScanner.FileType.VPK();
                vpk.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // WAD
            else if (ft == WrapperType.WAD)
            {
                // Build the archive information
                Console.WriteLine("Extracting WAD contents");
                Console.WriteLine();

                // Extract using the FileType
                var wad = new BinaryObjectScanner.FileType.WAD();
                wad.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // xz
            else if (ft == WrapperType.XZ)
            {
                // Build the xz information
                Console.WriteLine("Extracting xz contents");
                Console.WriteLine();

#if NET20 || NET35 || NET40 || NET452
                Console.WriteLine("Extraction is not supported for this framework!");
                Console.WriteLine();
#else
                // Extract using the FileType
                var xz = new BinaryObjectScanner.FileType.XZ();
                xz.Extract(stream, file, outputDirectory, includeDebug: true);
#endif
            }

            // XZP
            else if (ft == WrapperType.XZP)
            {
                // Build the archive information
                Console.WriteLine("Extracting XZP contents");
                Console.WriteLine();

                // Extract using the FileType
                var xzp = new BinaryObjectScanner.FileType.XZP();
                xzp.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // Everything else
            else
            {
                Console.WriteLine("Not a supported extractable file format, skipping...");
                Console.WriteLine();
                return;
            }
        }
    }
}