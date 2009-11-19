// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#if !SILVERLIGHT
namespace System.ComponentModel.DataAnnotations {

    public class AssociatedMetadataTypeTypeDescriptionProvider : TypeDescriptionProvider {
        private AssociatedMetadataTypeTypeDescriptor Descriptor { get; set; }

        public AssociatedMetadataTypeTypeDescriptionProvider(Type type)
            : base(TypeDescriptor.GetProvider(type)) {
            Descriptor = new AssociatedMetadataTypeTypeDescriptor(base.GetTypeDescriptor(type, null), type);
        }

        public AssociatedMetadataTypeTypeDescriptionProvider(Type type, Type associatedMetadataType)
            : base(TypeDescriptor.GetProvider(type)) {
            if (associatedMetadataType == null) {
                throw new ArgumentNullException("associatedMetadataType");
            }

            Descriptor = new AssociatedMetadataTypeTypeDescriptor(base.GetTypeDescriptor(type, null), type, associatedMetadataType);
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) {
            return Descriptor;
        }
    }
}
#endif
