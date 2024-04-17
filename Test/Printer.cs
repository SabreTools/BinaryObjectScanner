using System;
using System.IO;
using System.Text;
using BinaryObjectScanner.Utilities;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Wrappers;

namespace Test
{
    internal static class Printer
    {
        /// <summary>
        /// Wrapper to print information for a single path
        /// </summary>
        /// <param name="path">File or directory path</param>
        /// <param name="json">Enable JSON output, if supported</param>
        /// <param name="debug">Enable debug output</param>
        public static void PrintPathInfo(string path, bool json, bool debug)
        {
            Console.WriteLine($"Checking possible path: {path}");

            // Check if the file or directory exists
            if (File.Exists(path))
            {
                PrintFileInfo(path, json, debug);
            }
            else if (Directory.Exists(path))
            {
#if NET20 || NET35
                foreach (string file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
#else
                foreach (string file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
#endif
                {
                    PrintFileInfo(file, json, debug);
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
        private static void PrintFileInfo(string file, bool json, bool debug)
        {
            Console.WriteLine($"Attempting to print info for {file}");

            try
            {
                using Stream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // Read the first 8 bytes
                byte[]? magic = stream.ReadBytes(8);
                stream.Seek(0, SeekOrigin.Begin);

                // Get the file type
                SupportedFileType ft = FileTypes.GetFileType(magic ?? []);
                if (ft == SupportedFileType.UNKNOWN)
                {
                    string extension = Path.GetExtension(file).TrimStart('.');
                    ft = FileTypes.GetFileType(extension);
                }

                // Print out the file format
                Console.WriteLine($"File format found: {ft}");

                // Setup the wrapper to print
                var wrapper = WrapperFactory.CreateWrapper(ft, stream);

                // If we don't have a wrapper
                if (wrapper == null)
                {
                    Console.WriteLine($"Either {ft} is not supported or something went wrong during parsing!");
                    Console.WriteLine();
                    return;
                }

                // Get the base info output name
                string filenameBase = $"info-{DateTime.Now:yyyy-MM-dd_HHmmss.ffff}";

#if NET6_0_OR_GREATER
                // If we have the JSON flag
                if (json)
                {
                    // Create the output data
                    string serializedData = wrapper.ExportJSON();
                    Console.WriteLine(serializedData);

                    // Write the output data
                    using var jsw = new StreamWriter(File.OpenWrite($"{filenameBase}.json"));
                    jsw.WriteLine(serializedData);
                }
#endif

                // Create the output data
                var builder = wrapper.PrettyPrint();
                Console.WriteLine(builder);

                // Write the output data
                using var sw = new StreamWriter(File.OpenWrite($"{filenameBase}.txt"));
                sw.WriteLine(builder.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(debug ? ex : "[Exception opening file, please try again]");
                Console.WriteLine();
            }
        }

        #region Printing Implementations

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this object wrapper)
        {
            return wrapper switch
            {
                AACSMediaKeyBlock item => item.PrettyPrint(),
                BDPlusSVM item => item.PrettyPrint(),
                BFPK item => item.PrettyPrint(),
                BSP item => item.PrettyPrint(),
                CFB item => item.PrettyPrint(),
                CIA item => item.PrettyPrint(),
                GCF item => item.PrettyPrint(),
                InstallShieldCabinet item => item.PrettyPrint(),
                IRD item => item.PrettyPrint(),
                LinearExecutable item => item.PrettyPrint(),
                MicrosoftCabinet item => item.PrettyPrint(),
                MoPaQ item => item.PrettyPrint(),
                MSDOS item => item.PrettyPrint(),
                N3DS item => item.PrettyPrint(),
                NCF item => item.PrettyPrint(),
                NewExecutable item => item.PrettyPrint(),
                Nitro item => item.PrettyPrint(),
                PAK item => item.PrettyPrint(),
                PFF item => item.PrettyPrint(),
                PIC item => item.PrettyPrint(),
                PlayJAudioFile item => item.PrettyPrint(),
                //PlayJPlaylist item => item.PrettyPrint(),
                PortableExecutable item => item.PrettyPrint(),
                Quantum item => item.PrettyPrint(),
                SGA item => item.PrettyPrint(),
                VBSP item => item.PrettyPrint(),
                VPK item => item.PrettyPrint(),
                WAD item => item.PrettyPrint(),
                XeMID item => item.PrettyPrint(),
                XMID item => item.PrettyPrint(),
                XZP item => item.PrettyPrint(),
                _ => new StringBuilder(),
            }; ;
        }

#if NET6_0_OR_GREATER
        /// <summary>
        /// Export the item information as JSON
        /// </summary>
        private static string ExportJSON(this object wrapper)
        {
            return wrapper switch
            {
                AACSMediaKeyBlock item => item.ExportJSON(),
                BDPlusSVM item => item.ExportJSON(),
                BFPK item => item.ExportJSON(),
                BSP item => item.ExportJSON(),
                CFB item => item.ExportJSON(),
                CIA item => item.ExportJSON(),
                GCF item => item.ExportJSON(),
                InstallShieldCabinet item => item.ExportJSON(),
                IRD item => item.ExportJSON(),
                LinearExecutable item => item.ExportJSON(),
                MicrosoftCabinet item => item.ExportJSON(),
                MoPaQ item => item.ExportJSON(),
                MSDOS item => item.ExportJSON(),
                N3DS item => item.ExportJSON(),
                NCF item => item.ExportJSON(),
                NewExecutable item => item.ExportJSON(),
                Nitro item => item.ExportJSON(),
                PAK item => item.ExportJSON(),
                PFF item => item.ExportJSON(),
                PIC item => item.ExportJSON(),
                PlayJAudioFile item => item.ExportJSON(),
                PlayJPlaylist item => item.ExportJSON(),
                PortableExecutable item => item.ExportJSON(),
                Quantum item => item.ExportJSON(),
                SGA item => item.ExportJSON(),
                VBSP item => item.ExportJSON(),
                VPK item => item.ExportJSON(),
                WAD item => item.ExportJSON(),
                XeMID item => item.ExportJSON(),
                XMID item => item.ExportJSON(),
                XZP item => item.ExportJSON(),
                _ => string.Empty,
            };
        }
#endif

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this AACSMediaKeyBlock item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.AACSMediaKeyBlock.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this BDPlusSVM item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.BDPlusSVM.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this BFPK item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.BFPK.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this BSP item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.BSP.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this CFB item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.CFB.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this CIA item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.CIA.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this GCF item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.GCF.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this InstallShieldCabinet item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.InstallShieldCabinet.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this IRD item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.IRD.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this LinearExecutable item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.LinearExecutable.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this MicrosoftCabinet item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.MicrosoftCabinet.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this MoPaQ item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.MoPaQ.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this MSDOS item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.MSDOS.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this N3DS item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.N3DS.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this NCF item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.NCF.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this NewExecutable item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.NewExecutable.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Nitro item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.Nitro.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this PAK item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.PAK.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this PFF item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.PFF.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this PIC item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.PIC.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this PlayJAudioFile item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.PlayJAudioFile.Print(builder, item.Model);
            return builder;
        }

        ///// <summary>
        ///// Export the item information as pretty-printed text
        ///// </summary>
        //private static StringBuilder PrettyPrint(this PlayJPlaylist item)
        //{
        //    Console.WriteLine($"{item.Description()} wrapper created successfully!");
        //    var builder = new StringBuilder();
        //    SabreTools.Printing.PlayJPlaylist.Print(builder, item.Model);
        //    return builder;
        //}

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this PortableExecutable item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.PortableExecutable.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Quantum item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.Quantum.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this SGA item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.SGA.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this VBSP item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.VBSP.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this VPK item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.VPK.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this WAD item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.WAD.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this XeMID item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.XeMID.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this XMID item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.XMID.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this XZP item)
        {
            Console.WriteLine($"{item.Description()} wrapper created successfully!");
            var builder = new StringBuilder();
            SabreTools.Printing.XZP.Print(builder, item.Model);
            return builder;
        }

        #endregion
    }
}