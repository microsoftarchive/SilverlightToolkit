using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel.DataAnnotations {
    /// <summary>
    /// An attribute used to specify the filtering behavior for a column.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments")]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class FilterUIHintAttribute : Attribute {
        private UIHintAttribute.UIHintImplementation _implementation;

        /// <summary>
        /// Gets the name of the control that is most appropriate for this associated property or field
        /// </summary>
        public string FilterUIHint {
            get {
                return _implementation.UIHint;
            }
        }

        /// <summary>
        /// Gets the name of the presentation layer that supports the control type in <see cref="FilterUIHint"/>
        /// </summary>
        public string PresentationLayer {
            get {
                return _implementation.PresentationLayer;
            }
        }

        /// <summary>
        /// Gets the name-value pairs used as parameters to the control's constructor
        /// </summary>
        /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
        public IDictionary<string, object> ControlParameters {
            get {
                return _implementation.ControlParameters;
            }
        }

        /// <summary>
        /// Constructor that accepts the name of the control, without specifying which presentation layer to use
        /// </summary>
        /// <param name="uiHint"></param>
        public FilterUIHintAttribute(string filterUIHint)
            : this(filterUIHint, null, new object[0]) {
        }

        /// <summary>
        /// Constructor that accepts both the name of the control as well as the presentation layer
        /// </summary>
        /// <param name="uiHint">The name of the control to use</param>
        /// <param name="presentationLayer">The name of the presentation layer that supports this control</param>
        public FilterUIHintAttribute(string filterUIHint, string presentationLayer)
            : this(filterUIHint, presentationLayer, new object[0]) {
        }

        /// <summary>
        /// Full constructor that accepts the name of the control, presentation layer, and optional parameters
        /// to use when constructing the control
        /// </summary>
        /// <param name="uiHint">The name of the control</param>
        /// <param name="presentationLayer">The presentation layer</param>
        /// <param name="controlParameters">The list of parameters for the control</param>
        public FilterUIHintAttribute(string filterUIHint, string presentationLayer, params object[] controlParameters) {
            _implementation = new UIHintAttribute.UIHintImplementation(filterUIHint, presentationLayer, controlParameters);
        }
    }
}
