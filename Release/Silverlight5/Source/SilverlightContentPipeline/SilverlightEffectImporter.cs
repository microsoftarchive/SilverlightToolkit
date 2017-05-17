using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace SilverlightContentPipeline
{
    [ContentImporter(".fx", ".slfx", DisplayName = "Effect - Silverlight", DefaultProcessor = "SilverlightEffectProcessor")]
    public class SilverlightEffectImporter : ContentImporter<EffectSourceCode>
    {
        public override EffectSourceCode Import(string filename, ContentImporterContext context)
        {
            ExceptionHelper.Filename = filename;

            try
            {
                string sourceCode = File.ReadAllText(filename);

                return new EffectSourceCode(sourceCode);
            }
            catch (Exception ex)
            {
                ExceptionHelper.RaiseException("Unable to read effect.", ex);                
            }

            return null;
        }
    }
}
