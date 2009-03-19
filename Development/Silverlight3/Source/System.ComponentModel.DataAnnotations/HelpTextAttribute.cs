using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations.Resources;

namespace System.ComponentModel.DataAnnotations {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = false)]
    [SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "The values are passed to the resource proxy")]
    public sealed class HelpTextAttribute : Attribute {
        private ResourceProxy _proxy;
        private string _helpText;
        private Type _resourceManagerType;
        private bool _isUsingResourceMode;

        public HelpTextAttribute(string helpText) {
            _helpText = helpText;
        }

        public HelpTextAttribute(Type resourceManagerType, string resource) {
            _resourceManagerType = resourceManagerType;
            _helpText = resource;
            _isUsingResourceMode = true;
        }

        public string HelpText {
            get {
                if (string.IsNullOrEmpty(_helpText)) {
                    throw new InvalidOperationException(DataAnnotationsResources.HelpTextAttribute_HelpTextCannotBeNullOrEmpty);
                }

                if (_isUsingResourceMode && _proxy == null) {
                    if (_resourceManagerType == null) {
                        throw new InvalidOperationException(DataAnnotationsResources.HelpTextAttribute_ResourceManagerTypeCannotBeNullOrEmpty);
                    }

                    _proxy = new ResourceProxy(_resourceManagerType, _helpText);
                }

                return _proxy != null ? _proxy.GetResource() : _helpText;
            }
        }
    }
}
