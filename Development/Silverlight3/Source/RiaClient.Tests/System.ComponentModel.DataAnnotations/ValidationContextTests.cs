using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;

namespace System.ComponentModel.DataAnnotations.Test
{
    [TestClass]
    public class ValidationContextTests
    {
        [TestMethod]
        [Description("Tests all legal ctors for ValidationContext")]
        public void ValidationContext_Ctors()
        {
            ValidationContext context = new ValidationContext(typeof(object), "m1");
            Assert.AreEqual(typeof(object), context.ObjectType);
            Assert.AreEqual("m1", context.MemberName);
            Assert.AreEqual("m1", context.DisplayName);
            Assert.IsNull(context.MethodInfo);

            context = new ValidationContext(typeof(object), "member", "display");
            Assert.AreEqual(typeof(object), context.ObjectType);
            Assert.AreEqual("member", context.MemberName);
            Assert.AreEqual("display", context.DisplayName);
            Assert.IsNull(context.MethodInfo);

            MethodInfo mInfo = typeof(object).GetMethod("GetHashCode");
            context = new ValidationContext(typeof(object), mInfo, "display", null);
            Assert.AreEqual(typeof(object), context.ObjectType);
            Assert.AreEqual("GetHashCode", context.MemberName);
            Assert.AreEqual("display", context.DisplayName);
            Assert.AreEqual(mInfo, context.MethodInfo);
        }

        [TestMethod]
        [Description("ValidationContext ctor fails if type is null")]
        public void ValidationContext_Fail_Null_Type()
        {
            ExceptionHelper.ExpectArgumentNullExceptionStandard(delegate
            {
                ValidationContext context = new ValidationContext(null, "m1");
            }, "objectType");
        }

        [TestMethod]
        [Description("ValidationContext ctor fails if methodInfo is null")]
        public void ValidationContext_Fail_Null_MethodInfo()
        {
            ExceptionHelper.ExpectArgumentNullExceptionStandard(delegate
            {
                ValidationContext context = new ValidationContext(typeof(object), (MethodInfo) null, "display", null);
            }, "methodInfo");
        }

        [TestMethod]
        [Description("ValidationContext ctor fails if member name is null")]
        public void ValidationContext_Fail_Null_MemberName()
        {
            ExceptionHelper.ExpectArgumentNullExceptionStandard(delegate
            {
                ValidationContext context = new ValidationContext(typeof(object), null);
            }, "memberName");

            ExceptionHelper.ExpectArgumentNullExceptionStandard(delegate
            {
                ValidationContext context = new ValidationContext(typeof(object), "");
            }, "memberName");
        }
    }
}
