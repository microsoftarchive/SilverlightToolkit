using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Xna.Framework.Toolkit;

namespace Microsoft.Xna.Framework.Graphics
{
    internal class SilverlightEffectPass : EffectPass
    {
        #region Instance Data

        const int SamplerStatesCount = 16;

        readonly GraphicsDevice device;
        readonly VertexShader vertexShader;
        readonly PixelShader pixelShader;
        readonly List<SilverlightEffectInternalParameter> parameters;
        readonly SilverlightEffectDepthStencilState depthStencilState = new SilverlightEffectDepthStencilState();
        readonly SilverlightEffectBlendState blendState = new SilverlightEffectBlendState();
        readonly SilverlightEffectRasterizerState rasterizerState = new SilverlightEffectRasterizerState();
        readonly SilverlightEffectSamplerState[] samplerStates = new SilverlightEffectSamplerState[SamplerStatesCount];

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the name of the pass
        /// </summary>
        public string Name { get; private set; }

        internal SilverlightEffect ParentEffect { get; set; }

        internal List<SilverlightEffectInternalParameter> Parameters
        {
            get { return parameters; }
        }

        #endregion

        #region Creation

        /// <summary>
        /// Creates a new effect pass.
        /// Marked as internal to prevent users from creating it.
        /// </summary>
        internal SilverlightEffectPass(string name, GraphicsDevice device, Stream vertexShaderCode, Stream pixelShaderCode, Stream vertexShaderParameters, Stream pixelShaderParameters)
        {
            this.device = device;
            Name = name;
            // Shaders
            vertexShader = VertexShader.FromStream(device, vertexShaderCode);
            pixelShader = PixelShader.FromStream(device, pixelShaderCode);

            // Assembly codes
            Dictionary<string, SilverlightEffectInternalParameter> tempParameters = new Dictionary<string, SilverlightEffectInternalParameter>();
            ExtractConstantsRegisters(vertexShaderParameters, false, tempParameters);
            ExtractConstantsRegisters(pixelShaderParameters, true, tempParameters);

            parameters = new List<SilverlightEffectInternalParameter>(tempParameters.Values);

            for (int index = 0; index < SamplerStatesCount; index++)
            {
                samplerStates[index] = new SilverlightEffectSamplerState(index);
            }
        }

        #endregion

        #region Public methods

        /// <summary>Begins this pass</summary>
        public override void Apply()
        {
            // Shaders
            bool shadersChanged = false;
            if (vertexShader != device.GetVertexShader())
            {
                device.SetVertexShader(vertexShader);
                shadersChanged = true;
            }

            if (pixelShader != device.GetPixelShader())
            {
                device.SetPixelShader(pixelShader);
                shadersChanged = true;
            }

            // Parameters
            // If shaderChanged == true, we don't know what was set in the registers before so
            // force all registers as dirty.
            ParentEffect.Apply(shadersChanged);

            foreach (SilverlightEffectInternalParameter effectParameter in Parameters)
            {
                effectParameter.Apply();
            }

            // Render states
            if (depthStencilState.IsActive)
                depthStencilState.ProcessState(device);

            if (blendState.IsActive)
                blendState.ProcessState(device);

            if (rasterizerState.IsActive)
                rasterizerState.ProcessState(device);

            for (int index = 0; index < SamplerStatesCount; index++)
            {
                if (samplerStates[index].IsActive)
                    samplerStates[index].ProcessState(device);
            }
        }

        #endregion

        #region Private methods

        void ExtractConstantsRegisters(Stream stream, bool isForPixelShader, Dictionary<string, SilverlightEffectInternalParameter> tempParameters)
        {
            XmlReader xmlReader = XmlReader.Create(stream);

            while (xmlReader.ReadToFollowing("Constant"))
            {
                xmlReader.ReadToDescendant("Name");
                string name = xmlReader.ReadElementContentAsString();

                SilverlightEffectInternalParameter effectParameter;
                if (tempParameters.ContainsKey(name))
                {
                    effectParameter = tempParameters[name];
                }
                else
                {
                    effectParameter = new SilverlightEffectInternalParameter(device, name);
                    tempParameters.Add(name, effectParameter);
                }

                xmlReader.ReadToNextSibling("RegisterIndex");
                int index = xmlReader.ReadElementContentAsInt();

                if (isForPixelShader)
                    effectParameter.PixelShaderRegisterIndex = index;
                else
                    effectParameter.VertexShaderRegisterIndex = index;

                xmlReader.ReadToNextSibling("RegisterCount");
                effectParameter.RegisterCount = Math.Max(effectParameter.RegisterCount, xmlReader.ReadElementContentAsInt());
            }

            xmlReader.Close();
        }

        #endregion

        #region Internal methods

        internal void AppendState(string name, string value)
        {
            // value are supposed to be correct because we used the EffectProcessor to check effect source code.
            
            // DepthStencilStates
            if (ProcessDepthStencilStates(name, value))
            {
                depthStencilState.IsActive = true;
                return;
            }

            // BlendStates
            if (ProcessBlendStates(name, value))
            {
                blendState.IsActive = true;
                return;
            }

            // RasterizerStates
            if (ProcessRasterizerStates(name, value))
            {
                rasterizerState.IsActive = true;
                return;
            }

            // SamplerStates
            ProcessSamplerStates(name, value);
        }
        
