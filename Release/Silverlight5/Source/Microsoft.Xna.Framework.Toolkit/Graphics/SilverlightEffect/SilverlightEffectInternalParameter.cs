using System;
using System.Diagnostics;

namespace Microsoft.Xna.Framework.Graphics
{
    /// <summary>
    /// Internal parameter class for SilverlightEffect
    /// </summary>
    [DebuggerDisplay("{Name}")]
    internal class SilverlightEffectInternalParameter
    {
        #region Instance Data

        bool isDirty = true;
        readonly GraphicsDevice device;
        Vector4[] data;

        #endregion

        #region Internal properties

        internal int VertexShaderRegisterIndex { get; set; }
        
        internal int PixelShaderRegisterIndex { get; set; }
        
        internal int RegisterCount { get; set; }

        internal Vector4[] Data
        {
            set
            {
                isDirty = true;
                data = value;
            }
        }

        #endregion

        #region Creation

        /// <summary>Creates a new effect parameter</summary>
        internal SilverlightEffectInternalParameter(GraphicsDevice device, string name)
        {
            this.device = device;
            Name = name;
            VertexShaderRegisterIndex = -1;
            PixelShaderRegisterIndex = -1;
            RegisterCount = 1;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Get or set the name of the parameter
        /// </summary>
        public string Name { get; internal set; }
        
        #endregion

        #region Internal methods

        internal void Apply()
        {
            // Checking dirty state
            if (!isDirty)
            {
                return;
            }

            if (data == null)
                return;

            // Compute correct register size
            int size = Math.Min(RegisterCount, data.Length);

            // Transmit to device
            for (int index = 0; index < size; index++)
            {
                if (VertexShaderRegisterIndex >= 0)
                    device.SetVertexShaderConstantFloat4(VertexShaderRegisterIndex + index, ref data[index]);
                if (PixelShaderRegisterIndex >= 0)
                    device.SetPixelShaderConstantFloat4(PixelShaderRegisterIndex + index, ref data[index]);
            }

            isDirty = false;
        }

        #endregion

    }
}
