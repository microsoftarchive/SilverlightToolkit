//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace System.ComponentModel
{
    /// <summary>
    /// Represents an entity action object
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class EntityAction
    {
        private List<object> _parameters;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="name">name of the entity action</param>
        /// <param name="parameters">parameters list to invoke entity action with</param>
        public EntityAction(string name, params object[] parameters)
        {
            this.Name = name;
            this._parameters = new List<object>();
            if (parameters != null)
            {
                this._parameters.AddRange(parameters);
            }
        }

        /// <summary>
        /// Gets the name of the entity action
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the parameters to invoke the entity action with
        /// </summary>
        public IEnumerable<object> Parameters 
        {
            get
            {
                return this._parameters;
            }
        }

        /// <summary>
        /// Gets whether any parameters were associated with this action.
        /// </summary>
        public bool HasParameters
        {
            get
            {
                return (this._parameters.Count > 0);
            }
        }
    }
}
