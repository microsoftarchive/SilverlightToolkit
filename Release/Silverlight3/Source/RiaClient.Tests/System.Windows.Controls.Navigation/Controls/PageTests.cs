//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Markup;

namespace System.Windows.Controls.UnitTests
{
    [TestClass]
    public class PageTests : SilverlightTest
    {
        #region Properties

        private Page Page { get; set; }

        #endregion

        #region Methods

        [TestInitialize]
        public void TestInitialize()
        {
            this.Page = new Page();
        }

        [TestMethod]
        public void DefaultProperties()
        {
            Assert.IsNull(this.Page.Title);
            Assert.IsNull(this.Page.NavigationContext);
        }

        #endregion
    }
}
