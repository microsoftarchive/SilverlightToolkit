// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class ValidationContextTest {
        [TestMethod]
        [Description("Tests all legal ctors for ValidationContext")]
        public void ValidationContext_Ctors() {
            object instance = new object();
            ValidationContext context = new ValidationContext(instance, null, null);
            Assert.AreSame(instance, context.ObjectInstance);
            Assert.AreEqual(typeof(object), context.ObjectType);
        }

        [TestMethod]
        [Description("Tests all legal property setters for ValidationContext")]
        public void ValidationContext_Properties() {
            object instance = new object();
            ValidationContext context = new ValidationContext(instance, null, null);

            context.MemberName = "m1";
            Assert.AreEqual("m1", context.MemberName);

            context.DisplayName = "d1";
            Assert.AreEqual("d1", context.DisplayName);
        }

        [TestMethod]
        [Description("Tests DisplayName inherits from Type name in absence of MemberName")]
        public void ValidationContext_DisplayName_Inherits_Type_Name() {
            ValidationContext context = new ValidationContext(this, null, null);
            Assert.AreEqual(this.GetType().Name, context.DisplayName);
        }

        [TestMethod]
        [Description("Tests DisplayName inherits from MemberName for Property with DisplayAttribute")]
        public void ValidationContext_DisplayName_Inherits_For_Property_With_Display() {
            ValidationContext context = new ValidationContext(this, null, null);
            context.MemberName = "DisplayProperty";
            Assert.AreEqual("Property Display", context.DisplayName);
        }

        [TestMethod]
        [Description("Tests DisplayName inherits from MemberName for Property without a DisplayAttribute")]
        public void ValidationContext_DisplayName_Inherits_For_Property_Without_Display() {
            ValidationContext context = new ValidationContext(this, null, null);
            context.MemberName = "NonDisplayProperty";
            Assert.AreEqual("NonDisplayProperty", context.DisplayName);
        }

        [TestMethod]
        [Description("Tests DisplayName inherits from MemberName for Method")]
        public void ValidationContext_DisplayName_Inherits_For_Method() {
            ValidationContext context = new ValidationContext(this, null, null);
            context.MemberName = "ValidationContext_DisplayName_Inherits_For_Method";
            Assert.AreEqual("ValidationContext_DisplayName_Inherits_For_Method", context.DisplayName);
        }

        [TestMethod]
        [Description("ValidationContext ctor fails if instance is null")]
        public void ValidationContext_Fail_Null_Instance() {
            ExceptionHelper.ExpectArgumentNullExceptionStandard(delegate {
                ValidationContext context = new ValidationContext((object)null, null, null);
            }, "instance");
        }

        [TestMethod]
        [Description("ValidationContext allows null MemberName without throwing an argument null exception")]
        public void ValidationContext_Allow_Null_MemberName() {
            ValidationContext context = new ValidationContext(new object(), null, null);
            context.MemberName = null;

            Assert.IsNull(context.MemberName);
        }

        [TestMethod]
        [Description("ValidationContext throws if set null DisplayName")]
        public void ValidationContext_Fail_Null_DisplayName() {
            ValidationContext context = new ValidationContext(new object(), null, null);
            ExceptionHelper.ExpectArgumentNullExceptionStandard(delegate {
                context.DisplayName = null;
            }, "value");

            ExceptionHelper.ExpectArgumentNullExceptionStandard(delegate {
                context.DisplayName = string.Empty;
            }, "value");
        }

        [TestMethod]
        [Description("ValidationContext accepts null service provider")]
        public void ValidationContext_ServiceProvider_Null() {
            ValidationContext context = new ValidationContext(new object(), null, null);
            object service = context.GetService(typeof(string));
            Assert.IsNull(service);
        }

        [TestMethod]
        [Description("ValidationContext accepts valid service provider")]
        public void ValidationContext_ServiceProvider() {
            IServiceProvider provider = new MyServiceProvider();
            ValidationContext context = new ValidationContext(new object(), provider, null);
            object service = context.GetService(typeof(string));
            Assert.IsNotNull(service);
            Assert.AreEqual("hello", service);

            service = context.GetService(typeof(Guid));
            Assert.IsNull(service);
        }

        [TestMethod]
        [Description("ValidationContext default Items")]
        public void ValidationContext_Default_Items() {
            object instance = new object();
            ValidationContext context = new ValidationContext(instance, null, null);
            IDictionary<object, object> items = context.Items;
            Assert.IsNotNull(items);
            Assert.AreEqual(0, items.Count);

            items["key"] = "value";
            Assert.AreEqual("value", context.Items["key"]);
        }

        [TestMethod]
        [Description("ValidationContext explicit Items is immutable")]
        public void ValidationContext_Explicit_Items() {
            Dictionary<object, object> dictionary = new Dictionary<object, object>();
            dictionary["defaultKey"] = "defaultValue";
            ValidationContext context = new ValidationContext(new object(), new MyServiceProvider(), dictionary);
            IDictionary<object, object> items = context.Items;
            Assert.IsNotNull(items);
            Assert.AreEqual(1, items.Count);
            Assert.AreEqual("defaultValue", context.Items["defaultKey"]);

            items["key"] = "value";
            Assert.AreEqual("value", context.Items["key"]);
            Assert.AreEqual(2, items.Count);

            // Ensure changing VC's dictionary did not affect original
            Assert.IsFalse(dictionary.ContainsKey("key"));
            Assert.AreEqual(1, dictionary.Count);
        }

