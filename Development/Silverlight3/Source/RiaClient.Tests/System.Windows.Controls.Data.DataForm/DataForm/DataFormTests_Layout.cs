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
        /// Ensure that grouping fields together in a horizontal layout at the DataForm level works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that grouping fields together in a horizontal layout at the DataForm level works properly.")]
        public void DataFormGroupHorizontal()
        {
            DataFormApp_Fields3GroupHorizontal dataFormApp = new DataFormApp_Fields3GroupHorizontal();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;

                dataFormApp.dataForm.CurrentItem = new DataClass();
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                FieldLabel fieldLabel1 = dataFormApp.dataForm.Labels[0] as FieldLabel;
                FieldLabel fieldLabel2 = dataFormApp.dataForm.Labels[1] as FieldLabel;
                FieldLabel fieldLabel3 = dataFormApp.dataForm.Labels[2] as FieldLabel;
                FieldLabel fieldLabel4 = dataFormApp.dataForm.Labels[3] as FieldLabel;
                FieldLabel fieldLabel5 = dataFormApp.dataForm.Labels[4] as FieldLabel;

                DescriptionViewer des1 = dataFormApp.dataForm.Descriptions[0] as DescriptionViewer;
                DescriptionViewer des2 = dataFormApp.dataForm.Descriptions[1] as DescriptionViewer;
                DescriptionViewer des3 = dataFormApp.dataForm.Descriptions[2] as DescriptionViewer;
                DescriptionViewer des4 = dataFormApp.dataForm.Descriptions[3] as DescriptionViewer;
                DescriptionViewer des5 = dataFormApp.dataForm.Descriptions[4] as DescriptionViewer;

                Assert.IsTrue(this.GetPosition(fieldLabel1).X < this.GetPosition(fieldLabel2).X);
                Assert.IsTrue(this.GetPosition(fieldLabel1).Y < this.GetPosition(fieldLabel3).Y);

                Assert.IsTrue(this.GetPosition(fieldLabel3).Y == this.GetPosition(fieldLabel4).Y);
                Assert.IsTrue(this.GetPosition(fieldLabel3).Y < this.GetPosition(fieldLabel5).Y);

                Assert.IsTrue(this.GetPosition(des1).X < this.GetPosition(des2).X);
                Assert.IsTrue(this.GetPosition(des1).X == this.GetPosition(des3).X);
                Assert.IsTrue(this.GetPosition(des1).Y < this.GetPosition(des3).Y);

                Assert.IsTrue(this.GetPosition(des3).X < this.GetPosition(des4).X);
                Assert.IsTrue(this.GetPosition(des3).Y == this.GetPosition(des4).Y);
                Assert.IsTrue(this.GetPosition(des3).X == this.GetPosition(des5).X);
                Assert.IsTrue(this.GetPosition(des3).Y < this.GetPosition(des5).Y);

                Assert.IsTrue(this.GetPosition(des3).X > this.GetPosition(fieldLabel3).X);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that grouping fields together with WrapAfter set at the DataForm level works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that grouping fields together with WrapAfter set at the DataForm level works properly.")]
        public void DataFormGroupWithWrapAfter()
        {
            DataFormApp_Fields3 dataFormApp = new DataFormApp_Fields3();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;

                dataFormApp.dataForm.CurrentItem = new DataClass();
            });

            this.AddToPanelAndWaitForLoad();
            this.EnqueueCallback(() =>
            {
                FieldLabel fieldLabel1 = dataFormApp.dataForm.Labels[0] as FieldLabel;
                FieldLabel fieldLabel2 = dataFormApp.dataForm.Labels[1] as FieldLabel;
                FieldLabel fieldLabel3 = dataFormApp.dataForm.Labels[2] as FieldLabel;
                FieldLabel fieldLabel4 = dataFormApp.dataForm.Labels[3] as FieldLabel;
                FieldLabel fieldLabel5 = dataFormApp.dataForm.Labels[4] as FieldLabel;

                Assert.IsTrue(this.GetPosition(fieldLabel1).Y < this.GetPosition(fieldLabel2).Y);
                Assert.IsTrue(this.GetPosition(fieldLabel1).X < this.GetPosition(fieldLabel3).X);
                Assert.IsTrue(this.GetPosition(fieldLabel1).Y == this.GetPosition(fieldLabel3).Y);

                Assert.IsTrue(this.GetPosition(fieldLabel3).Y < this.GetPosition(fieldLabel4).Y);
                Assert.IsTrue(this.GetPosition(fieldLabel3).X < this.GetPosition(fieldLabel5).X);
                Assert.IsTrue(this.GetPosition(fieldLabel3).Y == this.GetPosition(fieldLabel5).Y);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that grouping fields together in a horizontal layout works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that grouping fields together in a horizontal layout works properly.")]
        public void GroupHorizontal()
        {
            DataFormApp_FieldsGroupHorizontal dataFormApp = new DataFormApp_FieldsGroupHorizontal();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;

                dataFormApp.dataForm.CurrentItem = new DataClass();
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                FieldLabel fieldLabel6 = dataFormApp.dataForm.Labels[5] as FieldLabel;
                DescriptionViewer descriptionViewer6 = dataFormApp.dataForm.Descriptions[5] as DescriptionViewer;
                TextBox innerTextBox1 = dataFormApp.dataForm.InputControls[5] as TextBox;
                FieldLabel fieldLabel7 = dataFormApp.dataForm.Labels[6] as FieldLabel;
                DescriptionViewer descriptionViewer7 = dataFormApp.dataForm.Descriptions[6] as DescriptionViewer;
                TextBox innerTextBox2 = dataFormApp.dataForm.InputControls[6] as TextBox;

                Assert.IsTrue(this.GetPosition(fieldLabel6).Y == this.GetPosition(fieldLabel7).Y);
                Assert.IsTrue(this.GetPosition(descriptionViewer6).Y == this.GetPosition(descriptionViewer7).Y);
                Assert.IsTrue(this.GetPosition(fieldLabel6).X < this.GetPosition(fieldLabel7).X);
                Assert.IsTrue(this.GetPosition(descriptionViewer6).X < this.GetPosition(descriptionViewer7).X);
                Assert.IsTrue(this.GetPosition(descriptionViewer6).X > this.GetPosition(fieldLabel6).X);
                Assert.IsTrue(this.GetPosition(descriptionViewer7).X > this.GetPosition(fieldLabel7).X);
                Assert.IsTrue(this.GetPosition(innerTextBox1).Y == this.GetPosition(innerTextBox2).Y);
                Assert.IsTrue(this.GetPosition(innerTextBox1).X < this.GetPosition(innerTextBox2).X);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that grouping fields together in a horizontal layout with WrapAfter set works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that grouping fields together in a horizontal layout with WrapAfter set works properly.")]
        public void GroupHorizontalWithWrapAfter()
        {
            DataFormApp_Fields2GroupHorizontal dataFormApp = new DataFormApp_Fields2GroupHorizontal();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;

                dataFormApp.dataForm.CurrentItem = new DataClass();
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                FieldLabel fieldLabel1 = dataFormApp.dataForm.Labels[0] as FieldLabel;
                FieldLabel fieldLabel2 = dataFormApp.dataForm.Labels[1] as FieldLabel;
                FieldLabel fieldLabel3 = dataFormApp.dataForm.Labels[2] as FieldLabel;
                FieldLabel fieldLabel4 = dataFormApp.dataForm.Labels[3] as FieldLabel;
                FieldLabel fieldLabel5 = dataFormApp.dataForm.Labels[4] as FieldLabel;

                DescriptionViewer des1 = dataFormApp.dataForm.Descriptions[0] as DescriptionViewer;
                DescriptionViewer des2 = dataFormApp.dataForm.Descriptions[1] as DescriptionViewer;
                DescriptionViewer des3 = dataFormApp.dataForm.Descriptions[2] as DescriptionViewer;
                DescriptionViewer des4 = dataFormApp.dataForm.Descriptions[3] as DescriptionViewer;
                DescriptionViewer des5 = dataFormApp.dataForm.Descriptions[4] as DescriptionViewer;

                Assert.IsTrue(this.GetPosition(fieldLabel1).X < this.GetPosition(fieldLabel2).X);
                Assert.IsTrue(this.GetPosition(fieldLabel1).Y < this.GetPosition(fieldLabel3).Y);

                Assert.IsTrue(this.GetPosition(fieldLabel3).Y == this.GetPosition(fieldLabel4).Y);
                Assert.IsTrue(this.GetPosition(fieldLabel3).Y < this.GetPosition(fieldLabel5).Y);

                Assert.IsTrue(this.GetPosition(des1).X < this.GetPosition(des2).X);
                Assert.IsTrue(this.GetPosition(des1).X == this.GetPosition(des3).X);
                Assert.IsTrue(this.GetPosition(des1).Y < this.GetPosition(des3).Y);

                Assert.IsTrue(this.GetPosition(des3).X < this.GetPosition(des4).X);
                Assert.IsTrue(this.GetPosition(des3).Y == this.GetPosition(des4).Y);
                Assert.IsTrue(this.GetPosition(des3).X == this.GetPosition(des5).X);
                Assert.IsTrue(this.GetPosition(des3).Y < this.GetPosition(des5).Y);

                Assert.IsTrue(this.GetPosition(des3).X > this.GetPosition(fieldLabel3).X);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that grouping fields together in a vertical layout works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that grouping fields together in a vertical layout works properly.")]
        public void GroupVertical()
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
                FieldLabel fieldLabel6 = dataFormApp.dataForm.Labels[5] as FieldLabel;
                DescriptionViewer des6 = dataFormApp.dataForm.Descriptions[5] as DescriptionViewer;
                TextBox innerTextBox1 = dataFormApp.dataForm.InputControls[5] as TextBox;
                FieldLabel fieldLabel7 = dataFormApp.dataForm.Labels[6] as FieldLabel;
                DescriptionViewer des7 = dataFormApp.dataForm.Descriptions[6] as DescriptionViewer;
                TextBox innerTextBox2 = dataFormApp.dataForm.InputControls[6] as TextBox;

                Assert.IsTrue(this.GetPosition(fieldLabel6).Y < this.GetPosition(fieldLabel7).Y);
                Assert.IsTrue(this.GetPosition(innerTextBox1).Y < this.GetPosition(innerTextBox2).Y);
                Assert.IsTrue(this.GetPosition(innerTextBox1).X == this.GetPosition(innerTextBox2).X);
                Assert.IsTrue(this.GetPosition(des6).Y < this.GetPosition(des7).Y);
                Assert.IsTrue(this.GetPosition(des6).X == this.GetPosition(des7).X);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that grouping fields together in a vertical layout with WrapAfter set works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that grouping fields together in a vertical layout with WrapAfter set works properly.")]
        public void GroupVerticalWithWrapAfter()
        {
            DataFormApp_Fields2 dataFormApp = new DataFormApp_Fields2();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;

                dataFormApp.dataForm.CurrentItem = new DataClass();
            });

            this.AddToPanelAndWaitForLoad();
            this.EnqueueCallback(() =>
            {
                FieldLabel fieldLabel1 = dataFormApp.dataForm.Labels[0] as FieldLabel;
                FieldLabel fieldLabel2 = dataFormApp.dataForm.Labels[1] as FieldLabel;
                FieldLabel fieldLabel3 = dataFormApp.dataForm.Labels[2] as FieldLabel;
                FieldLabel fieldLabel4 = dataFormApp.dataForm.Labels[3] as FieldLabel;
                FieldLabel fieldLabel5 = dataFormApp.dataForm.Labels[4] as FieldLabel;

                Assert.IsTrue(this.GetPosition(fieldLabel1).Y < this.GetPosition(fieldLabel2).Y);
                Assert.IsTrue(this.GetPosition(fieldLabel1).X < this.GetPosition(fieldLabel3).X);
                Assert.IsTrue(this.GetPosition(fieldLabel1).Y == this.GetPosition(fieldLabel3).Y);

                Assert.IsTrue(this.GetPosition(fieldLabel3).Y < this.GetPosition(fieldLabel4).Y);
                Assert.IsTrue(this.GetPosition(fieldLabel3).X < this.GetPosition(fieldLabel5).X);
                Assert.IsTrue(this.GetPosition(fieldLabel3).Y == this.GetPosition(fieldLabel5).Y);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that setting FieldLabelPosition to Left on the DataForm and to Top on a field works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting FieldLabelPosition to Left on the DataForm and to Top on a field works properly.")]
        public void FieldLabelPositionLeftWithInnerTop()
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
                FieldLabel fieldLabel1 = dataFormApp.dataForm.Labels[0] as FieldLabel;
                CheckBox checkBox = dataFormApp.dataForm.InputControls[0] as CheckBox;

                Assert.IsTrue(this.GetPosition(fieldLabel1).X < this.GetPosition(checkBox).X);

                FieldLabel fieldLabel2 = dataFormApp.dataForm.Labels[1] as FieldLabel;
                DatePicker datePicker = dataFormApp.dataForm.InputControls[1] as DatePicker;

                Assert.IsTrue(this.GetPosition(fieldLabel2).X < this.GetPosition(datePicker).X);

                FieldLabel fieldLabel3 = dataFormApp.dataForm.Labels[2] as FieldLabel;
                TextBox textBox = dataFormApp.dataForm.InputControls[2] as TextBox;

                Assert.IsTrue(this.GetPosition(fieldLabel3).X < this.GetPosition(textBox).X);

                FieldLabel fieldLabel4 = dataFormApp.dataForm.Labels[3] as FieldLabel;
                TextBlock textBlock = dataFormApp.dataForm.InputControls[3] as TextBlock;

                Assert.IsTrue(this.GetPosition(fieldLabel4).Y < this.GetPosition(textBlock).Y);

                FieldLabel fieldLabel5 = dataFormApp.dataForm.Labels[4] as FieldLabel;
                ComboBox comboBox = dataFormApp.dataForm.InputControls[4] as ComboBox;

                Assert.IsTrue(this.GetPosition(fieldLabel5).X < this.GetPosition(comboBox).X);

                FieldLabel fieldLabel6 = dataFormApp.dataForm.Labels[5] as FieldLabel;
                TextBox innerTextBox1 = dataFormApp.dataForm.InputControls[5] as TextBox;

                Assert.IsTrue(this.GetPosition(fieldLabel6).X < this.GetPosition(innerTextBox1).X);

                FieldLabel fieldLabel7 = dataFormApp.dataForm.Labels[6] as FieldLabel;
                TextBox innerTextBox2 = dataFormApp.dataForm.InputControls[6] as TextBox;

                Assert.IsTrue(this.GetPosition(fieldLabel7).X < this.GetPosition(innerTextBox2).X);
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
                DescriptionViewer des1 = dataFormApp.dataForm.Descriptions[0] as DescriptionViewer;
                FieldLabel fieldLabel1 = dataFormApp.dataForm.Labels[0] as FieldLabel;
                DatePicker datePicker = dataFormApp.dataForm.InputControls[0] as DatePicker;

                Assert.IsTrue(this.GetPosition(fieldLabel1).X < this.GetPosition(des1).X);
                Assert.IsTrue(this.GetPosition(fieldLabel1).X < this.GetPosition(datePicker).X);
                Assert.IsTrue(this.GetPosition(datePicker).Y == this.GetPosition(des1).Y);

                DescriptionViewer des2 = dataFormApp.dataForm.Descriptions[1] as DescriptionViewer;
                FieldLabel fieldLabel2 = dataFormApp.dataForm.Labels[1] as FieldLabel;
                TextBox textBox = dataFormApp.dataForm.InputControls[1] as TextBox;

                Assert.IsTrue(this.GetPosition(fieldLabel2).X < this.GetPosition(des2).X);
                Assert.IsTrue(this.GetPosition(fieldLabel2).X < this.GetPosition(textBox).X);
                Assert.IsTrue(this.GetPosition(textBox).Y == this.GetPosition(des2).Y);

                DescriptionViewer des3 = dataFormApp.dataForm.Descriptions[2] as DescriptionViewer;
                FieldLabel fieldLabel3 = dataFormApp.dataForm.Labels[2] as FieldLabel;
                TextBox innerTextBox1 = dataFormApp.dataForm.InputControls[2] as TextBox;

                Assert.IsTrue(this.GetPosition(fieldLabel3).X < this.GetPosition(innerTextBox1).X);
                Assert.IsTrue(this.GetPosition(innerTextBox1).X < this.GetPosition(des3).X);
                Assert.IsTrue(this.GetPosition(innerTextBox1).Y == this.GetPosition(des3).Y);

                DescriptionViewer des4 = dataFormApp.dataForm.Descriptions[3] as DescriptionViewer;
                FieldLabel fieldLabel4 = dataFormApp.dataForm.Labels[3] as FieldLabel;
                TextBox innerTextBox2 = dataFormApp.dataForm.InputControls[3] as TextBox;

                Assert.IsTrue(this.GetPosition(fieldLabel4).X < this.GetPosition(innerTextBox2).X);
                Assert.IsTrue(this.GetPosition(fieldLabel4).X < this.GetPosition(des4).X);
                Assert.IsTrue(this.GetPosition(des4).X < this.GetPosition(innerTextBox2).X);
                Assert.IsTrue(this.GetPosition(des4).Y == this.GetPosition(fieldLabel4).Y);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure that setting FieldLabelPosition to Top on the DataForm and to Left on a field works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting FieldLabelPosition to Top on the DataForm and to Left on a field works properly.")]
        public void FieldLabelPositionTopWithInnerLeft()
        {
            DataFormApp_FieldsInverseFieldLabelPosition dataFormApp = new DataFormApp_FieldsInverseFieldLabelPosition();

            this.EnqueueCallback(() =>
            {
                this.DataFormAppBase = dataFormApp;

                dataFormApp.dataForm.CurrentItem = new DataClass();
            });

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                FieldLabel fieldLabel1 = dataFormApp.dataForm.Labels[0] as FieldLabel;
                CheckBox checkBox = dataFormApp.dataForm.InputControls[0] as CheckBox;

                Assert.IsTrue(this.GetPosition(fieldLabel1).Y < this.GetPosition(checkBox).Y);



                FieldLabel fieldLabel3 = dataFormApp.dataForm.Labels[2] as FieldLabel;
                TextBox textBox = dataFormApp.dataForm.InputControls[2] as TextBox;

                Assert.IsTrue(this.GetPosition(fieldLabel3).Y < this.GetPosition(textBox).Y);

                FieldLabel fieldLabel4 = dataFormApp.dataForm.Labels[3] as FieldLabel;
                TextBlock textBlock = dataFormApp.dataForm.InputControls[3] as TextBlock;

                Assert.IsTrue(this.GetPosition(fieldLabel4).X < this.GetPosition(textBlock).X);

                FieldLabel fieldLabel5 = dataFormApp.dataForm.Labels[4] as FieldLabel;
                ComboBox comboBox = dataFormApp.dataForm.InputControls[4] as ComboBox;

                Assert.IsTrue(this.GetPosition(fieldLabel5).Y < this.GetPosition(comboBox).Y);

                FieldLabel fieldLabel6 = dataFormApp.dataForm.Labels[5] as FieldLabel;
                TextBox innerTextBox1 = dataFormApp.dataForm.InputControls[5] as TextBox;

                Assert.IsTrue(this.GetPosition(fieldLabel6).Y < this.GetPosition(innerTextBox1).Y);

                FieldLabel fieldLabel7 = dataFormApp.dataForm.Labels[6] as FieldLabel;
                TextBox innerTextBox2 = dataFormApp.dataForm.InputControls[6] as TextBox;

                Assert.IsTrue(this.GetPosition(fieldLabel7).Y < this.GetPosition(innerTextBox2).Y);
            });

            this.EnqueueTestComplete();
        }
    }
}
