namespace Microsoft.Xna.Framework.Graphics
{
    internal class SilverlightEffectSamplerState : SilverlightEffectState
    {
        readonly int samplerIndex;

        #region Properties

        public TextureFilter? Filter { get; set; }
        public TextureAddressMode? AddressU { get; set; }
        public TextureAddressMode? AddressV { get; set; }
        public int? MaxAnisotropy { get; set; }
        public int? MaxMipLevel { get; set; }
        public float? MipMapLevelOfDetailBias { get; set; }

        #endregion

        #region Creation

        public SilverlightEffectSamplerState(int index)
        {
            samplerIndex = index;
        }

        #endregion

        #region Methods

        public void Affect(GraphicsDevice device, SamplerState currentState)
        {
            SamplerState internalState = new SamplerState();

            // Filter
            internalState.Filter = Filter.HasValue ? Filter.Value : currentState.Filter;

            // AddressU
            internalState.AddressU = AddressU.HasValue ? AddressU.Value : currentState.AddressU;

            // AddressV
            internalState.AddressV = AddressV.HasValue ? AddressV.Value : currentState.AddressV;

            // MaxAnisotropy
            internalState.MaxAnisotropy = MaxAnisotropy.HasValue ? MaxAnisotropy.Value : currentState.MaxAnisotropy;

            // MaxMipLevel
            internalState.MaxMipLevel = MaxMipLevel.HasValue ? MaxMipLevel.Value : currentState.MaxMipLevel;

            // MipMapLevelOfDetailBias
            internalState.MipMapLevelOfDetailBias = MipMapLevelOfDetailBias.HasValue ? MipMapLevelOfDetailBias.Value : currentState.MipMapLevelOfDetailBias;

            // Finally apply the state
            device.SamplerStates[samplerIndex] = internalState;
        }

        public override void ProcessState(GraphicsDevice device)
        {
            SamplerState currentState = device.SamplerStates[samplerIndex];

            // Filter
            if (Filter.HasValue && Filter.Value != currentState.Filter)
            {
                Affect(device, currentState);
                return;
            }
            
            // AddressU
            if (AddressU.HasValue && AddressU.Value != currentState.AddressU)
            {
                Affect(device, currentState);
                return;
            }

            // AddressV
            if (AddressV.HasValue && AddressV.Value != currentState.AddressV)
            {
                Affect(device, currentState);
                return;
            }

            // MaxAnisotropy
            if (MaxAnisotropy.HasValue && MaxAnisotropy.Value != currentState.MaxAnisotropy)
            {
                Affect(device, currentState);
                return;
            }

            // MaxMipLevel
            if (MaxMipLevel.HasValue && MaxMipLevel.Value != currentState.MaxMipLevel)
            {
                Affect(device, currentState);
                return;
            }

            // MipMapLevelOfDetailBias
            if (MipMapLevelOfDetailBias.HasValue && MipMapLevelOfDetailBias.Value != currentState.MipMapLevelOfDetailBias)
            {
                Affect(device, currentState);
                return;
            }
        }

        #endregion
    }
}
