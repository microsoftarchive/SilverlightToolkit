using System.Collections.Generic;

namespace SilverlightContentPipeline
{
    public class EffectTechniqueBinary
    {
        public string Name { get; set; }
        public List<EffectPassBinary> PassBinaries { get; private set; } 

        public EffectTechniqueBinary()
        {
            PassBinaries = new List<EffectPassBinary>();
        }
    }
}
