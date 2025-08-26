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
            // Normalize by getting the full path
            path = Path.GetFullPath(path);
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
                if (includeDebug) Console.Error.WriteLine(ex);
                return;
            }

            // TODO: When extractable wrapper types are exposed to this, use them instead of guessing

            // Get the file type
            WrapperType ft = WrapperFactory.GetFileType(magic, extension);
            var wrapper = WrapperFactory.CreateWrapper(ft, stream);

            // Create the output directory
            Directory.CreateDirectory(outputDirectory);

            // 7-zip -- Implementation moved to Serialization
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
            else if (wrapper is SabreTools.Serialization.Wrappers.BFPK bfpk)
            {
                Console.WriteLine("Extracting BFPK contents");
                Console.WriteLine();

                bfpk.ExtractAll(outputDirectory);
            }

            // BSP
            else if (wrapper is SabreTools.Serialization.Wrappers.BSP bsp)
            {
                Console.WriteLine("Extracting BSP contents");
                Console.WriteLine();

                bsp.ExtractAllLumps(outputDirectory);
            }

            // bzip2 -- Implementation moved to Serialization
            else if (ft == WrapperType.BZip2)
            {
                // Build the bzip2 information
                Console.WriteLine("Extracting bzip2 contents");
                Console.WriteLine();

                // Extract using the FileType
                var bzip2 = new BZip2();
                bzip2.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // CFB -- Implementation moved to Serialization
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

            // Executable -- Implementation partially moved to Serialization
            else if (ft == WrapperType.Executable)
            {
                // Build the executable information
                Console.WriteLine("Extracting executable contents");
                Console.WriteLine();

                // Extract using the FileType
                var exe = WrapperFactory.CreateExecutableWrapper(stream);
                if (exe == null)
                    return;

                // New Executable
                if (exe is SabreTools.Serialization.Wrappers.NewExecutable nex)
                {
                    // Wise Installer
                    var wi = new WiseInstaller();
                    if (wi.CheckExecutable(file, nex, includeDebug) != null)
                        wi.Extract(file, nex, outputDirectory, includeDebug);
                }

                // Portable Executable
                else if (exe is SabreTools.Serialization.Wrappers.PortableExecutable pex)
                {
                    // 7-zip SFX
                    var szsfx = new SevenZipSFX();
                    if (szsfx.CheckExecutable(file, pex, includeDebug) != null)
                        szsfx.Extract(file, pex, outputDirectory, includeDebug);

                    // CExe -- Implementation moved to Serialization
                    var ce = new CExe();
                    if (ce.CheckExecutable(file, pex, includeDebug) != null)
                        ce.Extract(file, pex, outputDirectory, includeDebug);

                    // Embedded archives -- Implementation moved to Serialization
                    var ea = new EmbeddedArchive();
                    if (ea.CheckExecutable(file, pex, includeDebug) != null)
                        ea.Extract(file, pex, outputDirectory, includeDebug);

                    // Embedded executables -- Implementation moved to Serialization
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
            }

            // GCF
            else if (wrapper is SabreTools.Serialization.Wrappers.GCF gcf)
            {
                Console.WriteLine("Extracting GCF contents");
                Console.WriteLine();

                gcf.ExtractAll(outputDirectory);
            }

            // gzip -- Implementation moved to Serialization
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
            else if (wrapper is SabreTools.Serialization.Wrappers.InstallShieldArchiveV3 isv3)
            {
                Console.WriteLine("Extracting InstallShield Archive V3 contents");
                Console.WriteLine();

                isv3.ExtractAll(outputDirectory);
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
            else if (wrapper is SabreTools.Serialization.Wrappers.LZKWAJ kwaj)
            {
                Console.WriteLine("Extracting LZ-compressed file, KWAJ variant contents");
                Console.WriteLine();

                kwaj.Extract(outputDirectory);
            }

            // LZ-compressed file, QBasic variant
            else if (wrapper is SabreTools.Serialization.Wrappers.LZQBasic qbasic)
            {
                Console.WriteLine("Extracting LZ-compressed file, QBasic variant contents");
                Console.WriteLine();

                qbasic.Extract(outputDirectory);
            }

            // LZ-compressed file, SZDD variant
            else if (wrapper is SabreTools.Serialization.Wrappers.LZSZDD szdd)
            {
                Console.WriteLine("Extracting LZ-compressed file, SZDD variant contents");
                Console.WriteLine();

                szdd.Extract(Path.GetFileName(file), outputDirectory);
            }

            // Microsoft Cabinet archive -- Implementation moved to Serialization
            else if (ft == WrapperType.MicrosoftCAB)
            {
                // Build the cabinet information
                Console.WriteLine("Extracting MS-CAB contents");
                Console.WriteLine("WARNING: LZX and Quantum compression schemes are not supported so some files may be skipped!");
                Console.WriteLine();

                // Extract using the FileType
                var mscab = new MicrosoftCAB();
                mscab.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // MoPaQ (MPQ) archive
            else if (ft == WrapperType.MoPaQ)
            {
                // Build the cabinet information
                Console.WriteLine("Extracting MoPaQ contents");
                Console.WriteLine();

#if NET20 || NET35 || !(WINX86 || WINX64)
                Console.WriteLine("Extraction is not supported for this framework!");
                Console.WriteLine();
#else
                // Extract using the FileType
                var mpq = new MPQ();
                mpq.Extract(stream, file, outputDirectory, includeDebug: true);
#endif
            }

            // PAK
            else if (wrapper is SabreTools.Serialization.Wrappers.PAK pak)
            {
                Console.WriteLine("Extracting PAK contents");
                Console.WriteLine();

                pak.ExtractAll(outputDirectory);
            }

            // PFF
            else if (wrapper is SabreTools.Serialization.Wrappers.PFF pff)
            {
                Console.WriteLine("Extracting PFF contents");
                Console.WriteLine();

                pff.ExtractAll(outputDirectory);
            }

            // PKZIP -- Implementation moved to Serialization
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

            // Quantum -- Implementation moved to Serialization
            else if (ft == WrapperType.Quantum)
            {
                // Build the archive information
                Console.WriteLine("Extracting Quantum contents");
                Console.WriteLine();

                // Extract using the FileType
                var quantum = new Quantum();
                quantum.Extract(stream, file, outputDirectory, includeDebug: true);
            }

            // RAR -- Implementation moved to Serialization
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
            else if (wrapper is SabreTools.Serialization.Wrappers.SGA sga)
            {
                Console.WriteLine("Extracting SGA contents");
                Console.WriteLine();

                sga.ExtractAll(outputDirectory);
            }

            // Tape Archive -- Implementation moved to Serialization
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
            else if (wrapper is SabreTools.Serialization.Wrappers.VBSP vbsp)
            {
                Console.WriteLine("Extracting VBSP contents");
                Console.WriteLine();

                vbsp.ExtractAllLumps(outputDirectory);
            }

            // VPK
            else if (wrapper is SabreTools.Serialization.Wrappers.VPK vpk)
            {
                Console.WriteLine("Extracting VPK contents");
                Console.WriteLine();

                vpk.ExtractAll(outputDirectory);
            }

            // WAD3
            else if (wrapper is SabreTools.Serialization.Wrappers.WAD3 wad)
            {
                Console.WriteLine("Extracting WAD3 contents");
                Console.WriteLine();

                wad.ExtractAllLumps(outputDirectory);
            }

            // xz -- Implementation moved to Serialization
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
            else if (wrapper is SabreTools.Serialization.Wrappers.XZP xzp)
            {
                Console.WriteLine("Extracting XZP contents");
                Console.WriteLine();

                xzp.ExtractAll(outputDirectory);
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
