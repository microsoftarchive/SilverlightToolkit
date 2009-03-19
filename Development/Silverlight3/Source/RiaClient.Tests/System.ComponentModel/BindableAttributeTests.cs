using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Windows.Data.Test.Utilities;
using System.Reflection;

namespace System.ComponentModel.DataAnnotations.Test
{
    [TestClass]
    public class BindableAttributeTests
    {
        [TestMethod]
        [Description("[Bindable] ctors")]
        public void BindableAttribute_Ctors()
        {
            BindableAttribute ba = null;

            ba = new BindableAttribute(true);
            Assert.AreEqual(true, ba.Bindable);
            Assert.AreEqual(BindingDirection.OneWay, ba.Direction);

            ba = new BindableAttribute(false);
            Assert.AreEqual(false, ba.Bindable);
            Assert.AreEqual(BindingDirection.OneWay, ba.Direction);

            ba = new BindableAttribute(true, BindingDirection.TwoWay);
            Assert.AreEqual(true, ba.Bindable);
            Assert.AreEqual(BindingDirection.TwoWay, ba.Direction);

            ba = new BindableAttribute(false, BindingDirection.TwoWay);
            Assert.AreEqual(false, ba.Bindable);
            Assert.AreEqual(BindingDirection.TwoWay, ba.Direction);
        }
    }
}
