// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeConverterAttribute = System.ComponentModel.TypeConverterAttribute;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// DependencyPropertyTest provides rich support for testing the behavior
    /// of a dependency property.
    /// </summary>
    /// <typeparam name="T">Type containing the dependency property.</typeparam>
    /// <typeparam name="P">Type of the dependency property.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T", Justification = "The name provides enough context.")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "P", Justification = "The name provides enough context.")]
    public partial class DependencyPropertyTest<T, P>
        where T : FrameworkElement
    {
        /// <summary>
        /// Gets a reference to the test class.
        /// </summary>
        public TestBase Test { get; private set; }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the actual DependencyProperty.
        /// </summary>
        public DependencyProperty Property { get; set; }

        /// <summary>
        /// Gets or sets a function used to create a new instance of the type
        /// that contains the dependency property.
        /// </summary>
        public Func<T> Initializer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the property is attached.
        /// </summary>
        public bool IsAttached { get; set; }

        /// <summary>
        /// Gets or sets a function used to create a new instance of a type
        /// that an attached property can be set on.
        /// </summary>
        public Func<FrameworkElement> AttachedInitializer { get; set; }

        /// <summary>
        /// Gets or sets the default value of the dependency property.
        /// </summary>
        public P DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets a collection of acceptable non-default values for the
        /// dependency property.
        /// </summary>
        public IEnumerable<P> OtherValues { get; set; }

        /// <summary>
        /// Gets a combination of the DefaultValue and OtherValues.
        /// </summary>
        protected IEnumerable<P> AllValues
        {
            get
            {
                yield return DefaultValue;
                if (OtherValues != null)
                {
                    foreach (P value in OtherValues)
                    {
                        yield return value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a mapping of invalid values to the types of exceptions
        /// that they throw.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "By design to allow inline initialization")]
        public IDictionary<P, Type> InvalidValues { get; set; }

        /// <summary>
        /// Gets or sets the dependency property test used as the DataTemplate
        /// for the dependency property.  This is only useful for properties
        /// like Content which has a ContentTemplate, Header which has a
        /// HeaderTemplate, etc.
        /// </summary>
        public DependencyPropertyTest<T, DataTemplate> TemplateProperty { get; set; }

        /// <summary>
        /// Initializes a new instance of the DependencyPropertyTest class.
        /// </summary>
        /// <param name="test">Reference to the test class.</param>
        /// <param name="name">Name of the property.</param>
        public DependencyPropertyTest(TestBase test, string name)
        {
            Assert.IsNotNull(test, "Reference to the test class should not be null!");
            Assert.IsFalse(string.IsNullOrEmpty(name), "The property name must be provided!");
            Test = test;
            Name = name;
        }

        /// <summary>
        /// Get an instance that the property can be set on.
        /// </summary>
        /// <returns>Instance that the property can be set on.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method is more appropriate.")]
        public FrameworkElement GetInstance()
        {
            if (!IsAttached)
            {
                Assert.IsNotNull(Initializer, "Initializer");
                return Initializer();
            }
            else
            {
                Assert.IsNotNull(AttachedInitializer, "AttachedInitializer");
                return AttachedInitializer();
            }
        }

        /// <summary>
        /// Get the value of the dependency property using its CLR getter.
        /// </summary>
        /// <param name="instance">Instance to get the value from.</param>
        /// <returns>The value of the property.</returns>
        public P GetValue(FrameworkElement instance)
        {
            Type type = typeof(T);
            if (!IsAttached)
            {
                PropertyInfo property = type.GetProperty(Name, BindingFlags.Instance | BindingFlags.Public);
                Assert.IsNotNull("Failed to find property {0} on type {1}!", Name, type);
                return (P) property.GetValue(instance, new object[] { });
            }
            else
            {
                MethodInfo method = type.GetMethod("Get" + Name, BindingFlags.Static | BindingFlags.Public);
                Assert.IsNotNull("Failed to find method Get{0} on type {1}", Name, type);
                return (P) method.Invoke(null, new object[] { instance });
            }
        }

        /// <summary>
        /// Set the value of the dependency property using its CLR setter.
        /// </summary>
        /// <param name="instance">Instance to set the value on.</param>
        /// <param name="value">The value of the property.</param>
        public void SetValue(FrameworkElement instance, P value)
        {
            Type type = typeof(T);
            if (!IsAttached)
            {
                PropertyInfo property = type.GetProperty(Name, BindingFlags.Instance | BindingFlags.Public);
                Assert.IsNotNull("Failed to find property {0} on type {1}!", Name, type);
                property.SetValue(instance, value, new object[] { });
            }
            else
            {
                MethodInfo method = type.GetMethod("Set" + Name, BindingFlags.Static | BindingFlags.Public);
                Assert.IsNotNull("Failed to find method Set{0} on type {1}", Name, type);
                method.Invoke(null, new object[] { instance, value });
            }
        }

        /// <summary>
        /// Assert that two values are equal.
        /// </summary>
        /// <typeparam name="V">Type of the values.</typeparam>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="message">Assertion message.</param>
        /// <param name="arguments">Assertion message arguments.</param>
        private static void AssertAreEqual<V>(V expected, V actual, string message, params object[] arguments)
        {
            if (typeof(Brush).IsAssignableFrom(typeof(V)))
            {
                TestExtensions.AssertBrushesAreEqual(expected as Brush, actual as Brush, message, arguments);
            }
            else
            {
                Assert.AreEqual<V>(expected, actual, message, arguments);
            }
        }

        /// <summary>
        /// Determine whether a UIElement is currently parented.
        /// </summary>
        /// <param name="value">The element.</param>
        /// <returns>
        /// A value indicating whether a UIElement is currently parented.
        /// </returns>
        private static bool IsParented(object value)
        {
            UIElement element = value as UIElement;
            return (element != null) && (VisualTreeHelper.GetParent(element) != null);
        }

        /// <summary>
        /// Get the metadata for a test method.
        /// </summary>
        /// <param name="methodName">Name of the test method.</param>
        /// <param name="test">Test action to perform.</param>
        /// <returns>Metadata for the test method.</returns>
        protected DependencyPropertyTestMethod GetTestMethod(string methodName, Action test)
        {
            MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.IsNotNull(method, "Failed to find method {0}.", methodName);
            return new DependencyPropertyTestMethod(method, Name, test);
        }

        #region CheckDefaultValue
        /// <summary>
        /// Gets the CheckDefaultValue test.
        /// </summary>
        public DependencyPropertyTestMethod CheckDefaultValueTest
        {
            get { return GetTestMethod("CheckDefaultValue", () => CheckDefaultValue()); }
        }

        /// <summary>
        /// Check the default value of the dependency property.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Check the default value of the dependency property.")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        protected virtual void CheckDefaultValue()
        {
            FrameworkElement instance = GetInstance();
            Test.TestAsync(
                instance,
                () => AssertAreEqual(DefaultValue, GetValue(instance), "Property {0} does not have the expected default value!", Name));
        }
        #endregion CheckDefaultValue

        #region ChangeClrSetter
        /// <summary>
        /// Gets the ChangeClrSetter test.
        /// </summary>
        public DependencyPropertyTestMethod ChangeClrSetterTest
        {
            get { return GetTestMethod("ChangeClrSetter", () => ChangeClrSetter()); }
        }

        /// <summary>
        /// Change the value of the property to known good values using the CLR setter.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Change the value of the property to known good values using the CLR setter.")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        protected virtual void ChangeClrSetter()
        {
            FrameworkElement instance = GetInstance();
            List<Action> actions = new List<Action>();
            foreach (P value in AllValues)
            {
                P capturedValue = value;
                if (!IsParented(capturedValue))
                {
                    actions.Add(() => SetValue(instance, capturedValue));
                    actions.Add(() => AssertAreEqual(capturedValue, GetValue(instance), "Property {0} did not correctly round-trip a value using the CLR setter!", Name));
                }
            }
            actions.Add(() => SetValue(instance, DefaultValue));
            Test.TestAsync(instance, actions.ToArray());
        }
        #endregion ChangeClrSetter

        #region ChangeSetValue
        /// <summary>
        /// Gets the ChangeSetValue test.
        /// </summary>
        public DependencyPropertyTestMethod ChangeSetValueTest
        {
            get { return GetTestMethod("ChangeSetValue", () => ChangeSetValue()); }
        }

        /// <summary>
        /// Change the value of the property to known good values using SetValue.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Change the value of the property to known good values using SetValue.")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        protected virtual void ChangeSetValue()
        {
            Assert.IsNotNull(Property, "Property");

            FrameworkElement instance = GetInstance();
            List<Action> actions = new List<Action>();
            foreach (P value in AllValues)
            {
                P capturedValue = value;
                if (!IsParented(capturedValue))
                {
                    actions.Add(() => instance.SetValue(Property, capturedValue));
                    actions.Add(() => AssertAreEqual(capturedValue, instance.GetValue(Property), "Property {0} did not correctly round-trip a value using SetValue!", Name));
                }
            }
            actions.Add(() => SetValue(instance, DefaultValue));
            Test.TestAsync(instance, actions.ToArray());
        }
        #endregion ChangeSetValue

        #region SetNull
        /// <summary>
        /// Gets the SetNull test.
        /// </summary>
        public DependencyPropertyTestMethod SetNullTest
        {
            get { return GetTestMethod("SetNull", () => SetNull()); }
        }

        /// <summary>
        /// Set the property to null.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Set the property to null.")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        protected virtual void SetNull()
        {
            FrameworkElement instance = GetInstance();
            Test.TestAsync(
                instance,
                () => SetValue(instance, default(P)),
                () => Assert.IsNull(GetValue(instance), "Property {0} should be null!", Name));
        }
        #endregion SetNull

        #region ClearValueResetsDefault
        /// <summary>
        /// Gets the ClearValueResetsDefault test.
        /// </summary>
        public DependencyPropertyTestMethod ClearValueResetsDefaultTest
        {
            get { return GetTestMethod("ClearValueResetsDefault", () => ClearValueResetsDefault()); }
        }

        /// <summary>
        /// Verify that clearing a dependency property restores its default.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that clearing a dependency property restores its default.")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        protected virtual void ClearValueResetsDefault()
        {
            Assert.IsNotNull(Property, "Property");
            Assert.IsNotNull(OtherValues, "OtherValues");

            FrameworkElement instance = GetInstance();
            P value = OtherValues.First();
            Test.TestAsync(
                instance,
                () => SetValue(instance, value),
                () => instance.ClearValue(Property),
                () => AssertAreEqual(DefaultValue, GetValue(instance), "Expected ClearValue to restore the default value!"));
        }
        #endregion ClearValueResetsDefault

        #region InvalidValueFails
        /// <summary>
        /// Gets the InvalidValueFails test.
        /// </summary>
        public DependencyPropertyTestMethod InvalidValueFailsTest
        {
            get { return GetTestMethod("InvalidValueFails", () => InvalidValueFails()); }
        }

        /// <summary>
        /// Verify the dependency throws exceptions when setting invalid values.
        /// </summary>
        [TestMethod]
        [Description("Verify the dependency throws exceptions when setting invalid values.")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Required to test exceptions")]
        protected virtual void InvalidValueFails()
        {
            Assert.IsNotNull(InvalidValues, "InvalidValues");
            foreach (KeyValuePair<P, Type> pair in InvalidValues)
            {
                FrameworkElement instance = GetInstance();
                try
                {
                    SetValue(instance, pair.Key);
                    Assert.Fail("Value {0} failed to throw an exception!", pair.Key);
                }
                catch (AssertFailedException)
                {
                    throw;
                }
                catch (TargetInvocationException ex)
                {
                    Assert.IsInstanceOfType(ex.InnerException, pair.Value, "Value {0} should have thrown an exception of type {1}, not {2}!", pair.Key, pair.Value.FullName, ex.GetType().FullName);
                }
                catch (Exception ex)
                {
                    Assert.IsInstanceOfType(ex, pair.Value, "Value {0} should have thrown an exception of type {1}, not {2}!", pair.Key, pair.Value.FullName, ex.GetType().FullName);
                }
            }
        }
        #endregion InvalidValueFails

        #region InvalidValueIsIgnored
        /// <summary>
        /// Gets the InvalidValueIsIgnored test.
        /// </summary>
        public DependencyPropertyTestMethod InvalidValueIsIgnoredTest
        {
            get { return GetTestMethod("InvalidValueIsIgnored", () => InvalidValueIsIgnored()); }
        }

        /// <summary>
        /// Verify that any invalid values are ignored when set.
        /// </summary>
        [TestMethod]
        [Description("Verify that any invalid values are ignored when set.")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Required to test exceptions")]
        protected virtual void InvalidValueIsIgnored()
        {
            Assert.IsNotNull(InvalidValues, "InvalidValues");
            foreach (KeyValuePair<P, Type> pair in InvalidValues)
            {
                FrameworkElement instance = GetInstance();
                P original = GetValue(instance);
                try
                {
                    SetValue(instance, pair.Key);
                    Assert.Fail("Value {0} failed to throw an exception!", pair.Key);
                }
                catch
                {
                    P current = GetValue(instance);
                    AssertAreEqual(original, current, "Original value {0} was replaced by invalid value {1}!", original, current);
                }
            }
        }
        #endregion InvalidValueIsIgnored

        #region InvalidValueDoesNotChangeVisualState
        /// <summary>
        /// Gets the InvalidValueDoesNotChangeVisualState test.
        /// </summary>
        public DependencyPropertyTestMethod InvalidValueDoesNotChangeVisualStateTest
        {
            get { return GetTestMethod("InvalidValueDoesNotChangeVisualState", () => InvalidValueDoesNotChangeVisualState()); }
        }

        /// <summary>
        /// Ensure that an invalid value does not change the visual state.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that an invalid value does not change the visual state.")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        [Tag("VisualTree")]
        [Tag("VisualState")]
        protected virtual void InvalidValueDoesNotChangeVisualState()
        {
            Assert.IsNotNull(InvalidValues, "InvalidValues");
            foreach (KeyValuePair<P, Type> pair in InvalidValues)
            {
                FrameworkElement instance = GetInstance();
                TestVisualStateManager vsm = new TestVisualStateManager();

                Test.TestTaskAsync(
                    instance,
                    () =>
                    {
                        FrameworkElement root = instance.GetVisualDescendents().FirstOrDefault() as FrameworkElement;
                        Assert.IsNotNull(root, "Failed to find template root!");
                        VisualStateManager.SetCustomVisualStateManager(root, vsm);
                    },
                    () =>
                    {
                        try
                        {
                            SetValue(instance, pair.Key);
                        }
                        catch
                        {
                        }
                    },
                    () => Assert.AreEqual(0, vsm.ChangedStates.Count, "Changing the value to {0} changed the visual state!", pair.Key));
            }
            Test.EnqueueTestComplete();
        }
        #endregion InvalidValueDoesNotChangeVisualState

        #region IsReadOnly
        /// <summary>
        /// Gets the IsReadOnly test.
        /// </summary>
        public DependencyPropertyTestMethod IsReadOnlyTest
        {
            get { return GetTestMethod("IsReadOnly", () => IsReadOnly()); }
        }

        /// <summary>
        /// Determine whether a property is read only.
        /// </summary>
        [TestMethod]
        [Description("Determine whether a property is read only.")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Required to test exceptions!")]
        protected virtual void IsReadOnly()
        {
            Assert.IsNotNull(Property, "Property");
            Assert.IsNotNull(OtherValues, "OtherValues");

            FrameworkElement instance = GetInstance();
            P original = (P) instance.GetValue(Property);
            foreach (P value in OtherValues)
            {
                try
                {
                    instance.SetValue(Property, value);
                    Assert.Fail("Property should not allow setting the value {0}!", value);
                }
                catch (AssertFailedException)
                {
                    throw;
                }
                catch
                {
                    AssertAreEqual(original, GetValue(instance), "Property did not ignore the failed re-write attempt!");
                }
            }
        }
        #endregion IsReadOnly

        #region CanBeStyled
        /// <summary>
        /// Gets the CanBeStyled test.
        /// </summary>
        public DependencyPropertyTestMethod CanBeStyledTest
        {
            get { return GetTestMethod("CanBeStyled", () => CanBeStyled()); }
        }

        /// <summary>
        /// Ensure that a property can be styled.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that a property can be styled.")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        protected virtual void CanBeStyled()
        {
            Assert.IsNotNull(Property, "Property");
            Assert.IsNotNull(OtherValues, "OtherValues");
            FrameworkElement instance = GetInstance();

            P value = OtherValues.First();
            Style style = new Style(instance.GetType());
            style.Setters.Add(new Setter(Property, value));

            Test.TestAsync(
                instance,
                () => instance.Style = style,
                () => AssertAreEqual(value, GetValue(instance), "Property did not respect Style!"));
        }
        #endregion CanBeStyled

        #region TemplateBind
        /// <summary>
        /// Gets the TemplateBind test.
        /// </summary>
        public DependencyPropertyTestMethod TemplateBindTest
        {
            get { return GetTestMethod("TemplateBind", () => TemplateBind()); }
        }

        /// <summary>
        /// Verify the dependency property can be templated bound.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the dependency property can be templated bound.")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        [Tag("VisualTree")]
        protected virtual void TemplateBind()
        {
            // Determine whether the property is a core dependency property
            // (i.e. created by Silverlight) or a custom dependency property
            // to work around a known bug in core dependency properties.
            bool isCoreDependencyProperty = Property.GetType().Name == "CoreDependencyProperty";

            Assert.IsFalse(IsAttached, "Attached propeties cannot be template bound!");
            Assert.IsNotNull(OtherValues, "OtherValues");

            // Get the control
            FrameworkElement instance = GetInstance();
            Control control = instance as Control;
            Assert.IsNotNull(control, "Initializer did not create a Control!");
            Type controlType = control.GetType();

            // Create the template
            XamlBuilder<ControlTemplate> template = new XamlBuilder<ControlTemplate>
            {
                Name = "controlTemplate",
                ExplicitNamespaces = new Dictionary<string, string> { { "ctrl", XamlBuilder.GetNamespace(controlType) } },
                AttributeProperties = new Dictionary<string, string> { { "TargetType", "ctrl:" + controlType.Name } },
                Children = new List<XamlBuilder>
                {
                    new XamlBuilder<ContentControl>
                    {
                        Name = "boundContent",
                        AttributeProperties = new Dictionary<string, string> { { "Content", "{TemplateBinding " + Name + "}" } }
                    }
                }
            };

            // The XAML parser throws if a core control has an explicit prefix
            if (controlType.Assembly == typeof(DependencyObject).Assembly)
            {
                template.ExplicitNamespaces.Clear();
                template.AttributeProperties["TargetType"] = controlType.Name;
            }

            // Apply the template
            ContentControl boundContent = null;
            List<Action> actions = new List<Action>();
            actions.Add(() => control.Template = template.Load());
            actions.Add(() =>
                {
                    boundContent = control.GetVisualChild("boundContent") as ContentControl;
                    Assert.IsNotNull(boundContent, "Failed to find boundContent ContentControl in visual tree!");
                });

            // Change the values and make sure the changes are found
            foreach (P value in OtherValues)
            {
                // Explicitly capture the value so it doesn't get captured by
                // any of our async closures.
                P v = value;

                actions.Add(() => SetValue(instance, v));

                // Core Jolt types are being set as either literal uints or
                // strings when template-bound.  We'll ToString to do their
                // checks.
                if (isCoreDependencyProperty && v is Enum)
                {
                    actions.Add(() => AssertAreEqual(((int)(object)v).ToString(CultureInfo.InvariantCulture), boundContent.Content.ToString(), "Did not find template bound content."));
                }
                else if (isCoreDependencyProperty && v is ValueType)
                {
                    actions.Add(() => AssertAreEqual(v.ToString(), boundContent.Content.ToString(), "Did not find template bound content."));
                }
                else if (v is FontFamily)
                {
                    actions.Add(() => AssertAreEqual(((FontFamily)(object)v).Source, boundContent.Content.ToString(), "Did not find template bound content."));
                }
                else if (v is Array)
                {
                    actions.Add(() =>
                        {
                            string[] strings = ((IEnumerable)(object)v).Cast<object>().Select(o => o.ToString()).ToArray();
                            AssertAreEqual(string.Join(",", strings), boundContent.Content, "Did not find template bound content.");
                        });
                }
                else
                {
                    actions.Add(() => AssertAreEqual(v, boundContent.Content, "Did not find template bound content."));
                }
            }

            Test.TestAsync(5, control, actions.ToArray());
        }
        #endregion TemplateBind

        #region Binding
        /// <summary>
        /// Gets the Binding test.
        /// </summary>
        public DependencyPropertyTestMethod BindingTest
        {
            get { return GetTestMethod("Binding", () => Binding()); }
        }

        /// <summary>
        /// Verify the dependency property can be bound.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the dependency property can be bound.")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        protected virtual void Binding()
        {
            Assert.IsFalse(IsAttached, "Attached propeties cannot be bound!");
            Assert.IsNotNull(OtherValues, "OtherValues");

            // Get the instance and apply the binding
            FrameworkElement instance = GetInstance();
            instance.SetBinding(Property, new Binding());

            // Change the binding and make sure the changes are seen
            List<Action> actions = new List<Action>();
            foreach (P value in OtherValues.Where(v => null != v))
            {
                // Explicitly capture the value so it doesn't get captured by
                // any of our async closures.
                P v = value;

                // Set the DataContext and verify the binding was successful
                actions.Add(() => instance.DataContext = v);
                actions.Add(() => AssertAreEqual(v, GetValue(instance), "Binding was not applied successfully."));
            }

            Test.TestAsync(5, instance, actions.ToArray());
        }
        #endregion Binding

        #region ChangesVisualState
        /// <summary>
        /// Gets the ChangesVisualState test.
        /// </summary>
        /// <param name="fromValue">Initial value of the property.</param>
        /// <param name="toValue">New value of the property.</param>
        /// <param name="newState">State that should be changed to.</param>
        /// <returns>The ChangesVisualState test.</returns>
        public DependencyPropertyTestMethod ChangesVisualStateTest(P fromValue, P toValue, string newState)
        {
            DependencyPropertyTestMethod test = GetTestMethod("ChangesVisualState", () => ChangesVisualState(fromValue, toValue, newState));
            test.PropertyDetail = newState;
            return test;
        }

        /// <summary>
        /// Verify the dependency property changes the visual state of the
        /// control.
        /// </summary>
        /// <param name="fromValue">Initial value of the property.</param>
        /// <param name="toValue">New value of the property.</param>
        /// <param name="newState">State that should be changed to.</param>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the dependency property changes the visual state of the control.")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        [Tag("VisualTree")]
        [Tag("VisualState")]
        protected virtual void ChangesVisualState(P fromValue, P toValue, string newState)
        {
            FrameworkElement instance = GetInstance();
            TestVisualStateManager vsm = new TestVisualStateManager();
            Test.TestAsync(
                instance,
                () => SetValue(instance, fromValue),
                () =>
                {
                    FrameworkElement root = instance.GetVisualDescendents().First() as FrameworkElement;
                    VisualStateManager.SetCustomVisualStateManager(root, vsm);
                },
                () => SetValue(instance, toValue),
                () => Assert.IsTrue(vsm.ChangedStates.Contains(newState), "Changing from {0} to {1} did not change the visual state to {2}!", fromValue, toValue, newState));
        }
        #endregion ChangesVisualState

        #region DoesNotChangeVisualState
        /// <summary>
        /// Gets the DoesNotChangeVisualState test.
        /// </summary>
        /// <param name="fromValue">Initial value of the property.</param>
        /// <param name="toValue">New value of the property.</param>
        /// <returns>The DoesNotChangeVisualState test.</returns>
        public DependencyPropertyTestMethod DoesNotChangeVisualStateTest(P fromValue, P toValue)
        {
            return GetTestMethod("DoesNotChangeVisualState", () => DoesNotChangeVisualState(fromValue, toValue));
        }

        /// <summary>
        /// Ensure that setting the dependency property to an invalid value does
        /// not change the visual state.
        /// </summary>
        /// <param name="fromValue">Initial value of the property.</param>
        /// <param name="toValue">New value of the property.</param>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting the dependency property to an invalid value does not change the visual state.")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        [Tag("VisualTree")]
        [Tag("VisualState")]
        protected virtual void DoesNotChangeVisualState(P fromValue, P toValue)
        {
            FrameworkElement instance = GetInstance();
            TestVisualStateManager vsm = new TestVisualStateManager();
            Test.TestAsync(
                instance,
                () => SetValue(instance, fromValue),
                () =>
                {
                    FrameworkElement root = instance.GetVisualDescendents().First() as FrameworkElement;
                    VisualStateManager.SetCustomVisualStateManager(root, vsm);
                },
                () =>
                {
                    try
                    {
                        SetValue(instance, toValue);
                    }
                    catch
                    {
                    }
                },
                () => Assert.AreEqual(0, vsm.ChangedStates.Count, "Changing from {0} to {1} changed the visual state!", fromValue, toValue));
        }
        #endregion DoesNotChangeVisualState

        #region SetXamlAttribute
        /// <summary>
        /// Gets the SetXamlAttribute test.
        /// </summary>
        public DependencyPropertyTestMethod SetXamlAttributeTest
        {
            get { return GetTestMethod("SetXamlAttribute", () => SetXamlAttribute()); }
        }

        /// <summary>
        /// Verify the property can be set in XAML as an attribute.
        /// </summary>
        [TestMethod]
        [Description("Verify the property can be set in XAML as an attribute.")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        [Tag("XAML")]
        protected virtual void SetXamlAttribute()
        {
            FrameworkElement instance = GetInstance();
            Type type = instance.GetType();

            foreach (P value in AllValues)
            {
                // Ignore values that can't be converter to an attribute
                if (!XamlConverter.CanConvertToAttribute(value))
                {
                    continue;
                }

                // Wrap the element in a ContentControl to properly handle the
                // namespaces
                XamlBuilder<ContentControl> builder = new XamlBuilder<ContentControl>
                {
                    ExplicitNamespaces = new Dictionary<string, string>(),
                    Children = new List<XamlBuilder>()
                    {
                        new XamlBuilder { ElementType = type }
                    }
                };

                // Add an explicit namespace for controls not in the core
                if (type.Assembly != typeof(DependencyObject).Assembly)
                {
                    builder.ExplicitNamespaces["controls"] = XamlBuilder.GetNamespace(type);
                }

                // Set the attribute
                if (!IsAttached)
                {
                    builder.Children[0].AttributeProperties = new Dictionary<string, string>
                    {
                        { Name, XamlConverter.ConvertToAttribute(value) }
                    };
                }
                else
                {
                    builder.ExplicitNamespaces["attached"] = XamlBuilder.GetNamespace(typeof(T));
                    builder.Children[0].AttachedAttributeProperties = new Dictionary<KeyValuePair<Type, string>, string>()
                    {
                        { new KeyValuePair<Type, string>(typeof(T), Name), XamlConverter.ConvertToAttribute(value) }
                    };
                }

                try
                {
                    ContentControl wrapper = builder.Load();
                    P xamlValue = GetValue(wrapper.Content as FrameworkElement);
                    
                    // Properties of type object that are parsed from an
                    // attribute are represented as strings in XAML.  We'll need
                    // to convert them before comparing.
                    if (typeof(P) == typeof(object) && !object.Equals(xamlValue, null))
                    {
                        try
                        {
                            xamlValue = (P) Convert.ChangeType(xamlValue, value.GetType(), CultureInfo.InvariantCulture);
                        }
                        catch (InvalidCastException)
                        {
                        }
                    }

                    AssertAreEqual(value, xamlValue, "The value {0} does not match the XAML attribute value {1}!", value, xamlValue);
                }
                catch (XamlParseException)
                {
                    string xaml = builder.Build();
                    Assert.Fail("Failed to load the XAML for property value {0}!  (XAML: {1})", value, xaml);
                }
            }
        }
        #endregion SetXamlAttribute

        #region SetXamlElement
        /// <summary>
        /// Gets the SetXamlElement test.
        /// </summary>
        public DependencyPropertyTestMethod SetXamlElementTest
        {
            get { return GetTestMethod("SetXamlElement", () => SetXamlElement()); }
        }

        /// <summary>
        /// Verify the property can be set in XAML as an element.
        /// </summary>
        [TestMethod]
        [Description("Verify the property can be set in XAML as an element.")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        [Tag("XAML")]
        protected virtual void SetXamlElement()
        {
            FrameworkElement instance = GetInstance();
            Type type = instance.GetType();

            foreach (P value in AllValues)
            {
                // Ignore values that can't be converter to an element
                if (!XamlConverter.CanConvertToElement(value))
                {
                    continue;
                }

                // Wrap the element in a ContentControl to properly handle the
                // namespaces
                XamlBuilder<ContentControl> builder = new XamlBuilder<ContentControl>
                {
                    ExplicitNamespaces = new Dictionary<string, string>(),
                    Children = new List<XamlBuilder>()
                    {
                        new XamlBuilder { ElementType = type }
                    }
                };

                // Add an explicit namespace for controls not in the core
                if (type.Assembly != typeof(DependencyObject).Assembly)
                {
                    builder.ExplicitNamespaces["controls"] = XamlBuilder.GetNamespace(type);
                }

                // Add an explict namespace for properties not in the core
                if (typeof(P).Assembly != typeof(DependencyObject).Assembly)
                {
                    builder.ExplicitNamespaces["prop"] = XamlBuilder.GetNamespace(typeof(P));
                }

                // Set the element
                if (!IsAttached)
                {
                    builder.Children[0].ElementProperties = new Dictionary<string, XamlBuilder>
                    {
                        { Name, XamlConverter.ConvertToElement(value) }
                    };
                }
                else
                {
                    builder.ExplicitNamespaces["attached"] = XamlBuilder.GetNamespace(typeof(T));
                    builder.Children[0].AttachedElementProperties = new Dictionary<KeyValuePair<Type, string>, XamlBuilder>()
                    {
                        { new KeyValuePair<Type, string>(typeof(T), Name), XamlConverter.ConvertToElement(value) }
                    };
                }
                
                try
                {
                    ContentControl wrapper = builder.Load();
                    P xamlValue = GetValue(wrapper.Content as FrameworkElement);
                    AssertAreEqual(value, xamlValue, "The value {0} does not match the XAML property value {1}!", value, xamlValue);
                }
                catch (XamlParseException)
                {
                    string xaml = builder.Build();
                    Assert.Fail("Failed to load the XAML for property value {0}!  (XAML: {1})", value, xaml);
                }
            }
        }
        #endregion SetXamlElement

        #region SetXamlContent
        /// <summary>
        /// Gets the SetXamlContent test.
        /// </summary>
        public DependencyPropertyTestMethod SetXamlContentTest
        {
            get { return GetTestMethod("SetXamlContent", () => SetXamlContent()); }
        }

        /// <summary>
        /// Verify the property can be set in XAML as content.
        /// </summary>
        [TestMethod]
        [Description("Verify the property can be set in XAML as content.")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        [Tag("XAML")]
        protected virtual void SetXamlContent()
        {
            Assert.IsFalse(IsAttached, "Attached properties cannot be XAML content!");
            FrameworkElement instance = GetInstance();
            Type type = instance.GetType();

            foreach (P value in AllValues)
            {
                // Ignore values that can't be converted to an element
                if (!XamlConverter.CanConvertToElement(value))
                {
                    continue;
                }

                // Wrap the element in a ContentControl to properly handle the
                // namespaces
                XamlBuilder<ContentControl> builder = new XamlBuilder<ContentControl>
                {
                    ExplicitNamespaces = new Dictionary<string, string>(),
                    Children = new List<XamlBuilder>()
                    {
                        new XamlBuilder
                        {
                            ElementType = type,
                            Children = new List<XamlBuilder>
                            {
                                XamlConverter.ConvertToElement(value)
                            }
                        }
                    }
                };

                // Add an explicit namespace for controls not in the core
                if (type.Assembly != typeof(DependencyObject).Assembly)
                {
                    builder.ExplicitNamespaces["controls"] = XamlBuilder.GetNamespace(type);
                }

                // Add an explict namespace for properties not in the core
                if (typeof(P).Assembly != typeof(DependencyObject).Assembly)
                {
                    builder.ExplicitNamespaces["prop"] = XamlBuilder.GetNamespace(typeof(P));
                }

                try
                {
                    ContentControl wrapper = builder.Load();
                    P xamlValue = GetValue(wrapper.Content as FrameworkElement);
                    AssertAreEqual(value, xamlValue, "The value {0} does not match the XAML property value {1}!", value, xamlValue);
                }
                catch (XamlParseException)
                {
                    string xaml = builder.Build();
                    Assert.Fail("Failed to load the XAML for property value {0}!  (XAML: {1})", value, xaml);
                }
            }
        }
        #endregion SetXamlContent

        #region IsContentProperty
        /// <summary>
        /// Gets the IsContentProperty test.
        /// </summary>
        public DependencyPropertyTestMethod IsContentPropertyTest
        {
            get { return GetTestMethod("IsContentProperty", () => IsContentProperty()); }
        }

        /// <summary>
        /// Ensure the ContentProperty attribute is applied to its owning Type.
        /// </summary>
        [TestMethod]
        [Description("Ensure the ContentProperty attribute is applied to its owning Type.")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        protected virtual void IsContentProperty()
        {
            ContentPropertyAttribute attribute = typeof(T).GetAttributes<ContentPropertyAttribute>().FirstOrDefault();
            Assert.IsNotNull(attribute, "No ContentPropertyAttribute found on type {0}!", typeof(T).Name);
            Assert.AreEqual(Name, attribute.Name, "ContentPropertyAttribute is set for another property!");
        }
        #endregion IsContentProperty

        #region HasTypeConverter
        /// <summary>
        /// Gets the HasTypeConverter test.
        /// </summary>
        /// <param name="converterType">The type of the TypeConverter.</param>
        /// <returns>The HasTypeConverter test.</returns>
        public DependencyPropertyTestMethod HasTypeConverterTest(Type converterType)
        {
            return GetTestMethod("HasTypeConverter", () => HasTypeConverter(converterType));
        }

        /// <summary>
        /// Verify the property is associated with a TypeConverter.
        /// </summary>
        /// <param name="converterType">The type of the TypeConverter.</param>
        [TestMethod]
        [Description("Verify the property is associated with a TypeConverter.")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        protected virtual void HasTypeConverter(Type converterType)
        {
            Assert.IsNotNull(converterType, "converterType should not be null!");

            TypeConverterAttribute attribute = null;

            // First try the actual property
            if (!IsAttached)
            {
                attribute = typeof(T).GetProperty(Name).GetAttributes<TypeConverterAttribute>().FirstOrDefault();
            }

            // If that fails, try getting the attribute on the Type if that failed
            if (attribute == null)
            {
                attribute = typeof(P).GetAttributes<TypeConverterAttribute>().FirstOrDefault();
            }

            Assert.IsNotNull(attribute, "Failed to find TypeConverterAttribute!");
            Assert.AreEqual(converterType.AssemblyQualifiedName, attribute.ConverterTypeName, "Unexpected TypeConverterAttribute found!");
        }
        #endregion HasTypeConverter

        #region AttachedGetNullFails
        /// <summary>
        /// Gets the AttachedGetNullFails test.
        /// </summary>
        public DependencyPropertyTestMethod AttachedGetNullFailsTest
        {
            get { return GetTestMethod("AttachedGetNullFails", () => AttachedGetNullFails()); }
        }

        /// <summary>
        /// Test getting the target of an attached dependency property to null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Description("Test getting the target of an attached dependency property to null.")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        protected virtual void AttachedGetNullFails()
        {
            Assert.IsTrue(IsAttached, "This test is only valid for attached properties!");
            GetValue(null);
        }
        #endregion AttachedGetNullFails

        #region AttachedSetNullFails
        /// <summary>
        /// Gets the AttachedSetNullFails test.
        /// </summary>
        public DependencyPropertyTestMethod AttachedSetNullFailsTest
        {
            get { return GetTestMethod("AttachedSetNullFails", () => AttachedSetNullFails()); }
        }

        /// <summary>
        /// Test setting the target of an attached dependency property to null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Description("Test setting the target of an attached dependency property to null.")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        protected virtual void AttachedSetNullFails()
        {
            Assert.IsTrue(IsAttached, "This test is only valid for attached properties!");
            SetValue(null, DefaultValue);
        }
        #endregion AttachedSetNullFails

        #region CheckDataTemplate
        /// <summary>
        /// Create a DataTemplate test.
        /// </summary>
        /// <param name="data">Data for the template.</param>
        /// <param name="bindings">Bindings to create in the template.</param>
        /// <returns>DataTemplate property test.</returns>
        public DependencyPropertyTestMethod CheckDataTemplateTest(P data, Dictionary<string, object> bindings)
        {
            return GetTestMethod("DataTemplate", () => CheckDataTemplate(data, bindings));
        }

        /// <summary>
        /// Verify the application of a DataTemplate.
        /// </summary>
        /// <param name="data">Data for the template.</param>
        /// <param name="bindings">Bindings to create in the template.</param>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the application of a DataTemplate.")]
        [Tag("DataTemplate")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        [Tag("VisualTree")]
        protected virtual void CheckDataTemplate(P data, Dictionary<string, object> bindings)
        {
            Assert.IsNotNull(TemplateProperty, "TemplateProperty");
            FrameworkElement instance = GetInstance();

            // By default just bind the entire data object
            if (bindings == null)
            {
                bindings = new Dictionary<string, object>();
                UIElement element = data as UIElement;
                if (element != null)
                {
                    bindings.Add("", element);
                }
                else
                {
                    bindings.Add("", data.ToString());
                }
            }

            string nameFormat = "__binding{0}";

            Test.TestAsync(
                5,
                instance,
                () => SetValue(instance, data),
                () =>
                {
                    // Create the DataTemplate
                    XamlBuilder<DataTemplate> xamlTemplate = new XamlBuilder<DataTemplate> { Name = "template" };
                    XamlBuilder<StackPanel> xamlPanel = new XamlBuilder<StackPanel> { Children = new List<XamlBuilder>() };
                    xamlTemplate.Children = new List<XamlBuilder>(new XamlBuilder[] { xamlPanel });

                    // Add the bindings
                    int i = 0;
                    foreach (KeyValuePair<string, object> binding in bindings)
                    {
                        string expression = !string.IsNullOrEmpty(binding.Key) ?
                            "{Binding " + binding.Key + "}" :
                            "{Binding}";

                        XamlBuilder xamlBinding = null;
                        if (binding.Value is UIElement)
                        {
                            xamlBinding = new XamlBuilder<ContentControl>
                            {
                                AttributeProperties = new Dictionary<string, string> { { "Content", expression } }
                            };
                        }
                        else
                        {
                            xamlBinding = new XamlBuilder<TextBlock>
                            {
                                AttributeProperties = new Dictionary<string, string> { { "Text", expression } }
                            };
                        }
                        xamlBinding.Name = string.Format(CultureInfo.InvariantCulture, nameFormat, i++);

                        // Add the bound element with a label
                        xamlPanel.Children.Add(
                            new XamlBuilder<StackPanel>
                            {
                                AttributeProperties = new Dictionary<string, string> { { "Orientation", "Horizontal" } },
                                Children = new List<XamlBuilder>
                                {
                                    new XamlBuilder<TextBlock>
                                    {
                                        AttributeProperties = new Dictionary<string, string> { { "Text", "{}" + expression + ":  " } }
                                    },
                                    xamlBinding
                                }
                            });
                    }

                    // Generate the DataTemplate and set it
                    TemplateProperty.SetValue(instance, xamlTemplate.Load());
                },
                () =>
                {
                    // Verify the bindings were set correctly
                    int i = 0;
                    foreach (KeyValuePair<string, object> binding in bindings)
                    {
                        string name = string.Format(CultureInfo.InvariantCulture, nameFormat, i++);
                        DependencyObject child = instance.GetVisualChild(name);
                        TextBlock text = child as TextBlock;
                        if (text != null)
                        {
                            Assert.AreEqual(binding.Value as string, text.Text, "Binding {0} had unexpected text!", i - 1);
                        }
                        else
                        {
                            ContentControl content = child as ContentControl;
                            if (content != null)
                            {
                                try
                                {
                                    Assert.AreSame(binding.Value as UIElement, content.Content, "Binding {0} had unexpected content!", i - 1);
                                }
                                finally
                                {
                                    content.Content = null;
                                }
                            }
                            else
                            {
                                Assert.Fail("Expected object of type TextBlock or ContentControl, not {0}!", child == null ? "null" : child.GetType().Name);
                            }
                        }
                    }
                });
        }
        #endregion CheckDataTemplate

        #region DataTemplateWithInt
        /// <summary>
        /// Gets the DataTemplateWithInt test.
        /// </summary>
        public DependencyPropertyTestMethod DataTemplateWithIntTest
        {
            get { return GetTestMethod("DataTemplateWithInt", () => DataTemplateWithInt()); }
        }

        /// <summary>
        /// Verify the application of a DataTemplate with Int content.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the application of a DataTemplate with Int content.")]
        [Tag("DataTemplate")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        [Tag("VisualTree")]
        protected virtual void DataTemplateWithInt()
        {
            CheckDataTemplate((P)(object)42, null);
        }
        #endregion DataTemplateWithInt

        #region DataTemplateWithString
        /// <summary>
        /// Gets the DataTemplateWithString test.
        /// </summary>
        public DependencyPropertyTestMethod DataTemplateWithStringTest
        {
            get { return GetTestMethod("DataTemplateWithString", () => DataTemplateWithString()); }
        }

        /// <summary>
        /// Verify the application of a DataTemplate with String content.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the application of a DataTemplate with String content.")]
        [Tag("DataTemplate")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        [Tag("VisualTree")]
        protected virtual void DataTemplateWithString()
        {
            CheckDataTemplate((P)(object)"Test Content", null);
        }
        #endregion DataTemplateWithString

        #region DataTemplateWithStringAndProperty
        /// <summary>
        /// Gets the DataTemplateWithStringAndProperty test.
        /// </summary>
        public DependencyPropertyTestMethod DataTemplateWithStringAndPropertyTest
        {
            get { return GetTestMethod("DataTemplateWithStringAndProperty", () => DataTemplateWithStringAndProperty()); }
        }

        /// <summary>
        /// Verify the application of a DataTemplate with String content and a
        /// property binding.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the application of a DataTemplate with String content and a property binding.")]
        [Tag("DataTemplate")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        [Tag("VisualTree")]
        protected virtual void DataTemplateWithStringAndProperty()
        {
            string content = "Test Content";
            CheckDataTemplate(
                (P)(object)content,
                new Dictionary<string, object>
                {
                    { "", content },
                    { "Length", content.Length.ToString(CultureInfo.InvariantCulture) }
                });
        }
        #endregion DataTemplateWithStringAndProperty

        #region DataTemplateWithBusinessObject
        /// <summary>
        /// Gets the DataTemplateWithBusinessObject test.
        /// </summary>
        public DependencyPropertyTestMethod DataTemplateWithBusinessObjectTest
        {
            get { return GetTestMethod("DataTemplateWithBusinessObject", () => DataTemplateWithBusinessObject()); }
        }

        /// <summary>
        /// Verify the application of a DataTemplate with UIElement content.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the application of a DataTemplate with UIElement content.")]
        [Tag("DataTemplate")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        [Tag("VisualTree")]
        protected virtual void DataTemplateWithBusinessObject()
        {
            OperatingSystem os = Environment.OSVersion;
            CheckDataTemplate(
                (P)(object)os,
                new Dictionary<string, object>
                {
                    { "Platform", os.Platform.ToString() },
                    { "Version.Major", os.Version.Major.ToString(CultureInfo.InvariantCulture) },
                    { "Version.Minor", os.Version.Minor.ToString(CultureInfo.InvariantCulture) },
                    { "Version.Build", os.Version.Build.ToString(CultureInfo.InvariantCulture) },
                    { "Version.Revision", os.Version.Revision.ToString(CultureInfo.InvariantCulture) },
                });
        }
        #endregion DataTemplateWithBusinessObject

        #region DataTemplateWithUIElementFails
        /// <summary>
        /// Gets the DataTemplateWithUIElementFails test.
        /// </summary>
        public DependencyPropertyTestMethod DataTemplateWithUIElementFailsTest
        {
            get { return GetTestMethod("DataTemplateWithUIElementFails", () => DataTemplateWithUIElementFails()); }
        }

        /// <summary>
        /// Verify the application of a DataTemplate with UIElement content
        /// fails.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the application of a DataTemplate with UIElement content fails.")]
        [Tag("DataTemplate")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        [Tag("VisualTree")]
        protected virtual void DataTemplateWithUIElementFails()
        {
            Assert.IsNotNull(Initializer, "Initializer");
            Assert.IsNotNull(TemplateProperty, "TemplateProperty");

            T instance = Initializer();
            Test.TestAsync(
                instance,
                () =>
                {
                    StackPanel element = new StackPanel();
                    element.SetValue(FrameworkElement.NameProperty, "UIElementContent");
                    element.Children.Add(new Ellipse { Fill = new SolidColorBrush(Colors.Red), Width = 20, Height = 20 });
                    element.Children.Add(new TextBlock { Text = "UIElement Content" });
                    element.Children.Add(new Ellipse { Fill = new SolidColorBrush(Colors.Blue), Width = 20, Height = 20 });
                    P data = (P)(object)element;
                    SetValue(instance, data);
                },
                () =>
                {
                    // Create the DataTemplate
                    XamlBuilder<DataTemplate> xamlTemplate = new XamlBuilder<DataTemplate>
                    {
                        Name = "template",
                        Children = new List<XamlBuilder>
                        {
                            new XamlBuilder<StackPanel>
                            {
                                Children = new List<XamlBuilder>
                                {
                                    new XamlBuilder<StackPanel>
                                    {
                                        AttributeProperties = new Dictionary<string, string> { { "Orientation", "Horizontal" } },
                                        Children = new List<XamlBuilder>
                                        {
                                            new XamlBuilder<TextBlock>
                                            {
                                                AttributeProperties = new Dictionary<string, string> { { "Text", "{}{Binding Name}:  " } }
                                            },
                                            new XamlBuilder<TextBlock>
                                            {
                                                Name = "nameBinding",
                                                AttributeProperties = new Dictionary<string, string> { { "Text", "{Binding Name}" } }
                                            }
                                        }
                                    },
                                    new XamlBuilder<StackPanel>
                                    {
                                        AttributeProperties = new Dictionary<string, string> { { "Orientation", "Horizontal" } },
                                        Children = new List<XamlBuilder>
                                        {
                                            new XamlBuilder<TextBlock>
                                            {
                                                AttributeProperties = new Dictionary<string, string> { { "Text", "{}{Binding}:  " } }
                                            },
                                            new XamlBuilder<ContentControl>
                                            {
                                                Name = "contentBinding",
                                                AttributeProperties = new Dictionary<string, string> { { "Content", "{Binding}" } }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    };

                    // Generate the DataTemplate and set it
                    TemplateProperty.SetValue(instance, xamlTemplate.Load());
                },
                () =>
                {
                    // Verify the bindings didn't work
                    TextBlock text = instance.GetVisualChild("nameBinding") as TextBlock;
                    Assert.IsNotNull(text, "Failed to find nameBinding TextBlock!");
                    TestExtensions.AssertIsNullOrEmpty(text.Text);

                    ContentControl content = instance.GetVisualChild("contentBinding") as ContentControl;
                    Assert.IsNotNull(content, "Failed to find contentBinding ContentControl!");
                    Assert.IsNull(content.Content, "The bound Content should be null!");
                });
        }
        #endregion DataTemplateWithUIElementFails
    }
}