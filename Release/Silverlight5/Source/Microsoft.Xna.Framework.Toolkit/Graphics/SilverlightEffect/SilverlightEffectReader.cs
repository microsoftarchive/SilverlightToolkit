using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Graphics;

namespace Microsoft.Xna.Framework.Content
{
    /// <summary>
    /// Read SilverlightEffect.
    /// </summary>
    public class SilverlightEffectReader : ContentTypeReader<SilverlightEffect>
    {
        /// <summary>
        /// Read and create a SilverlightEffect
        /// </summary>
        protected override SilverlightEffect Read(ContentReader input, SilverlightEffect existingInstance)
        {
            int techniquesCount = input.ReadInt32();
            EffectTechnique[] techniques = new EffectTechnique[techniquesCount];

            for (int techniqueIndex = 0; techniqueIndex < techniquesCount; techniqueIndex++)
            {
                int passesCount = input.ReadInt32();
                EffectPass[] passes = new EffectPass[passesCount];

                for (int passIndex = 0; passIndex < passesCount; passIndex++)
                {
                    string passName = input.ReadString();

                    // Vertex shader
                    int vertexShaderByteCodeLength = input.ReadInt32();
                    byte[] vertexShaderByteCode = input.ReadBytes(vertexShaderByteCodeLength);
                    int vertexShaderParametersLength = input.ReadInt32();
                    byte[] vertexShaderParameters = input.ReadBytes(vertexShaderParametersLength);

                    // Pixel shader
                    int pixelShaderByteCodeLength = input.ReadInt32();
                    byte[] pixelShaderByteCode = input.ReadBytes(pixelShaderByteCodeLength);
                    int pixelShaderParametersLength = input.ReadInt32();
                    byte[] pixelShaderParameters = input.ReadBytes(pixelShaderParametersLength);

                    MemoryStream vertexShaderCodeStream = new MemoryStream(vertexShaderByteCode);
                    MemoryStream pixelShaderCodeStream = new MemoryStream(pixelShaderByteCode);
                    MemoryStream vertexShaderParametersStream = new MemoryStream(vertexShaderParameters);
                    MemoryStream pixelShaderParametersStream = new MemoryStream(pixelShaderParameters);

                    // Instanciate pass
                    SilverlightEffectPass currentPass = new SilverlightEffectPass(passName, GraphicsDeviceManager.Current.GraphicsDevice, vertexShaderCodeStream, pixelShaderCodeStream, vertexShaderParametersStream, pixelShaderParametersStream);
                    passes[passIndex] = currentPass;

                    vertexShaderCodeStream.Dispose();
                    pixelShaderCodeStream.Dispose();
                    vertexShaderParametersStream.Dispose();
                    pixelShaderParametersStream.Dispose();

                    // Render states
                    int renderStatesCount = input.ReadInt32();

                    for (int renderStateIndex = 0; renderStateIndex < renderStatesCount; renderStateIndex++)
                    {
                        currentPass.AppendState(input.ReadString(), input.ReadString());
                    }
                }

                // Instanciate technique
                techniques[techniqueIndex] = new EffectTechnique(passes);
            }

            return new SilverlightEffect(techniques);
        }
    }
}
