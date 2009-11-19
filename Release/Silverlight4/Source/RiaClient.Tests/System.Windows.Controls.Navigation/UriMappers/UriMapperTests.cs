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
        [Description("UriMapper is a UriMapperBase")]
        public void Implementation()
        {
            Assert.IsInstanceOfType(this.UriMapper, typeof(UriMapperBase));
            Assert.IsInstanceOfType(this.UriMapper, typeof(UriMapper));
        }

        [TestMethod]
        [Description("A new UriMapper has an empty UriMappings, but does not error when attempting to process a Uri because of this.")]
        public void InitialState()
        {
            Assert.AreEqual<int>(0, this.UriMapper.UriMappings.Count);

            Uri originalUri = new Uri("abc", UriKind.Relative);

            Assert.AreEqual(originalUri, this.UriMapper.MapUri(originalUri));
        }

        [TestMethod]
        [Description("A null Uri is processed as an error.")]
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

        [TestMethod]
        [Description("Empty Uri maps to empty or a MappedUri correctly")]
        public void EmptyUriMapping()
        {
            Uri emptyUri = new Uri(String.Empty, UriKind.Relative);
            Uri mappedUri = new Uri("mapped", UriKind.Relative);

            UriMapping emptyUriToMappedUri = new UriMapping()
                {
                    Uri = new Uri(String.Empty, UriKind.Relative),
                    MappedUri = mappedUri
                };

            UriMapping nullUriToMappedUri = new UriMapping()
                {
                    Uri = null,
                    MappedUri = mappedUri
                };

            UriMapping otherMapping = new UriMapping()
                {
                    Uri = new Uri("abc", UriKind.Relative),
                    MappedUri = new Uri("other mapped", UriKind.Relative)
                };


            // Should be the emptyUri when there's no UriMappings present
            this.UriMapper.UriMappings.Clear();
            Assert.AreEqual(emptyUri, this.UriMapper.MapUri(emptyUri), "did not map to emptyUri with no UriMappings present");

            // Should be the emptyUri when there's a UriMapping present that does not map from Uri=""
            this.UriMapper.UriMappings.Clear();
            this.UriMapper.UriMappings.Add(otherMapping);
            Assert.AreEqual(emptyUri, this.UriMapper.MapUri(emptyUri), "did not map to emptyUri with a UriMapping present where Uri!=\"\"");

            // Should be the MappedUri when there's a UriMapping present that maps from Uri=""
            this.UriMapper.UriMappings.Clear();
            this.UriMapper.UriMappings.Add(emptyUriToMappedUri);
            Assert.AreEqual(mappedUri, this.UriMapper.MapUri(emptyUri), "did not map to mappedUri with UriMapping.Uri=\"\"");

            // Should be the MappedUri when there's a UriMapping present that maps from Uri=null
            this.UriMapper.UriMappings.Clear();
            this.UriMapper.UriMappings.Add(nullUriToMappedUri);
            Assert.AreEqual(mappedUri, this.UriMapper.MapUri(emptyUri), "did not map to mappedUri with UriMapping.Uri=null");

            // Should be the MappedUri when there's multiple UriMappings present, one of which maps from Uri=""
            this.UriMapper.UriMappings.Clear();
            this.UriMapper.UriMappings.Add(otherMapping);
            this.UriMapper.UriMappings.Add(emptyUriToMappedUri);
            Assert.AreEqual(mappedUri, this.UriMapper.MapUri(emptyUri), "did not map to mappedUri with UriMapping.Uri=\"\" and multiple UriMappings");

            // Should be the MappedUri when there's multiple UriMappings present, one of which maps from Uri=null
            this.UriMapper.UriMappings.Clear();
            this.UriMapper.UriMappings.Add(otherMapping);
            this.UriMapper.UriMappings.Add(nullUriToMappedUri);
            Assert.AreEqual(mappedUri, this.UriMapper.MapUri(emptyUri), "did not map to mappedUri with UriMapping.Uri=null and multiple UriMappings");
        }

        #endregion
    }
}
