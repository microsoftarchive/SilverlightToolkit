// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.Silverlight.Testing.Harness;
using Microsoft.Silverlight.Testing.Service;

namespace Microsoft.Silverlight.Testing
{
    /// <summary>
    /// The CodeCoverage class is used to collect code coverage information from
    /// assemblies that have been instrumented to call the Visit function at the
    /// beginning of every basic block.
    /// </summary>
    public static partial class CodeCoverage
    {
        /// <summary>
        /// A bit array used to track which basic blocks have been executed.
        /// </summary>
        private static BitArray _blocks = new BitArray(0);

        /// <summary>
        /// A counter of the hit blocks.
        /// </summary>
        private static int _hitBlocks;

        /// <summary>
        /// Record that a basic block is being executed.
        /// </summary>
        /// <param name="id">Id of the basic block.</param>
        public static void Visit(uint id)
        {
            int block = (int) id;
            if (_blocks.Length <= block)
            {
                _blocks.Length = block + 1;
            }

            // Increment the hit blocks.
            if (!_blocks[block])
            {
                _hitBlocks++;
            }

            // Store the hit
            _blocks[block] = true;
        }

        /// <summary>
        /// Gets the current number of hit blocks.
        /// </summary>
        public static int HitBlockCount
        {
            get { return _blocks.Count; }
        }

        /// <summary>
        /// Gets the current size of the blocks counter. This is not actually 
        /// the number of hit blocks, but it should return 0 always except 
        /// when at least one block is hit.
        /// </summary>
        public static int BlockCount
        {
            get { return _blocks.Count; }
        }

        /// <summary>
        /// Get the coverage data serialized as a string for easy transport.
        /// </summary>
        /// <remarks>Previous releases of the framework used a simplistic bit
        /// list, this release updates the coverage reporting endpoint to an
        /// alternate version that ends in base 64. This maintains test runner
        /// compatibility with previous official releases.</remarks>
        /// <returns>Coverage data serialized as a string.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method is more appropriate.")]
        public static string GetCoverageData()
        {
            // Even out the set of bytes stored in the bit array
            int bits = _blocks.Length;
            int r = 8 - bits % 8;
            if (r < 8)
            {
                _blocks.Length += r;
                bits += r;
            }

            // Build bytes from bits
            byte[] bytes = new byte[bits / 8];
            for (int i = 0; i < bits; i += 8)
            {
                byte b = 0;
                for (int bit = 0; bit < 8; bit++)
                {
                    if (_blocks[i + bit])
                    {
                        b |= (byte)(1 << (7 - bit));
                    }
                }
                bytes[i / 8] = b;
            }

            // Serialize as a base 64 string
            return Convert.ToBase64String(bytes);
        }
    }
}
