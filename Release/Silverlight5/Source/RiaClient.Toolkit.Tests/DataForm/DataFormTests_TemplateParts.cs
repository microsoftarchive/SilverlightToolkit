//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="DataFormTests_TemplateParts.cs">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// Tests <see cref="DataForm"/> template parts.
    /// </summary>
    [TestClass]
    public class DataFormTests_TemplateParts : DataFormTests_Base
    {
        #region Navigation Template Parts

        /// <summary>
        /// Ensure that the template part FirstItemButton can be retrieved.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the template part FirstItemButton can be retrieved.")]
        public void GetFirstItemButton()
        {
            this.ExpectTemplatePart<ButtonBase>("FirstItemButton");
        }

        /// <summary>
        /// Ensure that the template part PreviousItemButton can be retrieved.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the template part PreviousItemButton can be retrieved.")]
        public void GetPreviousItemButton()
        {
            this.ExpectTemplatePart<ButtonBase>("PreviousItemButton");
        }

        /// <summary>
        /// Ensure that the template part ListPositionElement can be retrieved.
        /// </summary>
        //// 









        /// <summary>
        /// Ensure that the template part NextItemButton can be retrieved.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the template part NextItemButton can be retrieved.")]
        public void GetNextItemButton()
        {
            this.ExpectTemplatePart<ButtonBase>("NextItemButton");
        }

        /// <summary>
        /// Ensure that the template part LastItemButton can be retrieved.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the template part LastItemButton can be retrieved.")]
        public void GetLastItemButton()
        {
            this.ExpectTemplatePart<ButtonBase>("LastItemButton");
        }

        /// <summary>
        /// Ensure that the template part NewItemButton can be retrieved.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the template part NewItemButton can be retrieved.")]
        public void GetNewItemButton()
        {
            this.ExpectTemplatePart<ButtonBase>("NewItemButton");
        }

        /// <summary>
        /// Ensure that the template part DeleteItemButton can be retrieved.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the template part DeleteItemButton can be retrieved.")]
        public void GetDeleteItemButton()
        {
            this.ExpectTemplatePart<ButtonBase>("DeleteItemButton");
        }

        #endregion Navigation Template Parts

        #region Middle Template Parts

        /// <summary>
        /// Ensure that the template part HeaderElement can be retrieved.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the template part HeaderElement can be retrieved.")]
        public void GetHeaderElement()
        {
            this.ExpectTemplatePart<ContentControl>("HeaderElement");
        }

        /// <summary>
        /// Ensure that the template part ContentPresenter can be retrieved.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the template part ContentPresenter can be retrieved.")]
        public void GetContentPresenter()
        {
            this.ExpectTemplatePart<ContentPresenter>("ContentPresenter");
        }

        /// <summary>
        /// Ensure that the template part ValidationSummary can be retrieved.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the template part ValidationSummary can be retrieved.")]
        public void GetValidationSummary()
        {
            this.ExpectTemplatePart<ValidationSummary>("ValidationSummary");
        }

        #endregion Middle Template Parts

        #region Edit Template Parts

        /// <summary>
        /// Ensure that the template part EditButton can be retrieved.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the template part EditButton can be retrieved.")]
        public void GetEditButton()
        {
            this.ExpectTemplatePart<ButtonBase>("EditButton");
        }

        /// <summary>
        /// Ensure that the template part CommitButton can be retrieved.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the template part CommitButton can be retrieved.")]
        public void GetCommitButton()
        {
            this.ExpectTemplatePart<ButtonBase>("CommitButton");
        }

        /// <summary>
        /// Ensure that the template part CancelButton can be retrieved.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the template part CancelButton can be retrieved.")]
        public void GetCancelButton()
        {
            this.ExpectTemplatePart<ButtonBase>("CancelButton");
        }

        #endregion Edit Template Parts

        #region Template Parts Through Properties

        /// <summary>
        /// Ensure that the the styling and accessing of template parts through properties works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the the styling and accessing of template parts through properties works properly.")]
        public void EnsureTemplatePartsThroughProperties()
        {
            DataFormApp_LayoutProperties dataFormApp = new DataFormApp_LayoutProperties();
            dataFormApp.dataForm.FontFamily = new FontFamily("Arial");
            dataFormApp.dataForm.FontSize = 15;
            this.DataFormAppBase = dataFormApp;
            dataFormApp.dataForm.ItemsSource = DataClassList.GetDataClassList(3, ListOperations.All);

            this.AddToPanelAndWaitForLoad();

            this.EnqueueCallback(() =>
            {
                ButtonBase newItemButton = this.GetTemplatePart<ButtonBase>("NewItemButton");
                ButtonBase deleteItemButton = this.GetTemplatePart<ButtonBase>("DeleteItemButton");
                ButtonBase cancelButton = this.GetTemplatePart<ButtonBase>("CancelButton");
                ButtonBase editButton = this.GetTemplatePart<ButtonBase>("EditButton");
                ButtonBase commitButton = this.GetTemplatePart<ButtonBase>("CommitButton");
                ContentControl headerElement = this.GetTemplatePart<ContentControl>("HeaderElement");

                Assert.AreEqual(Visibility.Collapsed, newItemButton.Visibility);
                Assert.AreEqual(Visibility.Visible, deleteItemButton.Visibility);
                Assert.AreEqual(Visibility.Collapsed, editButton.Visibility);
                Assert.AreEqual("Cancel button", cancelButton.Content);
                Assert.AreEqual("Commit button", commitButton.Content);
                Assert.AreEqual(15, headerElement.FontSize);
                Assert.AreEqual("Arial", headerElement.FontFamily.Source);

                Assert.IsNotNull(dataFormApp.dataForm.ValidationSummary);
                Assert.IsNotNull(dataFormApp.dataForm.ValidationSummary.Style);
                Assert.AreEqual(20, dataFormApp.dataForm.ValidationSummary.FontSize);
            });

            this.EnqueueTestComplete();
        }

        #endregion Template Parts Through Properties

        #region Helper Methods

        /// <summary>
        /// Expect a template part.
        /// </summary>
        /// <param name="partName">The name of the template part.</param>
        /// <typeparam name="T">The type of the template part to expect.</typeparam>
        private void ExpectTemplatePart<T>(string partName)
            where T : DependencyObject
        {
            this.AddToPanelAndWaitForLoad();
            this.EnqueueCallback(() =>
            {
                T part = this.GetTemplatePart<T>(partName);
                Assert.IsNotNull(part);
            });
            this.EnqueueTestComplete();
        }

        #endregion Helper Methods
    }
}