#if !SILVERLIGHT
        [TestMethod]
        [Description("Can provide a container to ValidationContext and access its services through the ServiceContainer property")]
        public void ValidationContext_Can_Accept_Container_And_Return_Service_From_ServiceContainer() {
            MyServiceContainer container = new MyServiceContainer();
            MyTestService service = new MyTestService();
            container.AddService(service.GetType(), service);

            ValidationContext context = new ValidationContext(this, container, null);
            Assert.AreSame(service, context.ServiceContainer.GetService(service.GetType()));
        }

        [TestMethod]
        [Description("Can provide a container to ValidationContext and access its services through the GetSerice method")]
        public void ValidationContext_Can_Accept_Container_And_Return_Service_From_GetService() {
            MyServiceContainer container = new MyServiceContainer();
            MyTestService service = new MyTestService();
            container.AddService(service.GetType(), service);

            ValidationContext context = new ValidationContext(this, container, null);
            Assert.AreSame(service, context.GetService(service.GetType()));
        }

        [TestMethod]
        [Description("Can add services to empty container and get them back through container")]
        public void ValidationContext_Can_Add_And_Get_Service_From_Container() {
            ValidationContext context = new ValidationContext(this, null, null);
            MyTestService service = new MyTestService();
            context.ServiceContainer.AddService(service.GetType(), service);
            Assert.AreSame(service, context.ServiceContainer.GetService(service.GetType()));
        }

        [TestMethod]
        [Description("Can add services to empty container and get them back through GetService")]
        public void ValidationContext_Can_Add_And_Get_Service() {
            ValidationContext context = new ValidationContext(this, null, null);
            MyTestService service = new MyTestService();
            context.ServiceContainer.AddService(service.GetType(), service);
            Assert.AreSame(service, context.GetService(service.GetType()));
        }

        [TestMethod]
        [Description("Can add services to container without affecting parent container")]
        public void ValidationContext_Allows_Services_To_Not_Promote_To_Parent_Container() {
            MyServiceContainer container = new MyServiceContainer();
            ValidationContext context = new ValidationContext(this, container, null);

            MyTestService service = new MyTestService();
            context.ServiceContainer.AddService(service.GetType(), service, false);

            Assert.IsNull(container.GetService(service.GetType()));
        }

        [TestMethod]
        [Description("Can add services to container and have them promote to parent implicitly")]
        public void ValidationContext_Allows_Services_To_Implicitly_Promote_To_Parent() {
            MyServiceContainer container = new MyServiceContainer();
            ValidationContext context = new ValidationContext(this, container, null);

            MyTestService service = new MyTestService();
            context.ServiceContainer.AddService(service.GetType(), service); // Implicit promotion

            Assert.AreSame(service, container.GetService(service.GetType()));
        }

        [TestMethod]
        [Description("Can add services to container and have them promote to parent explicitly")]
        public void ValidationContext_Allows_Services_To_Explicitly_Promote_To_Parent() {
            MyServiceContainer container = new MyServiceContainer();
            ValidationContext context = new ValidationContext(this, container, null);

            MyTestService service = new MyTestService();
            context.ServiceContainer.AddService(service.GetType(), service, true); // Explicit promotion

            Assert.AreSame(service, container.GetService(service.GetType()));
        }

        [TestMethod]
        [Description("Can add service callbacks to container and get them through validation context's GetService")]
        public void ValidationContext_Exposes_Services_Through_Callbacks_In_Parent_Container() {
            MyServiceContainer container = new MyServiceContainer();
            container.AddService(typeof(MyTestService), new System.ComponentModel.Design.ServiceCreatorCallback((c, t) => { return new MyTestService(); }));

            ValidationContext context = new ValidationContext(this, container, null);
            Assert.IsInstanceOfType(context.GetService(typeof(MyTestService)), typeof(MyTestService));
        }

        [TestMethod]
        [Description("Can add service callbacks to container and get them through validation context's service container")]
        public void ValidationContext_Exposes_Services_Through_Callbacks_From_ServiceContainer() {
            ValidationContext context = new ValidationContext(this, null, null);
            context.ServiceContainer.AddService(typeof(MyTestService), new System.ComponentModel.Design.ServiceCreatorCallback((c, t) => { return new MyTestService(); }));
            Assert.IsInstanceOfType(context.GetService(typeof(MyTestService)), typeof(MyTestService));
        }

        [TestMethod]
        [Description("Requesting a service that isn't registered returns null")]
        public void ValidationContext_GetService_Returns_Null_When_Service_Not_Found() {
            ValidationContext context = new ValidationContext(this, null, null);
            Assert.IsNull(context.GetService(typeof(MyTestService)));
            Assert.IsNull(context.ServiceContainer.GetService(typeof(MyTestService)));
        }

        [TestMethod]
        [Description("Can remove service from container")]
        public void ValidationContext_Can_Remove_Service_From_Container() {
            ValidationContext context = new ValidationContext(this, null, null);
            context.ServiceContainer.AddService(typeof(MyTestService), new MyTestService());
            Assert.IsInstanceOfType(context.GetService(typeof(MyTestService)), typeof(MyTestService));
            context.ServiceContainer.RemoveService(typeof(MyTestService));
            Assert.IsNull(context.GetService(typeof(MyTestService)));
        }

        [TestMethod]
        [Description("Can remove service from container without affecting parent container")]
        public void ValidationContext_Remove_Service_Can_Not_Affect_Parent() {
            MyServiceContainer container = new MyServiceContainer();
            container.AddService(typeof(MyTestService), new MyTestService());

            ValidationContext context = new ValidationContext(this, container, null);
            Assert.IsInstanceOfType(context.GetService(typeof(MyTestService)), typeof(MyTestService));
            context.ServiceContainer.RemoveService(typeof(MyTestService), false);
            Assert.IsInstanceOfType(context.GetService(typeof(MyTestService)), typeof(MyTestService));
        }

        [TestMethod]
        [Description("Can remove service from container and have it explicitly promote the removal to the parent container")]
        public void ValidationContext_Can_Remove_Service_From_Parent_Container_Explicitly() {
            MyServiceContainer container = new MyServiceContainer();
            container.AddService(typeof(MyTestService), new MyTestService());

            ValidationContext context = new ValidationContext(this, container, null);
            Assert.IsInstanceOfType(context.GetService(typeof(MyTestService)), typeof(MyTestService));
            context.ServiceContainer.RemoveService(typeof(MyTestService), true);
            Assert.IsNull(context.GetService(typeof(MyTestService)));
        }

        [TestMethod]
        [Description("Can remove service from container and have it implicitly promote the removal to the parent container")]
        public void ValidationContext_Can_Remove_Service_From_Parent_Container_Implicitly() {
            MyServiceContainer container = new MyServiceContainer();
            container.AddService(typeof(MyTestService), new MyTestService());

            ValidationContext context = new ValidationContext(this, container, null);
            Assert.IsInstanceOfType(context.GetService(typeof(MyTestService)), typeof(MyTestService));
            context.ServiceContainer.RemoveService(typeof(MyTestService)); // Implicit promotion
            Assert.IsNull(context.GetService(typeof(MyTestService)));
        }

