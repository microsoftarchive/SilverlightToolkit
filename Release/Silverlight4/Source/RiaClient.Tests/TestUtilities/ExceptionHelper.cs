// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Data.Test.Utilities
{
    using System;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.ComponentModel.DataAnnotations;

#if !SILVERLIGHT
    using System.Web;
#endif
    using System.Net;

    public delegate void GenericDelegate();

    public static class ExceptionHelper {
        private static TException ExpectExceptionHelper<TException>(GenericDelegate del) where TException : Exception {
            return ExpectExceptionHelper<TException>(del, false);
        }

        private static TException ExpectExceptionHelper<TException>(GenericDelegate del, bool allowDerivedExceptions)
            where TException : Exception {
                try
                {
                    del();
                    Assert.Fail("Expected exception of type " + typeof(TException) + ".");
                    throw new Exception("can't happen");
                }
                catch (TException e)
                {
                    if (!allowDerivedExceptions)
                    {
                        Assert.AreEqual(typeof(TException), e.GetType());
                    }
                    return e;
                }
                catch (TargetInvocationException e)
                {
                    TException te = e.InnerException as TException;
                    if (te == null)
                    {
                        // Rethrow if it's not the right type
                        throw;
                    }
                    if (!allowDerivedExceptions)
                    {
                        Assert.AreEqual(typeof(TException), te.GetType());
                    }
                    return te;
                }
                // HACK: (ron) Starting with VS 2008 SP1, I found the "catch (TException)" is not always catching the expected type
                // when running under the VS debugger.  Added what looks like redundant code below, since that's the catch block
                // that's actually receiving the TException thrown.
                catch (Exception ex)
                {
                    TException te = ex as TException;
                    if (te != null)
                    {
                        if (!allowDerivedExceptions)
                        {
                            Assert.AreEqual(typeof(TException), ex.GetType());
                        }
                    }
                    else{
                        Type tExpected = typeof(TException);
                        Type tActual = ex.GetType();
                        Assert.Fail("Expected " + tExpected.Name + " but caught " + tActual.Name);
                    }
                    return te;
                }
        }

        public static TException ExpectException<TException>(GenericDelegate del) where TException : Exception {
            return ExpectException<TException>(del, false);
        }

        public static TException ExpectException<TException>(GenericDelegate del, bool allowDerivedExceptions)
            where TException : Exception {
            if (typeof(ArgumentNullException).IsAssignableFrom(typeof(TException))) {
                throw new InvalidOperationException(
                    "ExpectException<TException>() cannot be used with exceptions of type 'ArgumentNullException'. " +
                    "Use ExpectArgumentNullException() instead.");
            }
            else if (typeof(ArgumentException).IsAssignableFrom(typeof(TException))) {
                throw new InvalidOperationException(
                    "ExpectException<TException>() cannot be used with exceptions of type 'ArgumentException'. " +
                    "Use ExpectArgumentException() instead.");
            }
            return ExpectExceptionHelper<TException>(del, allowDerivedExceptions);
        }

        public static TException ExpectException<TException>(GenericDelegate del, string exceptionMessage)
                                                       where TException : Exception {
            TException e = ExpectException<TException>(del);
            // Only check exception message on English build and OS, since some exception messages come from the OS
            // and will be in the native language.
            if (UnitTestHelper.EnglishBuildAndOS) {
                Assert.AreEqual(exceptionMessage, e.Message, "Incorrect exception message.");
            }
            return e;
        }

        public static ArgumentException ExpectArgumentException(GenericDelegate del, string exceptionMessage) {
            ArgumentException e = ExpectExceptionHelper<ArgumentException>(del);
            // Only check exception message on English build and OS, since some exception messages come from the OS
            // and will be in the native language.
            if (UnitTestHelper.EnglishBuildAndOS) {
                Assert.AreEqual(exceptionMessage, e.Message, "Incorrect exception message.");
            }
            return e;
        }

        public static ArgumentException ExpectArgumentException(GenericDelegate del, string exceptionMessage, string paramName)
        {
            return ExpectArgumentException(del, exceptionMessage + "\r\nParameter name: " + paramName);
        }

        public static ArgumentException ExpectArgumentExceptionNullOrEmpty(GenericDelegate del, string paramName) {
            return ExpectArgumentException(del, "Value cannot be null or empty.\r\nParameter name: " + paramName);
        }

        public static ArgumentNullException ExpectArgumentNullExceptionStandard(GenericDelegate del, string paramName)
        {
            ArgumentNullException e = ExpectExceptionHelper<ArgumentNullException>(del);
            Assert.AreEqual("Value cannot be null.\r\nParameter name: " + paramName, e.Message);
            return e;
        }

        public static ArgumentNullException ExpectArgumentNullException(GenericDelegate del, string paramName) {
            ArgumentNullException e = ExpectExceptionHelper<ArgumentNullException>(del);
#if !SILVERLIGHT
            Assert.AreEqual(paramName, e.ParamName, "Incorrect exception parameter name.");
#endif
            return e;
        }

        public static ArgumentOutOfRangeException ExpectArgumentOutOfRangeException(GenericDelegate del, string paramName, string exceptionMessage) {
            ArgumentOutOfRangeException e = ExpectExceptionHelper<ArgumentOutOfRangeException>(del);
#if !SILVERLIGHT
            Assert.AreEqual(paramName, e.ParamName, "Incorrect exception parameter name.");
#endif
            // Only check exception message on English build and OS, since some exception messages come from the OS
            // and will be in the native language.
            if (exceptionMessage != null && UnitTestHelper.EnglishBuildAndOS) {
                Assert.AreEqual(exceptionMessage, e.Message, "Incorrect exception message.");
            }
            return e;
        }

        public static InvalidOperationException ExpectInvalidOperationException(GenericDelegate del, string message)
        {
            InvalidOperationException e = ExpectExceptionHelper<InvalidOperationException>(del);
            Assert.AreEqual(message, e.Message);
            return e;
        }

        public static ValidationException ExpectValidationException(GenericDelegate del, string message, Type validationAttributeType, object value)
        {
            ValidationException e = ExpectExceptionHelper<ValidationException>(del);
            Assert.AreEqual(message, e.Message);
            Assert.AreEqual(value, e.Value);

            if (validationAttributeType != null) {
                Assert.IsNotNull(e.ValidationAttribute);
                Assert.AreEqual(validationAttributeType, e.ValidationAttribute.GetType());
            } else {
                Assert.IsNull(e.ValidationAttribute);
            }

            return e;
        }

#if !SILVERLIGHT
        public static HttpException ExpectHttpException(GenericDelegate del, string message, int errorCode)
        {
            HttpException e = ExpectExceptionHelper<HttpException>(del);
            Assert.AreEqual(message, e.Message);
            Assert.AreEqual(errorCode, e.GetHttpCode());
            return e;
        }
#endif

        public static WebException ExpectWebException(GenericDelegate del, string message, int errorCode)
        {
            WebException e = ExpectExceptionHelper<WebException>(del);
            Assert.AreEqual(message, e.Message);

            HttpWebResponse response = e.Response as HttpWebResponse;
            Assert.AreEqual(errorCode, response.StatusCode);
            return e;
        }

    }
}
