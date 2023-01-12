using System;
using System.IO;
using BurnOutSharp;
using BurnOutSharp.Matching;
using BurnOutSharp.Utilities;
using BurnOutSharp.Wrappers;

namespace Test
{
    internal static class Printer
    {
        /// <summary>
        /// Wrapper to print information for a single path
        /// </summary>
        /// <param name="path">File or directory path</param>
        /// <param name="debug">Enable debug output</param>
        public static void PrintPathInfo(string path, bool debug)
        {
            Console.WriteLine($"Checking possible path: {path}");

            // Check if the file or directory exists
            if (File.Exists(path))
            {
                PrintFileInfo(path, debug);
            }
            else if (Directory.Exists(path))
            {
                foreach (string file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    PrintFileInfo(file, debug);
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
        private static void PrintFileInfo(string file, bool debug)
        {
            Console.WriteLine($"Attempting to print info for {file}");

            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Read the first 8 bytes
                byte[] magic = stream.ReadBytes(8);
                stream.Seek(0, SeekOrigin.Begin);

                // Get the file type
                SupportedFileType ft = BurnOutSharp.Tools.Utilities.GetFileType(magic);
                if (ft == SupportedFileType.UNKNOWN)
                {
                    string extension = Path.GetExtension(file).TrimStart('.');
                    ft = BurnOutSharp.Tools.Utilities.GetFileType(extension);
                }

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
                        if (debug) Console.WriteLine("New EXE header address invalid, skipping additional reading...");
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
                    else if (magic.StartsWith(BurnOutSharp.Models.LinearExecutable.Constants.LESignatureBytes)
                        || magic.StartsWith(BurnOutSharp.Models.LinearExecutable.Constants.LXSignatureBytes))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        var linearExecutable = LinearExecutable.Create(stream);
                        if (linearExecutable == null)
                        {
                            Console.WriteLine("Something went wrong parsing Linear Executable");
                            Console.WriteLine();
                            return;
                        }

                        // Print the executable info to screen
                        linearExecutable.Print();
                    }

                    // Portable Executable
                    else if (magic.StartsWith(BurnOutSharp.Models.PortableExecutable.Constants.SignatureBytes))
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
                        if (debug) Console.WriteLine($"Unrecognized header signature: {BitConverter.ToString(magic).Replace("-", string.Empty)}");
                        Console.WriteLine();
                        return;
                    }
                }

                // AACS Media Key Block
                else if (ft == SupportedFileType.AACSMediaKeyBlock)
                {
                    // Build the AACS MKB information
                    Console.WriteLine("Creating AACS media key block deserializer");
                    Console.WriteLine();

                    var mkb = AACSMediaKeyBlock.Create(stream);
                    if (mkb == null)
                    {
                        Console.WriteLine("Something went wrong parsing AACS media key block");
                        Console.WriteLine();
                        return;
                    }

                    // Print the AACS MKB info to screen
                    mkb.Print();
                }

                // BD+ SVM
                else if (ft == SupportedFileType.BDPlusSVM)
                {
                    // Build the BD+ SVM information
                    Console.WriteLine("Creating BD+ SVM deserializer");
                    Console.WriteLine();

                    var svm = BDPlusSVM.Create(stream);
                    if (svm == null)
                    {
                        Console.WriteLine("Something went wrong parsing BD+ SVM");
                        Console.WriteLine();
                        return;
                    }

                    // Print the BD+ SVM info to screen
                    svm.Print();
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

                // CFB
                else if (ft == SupportedFileType.CFB)
                {
                    // Build the CFB information
                    Console.WriteLine("Creating Compact File Binary deserializer");
                    Console.WriteLine();

                    var cfb = CFB.Create(stream);
                    if (cfb == null)
                    {
                        Console.WriteLine("Something went wrong parsing Compact File Binary");
                        Console.WriteLine();
                        return;
                    }

                    // Print the CFB to screen
                    cfb.Print();
                }

                // CIA
                else if (ft == SupportedFileType.CIA)
                {
                    // Build the CIA information
                    Console.WriteLine("Creating CIA deserializer");
                    Console.WriteLine();

                    var cia = CIA.Create(stream);
                    if (cia == null)
                    {
                        Console.WriteLine("Something went wrong parsing CIA");
                        Console.WriteLine();
                        return;
                    }

                    // Print the CIA info to screen
                    cia.Print();
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

                // N3DS
                else if (ft == SupportedFileType.N3DS)
                {
                    // Build the N3DS information
                    Console.WriteLine("Creating Nintendo 3DS deserializer");
                    Console.WriteLine();

                    var n3ds = N3DS.Create(stream);
                    if (n3ds == null)
                    {
                        Console.WriteLine("Something went wrong parsing Nintendo 3DS");
                        Console.WriteLine();
                        return;
                    }

                    // Print the N3DS info to screen
                    n3ds.Print();
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

                // Nitro
                else if (ft == SupportedFileType.Nitro)
                {
                    // Build the NCF information
                    Console.WriteLine("Creating Nintendo DS/DSi deserializer");
                    Console.WriteLine();

                    var nitro = Nitro.Create(stream);
                    if (nitro == null)
                    {
                        Console.WriteLine("Something went wrong parsing Nintendo DS/DSi");
                        Console.WriteLine();
                        return;
                    }

                    // Print the Nitro info to screen
                    nitro.Print();
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
                    if (debug) Console.WriteLine($"File format found: {ft}");
                    Console.WriteLine("Not a printable file format, skipping...");
                    Console.WriteLine();
                    return;
                }
            }
        }
    }
}