#endif

        [Display(Name = "Property Display")]
        public string DisplayProperty { get; set; }

        public string NonDisplayProperty { get; set; }
    }

    public class MyServiceProvider : IServiceProvider {

        #region IServiceProvider Members

        public object GetService(Type serviceType) {
            if (serviceType == typeof(string))
                return "hello";
            return null;
        }

        #endregion
    }

#if !SILVERLIGHT
    public class MyServiceContainer : Design.IServiceContainer {
        private Dictionary<Type, object> _services = new Dictionary<Type, object>();

        #region IServiceContainer Members

        public void AddService(Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback, bool promote) {
            this._services[serviceType] = callback;
        }

        public void AddService(Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback) {
            this._services[serviceType] = callback;
        }

        public void AddService(Type serviceType, object serviceInstance, bool promote) {
            this._services[serviceType] = serviceInstance;
        }

        public void AddService(Type serviceType, object serviceInstance) {
            this._services[serviceType] = serviceInstance;
        }

        public void RemoveService(Type serviceType, bool promote) {
            this._services.Remove(serviceType);
        }

        public void RemoveService(Type serviceType) {
            this._services.Remove(serviceType);
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType) {
            if (this._services.ContainsKey(serviceType)) {
                return this._services[serviceType];
            }

            return null;
        }

        #endregion
    }

    public class MyTestService {
        public string GetFoo() {
            return "foo";
        }
    }

#endif
}
