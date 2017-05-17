namespace Microsoft.Xna.Framework.Graphics
{
    internal class SilverlightEffectBlendState : SilverlightEffectState
    {
        #region Properties

        public Blend? ColorSourceBlend { get; set; }
        public Blend? ColorDestinationBlend { get; set; }
        public BlendFunction? ColorBlendFunction { get; set; }
        public Blend? AlphaSourceBlend { get; set; }
        public Blend? AlphaDestinationBlend { get; set; }
        public BlendFunction? AlphaBlendFunction { get; set; }
        public ColorWriteChannels? ColorWriteChannels { get; set; }
        public Color? BlendFactor { get; set; }
        public int? MultiSampleMask { get; set; }

        #endregion

        #region Methods

        public void Affect(GraphicsDevice device, BlendState currentState)
        {
            BlendState internalState = new BlendState();

            // ColorSourceBlend
            internalState.ColorSourceBlend = ColorSourceBlend.HasValue ? ColorSourceBlend.Value : currentState.ColorSourceBlend;

            // ColorDestinationBlend
            internalState.ColorDestinationBlend = ColorDestinationBlend.HasValue ? ColorDestinationBlend.Value : currentState.ColorDestinationBlend;

            // ColorBlendFunction
            internalState.ColorBlendFunction = ColorBlendFunction.HasValue ? ColorBlendFunction.Value : currentState.ColorBlendFunction;

            // AlphaSourceBlend
            internalState.AlphaSourceBlend = AlphaSourceBlend.HasValue ? AlphaSourceBlend.Value : currentState.AlphaSourceBlend;

            // AlphaDestinationBlend
            internalState.AlphaDestinationBlend = AlphaDestinationBlend.HasValue ? AlphaDestinationBlend.Value : currentState.AlphaDestinationBlend;

            // AlphaBlendFunction
            internalState.AlphaBlendFunction = AlphaBlendFunction.HasValue ? AlphaBlendFunction.Value : currentState.AlphaBlendFunction;

            // ColorWriteChannels
            internalState.ColorWriteChannels = ColorWriteChannels.HasValue ? ColorWriteChannels.Value : currentState.ColorWriteChannels;

            // BlendFactor
            internalState.BlendFactor = BlendFactor.HasValue ? BlendFactor.Value : currentState.BlendFactor;

            // MultiSampleMask
            internalState.MultiSampleMask = MultiSampleMask.HasValue ? MultiSampleMask.Value : currentState.MultiSampleMask;

            // Finally apply the state
            device.BlendState = internalState;
        }

        public override void ProcessState(GraphicsDevice device)
        {
            BlendState currentState = device.BlendState;

            // ColorSourceBlend
            if (ColorSourceBlend.HasValue && ColorSourceBlend.Value != currentState.ColorSourceBlend)
            {
                Affect(device, currentState);
                return;
            }

            // ColorDestinationBlend
            if (ColorDestinationBlend.HasValue && ColorDestinationBlend.Value != currentState.ColorDestinationBlend)
            {
                Affect(device, currentState);
                return;
            }

            // ColorBlendFunction
            if (ColorBlendFunction.HasValue && ColorBlendFunction.Value != currentState.ColorBlendFunction)
            {
                Affect(device, currentState);
                return;
            }

            // AlphaSourceBlend
            if (AlphaSourceBlend.HasValue && AlphaSourceBlend.Value != currentState.AlphaSourceBlend)
            {
                Affect(device, currentState);
                return;
            }

            // AlphaDestinationBlend
            if (AlphaDestinationBlend.HasValue && AlphaDestinationBlend.Value != currentState.AlphaDestinationBlend)
            {
                Affect(device, currentState);
                return;
            }

            // AlphaBlendFunction
            if (AlphaBlendFunction.HasValue && AlphaBlendFunction.Value != currentState.AlphaBlendFunction)
            {
                Affect(device, currentState);
                return;
            }

            // ColorWriteChannels
            if (ColorWriteChannels.HasValue && ColorWriteChannels.Value != currentState.ColorWriteChannels)
            {
                Affect(device, currentState);
                return;
            }

            // BlendFactor
            if (BlendFactor.HasValue && BlendFactor.Value != currentState.BlendFactor)
            {
                Affect(device, currentState);
                return;
            }

            // MultiSampleMask
            if (MultiSampleMask.HasValue && MultiSampleMask.Value != currentState.MultiSampleMask)
            {
                Affect(device, currentState);
                return;
            }
        }

        #endregion
    }
}
