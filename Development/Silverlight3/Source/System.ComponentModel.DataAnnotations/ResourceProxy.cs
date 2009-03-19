using System.ComponentModel.DataAnnotations.Resources;
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel.DataAnnotations {
    internal class ResourceProxy {
        private PropertyInfo prop { get; set; }

        public ResourceProxy(Type resourceManagerType, string resource) {
            if (resourceManagerType == null) {
                throw new ArgumentNullException("resourceManagerType");
            }

            if (resource == null) {
                throw new ArgumentNullException("resource");
            }

            prop = resourceManagerType.GetProperty(resource, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

            if (prop == null) {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    DataAnnotationsResources.ResourceProxy_UnableToFindResource,
                    resource, resourceManagerType.FullName));
            }
        }

        public string GetResource() {
            return (string)prop.GetValue(null, null);
        }
    }
}
