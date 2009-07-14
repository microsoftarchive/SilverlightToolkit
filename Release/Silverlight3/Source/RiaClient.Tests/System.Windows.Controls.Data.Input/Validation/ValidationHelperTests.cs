//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Controls.Test;
using System.Windows.Data.Test.Utilities;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.UnitTests
{
    [TestClass]
    [Tag("Validation")]
    public class ValidationHelperTests : SilverlightControlTest
    {
        [TestMethod]
        [Description("Get and set the ValidationMetadata attached property.")]
        public void ValidationMetadata()
        {
            ValidationHelperTestPage page = new ValidationHelperTestPage();
            Assert.IsNull(ValidationHelper.GetValidationMetadata(page.nameContentControl));
            ValidationMetadata vmd = new ValidationMetadata();
            vmd.Caption = "ValidationMetadata";
            ValidationHelper.SetValidationMetadata(page.nameContentControl, vmd);
            Assert.AreEqual(vmd, ValidationHelper.GetValidationMetadata(page.nameContentControl));
            ValidationHelper.SetValidationMetadata(page.nameContentControl, null);
            Assert.IsNull(ValidationHelper.GetValidationMetadata(page.nameContentControl));
            ExceptionHelper.ExpectArgumentNullException(
                delegate { ValidationHelper.GetValidationMetadata(null); }, 
                "inputControl");
            ExceptionHelper.ExpectArgumentNullException(
                delegate { ValidationHelper.SetValidationMetadata(null, vmd); }, 
                "inputControl");
        }

        [TestMethod]
        [Description("Test metadata parsing")]
        public void ParseMetadata()
        {
            Customer c = new Customer();

            // Normal
            ValidationMetadata vmd = ValidationHelper.ParseMetadata("Name", c);
            Assert.AreEqual("Name", vmd.Caption);
            Assert.AreEqual("This is your first name.", vmd.Description);

            // DisplayAttribute appears first
            // Name is different than property name
            vmd = ValidationHelper.ParseMetadata("BirthDate", c);
            Assert.AreEqual("Birth date", vmd.Caption);
            Assert.AreEqual("This is the day you were born.", vmd.Description);

            // No Name specified
            vmd = ValidationHelper.ParseMetadata("Password", c);
            Assert.AreEqual("Password", vmd.Caption);
            Assert.AreEqual("This is your password", vmd.Description);

            // Empty name specified
            vmd = ValidationHelper.ParseMetadata("EmptyName", c);
            Assert.AreEqual("", vmd.Caption);
            Assert.AreEqual("This is your EmptyName", vmd.Description);

            // No description
            vmd = ValidationHelper.ParseMetadata("SecretAnswer", c);
            Assert.AreEqual("Secret Answer", vmd.Caption);
            Assert.AreEqual(null, vmd.Description);

            // No DisplayAttribute
            vmd = ValidationHelper.ParseMetadata("Warnings", c);
            Assert.AreEqual("Warnings", vmd.Caption);
            Assert.AreEqual(null, vmd.Description);

            // Only ShortName
            vmd = ValidationHelper.ParseMetadata("OnlyShortName", c);
            Assert.AreEqual("OnlyShortName", vmd.Caption);
            Assert.AreEqual(null, vmd.Description);

            // Invalid MemberName
            vmd = ValidationHelper.ParseMetadata("test", c);
            Assert.IsNull(vmd);
        }
    }
}
