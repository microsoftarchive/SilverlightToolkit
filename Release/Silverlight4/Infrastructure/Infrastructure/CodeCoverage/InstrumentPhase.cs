// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

// #define LogBlockDissassembly

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.CodeCoverage
{
    /// <summary>
    /// This class is responsible for the instrumentation of each method.
    /// </summary>
    public class InstrumentPhase : Phx.Phases.Phase
    {
        /// <summary>
        /// Gets or sets the name of the assembly containing the coverage
        /// visitor method that will be injected before each basic block.
        /// </summary>
        public static string CoverageVisitAssembly { get; set; }

        /// <summary>
        /// Gets or sets the version of the coverage assembly.
        /// </summary>
        public static Version CoverageVisitAssemblyVersion { get; set; }

        /// <summary>
        /// Gets or sets the public key token, if any, for the coverage
        /// assembly.
        /// </summary>
        public static string CoverageVisitAssemblyPublicKey { get; set; }

        /// <summary>
        /// Gets or sets the name of the type containing the coverage visitor
        /// method that will be injected before each basic block.
        /// </summary>
        public static string CoverageVisitType { get; set; }

        /// <summary>
        /// Gets or sets the name of the coverage visitor method that will be
        /// injected before each basic block.
        /// </summary>
        public static string CoverageVisitMethod { get; set; }

        /// <summary>
        /// Gets or sets the number of blocks processed during instrumentation.
        /// </summary>
        public static uint BlockCounter { get; set; }

        /// <summary>
        /// Gets or sets the logger used to dump out map information.
        /// </summary>
        private LogWriter Log { get; set; }

        /// <summary>
        /// Gets or sets a dictionary containing function prototypes that have
        /// already been created.
        /// </summary>
        private static Dictionary<Phx.PEModuleUnit, Phx.Symbols.FunctionSymbol> FunctionSymbols { get; set; }

#if PHX_DEBUG_SUPPORT
        private static Phx.Controls.ComponentControl InstrumentPhaseControl;
#endif

        /// <summary>
        /// Initialize the instrumentation phase.
        /// </summary>
        /// <remarks>
        /// Sets up a component control under debug builds, so that we can dump
        /// IR before/after the phase and so on. In other words, because we
        /// create this control, you can pass in options like -predumpmtrace
        /// that will dump the IR for each method before this phase runs.
        /// </remarks>
        public static void Initialize()
        {
            BlockCounter = 0;
            FunctionSymbols = new Dictionary<Phx.PEModuleUnit, Phx.Symbols.FunctionSymbol>();

#if PHX_DEBUG_SUPPORT
            InstrumentPhase.InstrumentPhaseControl = Phx.Controls.ComponentControl.New("root", "Inject code coverage instrumentation", "InstrumentPhase.cs");
#endif
        }

        /// <summary>
        /// Create a new InstrumentationPhase object.
        /// </summary>
        /// <param name="config">The phase list container.</param>
        /// <param name="logger">
        /// Logger to save information about the basic blocks in the assembly.
        /// </param>
        /// <returns>A new InstrumentationPhase object.</returns>
        public static InstrumentPhase New(Phx.Phases.PhaseConfiguration config, LogWriter logger)
        {
            InstrumentPhase instrumentPhase = new InstrumentPhase();
            instrumentPhase.Initialize(config, "CodeCoverage instrumentation phase");

#if PHX_DEBUG_SUPPORT
            instrumentPhase.PhaseControl = InstrumentPhase.InstrumentPhaseControl;
#endif

            instrumentPhase.Log = logger;
            return instrumentPhase;
        }

        /// <summary>
        /// Inject code coverage information into a function.
        /// </summary>
        /// <param name="unit">The function to instrument.</param>
        protected override void Execute(Phx.Unit unit)
        {
            // Only instrument MSIL functions
            if (!unit.IsFunctionUnit)
            {
                return;
            }
            Phx.FunctionUnit function = unit.AsFunctionUnit;
            if (!function.Architecture.NameString.Equals("Msil"))
            {
                return;
            }

            Phx.PEModuleUnit module = function.ParentUnit.AsPEModuleUnit;
            Phx.Symbols.FunctionSymbol functionSymbol = function.FunctionSymbol;
            Phx.Types.FunctionType functionType = functionSymbol.FunctionType;

            // Create the method signature
            List<string> parameterTypes = new List<string>();
            foreach (Phx.Types.Parameter parameter in functionType.UserDefinedParameters)
            {
                parameterTypes.Add(parameter.Type.ToString());
            }
            string methodName = string.Format(
                CultureInfo.InvariantCulture,
                "{0}({1})",
                functionSymbol.NameString,
                string.Join(", ", parameterTypes.ToArray()));

            // Build the control flow graph for the current function
            function.BuildFlowGraph();
            Phx.Graphs.FlowGraph flow = function.FlowGraph;

            // Log method details
            string typeName = (functionSymbol.EnclosingAggregateType != null) ?
                functionSymbol.EnclosingAggregateType.TypeSymbol.NameString :
                "<Module>";
            Log.StartMethod(
                methodName,
                typeName,
                flow.StartBlock.FirstInstruction.GetFileName(),
                flow.StartBlock.FirstInstruction.GetLineNumber());

            // Create a mapping of the disassembled from instructions to the
            // basic block IDs so we can use them for coverage viewing.
            Dictionary<Phx.IR.Instruction, uint> dissassembly = new Dictionary<Phx.IR.Instruction, uint>();

            // Instrument and log the blocks for the current function.
            Log.StartBlocks();
            foreach (Phx.Graphs.BasicBlock block in flow.BasicBlocks)
            {
                // Find the first real instruction in the block
                Phx.IR.Instruction first = null;
                foreach (Phx.IR.Instruction instruction in block.Instructions)
                {
                    // Save the instructions to be dumped later
                    dissassembly.Add(instruction, BlockCounter);

                    // Ignore instructions that aren't actually "real"
                    if (first == null && instruction.IsReal)
                    {
                        Phx.Common.Opcode opcode = instruction.Opcode as Phx.Common.Opcode;
                        if (opcode == Phx.Common.Opcode.ReturnFinally ||
                            opcode == Phx.Common.Opcode.Leave ||
                            opcode == Phx.Common.Opcode.Unreached ||
                            opcode == Phx.Common.Opcode.ExitTypeFilter)
                        {
                            continue;
                        }

                        first = instruction;
                    }
                }

                // Inject a code coverage visitor call before the first
                // instruction of the basic block
                if (first != null)
                {
                    // Log the basic block
                    Phx.IR.Instruction last = block.LastInstruction;
                    bool skipBlock = first == last;
                    if (!skipBlock)
                    {
                        Log.StartBlock(
                            BlockCounter,
                            first.GetMsilOffset(),
                            first.GetFileName(),
                            first.GetLineNumber(),
                            first.GetColumnNumber(),
                            last.GetLineNumberEnd(),
                            last.GetColumnNumberEnd());
                    }

                    Phx.Symbols.FunctionSymbol coverageVisit = GetFunctionSymbol(module, CoverageVisitAssembly, CoverageVisitType, CoverageVisitMethod);
                    if (!skipBlock)
                    {
                        InjectCoverageCall(function, coverageVisit, BlockCounter, first);
                    }

#if LogBlockDissassembly
                    // Dump the disassembly for the current block
                    Log.StartBlockDisassembly();
                    foreach (Phx.IR.Instruction instruction in block.Instructions)
                    {
                        Log.WriteBlockDisassembly(
                            instruction.GetMsilOffset(),
                            instruction.ToString(),
                            BlockCounter);
                    }
                    Log.EndBlockDisassembly();
#endif
                    if (!skipBlock)
                    {
                        Log.EndBlock();
                    }
                }

                // Increment the number of basic blocks
                BlockCounter++;
            }

            function.DeleteFlowGraph();
            Log.EndBlocks();

            // Dump the disassembly for the current method
            Log.StartMethodDisassembly();
            foreach (KeyValuePair<Phx.IR.Instruction, uint> pair in dissassembly)
            {
                Log.WriteBlockDisassembly(
                    pair.Key.GetMsilOffset(),
                    pair.Key.ToString(),
                    pair.Value);
            }
            Log.EndMethodDisassembly();

            Log.EndMethod();
        }

        /// <summary>
        /// Inject a call to the coverage function with the id of a basic block
        /// before its first instruction.
        /// </summary>
        /// <param name="function">Function being instrumented.</param>
        /// <param name="coverageVisit">
        /// Coverage function to call before each block.
        /// </param>
        /// <param name="block">
        /// The id of the block to pass to the coverage function.
        /// </param>
        /// <param name="blockStartInstruction">
        /// First instruction of the basic block.
        /// </param>
        private static void InjectCoverageCall(Phx.FunctionUnit function, Phx.Symbols.FunctionSymbol coverageVisit, uint block, Phx.IR.Instruction blockStartInstruction)
        {
            Phx.Types.Table typeTable = function.ParentUnit.TypeTable;
            if (function.FlowGraph == null)
            {
                function.BuildFlowGraph();
            }

            // Create the instructions
            Phx.IR.Operand blockOperand = Phx.IR.ImmediateOperand.New(function, typeTable.UInt32Type, (uint)block);
            Phx.IR.Instruction loadBlock = Phx.IR.ValueInstruction.NewUnaryExpression(function, Phx.Targets.Architectures.Msil.Opcode.ldc, typeTable.UInt32Type, blockOperand);
            loadBlock.DestinationOperand.Register = Phx.Targets.Architectures.Msil.Register.SR0;
            Phx.IR.Instruction callCoverage = Phx.IR.CallInstruction.New(function, Phx.Targets.Architectures.Msil.Opcode.call, coverageVisit);

            // Insert the instructions
            blockStartInstruction.InsertBefore(loadBlock);
            callCoverage.AppendSource(loadBlock.DestinationOperand);
            loadBlock.DestinationOperand.BreakExpressionTemporary();
            loadBlock.DebugTag = blockStartInstruction.DebugTag;
            blockStartInstruction.InsertBefore(callCoverage);
        }

        /// <summary>
        /// Converts a public key token string to a byte array. Assumes that the
        /// string is in the proper hex format.
        /// </summary>
        /// <param name="token">The hex string of the public key.</param>
        /// <returns>Returns a byte array for the token, or null.</returns>
        private static byte[] PublicKeyTokenToBytes(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            int c = token.Length;
            byte[] bytes = new byte[c / 2];
            for (int i = 0; i < c; i += 2)
            {
                if (!byte.TryParse(
                    new string(
                        new char[] { token[i], token[i + 1] }),
                    NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture,
                    out bytes[i / 2]))
                {
                    return null;
                }
            }

            return bytes;
        }

        /// <summary>
        /// Get a symbol representing the assembly.
        /// </summary>
        /// <param name="unit">Module calling the assembly.</param>
        /// <param name="assembly">Name of the assembly.</param>
        /// <returns>Symbol representing the assembly.</returns>
        private static Phx.Symbols.AssemblySymbol GetAssemblySymbol(Phx.ModuleUnit unit, string assembly)
        {
            Phx.Symbols.AssemblySymbol assemblySymbol = null;

            // Look for any existing symbol
            foreach (Phx.Symbols.Symbol symbol in unit.SymbolTable.AllSymbols)
            {
                Phx.Symbols.AssemblySymbol s = symbol as Phx.Symbols.AssemblySymbol;
                if (s != null && s.NameString.Equals(assembly))
                {
                    assemblySymbol = s;
                    break;
                }
            }

            if (assemblySymbol == null)
            {
                // Create the assembly manifest
                Phx.Name name = Phx.Name.New(unit.Lifetime, assembly);
                Phx.Version version = Phx.Version.New(unit.Lifetime);

                // A bug in Phx.Version ToString renders this field incorrect in
                // the debugger, FYI.
                version.MajorVersion = (ushort)CoverageVisitAssemblyVersion.Major;
                version.MinorVersion = (ushort)CoverageVisitAssemblyVersion.Minor;
                version.BuildNumber = (ushort)CoverageVisitAssemblyVersion.Build;
                version.RevisionNumber = (ushort)CoverageVisitAssemblyVersion.Revision;

                Phx.Manifest manifest = Phx.Manifest.New(unit.Lifetime);
                manifest.Name = name;
                manifest.HashAlgorithm = 0x00008004;
                manifest.Version = version;

                if (!string.IsNullOrEmpty(CoverageVisitAssemblyPublicKey))
                {
                    byte[] key = PublicKeyTokenToBytes(CoverageVisitAssemblyPublicKey);
                    if (key != null)
                    {
                        Phx.PublicKey pk = Phx.PublicKey.New(unit.Lifetime);
                        pk.IsPublicKeyToken = true;
                        pk.KeyLength = (uint)key.Length;
                        pk.Key = key;
                        manifest.PublicKey = pk;
                    }
                }

                // Create the assembly symbol if not found
                assemblySymbol = Phx.Symbols.AssemblySymbol.New(null, manifest, name, unit.SymbolTable);
            }

            return assemblySymbol;
        }

        /// <summary>
        /// Get a symbol representing a type in the assembly.
        /// </summary>
        /// <param name="unit">Module calling the type.</param>
        /// <param name="type">Name of the type.</param>
        /// <param name="assembly">Assembly containing the type.</param>
        /// <returns>Symbol representing the type.</returns>
        private static Phx.Symbols.MsilTypeSymbol GetTypeSymbol(Phx.ModuleUnit unit, string type, Phx.Symbols.AssemblySymbol assembly)
        {
            Phx.Symbols.MsilTypeSymbol typeSymbol = null;

            // Look for any existing symbol
            foreach (Phx.Symbols.Symbol symbol in unit.SymbolTable.AllSymbols)
            {
                Phx.Symbols.MsilTypeSymbol s = symbol as Phx.Symbols.MsilTypeSymbol;
                if (s != null && s.NameString.Equals(type))
                {
                    typeSymbol = s;
                    break;
                }
            }

            if (typeSymbol == null)
            {
                // Create the symbol if not found
                Phx.Name classTypeName = Phx.Name.New(unit.Lifetime, type);
                typeSymbol = Phx.Symbols.MsilTypeSymbol.New(unit.SymbolTable, classTypeName, 0);

                // Add the type to the assembly's scope
                assembly.InsertInLexicalScope(typeSymbol, classTypeName);
            }

            return typeSymbol;
        }

        /// <summary>
        /// Get a symbol representing a function that takes an unsigned integer
        /// parameter.
        /// </summary>
        /// <param name="unit">Module calling the function.</param>
        /// <param name="assembly">Name of the assembly.</param>
        /// <param name="type">Name of the class.</param>
        /// <param name="method">Name of the function.</param>
        /// <returns>FunctionSymbol representing the function.</returns>
        private static Phx.Symbols.FunctionSymbol GetFunctionSymbol(Phx.PEModuleUnit unit, string assembly, string type, string method)
        {
            // Look the function up in the cache
            Phx.Symbols.FunctionSymbol functionSymbol = null;
            if (FunctionSymbols.TryGetValue(unit, out functionSymbol) && functionSymbol != null)
            {
                return functionSymbol;
            }

            // Create symbols for the assemblies and types
            Phx.Symbols.AssemblySymbol assemblySymbol = GetAssemblySymbol(unit, assembly);
            Phx.Symbols.MsilTypeSymbol typeSymbol = GetTypeSymbol(unit, type, assemblySymbol);

            // Build a symbol reference
            Phx.Types.FunctionTypeBuilder builder = Phx.Types.FunctionTypeBuilder.New(unit.TypeTable);
            builder.Begin();
            builder.CallingConventionKind = Phx.Types.CallingConventionKind.ClrCall;
            builder.AppendReturnParameter(unit.TypeTable.VoidType);
            builder.AppendParameter(unit.TypeTable.UInt32Type);

            // Create the function symbol
            Phx.Name name = Phx.Name.New(unit.Lifetime, method);
            functionSymbol = Phx.Symbols.FunctionSymbol.New(unit.SymbolTable, 0, name, builder.GetFunctionType(), Phx.Symbols.Visibility.GlobalReference);
            typeSymbol.InsertInLexicalScope(functionSymbol, name);

            // Create a type 
            Phx.Types.AggregateType aggregate = Phx.Types.AggregateType.NewDynamicSize(unit.TypeTable, typeSymbol);
            aggregate.IsDefinition = false;
            aggregate.AppendMethodSymbol(functionSymbol);

            // Cache the function
            FunctionSymbols[unit] = functionSymbol;

            return functionSymbol;
        }
    }
}