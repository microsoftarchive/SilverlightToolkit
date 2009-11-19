// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace Microsoft.CodeCoverage
{
    /// <summary>
    /// Application to instrument an assembly for code coverage.
    /// </summary>
    public static class Application
    {
        /// <summary>
        /// Gets or sets the name of the executable file to process.
        /// </summary>
        public static Phx.Controls.StringControl Input { get; set; }

        /// <summary>
        /// Gets or sets the name of the output binary.
        /// </summary>
        public static Phx.Controls.StringControl Output { get; set; }

        /// <summary>
        /// Gets or sets the name of the output pdb.
        /// </summary>
        public static Phx.Controls.StringControl PdbOut { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a block file is being used
        /// for a set of coverage data.
        /// </summary>
        public static bool UsingBlockFile { get; set; }

        /// <summary>
        /// File with block count.
        /// </summary>
        private const string BlockFile = "BlockCount.txt";

#if VS2010
        /// <summary>
        /// Backing store for the preferred CLR runtime version.
        /// </summary>
        private static string PreferredClrVersion;
#endif

        /// <summary>
        /// Retrieves an application setting value that is required.
        /// </summary>
        /// <param name="propertyName">The property key.</param>
        /// <returns>Returns the property value.</returns>
        private static string ReadRequiredConfigurationProperty(string propertyName)
        {
            string p = ConfigurationManager.AppSettings[propertyName];
            if (string.IsNullOrEmpty(p))
            {
                throw new ArgumentNullException(string.Format(CultureInfo.CurrentCulture, "The application configuration property of key {0} is required for this program to function.", propertyName));
            }

            return p;
        }

        /// <summary>
        /// Entry point for application.
        /// </summary>
        /// <param name="arguments">Command line argument strings.</param>
        /// <returns>Exit code.</returns>
        public static int Main(string[] arguments)
        {
            HardBlock();

            // Read configurable instrumentation assembly details
            InstrumentPhase.CoverageVisitAssembly = ReadRequiredConfigurationProperty("CoverageVisitAssembly");
            InstrumentPhase.CoverageVisitAssemblyVersion = new Version(ReadRequiredConfigurationProperty("CoverageVisitAssemblyVersion"));
            InstrumentPhase.CoverageVisitAssemblyPublicKey = ReadRequiredConfigurationProperty("CoverageVisitAssemblyKey");

            // Method details
            InstrumentPhase.CoverageVisitType = ReadRequiredConfigurationProperty("CoverageVisitType");
            InstrumentPhase.CoverageVisitMethod = ReadRequiredConfigurationProperty("CoverageVisitMethod");

            // CLR details
#if VS2010
            PreferredClrVersion = ReadRequiredConfigurationProperty("PreferredClrRuntimeVersion");
#endif

            string useBlockFile = ConfigurationManager.AppSettings["UseBlockFile"];
            if (useBlockFile != null)
            {
                bool usingBlockFile;
                if (bool.TryParse(useBlockFile, out usingBlockFile))
                {
                    UsingBlockFile = usingBlockFile;
                }
            }

            // Either merge coverage data or instrument an assembly (we check
            // for merges first because Phoenix has its own command line parser)
            if (arguments != null &&
                arguments.Length == 5 &&
                string.Compare(arguments[0], "/merge", StringComparison.OrdinalIgnoreCase) == 0)
            {
                string blocks = arguments[1];
                if (!File.Exists(blocks))
                {
                    Console.WriteLine("File \"{0}\" does not exist!", blocks);
                    return -1;
                }

                string data = arguments[2];
                if (!File.Exists(data))
                {
                    Console.WriteLine("File \"{0}\" does not exist!", blocks);
                    return -1;
                }

                string finalBlocks = arguments[3];
                string pathPrefix = arguments[4];
                MergeCoverageData(blocks, data, finalBlocks, pathPrefix);
                return 0;
            }
            else
            {
                // Instrument the assembly
                uint blockCounter = 0;

                Application.StaticInitialize(arguments);

                if (File.Exists(BlockFile))
                {
                    blockCounter = Convert.ToUInt32(File.ReadAllText(BlockFile), CultureInfo.InvariantCulture);
                    InstrumentPhase.BlockCounter = blockCounter;
                }

                Phx.Term.Mode termMode = Application.Process();

                blockCounter = InstrumentPhase.BlockCounter;
                if (UsingBlockFile)
                {
                    File.WriteAllText(BlockFile, blockCounter.ToString(CultureInfo.InvariantCulture));
                }

                Phx.Term.All(termMode);
                return (termMode == Phx.Term.Mode.Normal ? 0 : 1);
            }
        }

        /// <summary>
        /// Initialize the Phoenix framework and instrumentor.
        /// </summary>
        /// <param name="arguments">Array of command line arguments.</param>
        public static void StaticInitialize(string[] arguments)
        {
            Application.InitializeTargets();
            Phx.Initialize.BeginInitialization();
            Application.InitializeCommandLine();
            InstrumentPhase.Initialize();
            Phx.Initialize.EndInitialization("PHX|*|_PHX_", arguments);

            if (!Application.CheckCommandLine())
            {
                Application.Usage();
                Phx.Term.All(Phx.Term.Mode.Fatal);
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Write the usage string for the tool.
        /// </summary>
        /// <remarks>
        /// Options can include any of the standard phoenix controls, eg
        /// -dumptypes.
        /// </remarks>
        public static void Usage()
        {
            string name = Environment.GetCommandLineArgs()[0];
            Phx.Output.WriteLine("{0} /out <filename> /pdbout <filename> /in <image-name>", name);
            Phx.Output.WriteLine("{0} /merge Coverage.xml RawCodeCoverage.txt", name);
        }

        /// <summary>
        /// Checks the command line against the required options.
        /// </summary>
        /// <returns>
        /// True if the command line has all the required options,
        /// false otherwise.
        /// </returns>
        private static bool CheckCommandLine()
        {
            return !(string.IsNullOrEmpty(Application.Input.GetValue(null)) ||
                    string.IsNullOrEmpty(Application.Output.GetValue(null)) ||
                    string.IsNullOrEmpty(Application.PdbOut.GetValue(null)));
        }

        /// <summary>
        /// Registers string controls with Phoenix for command line option
        /// processing.
        /// </summary>
        private static void InitializeCommandLine()
        {
            // Initialize each command line option (string controls), so that
            // the framework knows about them.
            Application.Input = Phx.Controls.StringControl.New("in", "input file", Phx.Controls.Control.MakeFileLineLocationString("Application.cs", 89));
            Application.Output = Phx.Controls.StringControl.New("out", "output file", Phx.Controls.Control.MakeFileLineLocationString("Application.cs", 90));
            Application.PdbOut = Phx.Controls.StringControl.New("pdbout", "output pdb file", Phx.Controls.Control.MakeFileLineLocationString("Application.cs", 91));
        }

        /// <summary>
        /// Initialize the dependent architectures and runtimes.
        /// </summary>
        private static void InitializeTargets()
        {
            Phx.Targets.Architectures.Architecture msilArch = Phx.Targets.Architectures.Msil.Architecture.New();
            Phx.Targets.Runtimes.Runtime win32MSILRuntime = Phx.Targets.Runtimes.Vccrt.Win.Msil.Runtime.New(msilArch);
            Phx.GlobalData.RegisterTargetArchitecture(msilArch);
            Phx.GlobalData.RegisterTargetRuntime(win32MSILRuntime);
        }

        /// <summary>
        /// Instrument the assembly.
        /// </summary>
        /// <returns>Termination mode of the processing.</returns>
        public static Phx.Term.Mode Process()
        {
            string currentAssembly = Application.Input.GetValue(null);

            // Create the log file with details about the basic blocks
            using (LogWriter log = new LogWriter(Path.ChangeExtension(currentAssembly, "Coverage.xml")))
            {
                log.Start();

                Phx.Output.WriteLine("Instrumenting code coverage for " + currentAssembly + " ...");
                log.StartAssembly(currentAssembly);

                // Get the architecture and runtime from the existing assembly
                Phx.PEModuleUnit oldModule = Phx.PEModuleUnit.Open(currentAssembly);
                Phx.Targets.Architectures.Architecture architecture = oldModule.Architecture;
                Phx.Targets.Runtimes.Runtime runtime = oldModule.Runtime;
#if VS2010
                string clrVersion = oldModule.ClrVersionString;
#endif
                oldModule.Close();
                oldModule.Delete();

                // Create an empty program to contain the instrumented code
                Phx.Lifetime lifetime = Phx.Lifetime.New(Phx.LifetimeKind.Global, null);
                Phx.ProgramUnit program = Phx.ProgramUnit.New(
                    lifetime,
                    null,
                    Phx.GlobalData.TypeTable,
                    architecture,
                    runtime);
                Phx.PEModuleUnit module = Phx.PEModuleUnit.New(
                    lifetime,
                    Phx.Name.New(lifetime, Path.GetFullPath(currentAssembly)),
                    program,
                    Phx.GlobalData.TypeTable,
                    architecture,
                    runtime);

                // Set to metadata version 2 if none was copied
#if VS2010
                if (clrVersion == null)
                {
                    clrVersion = PreferredClrVersion;
                    module.ClrVersionString = clrVersion;
                }
#endif

                // Dev10 Phoenix seems to require this fix
#if VS2010
                module.RaiseMsilOnly = true;
#endif

                // Create the phase list:
                // 1. For each function
                //    a. Raise the binary executable code to LIR
                //    b. Instrument function with code coverage calls
                //    c. Encode the instrumented code
                // 2. Emit the instrumented program as a binary
                Phx.Phases.PhaseConfiguration phases = Phx.Phases.PhaseConfiguration.New(lifetime, "CodeCoverage Phases");
                phases.PhaseList.AppendPhase(Phx.PE.ReaderPhase.New(phases));
                Phx.Phases.PhaseList functionPhases = Phx.PE.UnitListPhaseList.New(phases, Phx.PE.UnitListWalkOrder.PrePass);
                functionPhases.AppendPhase(Phx.PE.RaiseIRPhase.New(phases, Phx.FunctionUnit.LowLevelIRBeforeLayoutFunctionUnitState));
                functionPhases.AppendPhase(InstrumentPhase.New(phases, log));
                functionPhases.AppendPhase(EncodePhase.New(phases));
                functionPhases.AppendPhase(Phx.PE.DiscardIRPhase.New(phases));
                phases.PhaseList.AppendPhase(functionPhases);
                phases.PhaseList.AppendPhase(EmitPhase.New(phases));
                Phx.GlobalData.BuildPlugInPhases(phases);

                // Run Phoenix using our phases
                phases.PhaseList.DoPhaseList(module);

                // Close the log file
                log.EndAssembly();
                log.Close();
            }

            return Phx.Term.Mode.Normal;
        }

        /// <summary>
        /// Merge raw coverage data back into the coverage block file.
        /// </summary>
        /// <param name="blocksPath">Coverage block file.</param>
        /// <param name="dataPath">Raw coverage data.</param>
        /// <param name="finalBlocksFile">Final coverage file.</param>
        /// <param name="pathPrefix">Path prefix for files.</param>
        public static void MergeCoverageData(string blocksPath, string dataPath, string finalBlocksFile, string pathPrefix)
        {
            // Populate the visited block information
            string data = File.ReadAllText(dataPath);
            BitArray blocks = new BitArray(data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                blocks[i] = data[i] == '1';
            }

            XDocument document = XDocument.Load(blocksPath);
            XDocument mainDoc;

            // Check if this is the first block file in the list of files.
            bool first = false;
            if (!File.Exists(finalBlocksFile))
            {
                first = true;
            }

            if (first)
            {
                // Create new xml document if this is the first file in the list
                // of files
                mainDoc = new XDocument();
                XElement xl = new XElement("CodeCoverage");
                mainDoc.Add(xl);
                mainDoc.Root.SetAttributeValue("pathprefix", pathPrefix);
            }
            else
            {
                mainDoc = XDocument.Load(finalBlocksFile);
            }

            // Merge with the coverage block file
            foreach (XElement block in document.Descendants("block"))
            {
                int id = -1;
                if (!int.TryParse(block.Attribute("id").Value, out id) || id == -1)
                {
                    continue;
                }

                bool visited = (id < blocks.Length) ?
                    blocks[id] :
                    false;
                block.Attribute("visited").Value = visited ? "1" : "0";
            }

            document.Save(blocksPath);
            mainDoc.Root.Add(document.Root.Elements());
            mainDoc.Save(finalBlocksFile);
        }

        /// <summary>
        /// A hard block is needed for .NET 4 due to bugs in the former
        /// instrumentation system.
        /// </summary>
        private static void HardBlock()
        {
            // If this assembly is .NET 4, never hard block. Note that the
            // .NET 3.5 version of the assembly specifically does not have a
            // manifest entry permitting .NET 4 execution, forcing users to the
            // newer build and different set of references.
            bool skipHardBlock = IsNet40Running();
            if (!skipHardBlock)
            {
                skipHardBlock = bool.Parse(ReadRequiredConfigurationProperty("SkipNet40HardBlock"));
            }

            if (!skipHardBlock && IsNet40Installed())
            {
                Console.Error.WriteLine(".NET 4 appears to be installed on this "
                    + "machine. Please use the newer .NET 4 build, "
                    + "CodeCoverage2010.exe, instead. To remove this hard"
                    + "block, edit the .config file for this program.");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Checks whether the current assembly's image runtime version begins
        /// with the string "v4". This is a temporary check that should be
        /// removed from future .NET 4-only and newer builds.
        /// </summary>
        /// <returns>Returns a value indicating whether the current assembly is
        /// a .NET 4 runtime image.</returns>
        private static bool IsNet40Running()
        {
            return Assembly.GetExecutingAssembly().ImageRuntimeVersion.StartsWith("v4", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks whether there are any .NET 4 installations on the machine,
        /// based on a simple directory name check in the 32-bit framework
        /// directory.
        /// </summary>
        /// <returns>Returns a value indicating whether the machine appears to
        /// have .NET 4 installed.</returns>
        private static bool IsNet40Installed()
        {
            string path = Environment.GetEnvironmentVariable("windir");
            path = Path.Combine(path, "Microsoft.NET");
            path = Path.Combine(path, "Framework");

            return (Directory.GetDirectories(path, "v4*").Length > 0);
        }
    }
}