        // Try to identify a component of DepthStencilState
        bool ProcessDepthStencilStates(string name, string value)
        {
            switch (name)
            {
                // DepthStencilState - Stencil
                case "stencilenable":
                    depthStencilState.StencilEnable = bool.Parse(value);
                    return true;
                case "stencilfail":
                    depthStencilState.StencilFail = value.Convert<StencilOperation>();
                    return true;
                case "stencilfunc":
                    depthStencilState.StencilFunction = value.Convert<CompareFunction>();
                    return true;
                case "stencilmask":
                    depthStencilState.StencilMask = value.ConvertToInt32();
                    return true;
                case "stencilpass":
                    depthStencilState.StencilPass = value.Convert<StencilOperation>();
                    return true;
                case "stencilref":
                    depthStencilState.ReferenceStencil = value.ConvertToInt32();
                    return true;
                case "stencilwritemask":
                    depthStencilState.StencilWriteMask = value.ConvertToInt32();
                    return true;
                case "stencilzfail":
                    depthStencilState.StencilDepthBufferFail = value.Convert<StencilOperation>();
                    return true;
                case "twosidedstencilmode":
                    depthStencilState.TwoSidedStencilMode = bool.Parse(value);
                    return true;
                case "ccw_stencilzfail":
                    depthStencilState.CounterClockwiseStencilDepthBufferFail = value.Convert<StencilOperation>();
                    return true;
                case "ccw_stencilfail":
                    depthStencilState.CounterClockwiseStencilFail = value.Convert<StencilOperation>();
                    return true;
                case "ccw_stencilfunc":
                    depthStencilState.CounterClockwiseStencilFunction = value.Convert<CompareFunction>();
                    return true;
                case "ccw_stencilpass":
                    depthStencilState.CounterClockwiseStencilPass = value.Convert<StencilOperation>();
                    return true;
                // DepthStencilState - Depth Buffer
                case "zenable":
                    depthStencilState.DepthBufferEnable = bool.Parse(value);
                    return true;
                case "zfunc":
                    depthStencilState.DepthBufferFunction = value.Convert<CompareFunction>();
                    return true;
                case "zwriteenable":
                    depthStencilState.DepthBufferWriteEnable = bool.Parse(value);
                    return true;               
            }

            return false;
        }

        // Try to identify a component of BlendState
        bool ProcessBlendStates(string name, string value)
        {
            switch (name)
            {               
                // BlendState
                case "blendop":
                    blendState.ColorBlendFunction = value.Convert<BlendFunction>();
                    blendState.AlphaBlendFunction = blendState.ColorBlendFunction;
                    return true;
                case "destblend":
                    blendState.ColorDestinationBlend = value.Convert<Blend>();
                    blendState.AlphaDestinationBlend = blendState.ColorDestinationBlend;
                    return true;
                case "srcblend":
                    blendState.ColorSourceBlend = value.Convert<Blend>();
                    blendState.AlphaSourceBlend = blendState.ColorSourceBlend;
                    return true;
                case "blendfactor":
                    blendState.BlendFactor = value.ConvertToInt32().ConvertToColor();
                    return true;
                case "colorwriteenable":
                    blendState.ColorWriteChannels = (ColorWriteChannels)value.ConvertToInt32();
                    return true;
                case "multisamplemask":
                    blendState.MultiSampleMask = value.ConvertToInt32();
                    return true;                
            }

            return false;
        }

        // Try to identify a component of RasterizerState
        bool ProcessRasterizerStates(string name, string value)
        {
            switch (name)
            {               
                // RasterizerState
                case "cullmode":
                    CullMode cullMode = CullMode.None;
                    switch (value)
                    {
                        case "CCW":
                            cullMode = CullMode.CullCounterClockwiseFace;
                            break;
                        case "CW":
                            cullMode = CullMode.CullClockwiseFace;
                            break;
                    }
                    rasterizerState.CullMode = cullMode;
                    return true;
                case "depthbias":
                    rasterizerState.DepthBias = value.ConvertToFloat();
                    return true;
                case "fillmode":
                    rasterizerState.FillMode = value.Convert<FillMode>();
                    return true;
                case "multisampleantialias":
                    rasterizerState.MultiSampleAntiAlias = bool.Parse(value);
                    return true;
                case "scissortestenable":
                    rasterizerState.ScissorTestEnable = bool.Parse(value);
                    return true;
                case "slopescaledepthbias":
                    rasterizerState.SlopeScaleDepthBias = value.ConvertToFloat();
                    return true;
            }

            return false;
        }

        // Try to identify a component of SamplerState
        bool ProcessSamplerStates(string name, string value)
        {
            if (name.EndsWith("]"))
            {
                Regex regex = new Regex(@"(?<name>\w+)\[(?<id>\d+)\]");
                Match match = regex.Match(name);

                int samplerStateID = int.Parse(match.Groups["id"].Value);
                string samplerStateName = match.Groups["name"].Value;

                switch (samplerStateName)
                {
                    case "addressu":
                        samplerStates[samplerStateID].AddressU = value.Convert<TextureAddressMode>();
                        samplerStates[samplerStateID].IsActive = true;
                        return true;
                    case "addressv":
                        samplerStates[samplerStateID].AddressU = value.Convert<TextureAddressMode>();
                        samplerStates[samplerStateID].IsActive = true;
                        return true;
                    case "filter":
                        samplerStates[samplerStateID].Filter = value.Convert<TextureFilter>();
                        samplerStates[samplerStateID].IsActive = true;
                        return true;
                    case "maxanisotropy":
                        samplerStates[samplerStateID].MaxAnisotropy = value.ConvertToInt32();
                        samplerStates[samplerStateID].IsActive = true;
                        return true;
                    case "maxmiplevel":
                        samplerStates[samplerStateID].MaxMipLevel = value.ConvertToInt32();
                        samplerStates[samplerStateID].IsActive = true;
                        return true;
                    case "mipmaplodbias":
                        samplerStates[samplerStateID].MipMapLevelOfDetailBias = value.ConvertToFloat();
                        samplerStates[samplerStateID].IsActive = true;
                        return true;
                }
            }

            return false;
        }

        #endregion
    }
}
