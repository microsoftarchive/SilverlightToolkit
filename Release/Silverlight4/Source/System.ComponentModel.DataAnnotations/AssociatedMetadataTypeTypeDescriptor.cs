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
#if !TARGET_35
using System.Collections.Concurrent;
#endif

namespace System.ComponentModel.DataAnnotations {
    internal class AssociatedMetadataTypeTypeDescriptor : CustomTypeDescriptor {
        private Type AssociatedMetadataType {
            get;
            set;
        }

        public AssociatedMetadataTypeTypeDescriptor(ICustomTypeDescriptor parent, Type type, Type associatedMetadataType)
            : base(parent) {
            AssociatedMetadataType = associatedMetadataType ?? TypeDescriptorCache.GetAssociatedMetadataType(type);
            if (AssociatedMetadataType != null) {
                TypeDescriptorCache.ValidateMetadataType(type, AssociatedMetadataType);
            }
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
            return GetPropertiesWithMetadata(base.GetProperties(attributes));
        }

        public override PropertyDescriptorCollection GetProperties() {
            return GetPropertiesWithMetadata(base.GetProperties());
        }

        private PropertyDescriptorCollection GetPropertiesWithMetadata(PropertyDescriptorCollection originalCollection) {
            if (AssociatedMetadataType == null) {
                return originalCollection;
            }

            bool customDescriptorsCreated = false;
            List<PropertyDescriptor> tempPropertyDescriptors = new List<PropertyDescriptor>();
            foreach (PropertyDescriptor propDescriptor in originalCollection) {
                Attribute[] newMetadata = TypeDescriptorCache.GetAssociatedMetadata(AssociatedMetadataType, propDescriptor.Name);
                PropertyDescriptor descriptor = propDescriptor;
                if (newMetadata.Length > 0) {
                    // Create a metadata descriptor that wraps the property descriptor
                    descriptor = new MetadataPropertyDescriptorWrapper(propDescriptor, newMetadata);
                    customDescriptorsCreated = true;
                }

                tempPropertyDescriptors.Add(descriptor);
            }

            if (customDescriptorsCreated) {
                return new PropertyDescriptorCollection(tempPropertyDescriptors.ToArray(), true);
            }
            return originalCollection;
        }

        public override AttributeCollection GetAttributes() {
            // Since normal TD behavior is to return cached attribute instances on subsequent
            // calls to GetAttributes, we must be sure below to use the TD APIs to get both
            // the base and associated attributes
            AttributeCollection attributes = base.GetAttributes();
            if (AssociatedMetadataType != null) {
                // Note that the use of TypeDescriptor.GetAttributes here opens up the possibility of
                // infinite recursion, in the corner case of two Types referencing each other as
                // metadata types (or a longer cycle)
                Attribute[] newAttributes = TypeDescriptor.GetAttributes(AssociatedMetadataType).OfType<Attribute>().ToArray();
                attributes = AttributeCollection.FromExisting(attributes, newAttributes);
            }
            return attributes;
        }

        private static class TypeDescriptorCache {
            private static readonly Attribute[] emptyAttributes = new Attribute[0];
#if !TARGET_35
            // Stores the associated metadata type for a type
            private static readonly ConcurrentDictionary<Type, Type> _metadataTypeCache = new ConcurrentDictionary<Type, Type>();

            // Stores the attributes for a member info
            private static readonly ConcurrentDictionary<Tuple<Type, string>, Attribute[]> _typeMemberCache = new ConcurrentDictionary<Tuple<Type, string>, Attribute[]>();

            // Stores whether or not a type and associated metadata type has been checked for validity
            private static readonly ConcurrentDictionary<Tuple<Type, Type>, bool> _validatedMetadataTypeCache = new ConcurrentDictionary<Tuple<Type, Type>, bool>();
#else
            // Stores the associated metadata type for a type
            private static readonly Dictionary<Type, Type> _metadataTypeCache = new Dictionary<Type, Type>();

            // For a type and a property name stores the attributes for that property name.
            private static readonly Dictionary<Tuple<Type, string>, Attribute[]> _typeMemberCache = new Dictionary<Tuple<Type, string>, Attribute[]>();

            // Stores whether or not a type and associated metadata type has been checked for validity
            private static readonly Dictionary<Tuple<Type, Type>, bool> _validatedMetadataTypeCache = new Dictionary<Tuple<Type, Type>, bool>();
#endif


