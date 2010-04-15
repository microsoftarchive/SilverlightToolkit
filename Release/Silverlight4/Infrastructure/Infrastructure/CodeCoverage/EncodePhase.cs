// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.CodeCoverage
{
    /// <summary>
    /// Encode the instrumented IR.
    /// </summary>
    public class EncodePhase : Phx.Phases.Phase
    {
        /// <summary>
        /// Initializes a new instance of the EncodePhase class.
        /// </summary>
        /// <param name="config">The phase list container.</param>
        /// <returns>New EncodePhase object.</returns>
        public static EncodePhase New(Phx.Phases.PhaseConfiguration config)
        {
            EncodePhase encodePhase = new EncodePhase();
            encodePhase.Initialize(config, "Encode");
            return encodePhase;
        }

        /// <summary>
        /// Encode the instrumented IR.
        /// </summary>
        /// <param name="unit">Instrumented unit.</param>
        protected override void Execute(Phx.Unit unit)
        {
            Phx.PE.Writer.EncodeUnit(unit);
        }
    }
}