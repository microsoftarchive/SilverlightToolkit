
#if !SILVERLIGHT
using System;
using System.ComponentModel;

namespace System.ComponentModel.DataAnnotations {
    internal class MetadataPropertyDescriptorWrapper : PropertyDescriptor {
        private PropertyDescriptor _descriptor;
        public MetadataPropertyDescriptorWrapper(PropertyDescriptor descriptor, Attribute[] attrs)
            : base(descriptor, attrs) {
            _descriptor = descriptor;
        }

        public override void AddValueChanged(object component, EventHandler handler) { _descriptor.AddValueChanged(component, handler); }

        public override bool CanResetValue(object component) { return _descriptor.CanResetValue(component); }

        public override Type ComponentType { get { return _descriptor.ComponentType; } }

        public override object GetValue(object component) { return _descriptor.GetValue(component); }

        public override bool IsReadOnly { get { return _descriptor.IsReadOnly; } }

        public override Type PropertyType { get { return _descriptor.PropertyType; } }

        public override void RemoveValueChanged(object component, EventHandler handler) { _descriptor.RemoveValueChanged(component, handler); }

        public override void ResetValue(object component) { _descriptor.ResetValue(component); }

        public override void SetValue(object component, object value) { _descriptor.SetValue(component, value); }

        public override bool ShouldSerializeValue(object component) { return _descriptor.ShouldSerializeValue(component); }

        public override bool SupportsChangeEvents { get { return _descriptor.SupportsChangeEvents; } }
    }
}
#endif
