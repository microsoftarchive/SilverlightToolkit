//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="DataFormTests_Base.cs">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Reflection;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Data;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// Provides the framework for testing the DataField.
    /// </summary>
    [TestClass]
    public class DataFieldTests : SilverlightTest
    {
        #region Helper Constants, Fields, and Properties

        /// <summary>
        /// Holds the time in milliseconds to wait for an event.
        /// </summary>
        private const int EventWaitTimeout = 1000;

        /// <summary>
        /// Private accessor to the DataField.
        /// </summary>
        private DataField _dataField;

        /// <summary>
        /// Private accessor to the second DataField.
        /// </summary>
        private DataField _dataField2;

        /// <summary>
        /// Holds whether or not the DataField content has been loaded.
        /// </summary>
        private bool _dataFieldContentLoaded;

        /// <summary>
        /// Holds whether or not the DataField has been loaded.
        /// </summary>
        private bool _dataFieldLoaded;

        /// <summary>
        /// Private accessor to the TextBox.
        /// </summary>
        private TextBox _textBox;

        #endregion Helper Constants, Fields, and Properties

        #region Initialization

        /// <summary>
        /// Initializes the test framework.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this._dataField = new DataField();
            this._dataField2 = new DataField();

            this._textBox = new TextBox();
            this._textBox.SetBinding(TextBox.TextProperty, new Binding("StringProperty") { Mode = BindingMode.TwoWay });

            this._dataField.Content = this._textBox;

            TextBox textBox2 = new TextBox();
            textBox2.SetBinding(TextBox.TextProperty, new Binding("IntProperty") { Mode = BindingMode.TwoWay });

            this._dataField2.Content = textBox2;
        }

        #endregion

        /// <summary>
        /// Ensure that the labels of fields in the same layout scope are aligned.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the labels of fields in the same layout scope are aligned.")]
        public void AlignLabelsInSameScope()
        {
            this.AddToPanelInStackPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this._dataField.LabelColumn.ActualWidth == this._dataField2.LabelColumn.ActualWidth);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure changes to DescriptionViewerDescription correctly take effect.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure changes to DescriptionViewerDescription correctly take effect.")]
        public void ChangeDescriptionViewerDescription()
        {
            this.EnqueueCallback(() =>
            {
                this._dataField.Description = "description 1";
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("description 1", this._dataField.DescriptionViewer.Description);
                this._dataField.Description = "description 2";
                Assert.AreEqual("description 2", this._dataField.DescriptionViewer.Description);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure changes to DescriptionViewerPosition correctly take effect.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure changes to DescriptionViewerPosition correctly take effect.")]
        public void ChangeDescriptionViewerPosition()
        {
            this.EnqueueCallback(() =>
            {
                this._dataField.DescriptionViewerPosition = DataFieldDescriptionViewerPosition.BesideLabel;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this.GetPosition(this._dataField.InternalLabel).X < this.GetPosition(this._dataField.DescriptionViewer).X);
                Assert.IsTrue(this.GetPosition(this._dataField.DescriptionViewer).X < this.GetPosition(this._dataField.Content).X);
                this._dataField.DescriptionViewerPosition = DataFieldDescriptionViewerPosition.BesideContent;
                this.ExpectFieldContentLoaded();
            });

            this.WaitForFieldContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this.GetPosition(this._dataField.InternalLabel).X < this.GetPosition(this._dataField.Content).X);
                Assert.IsTrue(this.GetPosition(this._dataField.Content).X < this.GetPosition(this._dataField.DescriptionViewer).X);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure changes to DescriptionViewerStyle correctly take effect.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure changes to DescriptionViewerStyle correctly take effect.")]
        public void ChangeDescriptionViewerStyle()
        {
            Style style = null;

            this.EnqueueCallback(() =>
            {
                style = new Style(typeof(DescriptionViewer));
                style.Setters.Add(new Setter(Control.FontSizeProperty, 20));
                this._dataField.DescriptionViewerStyle = style;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(style, this._dataField.DescriptionViewerStyle);
                Assert.AreEqual(20, this._dataField.DescriptionViewer.FontSize);

                Style style2 = new Style(typeof(DescriptionViewer));
                style2.Setters.Add(new Setter(Control.FontSizeProperty, 30));
                this._dataField.DescriptionViewerStyle = style2;

                Assert.AreEqual(style2, this._dataField.DescriptionViewerStyle);
                Assert.AreEqual(30, this._dataField.DescriptionViewer.FontSize);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure changes to DescriptionViewerVisibility correctly take effect.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure changes to DescriptionViewerVisibility correctly take effect.")]
        public void ChangeDescriptionViewerVisibility()
        {
            this.EnqueueCallback(() =>
            {
                this._dataField.DescriptionViewerVisibility = Visibility.Collapsed;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(Visibility.Collapsed, this._dataField.DescriptionViewer.Visibility);
                this._dataField.DescriptionViewerVisibility = Visibility.Visible;
            });

            // Wait a short while to allow the binding to update.
            this.EnqueueDelay(100);

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(Visibility.Visible, this._dataField.DescriptionViewer.Visibility);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure changes to Label correctly take effect.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure changes to Label correctly take effect.")]
        public void ChangeLabel()
        {
            this.EnqueueCallback(() =>
            {
                this._dataField.Label = "content 1";
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual("content 1", this._dataField.InternalLabel.Content);
                this._dataField.Label = "content 2";
                Assert.AreEqual("content 2", this._dataField.InternalLabel.Content);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure changes to LabelPosition correctly take effect.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure changes to LabelPosition correctly take effect.")]
        public void ChangeLabelPosition()
        {
            this.EnqueueCallback(() =>
            {
                this._dataField.LabelPosition = DataFieldLabelPosition.Top;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this.GetPosition(this._dataField.InternalLabel).Y < this.GetPosition(this._dataField.Content).Y);
                Assert.IsTrue(this.GetPosition(this._dataField.InternalLabel).Y < this.GetPosition(this._dataField.DescriptionViewer).Y);
                Assert.IsTrue(this.GetPosition(this._dataField.InternalLabel).X == this.GetPosition(this._dataField.Content).X);
                this._dataField.LabelPosition = DataFieldLabelPosition.Left;
                this.ExpectFieldContentLoaded();
            });

            this.WaitForFieldContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this.GetPosition(this._dataField.InternalLabel).X < this.GetPosition(this._dataField.Content).X);
                Assert.IsTrue(this.GetPosition(this._dataField.Content).X < this.GetPosition(this._dataField.DescriptionViewer).X);
                this._dataField.LabelPosition = DataFieldLabelPosition.Top;
                this._dataField.DescriptionViewerPosition = DataFieldDescriptionViewerPosition.BesideLabel;
                this.ExpectFieldContentLoaded();
            });

            this.WaitForFieldContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this.GetPosition(this._dataField.InternalLabel).X < this.GetPosition(this._dataField.DescriptionViewer).X);
                Assert.IsTrue(this.GetPosition(this._dataField.InternalLabel).Y < this.GetPosition(this._dataField.Content).Y);
                Assert.IsTrue(this.GetPosition(this._dataField.InternalLabel).X == this.GetPosition(this._dataField.Content).X);
                Assert.IsTrue(this.GetPosition(this._dataField.DescriptionViewer).Y < this.GetPosition(this._dataField.Content).Y);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure changes to LabelStyle correctly take effect.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure changes to LabelStyle correctly take effect.")]
        public void ChangeLabelStyle()
        {
            Style style = null;

            this.EnqueueCallback(() =>
            {
                style = new Style(typeof(Label));
                style.Setters.Add(new Setter(Control.FontSizeProperty, 20));
                this._dataField.LabelStyle = style;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(style, this._dataField.LabelStyle);
                Assert.AreEqual(20, this._dataField.InternalLabel.FontSize);

                Style style2 = new Style(typeof(Label));
                style2.Setters.Add(new Setter(Control.FontSizeProperty, 30));
                this._dataField.LabelStyle = style2;

                Assert.AreEqual(style2, this._dataField.LabelStyle);
                Assert.AreEqual(30, this._dataField.InternalLabel.FontSize);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure changes to LabelVisibility correctly take effect.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure changes to LabelVisibility correctly take effect.")]
        public void ChangeLabelVisibility()
        {
            this.EnqueueCallback(() =>
            {
                this._dataField.LabelVisibility = Visibility.Collapsed;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(Visibility.Collapsed, this._dataField.InternalLabel.Visibility);
                this._dataField.LabelVisibility = Visibility.Visible;
            });

            // Wait a short while to allow the binding to update.
            this.EnqueueDelay(100);

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(Visibility.Visible, this._dataField.InternalLabel.Visibility);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that setting IsReadOnly to true correctly leaves the field read-only at all times.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting IsReadOnly to true correctly leaves the field read-only at all times.")]
        public void EnsureIsReadOnlyFunctions()
        {
            this._dataField.IsReadOnly = true;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this._textBox.IsReadOnly);
                this._dataField.Mode = DataFieldMode.ReadOnly;
                this.ExpectFieldContentLoaded();
            });

            this.WaitForFieldContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this._textBox.IsReadOnly);
                this._dataField.Mode = DataFieldMode.Edit;
                this.ExpectFieldContentLoaded();
            });

            this.WaitForFieldContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this._textBox.IsReadOnly);
                this._dataField.Mode = DataFieldMode.AddNew;
                this.ExpectFieldContentLoaded();
            });

            this.WaitForFieldContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this._textBox.IsReadOnly);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that the label, content, and description are correctly put in place.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the label, content, and description are correctly put in place.")]
        public void EnsureLabelContentAndDescription()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(this._dataField.InternalLabel);
                Assert.AreEqual("String Property", this._dataField.InternalLabel.Content);

                Assert.IsInstanceOfType(this._dataField.Content, typeof(TextBox));

                Assert.IsNotNull(this._dataField.DescriptionViewer);
                Assert.AreEqual("String Property Description", this._dataField.DescriptionViewer.Description);

                Assert.IsTrue(this.GetPosition(this._dataField.InternalLabel).X < this.GetPosition(this._dataField.Content).X);
                Assert.IsTrue(this.GetPosition(this._dataField.Content).X < this.GetPosition(this._dataField.DescriptionViewer).X);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that changing the mode of the DataField has the expected result.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that changing the mode of the DataField has the expected result.")]
        public void SetDifferentModes()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFieldMode.Auto, this._dataField.Mode);
                Assert.IsFalse(this._textBox.IsReadOnly);
                this._dataField.Mode = DataFieldMode.ReadOnly;
                this.ExpectFieldContentLoaded();
            });

            this.WaitForFieldContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFieldMode.ReadOnly, this._dataField.Mode);
                Assert.IsTrue(this._textBox.IsReadOnly);
                this._dataField.Mode = DataFieldMode.Edit;
                this.ExpectFieldContentLoaded();
            });

            this.WaitForFieldContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFieldMode.Edit, this._dataField.Mode);
                Assert.IsFalse(this._textBox.IsReadOnly);
                this._dataField.Mode = DataFieldMode.ReadOnly;
                this.ExpectFieldContentLoaded();
            });

            this.WaitForFieldContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFieldMode.ReadOnly, this._dataField.Mode);
                Assert.IsTrue(this._textBox.IsReadOnly);
                this._dataField.Mode = DataFieldMode.AddNew;
                this.ExpectFieldContentLoaded();
            });

            this.WaitForFieldContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFieldMode.AddNew, this._dataField.Mode);
                Assert.IsFalse(this._textBox.IsReadOnly);
                this._dataField.Mode = DataFieldMode.ReadOnly;
                this.ExpectFieldContentLoaded();
            });

            this.WaitForFieldContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(DataFieldMode.ReadOnly, this._dataField.Mode);
                Assert.IsTrue(this._textBox.IsReadOnly);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that setting IsRequired on the DataField has the expected result.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting IsRequired on the DataField has the expected result.")]
        public void SetIsRequired()
        {
            this._dataField.IsRequired = true;
            this._dataField.Mode = DataFieldMode.ReadOnly;
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this._dataField.InternalLabel.IsRequired);
                this._dataField.IsRequired = false;
                Assert.IsFalse(this._dataField.InternalLabel.IsRequired);
                this._dataField.Mode = DataFieldMode.Edit;
                this.ExpectFieldContentLoaded();
            });

            this.WaitForFieldContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(this._dataField.InternalLabel.IsRequired);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that changing the mode of the DataField has the expected result with a control that does not have an IsReadOnly property.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that changing the mode of the DataField has the expected result with a control that does not have an IsReadOnly property.")]
        public void SetDifferentModesWithoutIsReadOnly()
        {
            CheckBox checkBox = new CheckBox();
            checkBox.SetBinding(CheckBox.IsCheckedProperty, new Binding("BoolProperty") { Mode = BindingMode.TwoWay });
            this._dataField.Content = checkBox;
            this._dataField.Mode = DataFieldMode.ReadOnly;

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsFalse(checkBox.IsEnabled);
                this._dataField.Mode = DataFieldMode.Edit;
                this.ExpectFieldContentLoaded();
            });

            this.WaitForFieldContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(checkBox.IsEnabled);
                this._dataField.Mode = DataFieldMode.AddNew;
                this.ExpectFieldContentLoaded();
            });

            this.WaitForFieldContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(checkBox.IsEnabled);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that Validate functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that Validate functions properly.")]
        public void ValidateField()
        {
            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.IsTrue(this._dataField.IsValid);

                this._textBox.Text = string.Empty;
                this._dataField.Validate();

                Assert.IsFalse(this._dataField.IsValid);

                this._textBox.Text = "valid text";
                this._dataField.Validate();

                Assert.IsTrue(this._dataField.IsValid);
            });

            this.EnqueueTestComplete();
        }

        #region Helper Methods

        /// <summary>
        /// Adds the DataField to the TestPanel and waits for it to load.
        /// </summary>
        private void AddToPanelAndWaitForLoad()
        {
            this.EnqueueCallback(() =>
            {
                this._dataField.DataContext =
                    new DataClassWithValidation()
                    {
                        BoolProperty = true,
                        IntProperty = 1,
                        StringProperty = "test string"
                    };

                this._dataFieldLoaded = false;
                this._dataField.Loaded += new RoutedEventHandler(this.OnDataFieldLoaded);
                this.TestPanel.Children.Add(this._dataField);
            });

            this.EnqueueConditional(() => this._dataFieldLoaded);
        }

        /// <summary>
        /// Adds the DataFields to the TestPanel in a StackPanel and waits for it to load.
        /// </summary>
        private void AddToPanelInStackPanelAndWaitForLoad()
        {
            this.EnqueueCallback(() =>
            {
                StackPanel stackPanel = new StackPanel();

                stackPanel.DataContext =
                    new DataClassWithValidation()
                    {
                        BoolProperty = true,
                        IntProperty = 1,
                        StringProperty = "test string"
                    };

                DataField.SetIsFieldGroup(stackPanel, true);
                stackPanel.Children.Add(this._dataField);
                stackPanel.Children.Add(this._dataField2);

                this._dataFieldLoaded = false;
                this._dataField.Loaded += new RoutedEventHandler(this.OnDataFieldLoaded);
                this.TestPanel.Children.Add(stackPanel);
            });

            this.EnqueueConditional(() => this._dataFieldLoaded);
        }

        /// <summary>
        /// Expect a field's content to be loaded.
        /// </summary>
        /// <param name="field">The field.</param>
        private void ExpectFieldContentLoaded()
        {
            this._dataFieldContentLoaded = false;
            FrameworkElement contentElement = this._dataField.Content as FrameworkElement;
            contentElement.Loaded += new RoutedEventHandler(this.OnDataFieldContentLoaded);
        }

        /// <summary>
        /// Gets the position of an element
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The position.</returns>
        private Point GetPosition(UIElement element)
        {
            GeneralTransform generalTransform = element.TransformToVisual(this.TestPanel);
            return generalTransform.Transform(new Point(0, 0));
        }

        /// <summary>
        /// Sets the value of an element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value to set the element's value to.</param>
        private static void SetValue(UIElement element, string value)
        {
            FrameworkElementAutomationPeer elementPeer = FrameworkElementAutomationPeer.CreatePeerForElement(element) as FrameworkElementAutomationPeer;
            IValueProvider elementValueProvider = elementPeer.GetPattern(PatternInterface.Value) as IValueProvider;
            elementValueProvider.SetValue(value);
        }

        /// <summary>
        /// Wait for the field's content to be loaded.
        /// </summary>
        private void WaitForFieldContentLoaded()
        {
            bool timeExpired = false;
            DateTime timeStartedWaiting = new DateTime();

            this.EnqueueCallback(() => timeStartedWaiting = DateTime.Now);

            this.EnqueueConditional(() =>
            {
                if ((DateTime.Now - timeStartedWaiting).TotalMilliseconds > EventWaitTimeout)
                {
                    timeExpired = true;
                    return true;
                }

                return this._dataFieldContentLoaded;
            });

            this.EnqueueCallback(() =>
            {
                if (timeExpired)
                {
                    Assert.Fail("The expected event Loaded did not occur.");
                }
            });
        }

        /// <summary>
        /// Handles the DataField having loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnDataFieldLoaded(object sender, RoutedEventArgs e)
        {
            this._dataFieldLoaded = true;
        }

        /// <summary>
        /// Handles a field's content being loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnDataFieldContentLoaded(object sender, RoutedEventArgs e)
        {
            this._dataFieldContentLoaded = true;
        }

        #endregion Helper Methods
    }
}
