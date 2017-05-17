//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="DataFormTests_Layout.cs">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// Tests <see cref="DataForm"/> layout.
    /// </summary>
    [TestClass]
    public class DataFormTests_Layout : DataFormTests_Base
    {
        /// <summary>
        /// Ensure that changes to DescriptionViewerPosition on the DataForm causes the DataFields to re-generate UI.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that changes to DescriptionViewerPosition on the DataForm causes the DataFields to re-generate UI.")]
        public void ChangeDescriptionViewerPositionOnDataForm()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();
            dataFormApp.dataForm.CurrentItem = new DataClass();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                foreach (DataField field in dataFormApp.dataForm.Fields)
                {
                    DescriptionViewer descriptionViewer = this.DataFormDescriptions[0];
                    Label fieldLabel = this.DataFormLabels[0];
                    FrameworkElement inputControl = this.DataFormInputControls[0];

                    Assert.IsTrue(this.GetPosition(fieldLabel).X < this.GetPosition(inputControl).X);
                    Assert.IsTrue(this.GetPosition(inputControl).X < this.GetPosition(descriptionViewer).X);
                }

                dataFormApp.dataForm.DescriptionViewerPosition = DataFieldDescriptionViewerPosition.BesideLabel;
            });

            // Give the DataFields time to update their UI.
            this.EnqueueDelay(100);

            this.EnqueueCallback(() =>
            {
                foreach (DataField field in dataFormApp.dataForm.Fields)
                {
                    DescriptionViewer descriptionViewer = this.DataFormDescriptions[0];
                    Label fieldLabel = this.DataFormLabels[0];
                    FrameworkElement inputControl = this.DataFormInputControls[0];

                    Assert.IsTrue(this.GetPosition(fieldLabel).X < this.GetPosition(descriptionViewer).X);
                    Assert.IsTrue(this.GetPosition(descriptionViewer).X < this.GetPosition(inputControl).X);
                }
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that changes to LabelPosition on the DataForm causes the DataFields to re-generate UI.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that changes to LabelPosition on the DataForm causes the DataFields to re-generate UI.")]
        public void ChangeLabelPositionOnDataForm()
        {
            DataFormApp_Fields dataFormApp = new DataFormApp_Fields();
            dataFormApp.dataForm.CurrentItem = new DataClass();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                foreach (DataField field in dataFormApp.dataForm.Fields)
                {
                    Label fieldLabel = this.DataFormLabels[0];
                    FrameworkElement inputControl = this.DataFormInputControls[0];

                    Assert.IsTrue(this.GetPosition(fieldLabel).X < this.GetPosition(inputControl).X);
                }

                dataFormApp.dataForm.LabelPosition = DataFieldLabelPosition.Top;
            });

            // Give the DataFields time to update their UI.
            this.EnqueueDelay(100);

            this.EnqueueCallback(() =>
            {
                foreach (DataField field in dataFormApp.dataForm.Fields)
                {
                    Label fieldLabel = this.DataFormLabels[0];
                    FrameworkElement inputControl = this.DataFormInputControls[0];

                    Assert.IsTrue(this.GetPosition(fieldLabel).X == this.GetPosition(inputControl).X);
                }
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that setting DescriptionViewerPosition on a field works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting DescriptionViewerPosition on a field works properly.")]
        public void DescriptionViewerPosition()
        {
            DataFormApp_FieldsDescriptionViewer dataFormApp = new DataFormApp_FieldsDescriptionViewer();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;

                dataFormApp.dataForm.CurrentItem = new DataClass();
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                DescriptionViewer des1 = this.DataFormDescriptions[0] as DescriptionViewer;
                Label fieldLabel1 = this.DataFormLabels[0] as Label;
                DatePicker datePicker = this.DataFormInputControls[0] as DatePicker;

                Assert.IsTrue(this.GetPosition(fieldLabel1).X < this.GetPosition(des1).X);
                Assert.IsTrue(this.GetPosition(fieldLabel1).X < this.GetPosition(datePicker).X);
                Assert.IsTrue(this.GetPosition(datePicker).Y == this.GetPosition(des1).Y);

                DescriptionViewer des2 = this.DataFormDescriptions[1] as DescriptionViewer;
                Label fieldLabel2 = this.DataFormLabels[1] as Label;
                TextBox textBox = this.DataFormInputControls[1] as TextBox;

                Assert.IsTrue(this.GetPosition(fieldLabel2).X < this.GetPosition(des2).X);
                Assert.IsTrue(this.GetPosition(fieldLabel2).X < this.GetPosition(textBox).X);
                Assert.IsTrue(this.GetPosition(textBox).Y == this.GetPosition(des2).Y);

                DescriptionViewer des3 = this.DataFormDescriptions[2] as DescriptionViewer;
                Label fieldLabel3 = this.DataFormLabels[2] as Label;
                TextBox innerTextBox1 = this.DataFormInputControls[2] as TextBox;

                Assert.IsTrue(this.GetPosition(fieldLabel3).X < this.GetPosition(innerTextBox1).X);
                Assert.IsTrue(this.GetPosition(innerTextBox1).X < this.GetPosition(des3).X);
                Assert.IsTrue(this.GetPosition(innerTextBox1).Y == this.GetPosition(des3).Y);

                DescriptionViewer des4 = this.DataFormDescriptions[3] as DescriptionViewer;
                Label fieldLabel4 = this.DataFormLabels[3] as Label;
                TextBox innerTextBox2 = this.DataFormInputControls[3] as TextBox;

                Assert.IsTrue(this.GetPosition(fieldLabel4).X < this.GetPosition(innerTextBox2).X);
                Assert.IsTrue(this.GetPosition(fieldLabel4).X < this.GetPosition(des4).X);
                Assert.IsTrue(this.GetPosition(des4).X < this.GetPosition(innerTextBox2).X);
                Assert.IsTrue(this.GetPosition(des4).Y == this.GetPosition(fieldLabel4).Y);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that setting HorizontalAlignment on a label has the correct effects.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting HorizontalAlignment on a label has the correct effects.")]
        public void EnsureLabelAlignmentIsCorrect()
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
                Label fieldLabel1 = this.DataFormLabels[0] as Label;
                Label fieldLabel2 = this.DataFormLabels[1] as Label;
                Label fieldLabel3 = this.DataFormLabels[2] as Label;

                Assert.IsTrue(this.GetPosition(fieldLabel1).X + fieldLabel1.ActualWidth == this.GetPosition(fieldLabel2).X + fieldLabel2.ActualWidth);
                Assert.IsTrue(this.GetPosition(fieldLabel2).X + fieldLabel2.ActualWidth == this.GetPosition(fieldLabel3).X + fieldLabel3.ActualWidth);

                Style innerStyle = new Style(typeof(Label));
                innerStyle.Setters.Add(new Setter(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left));
                Style style = new Style(typeof(DataField));
                style.Setters.Add(new Setter(DataField.LabelStyleProperty, innerStyle));

                dataFormApp.dataForm.DataFieldStyle = style;
            });

            // Wait for a short time to allow the fields to re-generate their UI.
            this.EnqueueDelay(100);

            this.EnqueueCallback(() =>
            {
                Label fieldLabel1 = this.DataFormLabels[0] as Label;
                Label fieldLabel2 = this.DataFormLabels[1] as Label;
                Label fieldLabel3 = this.DataFormLabels[2] as Label;

                Assert.IsTrue(this.GetPosition(fieldLabel1).X == this.GetPosition(fieldLabel2).X);
                Assert.IsTrue(this.GetPosition(fieldLabel2).X == this.GetPosition(fieldLabel3).X);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that setting LabelPosition to Left on the DataForm and to Top on a field works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting LabelPosition to Left on the DataForm and to Top on a field works properly.")]
        public void LabelPositionLeftWithInnerTop()
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
                Label fieldLabel1 = this.DataFormLabels[0] as Label;
                CheckBox checkBox = this.DataFormInputControls[0] as CheckBox;

                Assert.IsTrue(this.GetPosition(fieldLabel1).X < this.GetPosition(checkBox).X);

                Label fieldLabel2 = this.DataFormLabels[1] as Label;
                DatePicker datePicker = this.DataFormInputControls[1] as DatePicker;

                Assert.IsTrue(this.GetPosition(fieldLabel2).X < this.GetPosition(datePicker).X);

                Label fieldLabel3 = this.DataFormLabels[2] as Label;
                TextBox textBox = this.DataFormInputControls[2] as TextBox;

                Assert.IsTrue(this.GetPosition(fieldLabel3).X < this.GetPosition(textBox).X);

                Label fieldLabel4 = this.DataFormLabels[3] as Label;
                TextBlock textBlock = this.DataFormInputControls[3] as TextBlock;

                Assert.IsTrue(this.GetPosition(fieldLabel4).Y < this.GetPosition(textBlock).Y);

                Label fieldLabel5 = this.DataFormLabels[4] as Label;
                ComboBox comboBox = this.DataFormInputControls[4] as ComboBox;

                Assert.IsTrue(this.GetPosition(fieldLabel5).X < this.GetPosition(comboBox).X);

                Label fieldLabel6 = this.DataFormLabels[5] as Label;
                TextBox innerTextBox1 = this.DataFormInputControls[5] as TextBox;

                Assert.IsTrue(this.GetPosition(fieldLabel6).X < this.GetPosition(innerTextBox1).X);

                Label fieldLabel7 = this.DataFormLabels[6] as Label;
                TextBox innerTextBox2 = this.DataFormInputControls[6] as TextBox;

                Assert.IsTrue(this.GetPosition(fieldLabel7).X < this.GetPosition(innerTextBox2).X);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that setting LabelPosition to Top on the DataForm and to Left on a field works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting LabelPosition to Top on the DataForm and to Left on a field works properly.")]
        public void LabelPositionTopWithInnerLeft()
        {
            DataFormApp_FieldsInverseLabelPosition dataFormApp = new DataFormApp_FieldsInverseLabelPosition();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;

                dataFormApp.dataForm.CurrentItem = new DataClass();
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                Label fieldLabel1 = this.DataFormLabels[0] as Label;
                CheckBox checkBox = this.DataFormInputControls[0] as CheckBox;

                Assert.IsTrue(this.GetPosition(fieldLabel1).Y < this.GetPosition(checkBox).Y);

                Label fieldLabel2 = this.DataFormLabels[1] as Label;
                DatePicker datePicker = this.DataFormInputControls[1] as DatePicker;

                Assert.IsTrue(this.GetPosition(fieldLabel2).Y < this.GetPosition(datePicker).Y);

                Label fieldLabel3 = this.DataFormLabels[2] as Label;
                TextBox textBox = this.DataFormInputControls[2] as TextBox;

                Assert.IsTrue(this.GetPosition(fieldLabel3).Y < this.GetPosition(textBox).Y);

                Label fieldLabel4 = this.DataFormLabels[3] as Label;
                TextBlock textBlock = this.DataFormInputControls[3] as TextBlock;

                Assert.IsTrue(this.GetPosition(fieldLabel4).X < this.GetPosition(textBlock).X);

                Label fieldLabel5 = this.DataFormLabels[4] as Label;
                ComboBox comboBox = this.DataFormInputControls[4] as ComboBox;

                Assert.IsTrue(this.GetPosition(fieldLabel5).Y < this.GetPosition(comboBox).Y);

                Label fieldLabel6 = this.DataFormLabels[5] as Label;
                TextBox innerTextBox1 = this.DataFormInputControls[5] as TextBox;

                Assert.IsTrue(this.GetPosition(fieldLabel6).Y < this.GetPosition(innerTextBox1).Y);

                Label fieldLabel7 = this.DataFormLabels[6] as Label;
                TextBox innerTextBox2 = this.DataFormInputControls[6] as TextBox;

                Assert.IsTrue(this.GetPosition(fieldLabel7).Y < this.GetPosition(innerTextBox2).Y);
            });

            this.EnqueueTestComplete();
        }
    }
}
