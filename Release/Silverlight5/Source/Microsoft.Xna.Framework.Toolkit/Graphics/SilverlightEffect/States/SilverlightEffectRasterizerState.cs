namespace Microsoft.Xna.Framework.Graphics
{
    internal class SilverlightEffectRasterizerState : SilverlightEffectState
    {
        #region Properties

        public CullMode? CullMode { get; set; }
        public FillMode? FillMode { get; set; }
        public bool? ScissorTestEnable { get; set; }
        public bool? MultiSampleAntiAlias { get; set; }
        public float? DepthBias { get; set; }
        public float? SlopeScaleDepthBias { get; set; }

        #endregion

        #region Methods

        public void Affect(GraphicsDevice device, RasterizerState currentState)
        {
            RasterizerState internalState = new RasterizerState();

            // CullMode
            internalState.CullMode = CullMode.HasValue ? CullMode.Value : currentState.CullMode;

            // FillMode
            internalState.FillMode = FillMode.HasValue ? FillMode.Value : currentState.FillMode;

            // ScissorTestEnable
            internalState.ScissorTestEnable = ScissorTestEnable.HasValue ? ScissorTestEnable.Value : currentState.ScissorTestEnable;

            // MultiSampleAntiAlias
            internalState.MultiSampleAntiAlias = MultiSampleAntiAlias.HasValue ? MultiSampleAntiAlias.Value : currentState.MultiSampleAntiAlias;

            // DepthBias
            internalState.DepthBias = DepthBias.HasValue ? DepthBias.Value : currentState.DepthBias;

            // SlopeScaleDepthBias
            internalState.SlopeScaleDepthBias = SlopeScaleDepthBias.HasValue ? SlopeScaleDepthBias.Value : currentState.SlopeScaleDepthBias;

            // Finally apply the state
            device.RasterizerState = internalState;
        }

        public override void ProcessState(GraphicsDevice device)
        {
            RasterizerState currentState = device.RasterizerState;

            // CullMode
            if (CullMode.HasValue && CullMode.Value != currentState.CullMode)
            {
                Affect(device, currentState);
                return;
            }

            // FillMode
            if (FillMode.HasValue && FillMode.Value != currentState.FillMode)
            {
                Affect(device, currentState);
                return;
            }

            // ScissorTestEnable
            if (ScissorTestEnable.HasValue && ScissorTestEnable.Value != currentState.ScissorTestEnable)
            {
                Affect(device, currentState);
                return;
            }

            // MultiSampleAntiAlias
            if (MultiSampleAntiAlias.HasValue && MultiSampleAntiAlias.Value != currentState.MultiSampleAntiAlias)
            {
                Affect(device, currentState);
                return;
            }

            // DepthBias
            if (DepthBias.HasValue && DepthBias.Value != currentState.DepthBias)
            {
                Affect(device, currentState);
                return;
            }

            // SlopeScaleDepthBias
            if (SlopeScaleDepthBias.HasValue && SlopeScaleDepthBias.Value != currentState.SlopeScaleDepthBias)
            {
                Affect(device, currentState);
                return;
            }
        }

        #endregion
    }
}
