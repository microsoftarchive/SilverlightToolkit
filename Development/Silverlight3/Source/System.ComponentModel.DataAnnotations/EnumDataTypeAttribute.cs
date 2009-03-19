using System.ComponentModel.DataAnnotations.Resources;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.ComponentModel.DataAnnotations {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = false)]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "We want users to be able to extend this class")]
    public class EnumDataTypeAttribute : DataTypeAttribute {
        public Type EnumType { get; private set; }

        public EnumDataTypeAttribute(Type enumType)
            : base("Enumeration") {
            EnumType = enumType;
        }

        public override bool IsValid(object value) {
            if (EnumType == null) {
                throw new InvalidOperationException(DataAnnotationsResources.EnumDataTypeAttribute_TypeCannotBeNull);
            }
            if (!EnumType.IsEnum) {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, DataAnnotationsResources.EnumDataTypeAttribute_TypeNeedsToBeAnEnum, EnumType.FullName));
            }

            if (value == null) {
                return true;
            }
            string stringValue = value as string;
            if (stringValue != null && String.IsNullOrEmpty(stringValue)) {
                return true;
            }

            Type valueType = value.GetType();
            if (valueType.IsEnum && EnumType != valueType) {
                // don't match a different enum that might map to the same underlying integer
                return false;
            }

            if (!valueType.IsValueType && valueType != typeof(string)) {
                // non-value types cannot be converted
                return false;
            }

            if (valueType == typeof(bool) ||
                valueType == typeof(float) ||
                valueType == typeof(double) ||
                valueType == typeof(decimal) ||
                valueType == typeof(char)) {
                // non-integral types cannot be converted
                return false;
            }

            object convertedValue;
            if (valueType.IsEnum) {
                Debug.Assert(valueType == value.GetType());
                convertedValue = value;
            } else {
                try {
                    if (value is string) {
                        convertedValue = Enum.Parse(EnumType, (string)value, false);
                    } else {
                        convertedValue = Enum.ToObject(EnumType, value);
                    }
                } catch (ArgumentException) {
                    return false;
                }
            }

            if (IsEnumTypeInFlagsMode(EnumType)) {
                // Ensure that the value is a valid flag combination
                // If it is, the string representation of the enum value will be something like "A, B", while
                // the string representation of the underlying value will be "3". If the enum value does not
                // match a valid flag combination, then it would also be something like "3".
                string underlying = GetUnderlyingTypeValueString(EnumType, convertedValue);
                string converted = convertedValue.ToString();
                return !underlying.Equals(converted);
            } else {
                return Enum.IsDefined(EnumType, convertedValue);
            }
        }

        private static bool IsEnumTypeInFlagsMode(Type enumType) {
            return enumType.GetCustomAttributes(typeof(FlagsAttribute), false).Length != 0;
        }


        private static string GetUnderlyingTypeValueString(Type enumType, object enumValue) {
            return Convert.ChangeType(enumValue, Enum.GetUnderlyingType(enumType), CultureInfo.InvariantCulture).ToString();
        }
    }
}
