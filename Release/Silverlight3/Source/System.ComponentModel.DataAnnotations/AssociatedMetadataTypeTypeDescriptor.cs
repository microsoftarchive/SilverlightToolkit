// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#if !SILVERLIGHT
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Resources;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace System.ComponentModel.DataAnnotations {
    internal class AssociatedMetadataTypeTypeDescriptor : CustomTypeDescriptor {
        private Type AssociatedMetadataType { get; set; }
 
        public AssociatedMetadataTypeTypeDescriptor(ICustomTypeDescriptor parent, Type type)
            : this(parent, type, GetAssociatedMetadataType(type)) {
        }

        public AssociatedMetadataTypeTypeDescriptor(ICustomTypeDescriptor parent, Type type, Type associatedMetadataType)
            : base(parent) {
            if (associatedMetadataType != null) {
                CheckAssociatedMetadataType(type, associatedMetadataType);
                AssociatedMetadataType = associatedMetadataType;
            }
        }

        public override PropertyDescriptorCollection GetProperties() {
            PropertyDescriptorCollection originalCollection = base.GetProperties();

            bool customDescriptorsCreated = false;
            List<PropertyDescriptor> tempPropertyDescriptors = new List<PropertyDescriptor>();

            foreach (PropertyDescriptor propDescriptor in originalCollection) {
                Attribute[] newMetadata = GetFieldMetadata(propDescriptor).ToArray();
                if (newMetadata.Length > 0) {
                    tempPropertyDescriptors.Add(new MetadataPropertyDescriptorWrapper(propDescriptor, newMetadata));
                    customDescriptorsCreated = true;
                } else {
                    tempPropertyDescriptors.Add(propDescriptor);
                }
            }

            if (customDescriptorsCreated) {
                return new PropertyDescriptorCollection(tempPropertyDescriptors.ToArray(), true);
            } else {
                return originalCollection;
            }
        }

        public override AttributeCollection GetAttributes() {
            // Since normal TD behavior is to return cached attribute instances on subsequent
            // calls to GetAttributes, we must be sure below to use the TD APIs to get both
            // the base and associated attributes
            AttributeCollection attributes = base.GetAttributes();
            if (AssociatedMetadataType != null)
            {
                // Note that the use of TypeDescriptor.GetAttributes here opens up the possibility of
                // infinite recursion, in the corner case of two Types referencing each other as
                // metadata types (or a longer cycle)
                Attribute[] newAttributes = TypeDescriptor.GetAttributes(AssociatedMetadataType).OfType<Attribute>().ToArray();
                attributes = AttributeCollection.FromExisting(attributes, newAttributes);
            }
            return attributes;
        }

        private IEnumerable<Attribute> GetFieldMetadata(PropertyDescriptor fieldProperty) {

            if (AssociatedMetadataType != null) {
                MemberTypes allowedMemberTypes = MemberTypes.Property | MemberTypes.Field;
                foreach (MemberInfo buddyMember in AssociatedMetadataType.GetMembers()) {
                    if ((buddyMember.MemberType & allowedMemberTypes) != 0 && PropertyMatches(fieldProperty, buddyMember)) {
                        foreach (Attribute attribute in buddyMember.GetCustomAttributes(false)) {
                            yield return attribute;
                        }
                        yield break;
                    }
                }
            }
        }

        private bool PropertyMatches(PropertyDescriptor propertyA, MemberInfo memberB) {
            // 
            return propertyA.Name.Equals(memberB.Name, StringComparison.Ordinal);
        }

        private static Type GetAssociatedMetadataType(Type type) {
            Type associatedMetadataType = null;

            // try association attribute
            MetadataTypeAttribute attribute = (MetadataTypeAttribute)Attribute.GetCustomAttribute(type, typeof(MetadataTypeAttribute));
            if (attribute != null) {
                associatedMetadataType = attribute.MetadataClassType;
            }

            return associatedMetadataType;
        }

        private static void CheckAssociatedMetadataType(Type mainType, Type associatedMetadataType) {
            // check if buddy type has any public properties or fields whose names do not match
            // the names of properties on the main type
            List<string> invalidBuddyTypeMemberNames = new List<string>();

            // only properties from main type
            List<string> mainTypeMemberNames = new List<string>(mainType.GetProperties().Select<PropertyInfo, string>(prop => prop.Name));

            // properties and fields from buddy type
            List<MemberInfo> buddyTypeMembers = new List<MemberInfo>();
            buddyTypeMembers.AddRange(associatedMetadataType.GetProperties());
            buddyTypeMembers.AddRange(associatedMetadataType.GetFields());

            foreach (var buddyTypeMember in buddyTypeMembers) {
                // 
                if (!mainTypeMemberNames.Contains(buddyTypeMember.Name, StringComparer.Ordinal)) {
                    invalidBuddyTypeMemberNames.Add(buddyTypeMember.Name);
                }
            }

            if (invalidBuddyTypeMemberNames.Count > 0) {
                throw new InvalidOperationException(String.Format(
                    CultureInfo.CurrentCulture,
                    DataAnnotationsResources.AssociatedMetadataTypeTypeDescriptor_MetadataTypeContainsUnknownProperties,
                    mainType.FullName,
                    String.Join(", ", invalidBuddyTypeMemberNames.ToArray())));
            }
        }
    }
}
#endif
