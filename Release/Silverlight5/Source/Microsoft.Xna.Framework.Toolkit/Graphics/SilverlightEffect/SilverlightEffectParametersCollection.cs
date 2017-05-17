using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Xna.Framework.Graphics
{
    /// <summary>
    /// Define the collection of parameters for SilverlightEffect class.
    /// </summary>
    public sealed class SilverlightEffectParametersCollection : IEnumerable<SilverlightEffectParameter>
    {
        private readonly List<SilverlightEffectParameter> parameters;

        /// <summary>Gets a specific SilverlightEffectParameter object by using an index value.</summary>
        /// <param name="index">Index of the SilverlightEffectParameter to get.</param>
        public SilverlightEffectParameter this[int index]
        {
            get
            {
                if (index >= 0 && index < parameters.Count)
                {
                    return parameters[index];
                }
                return null;
            }
        }

        /// <summary>Gets a specific SilverlightEffectParameter by name.</summary>
        /// <param name="name">The name of the SilverlightEffectParameter to retrieve.</param>
        public SilverlightEffectParameter this[string name]
        {
            get
            {
                return parameters.FirstOrDefault(current => current.Name == name);
            }
        }

        /// <summary>Gets the number of EffectParameter objects in this EffectParameterCollection.</summary>
        public int Count
        {
            get
            {
                return parameters.Count;
            }
        }

        internal SilverlightEffectParametersCollection(IEnumerable<SilverlightEffectParameter> sourceParameters)
        {
            parameters = new List<SilverlightEffectParameter>(sourceParameters);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return parameters.GetEnumerator();
        }

        IEnumerator<SilverlightEffectParameter> IEnumerable<SilverlightEffectParameter>.GetEnumerator()
        {
            return parameters.GetEnumerator();
        }
    }
}
