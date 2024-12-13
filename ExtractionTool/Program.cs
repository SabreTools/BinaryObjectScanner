using System;
using System.IO;
using BinaryObjectScanner.FileType;
using BinaryObjectScanner.Packer;
using SabreTools.IO.Extensions;
using WrapperFactory = SabreTools.Serialization.Wrappers.WrapperFactory;
using WrapperType = SabreTools.Serialization.Wrappers.WrapperType;

namespace ExtractionTool
{
    class Program
    {
        static void Main(string[] args)
        {
#if NET462_OR_GREATER || NETCOREAPP
            // Register the codepages
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
#endif

            // Get the options from the arguments
            var options = Options.ParseOptions(args);

            // If we have an invalid state
            if (options == null)
            {
                Options.DisplayHelp();
                return;
            }

            // Loop through the input paths
            foreach (string inputPath in options.InputPaths)
            {
                ExtractPath(inputPath, options.OutputPath, options.Debug);
            }
        }

        /// <summary>
        /// Wrapper to extract data for a single path
        /// </summary>
        /// <param name="path">File or directory path</param>
        /// <param name="outputDirectory">Output directory path</param>
        /// <param name="includeDebug">Enable including debug information</param>
        private static void ExtractPath(string path, string outputDirectory, bool includeDebug)
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
                int read = stream.Read(magic, 0, 16);
                stream.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return;
            }

