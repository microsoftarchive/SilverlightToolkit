using System.Collections.Generic;

namespace SilverlightContentPipeline
{
    public class EffectBinary
    {
        public List<EffectTechniqueBinary> TechniquesBinaries { get; private set; }

        public EffectBinary()
        {
            TechniquesBinaries = new List<EffectTechniqueBinary>();
        }
    }
}
