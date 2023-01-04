using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp;
using BurnOutSharp.Compression;
using BurnOutSharp.Matching;
using BurnOutSharp.Utilities;
using BurnOutSharp.Wrappers;
using OpenMcdf;
using SharpCompress.Archives;
using SharpCompress.Archives.GZip;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Tar;
using SharpCompress.Archives.Zip;
using SharpCompress.Compressors;
using SharpCompress.Compressors.BZip2;
using SharpCompress.Compressors.Xz;
using UnshieldSharp.Archive;
using UnshieldSharp.Cabinet;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // Register the codepages
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Create progress indicator
            var p = new Progress<ProtectionProgress>();
            p.ProgressChanged += Changed;

            // Set initial values for scanner flags
            bool debug = false, archives = true, packers = true, info = false, extract = false;
            string outputPath = string.Empty;
            var inputPaths = new List<string>();

            // Loop through the arguments to get the flags
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                switch (arg)
                {
                    case "-?":
                    case "-h":
                    case "--help":
                        DisplayHelp();
                        Console.WriteLine("Press enter to close the program...");
                        Console.ReadLine();
                        return;

                    case "-d":
                    case "--debug":
                        debug = true;
                        break;

                    case "-na":
                    case "--no-archives":
                        archives = false;
                        break;

                    case "-np":
                    case "--no-packers":
                        packers = false;
                        break;

                    case "-i":
                    case "--info":
                        info = true;
                        break;

                    case "-x":
                    case "--extract":
                        extract = true;
                        break;

                    case "-o":
                    case "--outdir":
                        outputPath = i + 1 < args.Length ? args[++i] : null;
                        break;

                    default:
                        inputPaths.Add(arg);
                        break;
                }
            }

            // If we have no arguments, show the help
            if (inputPaths.Count == 0)
            {
                DisplayHelp();
                Console.WriteLine("Press enter to close the program...");
                Console.ReadLine();
                return;
            }

            // Create scanner for all paths
            var scanner = new Scanner(archives, packers, debug, p);

            // If we have extraction, check the output path exists and is valid
            if (extract)
            {
                // Null or empty output path
                if (string.IsNullOrWhiteSpace(outputPath))
                {
                    Console.WriteLine("Output directory required for extraction!");
                    Console.WriteLine();
                    DisplayHelp();
                    Console.WriteLine("Press enter to close the program...");
                    Console.ReadLine();
                    return;
                }

                // Malformed output path or invalid location
                try
                {
                    outputPath = Path.GetFullPath(outputPath);
                    Directory.CreateDirectory(outputPath);
                }
                catch
                {
                    Console.WriteLine("Output directory could not be created!");
                    Console.WriteLine();
                    DisplayHelp();
                    Console.WriteLine("Press enter to close the program...");
                    Console.ReadLine();
                    return;
                }
            }

            // Loop through the input paths
            foreach (string inputPath in inputPaths)
            {
                if (info)
                    PrintPathInfo(inputPath);
                else if (extract)
                    ExtractPath(inputPath, outputPath);
                else
                    GetAndWriteProtections(scanner, inputPath);
            }

            Console.WriteLine("Press enter to close the program...");
            Console.ReadLine();
        }

        /// <summary>
        /// Display help text
        /// </summary>
        private static void DisplayHelp()
        {
            Console.WriteLine("BurnOutSharp Test Program");
            Console.WriteLine();
            Console.WriteLine("test.exe <options> file|directory ...");
            Console.WriteLine();
            Console.WriteLine("Possible options:");
            Console.WriteLine("-?, -h, --help       Display this help text and quit");
            Console.WriteLine("-d, --debug          Enable debug mode");
            Console.WriteLine("-na, --no-archives   Disable scanning archives");
            Console.WriteLine("-np, --no-packers    Disable scanning for packers");
            Console.WriteLine("-i, --info           Print executable info");
            Console.WriteLine("-x, --extract        Extract archive formats");
            Console.WriteLine("-o, --outdir [PATH]  Set output path for extraction");
        }

        #region Protection

        /// <summary>
        /// Wrapper to get and log protections for a single path
        /// </summary>
        /// <param name="scanner">Scanner object to use</param>
        /// <param name="path">File or directory path</param>
        private static void GetAndWriteProtections(Scanner scanner, string path)
        {
            // An invalid path can't be scanned
            if (!Directory.Exists(path) && !File.Exists(path))
            {
                Console.WriteLine($"{path} does not exist, skipping...");
                return;
            }

            try
            {
                var protections = scanner.GetProtections(path);
                WriteProtectionResultFile(path, protections);
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter(File.OpenWrite($"{DateTime.Now:yyyy-MM-dd_HHmmss}-exception.txt")))
                {
                    sw.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Write the protection results from a single path to file, if possible
        /// </summary>
        /// <param name="path">File or directory path</param>
        /// <param name="protections">Dictionary of protections found, if any</param>
        private static void WriteProtectionResultFile(string path, ConcurrentDictionary<string, ConcurrentQueue<string>> protections)
        {
            if (protections == null)
            {
                Console.WriteLine($"No protections found for {path}");
                return;
            }

            using (var sw = new StreamWriter(File.OpenWrite($"{DateTime.Now:yyyy-MM-dd_HHmmss}.txt")))
            {
                foreach (string key in protections.Keys.OrderBy(k => k))
                {
                    // Skip over files with no protection
                    if (protections[key] == null || !protections[key].Any())
                        continue;

                    string line = $"{key}: {string.Join(", ", protections[key].OrderBy(p => p))}";
                    Console.WriteLine(line);
                    sw.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// Protection progress changed handler
        /// </summary>
        private static void Changed(object source, ProtectionProgress value)
        {
            Console.WriteLine($"{value.Percentage * 100:N2}%: {value.Filename} - {value.Protection}");
        }

        #endregion

        #region Printing

        /// <summary>
        /// Wrapper to print information for a single path
        /// </summary>
        /// <param name="path">File or directory path</param>
        private static void PrintPathInfo(string path)
        {
            Console.WriteLine($"Checking possible path: {path}");

            // Check if the file or directory exists
            if (File.Exists(path))
            {
                PrintFileInfo(path);
            }
            else if (Directory.Exists(path))
            {
                foreach (string file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    PrintFileInfo(file);
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
        private static void PrintFileInfo(string file)
        {
            Console.WriteLine($"Attempting to print info for {file}");

            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Read the first 8 bytes
                byte[] magic = stream.ReadBytes(8);
                stream.Seek(0, SeekOrigin.Begin);

                // Get the file type
                SupportedFileType ft = BurnOutSharp.Tools.Utilities.GetFileType(magic);

                // MS-DOS executable and decendents
                if (ft == SupportedFileType.Executable)
                {
                    // Build the executable information
                    Console.WriteLine("Creating MS-DOS executable builder");
                    Console.WriteLine();

                    var msdos = MSDOS.Create(stream);
                    if (msdos == null)
                    {
                        Console.WriteLine("Something went wrong parsing MS-DOS executable");
                        Console.WriteLine();
                        return;
                    }

                    // Print the executable info to screen
                    msdos.Print();

                    // Check for a valid new executable address
                    if (msdos.NewExeHeaderAddr >= stream.Length)
                    {
                        Console.WriteLine("New EXE header address invalid, skipping additional reading...");
                        Console.WriteLine();
                        return;
                    }

                    // Try to read the executable info
                    stream.Seek(msdos.NewExeHeaderAddr, SeekOrigin.Begin);
                    magic = stream.ReadBytes(4);

                    // New Executable
                    if (magic.StartsWith(BurnOutSharp.Models.NewExecutable.Constants.SignatureBytes))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        var newExecutable = NewExecutable.Create(stream);
                        if (newExecutable == null)
                        {
                            Console.WriteLine("Something went wrong parsing New Executable");
                            Console.WriteLine();
                            return;
                        }

                        // Print the executable info to screen
                        newExecutable.Print();
                    }

                    // Linear Executable
                    if (magic.StartsWith(BurnOutSharp.Models.LinearExecutable.Constants.LESignatureBytes)
                        || magic.StartsWith(BurnOutSharp.Models.LinearExecutable.Constants.LXSignatureBytes))
                    {
                        Console.WriteLine($"Linear executable found. No parsing currently available.");
                        Console.WriteLine();
                        return;
                    }

                    // Portable Executable
                    if (magic.StartsWith(BurnOutSharp.Models.PortableExecutable.Constants.SignatureBytes))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        var portableExecutable = PortableExecutable.Create(stream);
                        if (portableExecutable == null)
                        {
                            Console.WriteLine("Something went wrong parsing Portable Executable");
                            Console.WriteLine();
                            return;
                        }

                        // Print the executable info to screen
                        portableExecutable.Print();
                    }

                    // Unknown
                    else
                    {
                        Console.WriteLine($"Unrecognized header signature: {BitConverter.ToString(magic).Replace("-", string.Empty)}");
                        Console.WriteLine();
                        return;
                    }
                }

                // BFPK archive
                else if (ft == SupportedFileType.BFPK)
                {
                    // Build the BFPK information
                    Console.WriteLine("Creating BFPK deserializer");
                    Console.WriteLine();

                    var bfpk = BFPK.Create(stream);
                    if (bfpk == null)
                    {
                        Console.WriteLine("Something went wrong parsing BFPK archive");
                        Console.WriteLine();
                        return;
                    }

                    // Print the BFPK info to screen
                    bfpk.Print();
                }

                // BSP
                else if (ft == SupportedFileType.BSP)
                {
                    // Build the BSP information
                    Console.WriteLine("Creating BSP deserializer");
                    Console.WriteLine();

                    var bsp = BSP.Create(stream);
                    if (bsp == null)
                    {
                        Console.WriteLine("Something went wrong parsing BSP");
                        Console.WriteLine();
                        return;
                    }

                    // Print the BSP info to screen
                    bsp.Print();
                }

                // GCF
                else if (ft == SupportedFileType.GCF)
                {
                    // Build the GCF information
                    Console.WriteLine("Creating GCF deserializer");
                    Console.WriteLine();

                    var gcf = GCF.Create(stream);
                    if (gcf == null)
                    {
                        Console.WriteLine("Something went wrong parsing GCF");
                        Console.WriteLine();
                        return;
                    }

                    // Print the GCF info to screen
                    gcf.Print();
                }

                // IS-CAB archive
                else if (ft == SupportedFileType.InstallShieldCAB)
                {
                    // Build the archive information
                    Console.WriteLine("Creating IS-CAB deserializer");
                    Console.WriteLine();

                    // TODO: Write and use printing methods
                    Console.WriteLine("IS-CAB archive printing not currently enabled");
                    Console.WriteLine();
                    return;
                }

                // MoPaQ (MPQ) archive
                else if (ft == SupportedFileType.MPQ)
                {
                    // Build the archive information
                    Console.WriteLine("Creating MoPaQ deserializer");
                    Console.WriteLine();

                    // TODO: Write and use printing methods
                    Console.WriteLine("MoPaQ archive printing not currently enabled");
                    Console.WriteLine();
                    return;
                }

                // MS-CAB archive
                else if (ft == SupportedFileType.MicrosoftCAB)
                {
                    // Build the cabinet information
                    Console.WriteLine("Creating MS-CAB deserializer");
                    Console.WriteLine();

                    var cabinet = MicrosoftCabinet.Create(stream);
                    if (cabinet == null)
                    {
                        Console.WriteLine("Something went wrong parsing MS-CAB archive");
                        Console.WriteLine();
                        return;
                    }

                    // Print the cabinet info to screen
                    cabinet.Print();
                }

                // NCF
                else if (ft == SupportedFileType.NCF)
                {
                    // Build the NCF information
                    Console.WriteLine("Creating NCF deserializer");
                    Console.WriteLine();

                    var ncf = NCF.Create(stream);
                    if (ncf == null)
                    {
                        Console.WriteLine("Something went wrong parsing NCF");
                        Console.WriteLine();
                        return;
                    }

                    // Print the NCF info to screen
                    ncf.Print();
                }

                // PAK
                else if (ft == SupportedFileType.PAK)
                {
                    // Build the archive information
                    Console.WriteLine("Creating PAK deserializer");
                    Console.WriteLine();

                    var pak = PAK.Create(stream);
                    if (pak == null)
                    {
                        Console.WriteLine("Something went wrong parsing PAK");
                        Console.WriteLine();
                        return;
                    }

                    // Print the PAK info to screen
                    pak.Print();
                }

                // Quantum
                else if (ft == SupportedFileType.Quantum)
                {
                    // Build the archive information
                    Console.WriteLine("Creating Quantum deserializer");
                    Console.WriteLine();

                    var quantum = Quantum.Create(stream);
                    if (quantum == null)
                    {
                        Console.WriteLine("Something went wrong parsing Quantum");
                        Console.WriteLine();
                        return;
                    }

                    // Print the Quantum info to screen
                    quantum.Print();
                }

                // SGA
                else if (ft == SupportedFileType.SGA)
                {
                    // Build the archive information
                    Console.WriteLine("Creating SGA deserializer");
                    Console.WriteLine();

                    var sga = SGA.Create(stream);
                    if (sga == null)
                    {
                        Console.WriteLine("Something went wrong parsing SGA");
                        Console.WriteLine();
                        return;
                    }

                    // Print the SGA info to screen
                    sga.Print();
                }

                // VBSP
                else if (ft == SupportedFileType.VBSP)
                {
                    // Build the archive information
                    Console.WriteLine("Creating VBSP deserializer");
                    Console.WriteLine();

                    var vbsp = VBSP.Create(stream);
                    if (vbsp == null)
                    {
                        Console.WriteLine("Something went wrong parsing VBSP");
                        Console.WriteLine();
                        return;
                    }

                    // Print the VBSP info to screen
                    vbsp.Print();
                }

                // VPK
                else if (ft == SupportedFileType.VPK)
                {
                    // Build the archive information
                    Console.WriteLine("Creating VPK deserializer");
                    Console.WriteLine();

                    var vpk = VPK.Create(stream);
                    if (vpk == null)
                    {
                        Console.WriteLine("Something went wrong parsing VPK");
                        Console.WriteLine();
                        return;
                    }

                    // Print the VPK info to screen
                    vpk.Print();
                }

                // WAD
                else if (ft == SupportedFileType.WAD)
                {
                    // Build the archive information
                    Console.WriteLine("Creating WAD deserializer");
                    Console.WriteLine();

                    var wad = WAD.Create(stream);
                    if (wad == null)
                    {
                        Console.WriteLine("Something went wrong parsing WAD");
                        Console.WriteLine();
                        return;
                    }

                    // Print the WAD info to screen
                    wad.Print();
                }

                // XZP
                else if (ft == SupportedFileType.XZP)
                {
                    // Build the archive information
                    Console.WriteLine("Creating XZP deserializer");
                    Console.WriteLine();

                    var xzp = XZP.Create(stream);
                    if (xzp == null)
                    {
                        Console.WriteLine("Something went wrong parsing XZP");
                        Console.WriteLine();
                        return;
                    }

                    // Print the XZP info to screen
                    xzp.Print();
                }

                // Everything else
                else
                {
                    Console.WriteLine("Not a recognized file format, skipping...");
                    Console.WriteLine();
                    return;
                }
            }
        }

        #endregion

        #region Extraction

        /// <summary>
        /// Wrapper to extract data for a single path
        /// </summary>
        /// <param name="path">File or directory path</param>
        /// <param name="outputDirectory">Output directory path</param>
        private static void ExtractPath(string path, string outputDirectory)
        {
            Console.WriteLine($"Checking possible path: {path}");

            // Check if the file or directory exists
            if (File.Exists(path))
            {
                ExtractFile(path, outputDirectory);
            }
            else if (Directory.Exists(path))
            {
                foreach (string file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    ExtractFile(file, outputDirectory);
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
        private static void ExtractFile(string file, string outputDirectory)
        {
            Console.WriteLine($"Attempting to extract all files from {file}");

            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Read the first 8 bytes
                byte[] magic = stream.ReadBytes(8);
                stream.Seek(0, SeekOrigin.Begin);

                // Get the file type
                SupportedFileType ft = BurnOutSharp.Tools.Utilities.GetFileType(magic);

                // Executables technically can be "extracted", but let's ignore that
                // TODO: Support executables that include other stuff

                // 7-zip
                if (ft == SupportedFileType.SevenZip)
                {
                    // Build the archive information
                    Console.WriteLine("Extracting 7-zip contents");
                    Console.WriteLine();

                    // If the 7-zip file itself fails
                    try
                    {
                        using (SevenZipArchive sevenZipFile = SevenZipArchive.Open(stream))
                        {
                            foreach (var entry in sevenZipFile.Entries)
                            {
                                // If an individual entry fails
                                try
                                {
                                    // If we have a directory, skip it
                                    if (entry.IsDirectory)
                                        continue;

                                    string tempFile = Path.Combine(outputDirectory, entry.Key);
                                    entry.WriteToFile(tempFile);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Something went wrong extracting 7-zip entry {entry.Key}: {ex}");
                                    Console.WriteLine();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong extracting 7-zip: {ex}");
                        Console.WriteLine();
                    }
                }

                // BFPK archive
                else if (ft == SupportedFileType.BFPK)
                {
                    // Build the BFPK information
                    Console.WriteLine("Extracting BFPK contents");
                    Console.WriteLine();

                    var bfpk = BFPK.Create(stream);
                    if (bfpk == null)
                    {
                        Console.WriteLine("Something went wrong parsing BFPK archive");
                        Console.WriteLine();
                        return;
                    }

                    try
                    {
                        // Extract the BFPK contents to the directory
                        bfpk.ExtractAll(outputDirectory);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong extracting BFPK archive: {ex}");
                        Console.WriteLine();
                    }
                }

                // BSP
                else if (ft == SupportedFileType.BSP)
                {
                    // Build the BSP information
                    Console.WriteLine("Extracting BSP contents");
                    Console.WriteLine();

                    var bsp = BSP.Create(stream);
                    if (bsp == null)
                    {
                        Console.WriteLine("Something went wrong parsing BSP");
                        Console.WriteLine();
                        return;
                    }

                    try
                    {
                        // Extract the BSP contents to the directory
                        bsp.ExtractAllLumps(outputDirectory);
                        bsp.ExtractAllTextures(outputDirectory);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong extracting BSP: {ex}");
                        Console.WriteLine();
                    }
                }

                // bzip2
                else if (ft == SupportedFileType.BZip2)
                {
                    // Build the bzip2 information
                    Console.WriteLine("Extracting bzip2 contents");
                    Console.WriteLine();

                    using (var bz2File = new BZip2Stream(stream, CompressionMode.Decompress, true))
                    {
                        // If an individual entry fails
                        try
                        {
                            string tempFile = Path.Combine(outputDirectory, Guid.NewGuid().ToString());
                            using (FileStream fs = File.OpenWrite(tempFile))
                            {
                                bz2File.CopyTo(fs);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Something went wrong extracting bzip2: {ex}");
                            Console.WriteLine();
                        }
                    }
                }

                // GCF
                else if (ft == SupportedFileType.GCF)
                {
                    // Build the GCF information
                    Console.WriteLine("Extracting GCF contents");
                    Console.WriteLine();

                    var gcf = GCF.Create(stream);
                    if (gcf == null)
                    {
                        Console.WriteLine("Something went wrong parsing GCF");
                        Console.WriteLine();
                        return;
                    }

                    try
                    {
                        // Extract the GCF contents to the directory
                        gcf.ExtractAll(outputDirectory);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong extracting GCF: {ex}");
                        Console.WriteLine();
                    }
                }

                // gzip
                else if (ft == SupportedFileType.GZIP)
                {
                    // Build the gzip information
                    Console.WriteLine("Extracting gzip contents");
                    Console.WriteLine();

                    using (var zipFile = GZipArchive.Open(stream))
                    {
                        foreach (var entry in zipFile.Entries)
                        {
                            // If an individual entry fails
                            try
                            {
                                // If we have a directory, skip it
                                if (entry.IsDirectory)
                                    continue;

                                string tempFile = Path.Combine(outputDirectory, entry.Key);
                                entry.WriteToFile(tempFile);
                            }

                            catch (Exception ex)
                            {
                                Console.WriteLine($"Something went wrong extracting gzip entry {entry.Key}: {ex}");
                                Console.WriteLine();
                            }
                        }
                    }
                }

                // InstallShield Archive V3 (Z)
                else if (ft == SupportedFileType.InstallShieldArchiveV3)
                {
                    // Build the InstallShield Archive V3 information
                    Console.WriteLine("Extracting InstallShield Archive V3 contents");
                    Console.WriteLine();

                    // If the cab file itself fails
                    try
                    {
                        var archive = new InstallShieldArchiveV3(file);
                        foreach (var cfile in archive.Files.Select(kvp => kvp.Value))
                        {
                            // If an individual entry fails
                            try
                            {
                                string tempFile = Path.Combine(outputDirectory, cfile.FullPath);
                                if (!Directory.Exists(Path.GetDirectoryName(tempFile)))
                                    Directory.CreateDirectory(Path.GetDirectoryName(tempFile));

                                (byte[] fileContents, string error) = archive.Extract(cfile.FullPath);
                                if (!string.IsNullOrWhiteSpace(error))
                                    continue;

                                using (FileStream fs = File.OpenWrite(tempFile))
                                {
                                    fs.Write(fileContents, 0, fileContents.Length);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Something went wrong extracting InstallShield Archive V3 entry {cfile.Name}: {ex}");
                                Console.WriteLine();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong extracting InstallShield Archive V3: {ex}");
                        Console.WriteLine();
                    }
                }

                // IS-CAB archive
                else if (ft == SupportedFileType.InstallShieldCAB)
                {
                    // Build the archive information
                    Console.WriteLine("Extracting IS-CAB contents");
                    Console.WriteLine();

                    // If the cab file itself fails
                    try
                    {
                        InstallShieldCabinet cabfile = InstallShieldCabinet.Open(file);
                        for (int i = 0; i < cabfile.FileCount; i++)
                        {
                            // If an individual entry fails
                            try
                            {
                                string filename = cabfile.FileName(i);
                                string tempFile;
                                try
                                {
                                    tempFile = Path.Combine(outputDirectory, filename);
                                }
                                catch
                                {
                                    tempFile = Path.Combine(outputDirectory, $"BAD_FILENAME{i}");
                                }

                                cabfile.FileSave(i, tempFile);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Something went wrong extracting IS-CAB entry {i}: {ex}");
                                Console.WriteLine();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong extracting IS-CAB: {ex}");
                        Console.WriteLine();
                    }
                }

                // Microsoft Cabinet archive
                else if (ft == SupportedFileType.MicrosoftCAB)
                {
                    // Build the cabinet information
                    Console.WriteLine("Extracting MS-CAB contents");
                    Console.WriteLine();

                    var cabinet = MicrosoftCabinet.Create(stream);
                    if (cabinet == null)
                    {
                        Console.WriteLine("Something went wrong parsing MS-CAB archive");
                        Console.WriteLine();
                        return;
                    }

                    try
                    {
                        // Extract the MS-CAB contents to the directory
                        cabinet.ExtractAll(outputDirectory);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong extracting MS-CAB: {ex}");
                        Console.WriteLine();
                    }
                }

                // Microsoft LZ / LZ32
                else if (ft == SupportedFileType.MicrosoftLZ)
                {
                    // Build the Microsoft LZ / LZ32 information
                    Console.WriteLine("Extracting Microsoft LZ / LZ32 contents");
                    Console.WriteLine();

                    // If the LZ file itself fails
                    try
                    {
                        byte[] data = LZ.Decompress(stream);

                        // Create the temp filename
                        string tempFile = "temp.bin";
                        if (!string.IsNullOrEmpty(file))
                        {
                            string expandedFilePath = LZ.GetExpandedName(file, out _);
                            tempFile = Path.GetFileName(expandedFilePath).TrimEnd('\0');
                            if (tempFile.EndsWith(".ex"))
                                tempFile += "e";
                            else if (tempFile.EndsWith(".dl"))
                                tempFile += "l";
                        }

                        tempFile = Path.Combine(outputDirectory, tempFile);

                        // Write the file data to a temp file
                        using (Stream tempStream = File.Open(tempFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                        {
                            tempStream.Write(data, 0, data.Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong extracting Microsoft LZ / LZ32: {ex}");
                        Console.WriteLine();
                    }
                }

#if NET48
                // MoPaQ (MPQ) archive
                else if (ft == SupportedFileType.MPQ)
                {
                    // Build the archive information
                    Console.WriteLine("Extracting MoPaQ contents");
                    Console.WriteLine();

                    // If the MPQ file itself fails
                    try
                    {
                        using (var mpqArchive = new StormLibSharp.MpqArchive(file, FileAccess.Read))
                        {
                            // Try to open the listfile
                            string listfile = null;
                            StormLibSharp.MpqFileStream listStream = mpqArchive.OpenFile("(listfile)");

                            // If we can't read the listfile, we just return
                            if (!listStream.CanRead)
                            {
                                Console.WriteLine("Could not read the listfile, extraction halted!");
                                Console.WriteLine();
                            }

                            // Read the listfile in for processing
                            using (StreamReader sr = new StreamReader(listStream))
                            {
                                listfile = sr.ReadToEnd();
                            }

                            // Split the listfile by newlines
                            string[] listfileLines = listfile.Replace("\r\n", "\n").Split('\n');

                            // Loop over each entry
                            foreach (string sub in listfileLines)
                            {
                                // If an individual entry fails
                                try
                                {
                                    string tempFile = Path.Combine(outputDirectory, sub);
                                    Directory.CreateDirectory(Path.GetDirectoryName(tempFile));
                                    mpqArchive.ExtractFile(sub, tempFile);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Something went wrong extracting MoPaQ entry {sub}: {ex}");
                                    Console.WriteLine();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong extracting MoPaQ: {ex}");
                        Console.WriteLine();
                    }
                }
#endif

                // MSI
                else if (ft == SupportedFileType.MSI)
                {
                    // Build the installer information
                    Console.WriteLine("Extracting MSI contents");
                    Console.WriteLine();

                    // If the MSI file itself fails
                    try
                    {
                        using (CompoundFile msi = new CompoundFile(stream, CFSUpdateMode.ReadOnly, CFSConfiguration.Default))
                        {
                            msi.RootStorage.VisitEntries((e) =>
                            {
                                if (!e.IsStream)
                                    return;

                                var str = msi.RootStorage.GetStream(e.Name);
                                if (str == null)
                                    return;

                                byte[] strData = str.GetData();
                                if (strData == null)
                                    return;

                                string decoded = BurnOutSharp.FileType.MSI.DecodeStreamName(e.Name).TrimEnd('\0');
                                byte[] nameBytes = Encoding.UTF8.GetBytes(e.Name);

                                // UTF-8 encoding of 0x4840.
                                if (nameBytes[0] == 0xe4 && nameBytes[1] == 0xa1 && nameBytes[2] == 0x80)
                                    decoded = decoded.Substring(3);

                                foreach (char c in Path.GetInvalidFileNameChars())
                                {
                                    decoded = decoded.Replace(c, '_');
                                }

                                string filename = Path.Combine(outputDirectory, decoded);
                                using (Stream fs = File.OpenWrite(filename))
                                {
                                    fs.Write(strData, 0, strData.Length);
                                }
                            }, recursive: true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong extracting MSI: {ex}");
                        Console.WriteLine();
                    }
                }

                // PAK
                else if (ft == SupportedFileType.PAK)
                {
                    // Build the archive information
                    Console.WriteLine("Extracting PAK contents");
                    Console.WriteLine();

                    var pak = PAK.Create(stream);
                    if (pak == null)
                    {
                        Console.WriteLine("Something went wrong parsing PAK");
                        Console.WriteLine();
                        return;
                    }

                    try
                    {
                        // Extract the PAK contents to the directory
                        pak.ExtractAll(outputDirectory);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong extracting MS-CAB: {ex}");
                        Console.WriteLine();
                    }
                }

                // PKZIP
                else if (ft == SupportedFileType.PKZIP)
                {
                    // Build the archive information
                    Console.WriteLine("Extracting PKZIP contents");
                    Console.WriteLine();

                    // If the zip file itself fails
                    try
                    {
                        using (ZipArchive zipFile = ZipArchive.Open(stream))
                        {
                            foreach (var entry in zipFile.Entries)
                            {
                                // If an individual entry fails
                                try
                                {
                                    // If we have a directory, skip it
                                    if (entry.IsDirectory)
                                        continue;

                                    string tempFile = Path.Combine(outputDirectory, entry.Key);
                                    Directory.CreateDirectory(Path.GetDirectoryName(tempFile));
                                    entry.WriteToFile(tempFile);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Something went wrong extracting PKZIP entry {entry.Key}: {ex}");
                                    Console.WriteLine();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong extracting PKZIP: {ex}");
                        Console.WriteLine();
                    }
                }

                // RAR
                else if (ft == SupportedFileType.RAR)
                {
                    // Build the archive information
                    Console.WriteLine("Extracting RAR contents");
                    Console.WriteLine();

                    // If the rar file itself fails
                    try
                    {
                        using (RarArchive rarFile = RarArchive.Open(stream))
                        {
                            foreach (var entry in rarFile.Entries)
                            {
                                // If an individual entry fails
                                try
                                {
                                    // If we have a directory, skip it
                                    if (entry.IsDirectory)
                                        continue;

                                    string tempFile = Path.Combine(outputDirectory, entry.Key);
                                    entry.WriteToFile(tempFile);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Something went wrong extracting RAR entry {entry.Key}: {ex}");
                                    Console.WriteLine();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong extracting RAR: {ex}");
                        Console.WriteLine();
                    }
                }

                // SGA
                else if (ft == SupportedFileType.SGA)
                {
                    // Build the archive information
                    Console.WriteLine("Extracting SGA contents");
                    Console.WriteLine();

                    var sga = SGA.Create(stream);
                    if (sga == null)
                    {
                        Console.WriteLine("Something went wrong parsing SGA");
                        Console.WriteLine();
                        return;
                    }

                    try
                    {
                        // Extract the SGA contents to the directory
                        sga.ExtractAll(outputDirectory);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong extracting MS-CAB: {ex}");
                        Console.WriteLine();
                    }
                }

                // Tape Archive
                else if (ft == SupportedFileType.RAR)
                {
                    // Build the archive information
                    Console.WriteLine("Extracting Tape Archive contents");
                    Console.WriteLine();

                    // If the rar file itself fails
                    try
                    {
                        using (TarArchive tarFile = TarArchive.Open(stream))
                        {
                            foreach (var entry in tarFile.Entries)
                            {
                                // If an individual entry fails
                                try
                                {
                                    // If we have a directory, skip it
                                    if (entry.IsDirectory)
                                        continue;

                                    string tempFile = Path.Combine(outputDirectory, entry.Key);
                                    entry.WriteToFile(tempFile);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Something went wrong extracting Tape Archive entry {entry.Key}: {ex}");
                                    Console.WriteLine();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong extracting Tape Archive: {ex}");
                        Console.WriteLine();
                    }
                }

                // VBSP
                else if (ft == SupportedFileType.VBSP)
                {
                    // Build the archive information
                    Console.WriteLine("Extracting VBSP contents");
                    Console.WriteLine();

                    var vbsp = VBSP.Create(stream);
                    if (vbsp == null)
                    {
                        Console.WriteLine("Something went wrong parsing VBSP");
                        Console.WriteLine();
                        return;
                    }

                    try
                    {
                        // Extract the VBSP contents to the directory
                        vbsp.ExtractAllLumps(outputDirectory);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong extracting MS-CAB: {ex}");
                        Console.WriteLine();
                    }
                }

                // VPK
                else if (ft == SupportedFileType.VPK)
                {
                    // Build the archive information
                    Console.WriteLine("Extracting VPK contents");
                    Console.WriteLine();

                    var vpk = VPK.Create(stream);
                    if (vpk == null)
                    {
                        Console.WriteLine("Something went wrong parsing VPK");
                        Console.WriteLine();
                        return;
                    }

                    try
                    {
                        // Extract the VPK contents to the directory
                        vpk.ExtractAll(outputDirectory);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong extracting MS-CAB: {ex}");
                        Console.WriteLine();
                    }
                }

                // WAD
                else if (ft == SupportedFileType.WAD)
                {
                    // Build the archive information
                    Console.WriteLine("Extracting WAD contents");
                    Console.WriteLine();

                    var wad = WAD.Create(stream);
                    if (wad == null)
                    {
                        Console.WriteLine("Something went wrong parsing WAD");
                        Console.WriteLine();
                        return;
                    }

                    try
                    {
                        // Extract the WAD contents to the directory
                        wad.ExtractAllLumps(outputDirectory);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong extracting MS-CAB: {ex}");
                        Console.WriteLine();
                    }
                }

                // xz
                else if (ft == SupportedFileType.RAR)
                {
                    // Build the xz information
                    Console.WriteLine("Extracting xz contents");
                    Console.WriteLine();

                    using (var xzFile = new XZStream(stream))
                    {
                        // If an individual entry fails
                        try
                        {
                            string tempFile = Path.Combine(outputDirectory, Guid.NewGuid().ToString());
                            using (FileStream fs = File.OpenWrite(tempFile))
                            {
                                xzFile.CopyTo(fs);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Something went wrong extracting xz: {ex}");
                            Console.WriteLine();
                        }
                    }
                }

                // XZP
                else if (ft == SupportedFileType.XZP)
                {
                    // Build the archive information
                    Console.WriteLine("Extracting XZP contents");
                    Console.WriteLine();

                    var xzp = XZP.Create(stream);
                    if (xzp == null)
                    {
                        Console.WriteLine("Something went wrong parsing XZP");
                        Console.WriteLine();
                        return;
                    }

                    try
                    {
                        // Extract the XZP contents to the directory
                        xzp.ExtractAll(outputDirectory);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong extracting MS-CAB: {ex}");
                        Console.WriteLine();
                    }
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

        #endregion
    }
}
