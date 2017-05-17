namespace Microsoft.Xna.Framework.Graphics
{
    internal class SilverlightEffectDepthStencilState : SilverlightEffectState
    {
        #region Properties

        // DepthStencilState
        public bool? DepthBufferEnable { get; set; }
        public bool? DepthBufferWriteEnable { get; set; }
        public CompareFunction? DepthBufferFunction { get; set; }
        public bool? StencilEnable { get; set; }
        public CompareFunction? StencilFunction { get; set; }
        public StencilOperation? StencilPass { get; set; }
        public StencilOperation? StencilFail { get; set; }
        public StencilOperation? StencilDepthBufferFail { get; set; }
        public bool? TwoSidedStencilMode { get; set; }
        public CompareFunction? CounterClockwiseStencilFunction { get; set; }
        public StencilOperation? CounterClockwiseStencilPass { get; set; }
        public StencilOperation? CounterClockwiseStencilFail { get; set; }
        public StencilOperation? CounterClockwiseStencilDepthBufferFail { get; set; }
        public int? StencilMask { get; set; }
        public int? StencilWriteMask { get; set; }
        public int? ReferenceStencil { get; set; }

        #endregion

        #region Methods

        public void Affect(GraphicsDevice device, DepthStencilState currentState)
        {
            DepthStencilState internalState = new DepthStencilState();

            // DepthBufferEnable
            internalState.DepthBufferEnable = DepthBufferEnable.HasValue ? DepthBufferEnable.Value : currentState.DepthBufferEnable;

            // DepthBufferWriteEnable
            internalState.DepthBufferWriteEnable = DepthBufferWriteEnable.HasValue ? DepthBufferWriteEnable.Value : currentState.DepthBufferWriteEnable;

            // DepthBufferFunction
            internalState.DepthBufferFunction = DepthBufferFunction.HasValue ? DepthBufferFunction.Value : currentState.DepthBufferFunction;

            // StencilEnable
            internalState.StencilEnable = StencilEnable.HasValue ? StencilEnable.Value : currentState.StencilEnable;

            // StencilFunction
            internalState.StencilFunction = StencilFunction.HasValue ? StencilFunction.Value : currentState.StencilFunction;

            // StencilPass
            internalState.StencilPass = StencilPass.HasValue ? StencilPass.Value : currentState.StencilPass;

            // StencilFail
            internalState.StencilFail = StencilFail.HasValue ? StencilFail.Value : currentState.StencilFail;

            // StencilDepthBufferFail
            internalState.StencilDepthBufferFail = StencilDepthBufferFail.HasValue ? StencilDepthBufferFail.Value : currentState.StencilDepthBufferFail;

            // TwoSidedStencilMode
            internalState.TwoSidedStencilMode = TwoSidedStencilMode.HasValue ? TwoSidedStencilMode.Value : currentState.TwoSidedStencilMode;

            // CounterClockwiseStencilFunction
            internalState.CounterClockwiseStencilFunction = CounterClockwiseStencilFunction.HasValue ? CounterClockwiseStencilFunction.Value : currentState.CounterClockwiseStencilFunction;

            // CounterClockwiseStencilPass
            internalState.CounterClockwiseStencilPass = CounterClockwiseStencilPass.HasValue ? CounterClockwiseStencilPass.Value : currentState.CounterClockwiseStencilPass;

            // CounterClockwiseStencilFail
            internalState.CounterClockwiseStencilFail = CounterClockwiseStencilFail.HasValue ? CounterClockwiseStencilFail.Value : currentState.CounterClockwiseStencilFail;

            // CounterClockwiseStencilDepthBufferFail
            internalState.CounterClockwiseStencilDepthBufferFail = CounterClockwiseStencilDepthBufferFail.HasValue ? CounterClockwiseStencilDepthBufferFail.Value : currentState.CounterClockwiseStencilDepthBufferFail;

            // StencilMask
            internalState.StencilMask = StencilMask.HasValue ? StencilMask.Value : currentState.StencilMask;

            // StencilWriteMask
            internalState.StencilWriteMask = StencilWriteMask.HasValue ? StencilWriteMask.Value : currentState.StencilWriteMask;

            // ReferenceStencil
            internalState.ReferenceStencil = ReferenceStencil.HasValue ? ReferenceStencil.Value : currentState.ReferenceStencil;

            // Finally apply the state
            device.DepthStencilState = internalState;
        }

        public override void ProcessState(GraphicsDevice device)
        {
            DepthStencilState currentState = device.DepthStencilState;

            // DepthBufferEnable
            if (DepthBufferEnable.HasValue && DepthBufferEnable.Value != currentState.DepthBufferEnable)
            {
                Affect(device, currentState);
                return;
            }

            // DepthBufferWriteEnable
            if (DepthBufferWriteEnable.HasValue && DepthBufferWriteEnable.Value != currentState.DepthBufferWriteEnable)
            {
                Affect(device, currentState);
                return;
            }

            // DepthBufferFunction
            if (DepthBufferFunction.HasValue && DepthBufferFunction.Value != currentState.DepthBufferFunction)
            {
                Affect(device, currentState);
                return;
            }

            // StencilEnable
            if (StencilEnable.HasValue && StencilEnable.Value != currentState.StencilEnable)
            {
                Affect(device, currentState);
                return;
            }

            // StencilFunction
            if (StencilFunction.HasValue && StencilFunction.Value != currentState.StencilFunction)
            {
                Affect(device, currentState);
                return;
            }

            // StencilPass
            if (StencilPass.HasValue && StencilPass.Value != currentState.StencilPass)
            {
                Affect(device, currentState);
                return;
            }

            // StencilFail
            if (StencilFail.HasValue && StencilFail.Value != currentState.StencilFail)
            {
                Affect(device, currentState);
                return;
            }

            // StencilDepthBufferFail
            if (StencilDepthBufferFail.HasValue && StencilDepthBufferFail.Value != currentState.StencilDepthBufferFail)
            {
                Affect(device, currentState);
                return;
            }

            // TwoSidedStencilMode
            if (TwoSidedStencilMode.HasValue && TwoSidedStencilMode.Value != currentState.TwoSidedStencilMode)
            {
                Affect(device, currentState);
                return;
            }

            // CounterClockwiseStencilFunction
            if (CounterClockwiseStencilFunction.HasValue && CounterClockwiseStencilFunction.Value != currentState.CounterClockwiseStencilFunction)
            {
                Affect(device, currentState);
                return;
            }

            // CounterClockwiseStencilPass
            if (CounterClockwiseStencilPass.HasValue && CounterClockwiseStencilPass.Value != currentState.CounterClockwiseStencilPass)
            {
                Affect(device, currentState);
                return;
            }

            // CounterClockwiseStencilFail
            if (CounterClockwiseStencilFail.HasValue && CounterClockwiseStencilFail.Value != currentState.CounterClockwiseStencilFail)
            {
                Affect(device, currentState);
                return;
            }

            // CounterClockwiseStencilDepthBufferFail
            if (CounterClockwiseStencilDepthBufferFail.HasValue && CounterClockwiseStencilDepthBufferFail.Value != currentState.CounterClockwiseStencilDepthBufferFail)
            {
                Affect(device, currentState);
                return;
            }

            // StencilMask
            if (StencilMask.HasValue && StencilMask.Value != currentState.StencilMask)
            {
                Affect(device, currentState);
                return;
            }

            // StencilWriteMask
            if (StencilWriteMask.HasValue && StencilWriteMask.Value != currentState.StencilWriteMask)
            {
                Affect(device, currentState);
                return;
            }

            // ReferenceStencil
            if (ReferenceStencil.HasValue && ReferenceStencil.Value != currentState.ReferenceStencil)
            {
                Affect(device, currentState);
                return;
            }
        }

        #endregion
    }
}
