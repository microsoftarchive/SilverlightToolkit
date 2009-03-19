//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="DataFormTests_Fields.cs">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// Tests the <see cref="DataForm"/> field collection.
    /// </summary>
    [TestClass]
    public class DataFormTests_Fields : DataFormTests_Base
    {
        /// <summary>
        /// Ensure that adding a new field at runtime updates the UI properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that adding a new field at runtime updates the UI properly.")]
        public void AddFieldAtRuntime()
        {
            DataFormApp_Standard dataFormApp = new DataFormApp_Standard();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;

                dataFormApp.dataForm.CurrentItem = new DataClass();
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectContentLoaded();
                dataFormApp.dataForm.Fields.Add(new DataFormTextField() { Binding = new Binding() { Path = new PropertyPath("IntProperty") } });
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(1, dataFormApp.dataForm.Fields.Count);
                FieldLabel fieldLabel = dataFormApp.dataForm.Labels[0] as FieldLabel;
                Assert.IsNotNull(fieldLabel);
                TextBox textBox = dataFormApp.dataForm.InputControls[0] as TextBox;
                Assert.IsNotNull(textBox);
                Assert.AreEqual("0", textBox.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that fields can be defined in code.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "There are a lot of items to be tested.")]
        [Description("Ensure that fields can be defined in code.")]
        public void DefineFieldsInCode()
        {
            DataFormApp_Standard dataFormApp = new DataFormApp_Standard();
            IntCollection intCollection = new IntCollection();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;

                dataFormApp.dataForm.Fields.Add(new DataFormCheckBoxField() { Binding = new Binding("BoolProperty") { Mode = BindingMode.TwoWay, ValidatesOnExceptions = true, NotifyOnValidationError = true } });
                dataFormApp.dataForm.Fields.Add(new DataFormDateField() { Binding = new Binding("DateTimeProperty") { Mode = BindingMode.TwoWay, ValidatesOnExceptions = true, NotifyOnValidationError = true } });
                dataFormApp.dataForm.Fields.Add(new DataFormTextField() { Binding = new Binding("StringProperty") { Mode = BindingMode.TwoWay, ValidatesOnExceptions = true, NotifyOnValidationError = true } });
                dataFormApp.dataForm.Fields.Add(new DataFormTemplateField() { DisplayTemplate = new TemplateFieldDisplayTemplate(), EditTemplate = new TemplateFieldEditTemplate() });

                dataFormApp.dataForm.Fields.Add(new DataFormComboBoxField() { Binding = new Binding("IntProperty") { Mode = BindingMode.TwoWay, ValidatesOnExceptions = true, NotifyOnValidationError = true }, ItemsSource = intCollection });

                DataFormFieldGroup DataFormFieldGroup = new DataFormFieldGroup();
                DataFormFieldGroup.Fields.Add(new DataFormTextField() { Binding = new Binding("IntProperty") { Mode = BindingMode.OneTime, ValidatesOnExceptions = false, NotifyOnValidationError = false } });
                DataFormFieldGroup.Fields.Add(new DataFormTextField() { Binding = new Binding("StringProperty") });

                dataFormApp.dataForm.Fields.Add(DataFormFieldGroup);

                dataFormApp.dataForm.Fields.Add(new DataFormSeparator() { Stroke = new SolidColorBrush(Colors.Green), StrokeDashArray = new DoubleCollection() { 2, 3, 4, 5 }, StrokeThickness = 2, StrokeDashOffset = 1 });
                dataFormApp.dataForm.Fields.Add(new DataFormHeader { Content = new TextBlock() { Text = "Header Content" } });
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(8, dataFormApp.dataForm.Fields.Count);

                DataFormCheckBoxField checkBoxField = dataFormApp.dataForm.Fields[0] as DataFormCheckBoxField;
                Assert.IsNotNull(checkBoxField);
                Assert.AreEqual("BoolProperty", checkBoxField.Binding.Path.Path);
                Assert.AreEqual(BindingMode.TwoWay, checkBoxField.Binding.Mode);
                Assert.IsTrue(checkBoxField.Binding.ValidatesOnExceptions);
                Assert.IsTrue(checkBoxField.Binding.NotifyOnValidationError);

                DataFormDateField dateField = dataFormApp.dataForm.Fields[1] as DataFormDateField;
                Assert.IsNotNull(dateField);
                Assert.AreEqual("DateTimeProperty", dateField.Binding.Path.Path);
                Assert.AreEqual(BindingMode.TwoWay, dateField.Binding.Mode);
                Assert.IsTrue(dateField.Binding.ValidatesOnExceptions);
                Assert.IsTrue(dateField.Binding.NotifyOnValidationError);

                DataFormTextField textField = dataFormApp.dataForm.Fields[2] as DataFormTextField;
                Assert.IsNotNull(textField);
                Assert.AreEqual("StringProperty", textField.Binding.Path.Path);
                Assert.AreEqual(BindingMode.TwoWay, textField.Binding.Mode);
                Assert.IsTrue(textField.Binding.ValidatesOnExceptions);
                Assert.IsTrue(textField.Binding.NotifyOnValidationError);

                DataFormTemplateField templateField = dataFormApp.dataForm.Fields[3] as DataFormTemplateField;
                Assert.IsNotNull(templateField);
                Assert.IsInstanceOfType(templateField.DisplayTemplate, typeof(TemplateFieldDisplayTemplate));
                Assert.IsInstanceOfType(templateField.EditTemplate, typeof(TemplateFieldEditTemplate));

                DataFormComboBoxField comboBoxField = dataFormApp.dataForm.Fields[4] as DataFormComboBoxField;
                Assert.IsNotNull(comboBoxField);
                Assert.AreEqual(intCollection, comboBoxField.ItemsSource);
                Assert.AreEqual("IntProperty", comboBoxField.Binding.Path.Path);
                Assert.AreEqual(BindingMode.TwoWay, comboBoxField.Binding.Mode);
                Assert.IsTrue(comboBoxField.Binding.ValidatesOnExceptions);
                Assert.IsTrue(comboBoxField.Binding.NotifyOnValidationError);

                DataFormFieldGroup groupField = dataFormApp.dataForm.Fields[5] as DataFormFieldGroup;
                Assert.IsNotNull(groupField);
                Assert.AreEqual(2, groupField.Fields.Count);
                Assert.AreEqual(2, groupField.Fields.Count);

                DataFormTextField innerTextField1 = groupField.Fields[0] as DataFormTextField;
                Assert.IsNotNull(innerTextField1);
                Assert.AreEqual("IntProperty", innerTextField1.Binding.Path.Path);

                DataFormTextField innerTextField2 = groupField.Fields[1] as DataFormTextField;
                Assert.IsNotNull(innerTextField2);
                Assert.AreEqual("StringProperty", innerTextField2.Binding.Path.Path);

                DataFormSeparator separator = dataFormApp.dataForm.Fields[6] as DataFormSeparator;
                Assert.IsNotNull(separator);
                Assert.AreEqual(Colors.Green, (separator.Stroke as SolidColorBrush).Color);
                Assert.AreEqual(4, separator.StrokeDashArray.Count);
                Assert.AreEqual(1, separator.StrokeDashOffset);
                Assert.AreEqual(2, separator.StrokeThickness);

                DataFormHeader header = dataFormApp.dataForm.Fields[7] as DataFormHeader;
                Assert.IsNotNull(header);
                Assert.IsInstanceOfType(header.Content, typeof(TextBlock));
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that fields can be defined in XAML.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "There are a lot of items to be tested.")]
        [Description("Ensure that fields can be defined in XAML.")]
        public void DefineFieldsInXaml()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(8, dataFormApp.dataForm.Fields.Count);

                DataFormCheckBoxField checkBoxField = dataFormApp.dataForm.Fields[0] as DataFormCheckBoxField;
                Assert.IsNotNull(checkBoxField);
                Assert.AreEqual("BoolProperty", checkBoxField.Binding.Path.Path);
                Assert.AreEqual(BindingMode.TwoWay, checkBoxField.Binding.Mode);
                Assert.IsTrue(checkBoxField.Binding.ValidatesOnExceptions);
                Assert.IsTrue(checkBoxField.Binding.NotifyOnValidationError);

                DataFormDateField dateField = dataFormApp.dataForm.Fields[1] as DataFormDateField;
                Assert.IsNotNull(dateField);
                Assert.AreEqual("DateTimeProperty", dateField.Binding.Path.Path);
                Assert.AreEqual(BindingMode.TwoWay, dateField.Binding.Mode);
                Assert.IsTrue(dateField.Binding.ValidatesOnExceptions);
                Assert.IsTrue(dateField.Binding.NotifyOnValidationError);

                DataFormTextField textField = dataFormApp.dataForm.Fields[2] as DataFormTextField;
                Assert.IsNotNull(textField);
                Assert.AreEqual("StringProperty", textField.Binding.Path.Path);
                Assert.AreEqual(BindingMode.TwoWay, textField.Binding.Mode);
                Assert.IsTrue(textField.Binding.ValidatesOnExceptions);
                Assert.IsTrue(textField.Binding.NotifyOnValidationError);

                DataFormTemplateField templateField = dataFormApp.dataForm.Fields[3] as DataFormTemplateField;
                Assert.IsNotNull(templateField);
                Assert.IsNotNull(templateField.DisplayTemplate);
                Assert.IsNotNull(templateField.EditTemplate);

                DataFormComboBoxField comboBoxField = dataFormApp.dataForm.Fields[4] as DataFormComboBoxField;
                Assert.IsNotNull(comboBoxField);
                Assert.AreEqual(dataFormApp.Resources["intCollection"], comboBoxField.ItemsSource);
                Assert.AreEqual("IntProperty", comboBoxField.Binding.Path.Path);
                Assert.AreEqual(BindingMode.TwoWay, comboBoxField.Binding.Mode);
                Assert.IsTrue(comboBoxField.Binding.ValidatesOnExceptions);
                Assert.IsTrue(comboBoxField.Binding.NotifyOnValidationError);

                DataFormFieldGroup groupField = dataFormApp.dataForm.Fields[5] as DataFormFieldGroup;
                Assert.IsNotNull(groupField);
                Assert.AreEqual(2, groupField.Fields.Count);
                Assert.AreEqual(2, groupField.Fields.Count);

                DataFormTextField innerTextField1 = groupField.Fields[0] as DataFormTextField;
                Assert.IsNotNull(innerTextField1);
                Assert.AreEqual("IntProperty", innerTextField1.Binding.Path.Path);

                DataFormTextField innerTextField2 = groupField.Fields[1] as DataFormTextField;
                Assert.IsNotNull(innerTextField2);
                Assert.AreEqual("StringProperty", innerTextField2.Binding.Path.Path);

                DataFormSeparator separator = dataFormApp.dataForm.Fields[6] as DataFormSeparator;
                Assert.IsNotNull(separator);
                Assert.AreEqual(Colors.Green, (separator.Stroke as SolidColorBrush).Color);
                Assert.AreEqual(4, separator.StrokeDashArray.Count);
                Assert.AreEqual(1, separator.StrokeDashOffset);
                Assert.AreEqual(2, separator.StrokeThickness);

                DataFormHeader header = dataFormApp.dataForm.Fields[7] as DataFormHeader;
                Assert.IsNotNull(header);
                Assert.IsInstanceOfType(header.Content, typeof(TextBlock));
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure fields generate the correct UI elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure fields generate the correct UI elements.")]
        public void EnsureCorrectFieldUIGeneration()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;

                dataFormApp.dataForm.CurrentItem = new DataClass();
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                FieldLabel fieldLabel;

                fieldLabel = dataFormApp.dataForm.Labels[0] as FieldLabel;
                Assert.IsNotNull(fieldLabel);
                Assert.AreEqual("Bool Property", fieldLabel.Content);
                Assert.IsInstanceOfType(dataFormApp.dataForm.InputControls[0], typeof(CheckBox));

                fieldLabel = dataFormApp.dataForm.Labels[1] as FieldLabel;
                Assert.IsNotNull(fieldLabel);
                Assert.AreEqual("Date Time Property", fieldLabel.Content);
                Assert.IsInstanceOfType(dataFormApp.dataForm.InputControls[1], typeof(DatePicker));

                fieldLabel = dataFormApp.dataForm.Labels[2] as FieldLabel;
                Assert.IsNotNull(fieldLabel);
                Assert.AreEqual("String Property", fieldLabel.Content);
                Assert.IsInstanceOfType(dataFormApp.dataForm.InputControls[2], typeof(TextBox));

                fieldLabel = dataFormApp.dataForm.Labels[3] as FieldLabel;
                Assert.IsNotNull(fieldLabel);
                Assert.AreEqual("Custom label", fieldLabel.Content);
                Assert.IsInstanceOfType(dataFormApp.dataForm.InputControls[3], typeof(TextBlock));

                fieldLabel = dataFormApp.dataForm.Labels[4] as FieldLabel;
                Assert.IsNotNull(fieldLabel);
                Assert.AreEqual("Int Property", fieldLabel.Content);
                Assert.IsInstanceOfType(dataFormApp.dataForm.InputControls[4], typeof(ComboBox));

                fieldLabel = dataFormApp.dataForm.Labels[5] as FieldLabel;
                Assert.IsNotNull(fieldLabel);
                Assert.AreEqual("Int Property", fieldLabel.Content);
                Assert.IsInstanceOfType(dataFormApp.dataForm.InputControls[5], typeof(TextBox));

                fieldLabel = dataFormApp.dataForm.Labels[6] as FieldLabel;
                Assert.IsNotNull(fieldLabel);
                Assert.AreEqual("String Property", fieldLabel.Content);
                Assert.IsInstanceOfType(dataFormApp.dataForm.InputControls[6], typeof(TextBox));

                fieldLabel = dataFormApp.dataForm.Labels[7] as FieldLabel;
                Assert.IsNull(fieldLabel);
                Assert.IsInstanceOfType(dataFormApp.dataForm.InputControls[7], typeof(Grid));

                fieldLabel = dataFormApp.dataForm.Labels[8] as FieldLabel;
                Assert.IsNull(fieldLabel);
                Assert.IsInstanceOfType(dataFormApp.dataForm.InputControls[8], typeof(ContentPresenter));

                fieldLabel = dataFormApp.dataForm.Labels[9] as FieldLabel;
                Assert.IsNull(fieldLabel);
                Assert.IsInstanceOfType(dataFormApp.dataForm.InputControls[9], typeof(ContentPresenter));
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure fields generate the correct UI elements when using DataFormCheckBoxField.IsThreeState.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure fields generate the correct UI elements when using DataFormCheckBoxField.IsThreeState.")]
        public void EnsureCorrectFieldUIGenerationWithIsThreeState()
        {
            DataFormApp_Fields2 dataFormApp = new DataFormApp_Fields2();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;

                dataFormApp.dataForm.CurrentItem = new DataClassWithNullableBool();
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                CheckBox checkBox = dataFormApp.dataForm.InputControls[0] as CheckBox;
                Assert.IsTrue(checkBox.IsThreeState);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure fields generate the correct UI elements with styles applied.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure fields generate the correct UI elements with styles applied.")]
        public void EnsureCorrectFieldUIGenerationWithStyles()
        {
            DataFormApp_FieldsWithStyles dataFormApp = new DataFormApp_FieldsWithStyles();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;

                dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(2, ListOperations.All);
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                FieldLabel fieldLabel = dataFormApp.dataForm.Labels[2] as FieldLabel;
                TextBox textBox = dataFormApp.dataForm.InputControls[2] as TextBox;

                Assert.IsNotNull(fieldLabel.Style);
                Assert.AreEqual(16, fieldLabel.FontSize);
                Assert.IsNotNull(textBox.Style);
                Assert.AreEqual(Colors.Green, (textBox.BorderBrush as SolidColorBrush).Color);
                Assert.AreEqual(new Thickness(3, 3, 3, 3), textBox.BorderThickness);

                fieldLabel = dataFormApp.dataForm.Labels[0] as FieldLabel;
                Assert.IsNotNull(fieldLabel.Style);
                Assert.AreEqual(22, fieldLabel.FontSize);
                fieldLabel = dataFormApp.dataForm.Labels[1] as FieldLabel;
                Assert.IsNotNull(fieldLabel.Style);
                Assert.AreEqual(22, fieldLabel.FontSize);
                fieldLabel = dataFormApp.dataForm.Labels[3] as FieldLabel;
                Assert.IsNotNull(fieldLabel.Style);
                Assert.AreEqual(22, fieldLabel.FontSize);
                fieldLabel = dataFormApp.dataForm.Labels[4] as FieldLabel;
                Assert.IsNotNull(fieldLabel.Style);
                Assert.AreEqual(22, fieldLabel.FontSize);

                this.ExpectContentLoaded();
                dataFormApp.dataForm.BeginEdit();
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = dataFormApp.dataForm.InputControls[2] as TextBox;

                Assert.IsNotNull(textBox.Style);
                Assert.AreEqual(Colors.Blue, (textBox.BorderBrush as SolidColorBrush).Color);
                Assert.AreEqual(new Thickness(2, 2, 2, 2), textBox.BorderThickness);

                this.ExpectCurrentItemChange();
                dataFormApp.dataForm.AddItem();
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                TextBox textBox = dataFormApp.dataForm.InputControls[2] as TextBox;

                Assert.IsNotNull(textBox.Style);
                Assert.AreEqual(Colors.Yellow, (textBox.BorderBrush as SolidColorBrush).Color);
                Assert.AreEqual(new Thickness(4, 4, 4, 4), textBox.BorderThickness);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure fields' UI elements are given the correct values.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "There are a lot of items to be tested.")]
        [Description("Ensure fields' UI elements are given the correct values.")]
        public void EnsureCorrectFieldValues()
        {
            DataClassList dataClassList = DataClassList.GetDataClassList(2, ListOperations.All);

            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;

                dataFormApp.dataForm.CurrentItem = dataClassList[0];
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                CheckBox checkBox = dataFormApp.dataForm.InputControls[0] as CheckBox;
                Assert.AreEqual(dataClassList[0].BoolProperty, checkBox.IsChecked);

                DatePicker datePicker = dataFormApp.dataForm.InputControls[1] as DatePicker;
                Assert.AreEqual(dataClassList[0].DateTimeProperty, datePicker.SelectedDate);

                TextBox textBox = dataFormApp.dataForm.InputControls[2] as TextBox;
                Assert.AreEqual(dataClassList[0].StringProperty, textBox.Text);

                TextBlock textBlock = dataFormApp.dataForm.InputControls[3] as TextBlock;
                Assert.AreEqual("Display template", textBlock.Text);

                ComboBox comboBox = dataFormApp.dataForm.InputControls[4] as ComboBox;
                Assert.AreEqual(dataClassList[0].IntProperty, comboBox.SelectedItem);

                TextBox innerTextBox1 = dataFormApp.dataForm.InputControls[5] as TextBox;
                Assert.AreEqual(dataClassList[0].IntProperty.ToString(CultureInfo.InvariantCulture), innerTextBox1.Text);

                TextBox innerTextBox2 = dataFormApp.dataForm.InputControls[6] as TextBox;
                Assert.AreEqual(dataClassList[0].StringProperty, innerTextBox2.Text);

                ContentPresenter contentPresenter1 = dataFormApp.dataForm.InputControls[8] as ContentPresenter;
                Line line = contentPresenter1.Content as Line;
                Assert.IsNotNull(line);
                Assert.AreEqual(Colors.Green, (line.Stroke as SolidColorBrush).Color);
                Assert.AreEqual(4, line.StrokeDashArray.Count);
                Assert.AreEqual(1, line.StrokeDashOffset);
                Assert.AreEqual(2, line.StrokeThickness);

                ContentPresenter contentPresenter2 = dataFormApp.dataForm.InputControls[9] as ContentPresenter;
                TextBlock textBlock2 = contentPresenter2.Content as TextBlock;
                Assert.IsNotNull(textBlock2);
                Assert.AreEqual("Header Content", textBlock2.Text);
                Assert.AreEqual(12, textBlock2.FontSize);
            });

            DataFormApp_Fields dataFormApp2 = new DataFormApp_Fields();

            this.EnqueueCallback(() =>
            {
                this.RemoveFromPanel();
                this.DataFormAppBase = dataFormApp2;

                dataFormApp2.dataForm.CurrentItem = dataClassList[1];
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                CheckBox checkBox = dataFormApp2.dataForm.InputControls[0] as CheckBox;
                Assert.AreEqual(dataClassList[1].BoolProperty, checkBox.IsChecked);

                DatePicker datePicker = dataFormApp2.dataForm.InputControls[1] as DatePicker;
                Assert.AreEqual(dataClassList[1].DateTimeProperty, datePicker.SelectedDate);

                TextBox textBox = dataFormApp2.dataForm.InputControls[2] as TextBox;
                Assert.AreEqual(dataClassList[1].StringProperty, textBox.Text);

                TextBlock textBlock = dataFormApp2.dataForm.InputControls[3] as TextBlock;
                Assert.AreEqual("Display template", textBlock.Text);

                ComboBox comboBox = dataFormApp2.dataForm.InputControls[4] as ComboBox;
                Assert.AreEqual(dataClassList[1].IntProperty, comboBox.SelectedItem);

                TextBox innerTextBox1 = dataFormApp2.dataForm.InputControls[5] as TextBox;
                Assert.AreEqual(dataClassList[1].IntProperty.ToString(CultureInfo.InvariantCulture), innerTextBox1.Text);

                TextBox innerTextBox2 = dataFormApp2.dataForm.InputControls[6] as TextBox;
                Assert.AreEqual(dataClassList[1].StringProperty, innerTextBox2.Text);

                ContentPresenter contentPresenter1 = dataFormApp.dataForm.InputControls[8] as ContentPresenter;
                Line line = contentPresenter1.Content as Line;
                Assert.IsNotNull(line);
                Assert.AreEqual(Colors.Green, (line.Stroke as SolidColorBrush).Color);
                Assert.AreEqual(4, line.StrokeDashArray.Count);
                Assert.AreEqual(1, line.StrokeDashOffset);
                Assert.AreEqual(2, line.StrokeThickness);

                ContentPresenter contentPresenter2 = dataFormApp.dataForm.InputControls[9] as ContentPresenter;
                TextBlock textBlock2 = contentPresenter2.Content as TextBlock;
                Assert.IsNotNull(textBlock2);
                Assert.AreEqual("Header Content", textBlock2.Text);
                Assert.AreEqual(12, textBlock2.FontSize);
            });

            this.EnqueueTestComplete();
        }
        
        /// <summary>
        /// Ensure fields' UI elements are given the correct values when the current item is set after a load.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure fields' UI elements are given the correct values when the current item is set after a load.")]
        public void EnsureCorrectFieldValuesAfterLoad()
        {
            DataClassList dataClassList = DataClassList.GetDataClassList(1, ListOperations.All);

            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                this.ExpectCurrentItemChange();
                dataFormApp.dataForm.CurrentItem = dataClassList[0];
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                CheckBox checkBox = dataFormApp.dataForm.InputControls[0] as CheckBox;
                Assert.AreEqual(dataClassList[0].BoolProperty, checkBox.IsChecked);

                DatePicker datePicker = dataFormApp.dataForm.InputControls[1] as DatePicker;
                Assert.AreEqual(dataClassList[0].DateTimeProperty, datePicker.SelectedDate);

                TextBox textBox = dataFormApp.dataForm.InputControls[2] as TextBox;
                Assert.AreEqual(dataClassList[0].StringProperty, textBox.Text);

                TextBlock textBlock = dataFormApp.dataForm.InputControls[3] as TextBlock;
                Assert.AreEqual("Display template", textBlock.Text);

                ComboBox comboBox = dataFormApp.dataForm.InputControls[4] as ComboBox;
                Assert.AreEqual(dataClassList[0].IntProperty, comboBox.SelectedItem);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure the DataFormComboBoxField functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure the DataFormComboBoxField functions properly.")]
        public void EnsureDataFormComboBoxFieldFunctionsCorrectly()
        {
            DataFormApp_Fields2 dataFormApp = new DataFormApp_Fields2();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;

                dataFormApp.dataForm.CurrentItem = new DataClassWithNullableBool() { IntProperty = 5 };
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                ComboBox comboBox = dataFormApp.dataForm.InputControls[4] as ComboBox;
                Assert.AreEqual(5, comboBox.SelectedIndex);
                (dataFormApp.dataForm.CurrentItem as DataClassWithNullableBool).IntProperty = 3;

                Assert.AreEqual(3, comboBox.SelectedIndex);
                ((dataFormApp.dataForm.Fields[0] as DataFormFieldGroup).Fields[4] as DataFormComboBoxField).ItemsSource = new List<int>()
                {
                    3,
                    6,
                    9,
                    12,
                    15
                };

                Assert.AreEqual(-1, (dataFormApp.dataForm.CurrentItem as DataClassWithNullableBool).IntProperty);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure the DataFormDateField functions properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure the DataFormDateField functions properly.")]
        public void EnsureDataFormDateFieldFunctionsCorrectly()
        {
            CalendarDateRange calendarDateRange;
            DataFormApp_Fields2 dataFormApp = new DataFormApp_Fields2();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;

                DataFormDateField dateField = ((dataFormApp.dataForm.Fields[0] as DataFormFieldGroup).Fields[1]) as DataFormDateField;
                calendarDateRange = new CalendarDateRange(new DateTime(2009, 1, 2), new DateTime(2009, 1, 4));

                dateField.BlackoutDates.Add(calendarDateRange);
                dateField.DisplayDateStart = new DateTime(2009, 1, 10);
                dateField.DisplayDateEnd = new DateTime(2009, 4, 18);
                dateField.SelectedDateFormat = DatePickerFormat.Long;

                dataFormApp.dataForm.CurrentItem = new DataClassWithNullableBool() { IntProperty = 5 };
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                DatePicker datePicker = dataFormApp.dataForm.InputControls[1] as DatePicker;
                Assert.AreEqual(1, datePicker.BlackoutDates.Count);
                Assert.AreEqual(new DateTime(2009, 1, 10), datePicker.DisplayDateStart);
                Assert.AreEqual(new DateTime(2009, 4, 18), datePicker.DisplayDateEnd);
                Assert.AreEqual(DatePickerFormat.Long, datePicker.SelectedDateFormat);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure a DataFormTemplateField with no templates specified does not raise an exception.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure a DataFormTemplateField with no templates specified does not raise an exception.")]
        public void EnsureEmptyTemplateFieldDoesNotRaiseException()
        {
            DataFormApp_FieldsEmptyTemplateField dataFormApp = new DataFormApp_FieldsEmptyTemplateField();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
                dataFormApp.dataForm.CurrentItem = new DataClassWithNullableBool() { IntProperty = 5 };
            });

            this.AddToPanelAndWaitForLoad();
            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that DataFormTemplateField template fallback works properly with no display or edit template.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that DataFormTemplateField template fallback works properly with no display or edit template.")]
        public void TestTemplateFieldFallbackNoDisplayOrEditTemplate()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();
            this.DataFormAppBase = dataFormApp;
            DataFormTemplateField templateField = dataFormApp.dataForm.Fields[3] as DataFormTemplateField;
            templateField.DisplayTemplate = null;
            templateField.EditTemplate = null;
            TextBlock textBlock = null;
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(1, ListOperations.All);

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                textBlock = dataFormApp.dataForm.InputControls[3] as TextBlock;
                Assert.AreEqual("Insert template", textBlock.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that DataFormTemplateField template fallback works properly with no display template.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that DataFormTemplateField template fallback works properly with no display template.")]
        public void TestTemplateFieldFallbackNoDisplayTemplate()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();
            this.DataFormAppBase = dataFormApp;
            DataFormTemplateField templateField = dataFormApp.dataForm.Fields[3] as DataFormTemplateField;
            templateField.DisplayTemplate = null;
            TextBlock textBlock = null;
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(1, ListOperations.All);

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                textBlock = dataFormApp.dataForm.InputControls[3] as TextBlock;
                Assert.AreEqual("Edit template", textBlock.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that DataFormTemplateField template fallback works properly with no edit or insert templates.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that DataFormTemplateField template fallback works properly with no edit or insert templates.")]
        public void TestTemplateFieldFallbackNoEditOrInsertTemplate()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();
            this.DataFormAppBase = dataFormApp;
            DataFormTemplateField templateField = dataFormApp.dataForm.Fields[3] as DataFormTemplateField;
            templateField.EditTemplate = null;
            templateField.InsertTemplate = null;
            TextBlock textBlock = null;
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(1, ListOperations.All);

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                ButtonBase editButton = this.GetTemplatePart<ButtonBase>("EditButton");
                this.ExpectContentLoaded();
                InvokeButton(editButton);
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                textBlock = dataFormApp.dataForm.InputControls[3] as TextBlock;
                Assert.AreEqual("Display template", textBlock.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that DataFormTemplateField template fallback works properly with no edit template.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that DataFormTemplateField template fallback works properly with no edit template.")]
        public void TestTemplateFieldFallbackNoEditTemplate()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();
            this.DataFormAppBase = dataFormApp;
            DataFormTemplateField templateField = dataFormApp.dataForm.Fields[3] as DataFormTemplateField;
            templateField.EditTemplate = null;
            TextBlock textBlock = null;
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(1, ListOperations.All);

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                ButtonBase editButton = this.GetTemplatePart<ButtonBase>("EditButton");
                this.ExpectContentLoaded();
                InvokeButton(editButton);
            });

            this.WaitForContentLoaded();

            this.EnqueueCallback(() =>
            {
                textBlock = dataFormApp.dataForm.InputControls[3] as TextBlock;
                Assert.AreEqual("Insert template", textBlock.Text);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that DataFormTemplateField template fallback works properly with no insert template.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that DataFormTemplateField template fallback works properly with no insert template.")]
        public void TestTemplateFieldFallbackNoInsertTemplate()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();
            this.DataFormAppBase = dataFormApp;
            DataFormTemplateField templateField = dataFormApp.dataForm.Fields[3] as DataFormTemplateField;
            templateField.InsertTemplate = null;
            TextBlock textBlock = null;
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(1, ListOperations.All);

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                ButtonBase newItemButton = this.GetTemplatePart<ButtonBase>("NewItemButton");
                this.ExpectCurrentItemChange();
                InvokeButton(newItemButton);
            });

            this.WaitForCurrentItemChange();

            this.EnqueueCallback(() =>
            {
                textBlock = dataFormApp.dataForm.InputControls[3] as TextBlock;
                Assert.AreEqual("Edit template", textBlock.Text);
            });

            this.EnqueueTestComplete();
        }
    }
}