            public static void ValidateMetadataType(Type type, Type associatedType) {
                Tuple<Type, Type> typeTuple = new Tuple<Type, Type>(type, associatedType);
                if (!_validatedMetadataTypeCache.ContainsKey(typeTuple)) {
                    CheckAssociatedMetadataType(type, associatedType);
#if !TARGET_35
                    _validatedMetadataTypeCache.TryAdd(typeTuple, true);
#else
                    _validatedMetadataTypeCache.Add(typeTuple, true);
#endif
                }
            }

            public static Type GetAssociatedMetadataType(Type type) {
                Type associatedMetadataType = null;
                if (_metadataTypeCache.TryGetValue(type, out associatedMetadataType)) {
                    return associatedMetadataType;
                }

                // Try association attribute
                MetadataTypeAttribute attribute = (MetadataTypeAttribute)Attribute.GetCustomAttribute(type, typeof(MetadataTypeAttribute));
                if (attribute != null) {
                    associatedMetadataType = attribute.MetadataClassType;
                }
#if !TARGET_35
                _metadataTypeCache.TryAdd(type, associatedMetadataType);
#else
                _metadataTypeCache.Add(type, associatedMetadataType);
#endif
                return associatedMetadataType;
            }

            private static void CheckAssociatedMetadataType(Type mainType, Type associatedMetadataType) {
                // Only properties from main type
                HashSet<string> mainTypeMemberNames = new HashSet<string>(mainType.GetProperties().Select(p => p.Name));

                // Properties and fields from buddy type
                var buddyFields = associatedMetadataType.GetFields().Select(f => f.Name);
                var buddyProperties = associatedMetadataType.GetProperties().Select(p => p.Name);
                HashSet<string> buddyTypeMembers = new HashSet<string>(buddyFields.Concat(buddyProperties), StringComparer.Ordinal);

                // Buddy members should be a subset of the main type's members
                if (!buddyTypeMembers.IsSubsetOf(mainTypeMemberNames)) {
                    // Reduce the buddy members to the set not contained in the main members
                    buddyTypeMembers.ExceptWith(mainTypeMemberNames);

                    throw new InvalidOperationException(String.Format(
                        CultureInfo.CurrentCulture,
                        DataAnnotationsResources.AssociatedMetadataTypeTypeDescriptor_MetadataTypeContainsUnknownProperties,
                        mainType.FullName,
                        String.Join(", ", buddyTypeMembers.ToArray())));
                }
            }

            public static Attribute[] GetAssociatedMetadata(Type type, string memberName) {
                var memberTuple = new Tuple<Type, string>(type, memberName);
                Attribute[] attributes;
                if (_typeMemberCache.TryGetValue(memberTuple, out attributes)) {
                    return attributes;
                }

                // Allow fields and properties
                MemberTypes allowedMemberTypes = MemberTypes.Property | MemberTypes.Field;
                // Only public static/instance members
                BindingFlags searchFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
                // Try to find a matching member on type
                MemberInfo matchingMember = type.GetMember(memberName, allowedMemberTypes, searchFlags).FirstOrDefault();
                if (matchingMember != null) {
                    attributes = Attribute.GetCustomAttributes(matchingMember, true /* inherit */);
                }
                else {
                    attributes = emptyAttributes;
                }

#if !TARGET_35
                _typeMemberCache.TryAdd(memberTuple, attributes);
#else
                _typeMemberCache.Add(memberTuple, attributes);
#endif
                return attributes;
            }

#if TARGET_35
            public class Tuple<T1, T2> {
                public T1 Item1 { get; set; }
                public T2 Item2 { get; set; }

                public Tuple(T1 item1, T2 item2) {
                    Item1 = item1;
                    Item2 = item2;
                }

                public override int GetHashCode() {
                    int h1 = Item1.GetHashCode();
                    int h2 = Item2.GetHashCode();
                    return ((h1 << 5) + h1) ^ h2;
                }

                public override bool Equals(object obj) {
                    var other = obj as Tuple<T1, T2>;
                    if (other != null) {
                        return other.Item1.Equals(Item1) && other.Item2.Equals(Item2);
                    }
                    return false;
                }
            }
#endif

        }
    }
}
#endif
