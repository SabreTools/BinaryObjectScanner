using System;
using System.IO;
using BinaryObjectScanner.FileType;
using BinaryObjectScanner.Packer;
using SabreTools.IO.Extensions;
using WrapperFactory = SabreTools.Serialization.Wrappers.WrapperFactory;
using WrapperType = SabreTools.Serialization.Wrappers.WrapperType;

namespace PackerScan
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

            // Detect packers on the input file
            DetectPackers(options.InputPath, options.ScanArchives, options.ScanInstallers, options.ScanOthers, options.Debug);
        }

        /// <summary>
        /// Wrapper to detect packers for a single path
        /// </summary>
        /// <param name="path">File or directory path</param>
        /// <param name="includeDebug">Enable including debug information</param>
        private static void DetectPackers(string? file, bool scanArchives, bool scanInstallers, bool scanOthers, bool includeDebug)
        {
            if (file == null)
            {
                return;
            }
            
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

            if (ft == WrapperType.Executable)
            {
                // Checking for packers
                var exe = WrapperFactory.CreateExecutableWrapper(stream);

                if (exe == null || exe is not SabreTools.Serialization.Wrappers.PortableExecutable pex)
                {
                    Console.WriteLine("[WARNING] Only portable executables are supported");
                    Console.WriteLine();
                    return;
                }
                
                string? match;

                // ASPack
                var aspack = new ASPack();
                match = aspack.CheckExecutable(file, pex, includeDebug);
                if (match != null)
                {
                    Console.WriteLine(match);
                }

                // CEXE
                var cexe = new CExe();
                match = cexe.CheckExecutable(file, pex, includeDebug);
                if (match != null)
                {
                    Console.WriteLine(match);
                }

                // Crunch
                var crunch = new Crunch();
                match = crunch.CheckExecutable(file, pex, includeDebug);
                if (match != null)
                {
                    Console.WriteLine(match);
                }

                // DotFuscator
                var dotfuscator = new DotFuscator();
                match = dotfuscator.CheckExecutable(file, pex, includeDebug);
                if (match != null)
                {
                    Console.WriteLine(match);
                }

                // DotNetReactor
                var dotnetreactor = new DotNetReactor();
                match = dotnetreactor.CheckExecutable(file, pex, includeDebug);
                if (match != null)
                {
                    Console.WriteLine(match);
                }

                // EXEStealth
                var exestealth = new EXEStealth();
                match = exestealth.CheckExecutable(file, pex, includeDebug);
                if (match != null)
                {
                    Console.WriteLine(match);
                }

                // HyperTechCrackProof
                var htcp = new HyperTechCrackProof();
                match = htcp.CheckExecutable(file, pex, includeDebug);
                if (match != null)
                {
                    Console.WriteLine(match);
                }

                // NeoLite
                var neolite = new NeoLite();
                match = neolite.CheckExecutable(file, pex, includeDebug);
                if (match != null)
                {
                    Console.WriteLine(match);
                }

                // PECompact
                var pecompact = new PECompact();
                match = pecompact.CheckExecutable(file, pex, includeDebug);
                if (match != null)
                {
                    Console.WriteLine(match);
                }

                // PEtite
                var petite = new PEtite();
                match = petite.CheckExecutable(file, pex, includeDebug);
                if (match != null)
                {
                    Console.WriteLine(match);
                }

                // Shrinker
                var shrinker = new Shrinker();
                match = shrinker.CheckExecutable(file, pex, includeDebug);
                if (match != null)
                {
                    Console.WriteLine(match);
                }

                // UPX
                var upx = new UPX();
                match = upx.CheckExecutable(file, pex, includeDebug);
                if (match != null)
                {
                    Console.WriteLine(match);
                }

                if (scanArchives == true)
                {
                    // Checking for self-extracting archives

                    // MicrosoftCABSFX
                    var mscabsfx = new MicrosoftCABSFX();
                    match = mscabsfx.CheckExecutable(file, pex, includeDebug);
                    if (match != null)
                    {
                        Console.WriteLine(match);
                    }

                    // SevenZipSFX
                    var sevenzipsfx = new SevenZipSFX();
                    match = sevenzipsfx.CheckExecutable(file, pex, includeDebug);
                    if (match != null)
                    {
                        Console.WriteLine(match);
                    }

                    // WinRARSFX
                    var winrarsfx = new WinRARSFX();
                    match = winrarsfx.CheckExecutable(file, pex, includeDebug);
                    if (match != null)
                    {
                        Console.WriteLine(match);
                    }

                    // WinZipSFX
                    var winzipsfx = new WinZipSFX();
                    match = winzipsfx.CheckExecutable(file, pex, includeDebug);
                    if (match != null)
                    {
                        Console.WriteLine(match);
                    }
                }

                if (scanInstallers == true)
                {
                    // Checking for installers

                    // AdvancedInstaller
                    var advancedinstaller = new AdvancedInstaller();
                    match = advancedinstaller.CheckExecutable(file, pex, includeDebug);
                    if (match != null)
                    {
                        Console.WriteLine(match);
                    }

                    // GenteeInstaller
                    var genteeinstaller = new GenteeInstaller();
                    match = genteeinstaller.CheckExecutable(file, pex, includeDebug);
                    if (match != null)
                    {
                        Console.WriteLine(match);
                    }

                    // InnoSetup
                    var innosetup = new InnoSetup();
                    match = innosetup.CheckExecutable(file, pex, includeDebug);
                    if (match != null)
                    {
                        Console.WriteLine(match);
                    }

                    // InstallAnywhere
                    var installanywhere = new InstallAnywhere();
                    match = installanywhere.CheckExecutable(file, pex, includeDebug);
                    if (match != null)
                    {
                        Console.WriteLine(match);
                    }

                    // InstallerVISE
                    var installervise = new InstallerVISE();
                    match = installervise.CheckExecutable(file, pex, includeDebug);
                    if (match != null)
                    {
                        Console.WriteLine(match);
                    }

                    // IntelInstallationFramework
                    var intelif = new IntelInstallationFramework();
                    match = intelif.CheckExecutable(file, pex, includeDebug);
                    if (match != null)
                    {
                        Console.WriteLine(match);
                    }

                    // NSIS
                    var nsis = new NSIS();
                    match = nsis.CheckExecutable(file, pex, includeDebug);
                    if (match != null)
                    {
                        Console.WriteLine(match);
                    }

                    // SetupFactory
                    var setupfactory = new SetupFactory();
                    match = setupfactory.CheckExecutable(file, pex, includeDebug);
                    if (match != null)
                    {
                        Console.WriteLine(match);
                    }

                    // WiseInstaller
                    var wiseinstaller = new WiseInstaller();
                    match = wiseinstaller.CheckExecutable(file, pex, includeDebug);
                    if (match != null)
                    {
                        Console.WriteLine(match);
                    }
                }

                if (scanOthers == true)
                {
                    // Checking for other things like embedded archives or executables

                    // AutoPlayMediaStudio
                    var apmstudio = new AutoPlayMediaStudio();
                    match = apmstudio.CheckExecutable(file, pex, includeDebug);
                    if (match != null)
                    {
                        Console.WriteLine(match);
                    }

                    // EmbeddedArchive
                    var embeddedarchive = new EmbeddedArchive();
                    match = embeddedarchive.CheckExecutable(file, pex, includeDebug);
                    if (match != null)
                    {
                        Console.WriteLine(match);
                    }

                    // EmbeddedExecutable
                    var embeddedexecutable = new EmbeddedExecutable();
                    match = embeddedexecutable.CheckExecutable(file, pex, includeDebug);
                    if (match != null)
                    {
                        Console.WriteLine(match);
                    }
                }
            }
            
            // Not a Portable Executable
            else
            {
                Console.WriteLine("[ERROR] Not a Portable Executable");
                Console.WriteLine();
                return;
            }
        }
    }
}
