// extern alias Silverlight;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace SilverlightContentPipeline
{
    [ContentTypeWriter]
    public class SilverlightEffectWriter : ContentTypeWriter<EffectBinary>
    {
        protected override void Write(ContentWriter output, EffectBinary value)
        {
            // Browsing techniques
            output.Write(value.TechniquesBinaries.Count);
            foreach (var techniqueBinary in value.TechniquesBinaries)
            {
                // Browsing passes
                output.Write(techniqueBinary.PassBinaries.Count);
                foreach (var passBinary in techniqueBinary.PassBinaries)
                {
                    output.Write(passBinary.Name);

                    // Vertex shader
                    output.Write(passBinary.VertexShaderByteCode.Length);
                    output.Write(passBinary.VertexShaderByteCode);
                    output.Write(passBinary.VertexShaderParameters.Length);
                    output.Write(passBinary.VertexShaderParameters);

                    // Pixel shader
                    output.Write(passBinary.PixelShaderByteCode.Length);
                    output.Write(passBinary.PixelShaderByteCode);
                    output.Write(passBinary.PixelShaderParameters.Length);
                    output.Write(passBinary.PixelShaderParameters);

                    // Render states
                    output.Write(passBinary.RenderStates.Count);
                    foreach (string key in passBinary.RenderStates.Keys)
                    {
                        output.Write(key);
                        output.Write(passBinary.RenderStates[key]);
                    }
                }
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // Key signing and version updates can break static strings like below because the SN changes.
            return "Microsoft.Xna.Framework.Content.SilverlightEffectReader, Microsoft.Xna.Framework.Toolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=2c5c654d367bf4a7";

            // We could reference the SL assemblies and do this, but that would mean updating the templates with
            // correct references as well, which is tricky.
            // return typeof(Silverlight::Microsoft.Xna.Framework.Content.SilverlightEffectReader).AssemblyQualifiedName;
        }
    }
}
