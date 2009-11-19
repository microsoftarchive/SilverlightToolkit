// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#if !SILVERLIGHT
namespace System.ComponentModel.DataAnnotations {

    public class AssociatedMetadataTypeTypeDescriptionProvider : TypeDescriptionProvider {
        private Type _associatedMetadataType;
        public AssociatedMetadataTypeTypeDescriptionProvider(Type type)
            : base(TypeDescriptor.GetProvider(type)) {
        }

        public AssociatedMetadataTypeTypeDescriptionProvider(Type type, Type associatedMetadataType)
            : this(type) {
            if (associatedMetadataType == null) {
                throw new ArgumentNullException("associatedMetadataType");
            }

            _associatedMetadataType = associatedMetadataType;
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) {
            ICustomTypeDescriptor baseDescriptor = base.GetTypeDescriptor(objectType, instance);
            return new AssociatedMetadataTypeTypeDescriptor(baseDescriptor, objectType, _associatedMetadataType);
        }
    }
}
#endif
