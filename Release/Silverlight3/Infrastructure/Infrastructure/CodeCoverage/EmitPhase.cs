// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.CodeCoverage
{
    /// <summary>
    /// Write out the instrumented binary.
    /// </summary>
    public class EmitPhase : Phx.Phases.Phase
    {
        /// <summary>
        /// Creates a new instance of the EmitPhase class.
        /// </summary>
        /// <param name="config">The phase list container.</param>
        /// <returns>New EmitPhase object.</returns>
        public static EmitPhase New(Phx.Phases.PhaseConfiguration config)
        {
            EmitPhase emitPhase = new EmitPhase();
            emitPhase.Initialize(config, "Emit");
            return emitPhase;
        }

        /// <summary>
        /// Write out the instrumented binary.
        /// </summary>
        /// <param name="unit">Instrumented unit.</param>
        protected override void Execute(Phx.Unit unit)
        {
            if (!unit.IsPEModuleUnit)
            {
                return;
            }

            // Write the instrumented binary
            Phx.PEModuleUnit executable = unit.AsPEModuleUnit;
            Phx.PE.Writer writer = Phx.PE.Writer.New(
                Phx.GlobalData.GlobalLifetime,
                Application.Output.GetValue(null),
                null, // Writing an instrumented PDB currently crashes
                executable,
                executable.SymbolTable,
                executable.Architecture,
                executable.Runtime);
            writer.Write();
        }
    }
}