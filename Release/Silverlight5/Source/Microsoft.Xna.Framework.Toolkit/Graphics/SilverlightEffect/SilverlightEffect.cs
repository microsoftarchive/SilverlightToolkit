using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Graphics
{
    /// <summary>
    /// Provides support for .fx files.
    /// </summary>
    public class SilverlightEffect : Effect
    {
        #region Instance Data

        readonly SilverlightEffectParametersCollection parameters;

        #endregion

        #region IDisposable

        ///// <summary>
        ///// Dispose(bool disposing) executes in two distinct scenarios.
        ///// If disposing equals true, the method has been called directly
        ///// or indirectly by a user's code. Managed and unmanaged resources
        ///// can be disposed.
        ///// If disposing equals false, the method has been called by the 
        ///// runtime from inside the finalizer and you should not reference 
        ///// other objects. Only unmanaged resources can be disposed.
        ///// </summary>
        //protected override void Dispose(bool disposing)
        //{
        //    if (IsDisposed) 
        //        return;

        //    base.Dispose(disposing);

        //    if (disposing)
        //    {
        //        if (vertexShader != null)
        //        {
        //            vertexShader.Dispose();
        //            vertexShader = null;
        //        }
        //        if (pixelShader != null)
        //        {
        //            pixelShader.Dispose();
        //            pixelShader = null;
        //        }
        //    }
        //}

        #endregion

        #region Creation

        /// <summary>
        /// Creates a new SilverlightEffect with default parameter settings.
        /// </summary>
        internal SilverlightEffect(EffectTechnique[] techniques)
            : base(techniques)
        {
            Dictionary<string, SilverlightEffectParameter> tempParameters = new Dictionary<string, SilverlightEffectParameter>();

            foreach (var technique in techniques)
            {
                foreach (SilverlightEffectPass pass in technique.Passes)
                {
                    pass.ParentEffect = this;

                    foreach (SilverlightEffectInternalParameter parameter in pass.Parameters)
                    {
                        if (!tempParameters.ContainsKey(parameter.Name))
                        {
                            tempParameters.Add(parameter.Name, new SilverlightEffectParameter(parameter.Name));
                        }
                    }
                }
            }

            parameters = new SilverlightEffectParametersCollection(tempParameters.Values);
        }

        #endregion

        #region Internal methods

        internal void Apply(bool force)
        {
            foreach (SilverlightEffectParameter parameter in parameters)
            {
                if (parameter.IsDirty || force)
                {
                    // The SilverlightEffectParameters must transmit data to internal parameters if they are dirty
                    foreach (var technique in Techniques)
                    {
                        foreach (SilverlightEffectPass pass in technique.Passes)
                        {
                            parameter.Apply(pass.Parameters);
                        }
                    }
                }
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Get the parameters list.
        /// </summary>
        public SilverlightEffectParametersCollection Parameters
        {
            get { return parameters; }
        }

        #endregion
    }
}