            // Get the file type
            WrapperType ft = WrapperFactory.GetFileType(magic, extension);

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
                var sevenZip = new SevenZip();
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
                var bfpk = new BFPK();
                bfpk.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // BSP
            else if (ft == WrapperType.BSP)
            {
                // Build the BSP information
                Console.WriteLine("Extracting BSP contents");
                Console.WriteLine();

                // Extract using the FileType
                var bsp = new BSP();
                bsp.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // bzip2
            else if (ft == WrapperType.BZip2)
            {
                // Build the bzip2 information
                Console.WriteLine("Extracting bzip2 contents");
                Console.WriteLine();

                // Extract using the FileType
                var bzip2 = new BZip2();
                bzip2.Extract(stream, file, outputDirectory, includeDebug: true);
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
                var cfb = new CFB();
                cfb.Extract(stream, file, outputDirectory, includeDebug: true);
#endif
            }

            // Executable
            else if (ft == WrapperType.Executable)
            {
                // Build the executable information
                Console.WriteLine("Extracting executable contents");
                Console.WriteLine();

                // Extract using the FileType
                var exe = WrapperFactory.CreateExecutableWrapper(stream);
                if (exe == null || exe is not SabreTools.Serialization.Wrappers.PortableExecutable pex)
                {
                    Console.WriteLine("Only portable executables are supported");
                    Console.WriteLine();
                    return;
                }

                // 7-zip SFX
                var szsfx = new SevenZipSFX();
                if (szsfx.CheckExecutable(file, pex, includeDebug) != null)
                    szsfx.Extract(file, pex, outputDirectory, includeDebug);

                // CExe
                var ce = new CExe();
                if (ce.CheckExecutable(file, pex, includeDebug) != null)
                    ce.Extract(file, pex, outputDirectory, includeDebug);

                // Embedded archives
                var ea = new EmbeddedArchive();
                if (ea.CheckExecutable(file, pex, includeDebug) != null)
                    ea.Extract(file, pex, outputDirectory, includeDebug);

                // Embedded executables
                var ee = new EmbeddedExecutable();
                if (ee.CheckExecutable(file, pex, includeDebug) != null)
                    ee.Extract(file, pex, outputDirectory, includeDebug);

                // WinRAR SFX
                var wrsfx = new WinRARSFX();
                if (wrsfx.CheckExecutable(file, pex, includeDebug) != null)
                    wrsfx.Extract(file, pex, outputDirectory, includeDebug);

                // WinZip SFX
                var wzsfx = new WinZipSFX();
                if (wzsfx.CheckExecutable(file, pex, includeDebug) != null)
                    wzsfx.Extract(file, pex, outputDirectory, includeDebug);

                // Wise Installer
                var wi = new WiseInstaller();
                if (wi.CheckExecutable(file, pex, includeDebug) != null)
                    wi.Extract(file, pex, outputDirectory, includeDebug);
            }

            // GCF
            else if (ft == WrapperType.GCF)
            {
                // Build the GCF information
                Console.WriteLine("Extracting GCF contents");
                Console.WriteLine();

                // Extract using the FileType
                var gcf = new GCF();
                gcf.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // gzip
            else if (ft == WrapperType.GZIP)
            {
                // Build the gzip information
                Console.WriteLine("Extracting gzip contents");
                Console.WriteLine();

                // Extract using the FileType
                var gzip = new GZIP();
                gzip.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // InstallShield Archive V3 (Z)
            else if (ft == WrapperType.InstallShieldArchiveV3)
            {
                // Build the InstallShield Archive V3 information
                Console.WriteLine("Extracting InstallShield Archive V3 contents");
                Console.WriteLine();

                // Extract using the FileType
                var isav3 = new InstallShieldArchiveV3();
                isav3.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // IS-CAB archive
            else if (ft == WrapperType.InstallShieldCAB)
            {
                // Build the archive information
                Console.WriteLine("Extracting IS-CAB contents");
                Console.WriteLine();

                // Extract using the FileType
                var iscab = new InstallShieldCAB();
                iscab.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // LZ-compressed file, KWAJ variant
            else if (ft == WrapperType.LZKWAJ)
            {
                // Build the KWAJ
                Console.WriteLine("Extracting LZ-compressed file, KWAJ variant contents");
                Console.WriteLine();

                // Extract using the FileType
                var lz = new LZKWAJ();
                lz.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // LZ-compressed file, QBasic variant
            else if (ft == WrapperType.LZQBasic)
            {
                // Build the QBasic
                Console.WriteLine("Extracting LZ-compressed file, QBasic variant contents");
                Console.WriteLine();

                // Extract using the FileType
                var lz = new LZQBasic();
                lz.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // LZ-compressed file, SZDD variant
            else if (ft == WrapperType.LZSZDD)
            {
                // Build the SZDD
                Console.WriteLine("Extracting LZ-compressed file, SZDD variant contents");
                Console.WriteLine();

                // Extract using the FileType
                var lz = new LZSZDD();
                lz.Extract(stream, file, outputDirectory, includeDebug: true);
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
                var mscab = new MicrosoftCAB();
                mscab.Extract(stream, file, outputDirectory, includeDebug: true);
#endif
            }

            // MoPaQ (MPQ) archive
            else if (ft == WrapperType.MoPaQ)
            {
                // Build the cabinet information
                Console.WriteLine("Extracting MoPaQ contents");
                Console.WriteLine();

#if NET20 || NET35 || !WIN
                Console.WriteLine("Extraction is not supported for this framework!");
                Console.WriteLine();
#else
                // Extract using the FileType
                var mpq = new MPQ();
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
                var pak = new PAK();
                pak.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // PFF
            else if (ft == WrapperType.PFF)
            {
                // Build the archive information
                Console.WriteLine("Extracting PFF contents");
                Console.WriteLine();

                // Extract using the FileType
                var pff = new PFF();
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
                var pkzip = new PKZIP();
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
                var quantum = new Quantum();
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
                var rar = new RAR();
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
                var sga = new SGA();
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
                var tar = new TapeArchive();
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
                var vbsp = new VBSP();
                vbsp.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // VPK
            else if (ft == WrapperType.VPK)
            {
                // Build the archive information
                Console.WriteLine("Extracting VPK contents");
                Console.WriteLine();

                // Extract using the FileType
                var vpk = new VPK();
                vpk.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // WAD3
            else if (ft == WrapperType.WAD)
            {
                // Build the archive information
                Console.WriteLine("Extracting WAD3 contents");
                Console.WriteLine();

                // Extract using the FileType
                var wad = new WAD3();
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
                var xz = new XZ();
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
                var xzp = new XZP();
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
