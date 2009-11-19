// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#if !SILVERLIGHT
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class ScaffoldAttributeTest {

        [TestMethod]
        public void ArgumentConstructor() {
            Assert.AreEqual<bool>(true, new ScaffoldColumnAttribute(true).Scaffold);
            Assert.AreEqual<bool>(false, new ScaffoldColumnAttribute(false).Scaffold);
            Assert.AreEqual<bool>(true, new ScaffoldTableAttribute(true).Scaffold);
            Assert.AreEqual<bool>(false, new ScaffoldTableAttribute(false).Scaffold);
        }
    }
}
#endif
