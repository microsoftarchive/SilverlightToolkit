using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class FilterAttributeTest {
        [TestMethod]
        public void Order() {
            Assert.AreEqual(0, new FilterAttribute().Order);
        }

        [TestMethod]
        public void Enabled() {
            Assert.AreEqual(true, new FilterAttribute().Enabled);
        }

        [TestMethod]
        public void FilterControl() {
            Assert.IsNull(new FilterAttribute().FilterControl);
        }
    }
}
