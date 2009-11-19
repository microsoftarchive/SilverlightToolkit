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
    //

    /// <summary>
    /// Unit tests for the JournalEntry class.
    /// </summary>
    [TestClass]
    public class JournalEntryTests : SilverlightTest
    {
        #region Methods

        private static string GenerateGuidString()
        {
            return Guid.NewGuid().ToString();
        }

        [TestMethod]
        [Description("Verifies default behavior of the JournalEntry constructor.")]
        public void Constructor()
        {
            string name = GenerateGuidString();
            Uri uri = new Uri(GenerateGuidString(), UriKind.Relative);

            JournalEntry je = new JournalEntry(name, uri);

            Assert.AreEqual<string>(name, je.Name);
            Assert.AreEqual<Uri>(uri, je.Source);
        }

        [TestMethod]
        [Description("Uri may not be null")]
        public void UriNullThrows()
        {
            string name = GenerateGuidString();

            try
            {
                JournalEntry je = new JournalEntry(name, null);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                JournalEntry je = new JournalEntry(name, new Uri(GenerateGuidString(), UriKind.Relative));
                je.Source = null;
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
            }
        }

        #endregion
    }
}
