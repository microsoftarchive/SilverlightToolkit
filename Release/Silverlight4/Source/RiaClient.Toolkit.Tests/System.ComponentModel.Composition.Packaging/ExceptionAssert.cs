// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.UnitTesting
{
    public class ExceptionAssert
    {
        public static T Throws<T>(Action action) where T : Exception
        {
            T exceptionThrew = null;
            try
            {
                action();
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(T), "Expected exception of type {0} but received {1}", typeof(T).Name, e.GetType().Name);
                exceptionThrew = (T)e;
            }

            Assert.IsNotNull(exceptionThrew, "Expected exception of type {0} but no exception was thrown", typeof(T).Name);

            return exceptionThrew;
        }

        public static void ThrowsArgumentNull(string parameterName, Action action)
        {
            var e = Throws<ArgumentNullException>(action);

            //Assert.AreEqual(parameterName, e.ParamName); //ParamName property doesn't exist on Silverlight
            Assert.IsTrue(e.Message.Contains(parameterName), "Expected ArgumentNullException for parameter {0}", parameterName);
        }

        public static void ThrowsDisposed(Action action)
        {
            Throws<ObjectDisposedException>(action);
        }
    }
}
