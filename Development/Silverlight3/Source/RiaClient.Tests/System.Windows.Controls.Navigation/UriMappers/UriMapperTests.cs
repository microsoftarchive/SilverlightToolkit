//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Navigation.UnitTests
{
    [TestClass]
    public class UriMapperTests : SilverlightTest
    {
        #region Properties

        public UriMapper UriMapper { get; set; }

        #endregion

        #region Methods

        [TestInitialize]
        public void TestInitialize()
        {
            this.UriMapper = new UriMapper();
        }

        [TestMethod]
        [Description("Verifies that DefaultUriMapper is an IUriMapper")]
        public void Implementation()
        {
            Assert.IsInstanceOfType(this.UriMapper, typeof(UriMapperBase));
            Assert.IsInstanceOfType(this.UriMapper, typeof(UriMapper));
        }

        [TestMethod]
        [Description("Verifies that a new DefaultUriMapper has an empty UriMappings, but does not error when attempting to process a Uri because of this.")]
        public void InitialState()
        {
            Assert.AreEqual<int>(0, this.UriMapper.UriMappings.Count);

            Uri originalUri = new Uri("abc", UriKind.Relative);

            Assert.AreEqual(originalUri, this.UriMapper.MapUri(originalUri));
        }

        [TestMethod]
        [Description("Verifies that a null Uri is processed as an error.")]
        public void ProcessNullUriThrows()
        {
            try
            {
                this.UriMapper.MapUri(null);
            }
            catch (ArgumentNullException)
            {
                // Expected
            }
        }


        #endregion
    }
}
