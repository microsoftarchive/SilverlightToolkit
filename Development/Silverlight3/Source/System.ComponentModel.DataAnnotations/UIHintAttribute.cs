using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.ComponentModel.DataAnnotations {
    /// <summary>
    /// Attribute to provide a hint to the presentation layer about what control it should use
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "It's already exposed")]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "We want users to be able to extend this class")]
    public class UIHintAttribute : Attribute {
        private UIHintImplementation _implementation;

        /// <summary>
        /// Gets the name of the control that is most appropriate for this associated property or field
        /// </summary>
        public string UIHint {
            get {
                return _implementation.UIHint;
            }
        }

        /// <summary>
        /// Gets the name of the presentation layer that supports the control type in <see cref="UIHint"/>
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
        public UIHintAttribute(string uiHint)
            : this(uiHint, null, new object[0]) {
        }

        /// <summary>
        /// Constructor that accepts both the name of the control as well as the presentation layer
        /// </summary>
        /// <param name="uiHint">The name of the control to use</param>
        /// <param name="presentationLayer">The name of the presentation layer that supports this control</param>
        public UIHintAttribute(string uiHint, string presentationLayer)
            : this(uiHint, presentationLayer, new object[0]) {
        }

        /// <summary>
        /// Full constructor that accepts the name of the control, presentation layer, and optional parameters
        /// to use when constructing the control
        /// </summary>
        /// <param name="uiHint">The name of the control</param>
        /// <param name="presentationLayer">The presentation layer</param>
        /// <param name="controlParameters">The list of parameters for the control</param>
        public UIHintAttribute(string uiHint, string presentationLayer, params object[] controlParameters) {
            _implementation = new UIHintImplementation(uiHint, presentationLayer, controlParameters);
        }

        internal class UIHintImplementation {
            private IDictionary<string, object> _controlParameters;
            private object[] _inputControlParameters;

            /// <summary>
            /// Gets the name of the control that is most appropriate for this associated property or field
            /// </summary>
            public string UIHint { get; private set; }

            /// <summary>
            /// Gets the name of the presentation layer that supports the control type in <see cref="UIHint"/>
            /// </summary>
            public string PresentationLayer { get; private set; }

            public IDictionary<string, object> ControlParameters {
                get {
                    this.SetControlParameters();
                    return this._controlParameters;
                }
                private set {
                    this._controlParameters = value;
                }
            }

            public UIHintImplementation(string uiHint, string presentationLayer, params object[] controlParameters) {
                this.UIHint = uiHint;
                this.PresentationLayer = presentationLayer;
                this._inputControlParameters = controlParameters;
            }

            /// <summary>
            /// Validates the input control parameters and throws InvalidOperationException if they are not correct.
            /// </summary>
            private void SetControlParameters() {
                // First execution sets and prevents re-entrancy
                if (this._controlParameters == null) {
                    // Set empty dictionary in place till we do our work
                    this._controlParameters = new Dictionary<string, object>();
                    IDictionary<string, object> controlParameters = new Dictionary<string, object>();

                    object[] inputControlParameters = this._inputControlParameters;

                    // GC the ctor argument -- no longer needed
                    this._inputControlParameters = null;

                    if (inputControlParameters == null || inputControlParameters.Length == 0) {
                        return;
                    }

                    if (inputControlParameters.Length % 2 != 0) {
                        throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, DataAnnotationsResources.UIHintImplementation_NeedEvenNumberOfControlParameters));
                    }

                    for (int i = 0; i < inputControlParameters.Length; i += 2) {
                        object key = inputControlParameters[i];
                        object value = inputControlParameters[i + 1];
                        if (key == null) {
                            throw new InvalidOperationException(
                                String.Format(
                                CultureInfo.CurrentCulture,
                                DataAnnotationsResources.UIHintImplementation_ControlParameterKeyIsNull,
                                i));
                        }
                        string keyString = key as string;

                        if (keyString == null) {
                            throw new InvalidOperationException(
                                String.Format(
                                CultureInfo.CurrentCulture,
                                DataAnnotationsResources.UIHintImplementation_ControlParameterKeyIsNotAString,
                                i,
                                inputControlParameters[i].ToString()));
                        }

                        if (controlParameters.ContainsKey(keyString)) {
                            throw new InvalidOperationException(
                                String.Format(
                                CultureInfo.CurrentCulture,
                                DataAnnotationsResources.UIHintImplementation_ControlParameterKeyOccursMoreThanOnce,
                                i,
                                keyString));
                        }

                        controlParameters[keyString] = value;
                    }

                    // Put filled dictionary in place only when it is complete
                    this.ControlParameters = controlParameters;
                }
            }
        }
    }
}
