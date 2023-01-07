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
        public static void PrintPathInfo(string path)
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
                if (ft == SupportedFileType.UNKNOWN)
                    ft = BurnOutSharp.Tools.Utilities.GetFileType(Path.GetExtension(file).TrimStart('.'));

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
                    else if (magic.StartsWith(BurnOutSharp.Models.LinearExecutable.Constants.LESignatureBytes)
                        || magic.StartsWith(BurnOutSharp.Models.LinearExecutable.Constants.LXSignatureBytes))
                    {
                        Console.WriteLine($"Linear executable found. No parsing currently available.");
                        Console.WriteLine();
                        return;
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
    }
}