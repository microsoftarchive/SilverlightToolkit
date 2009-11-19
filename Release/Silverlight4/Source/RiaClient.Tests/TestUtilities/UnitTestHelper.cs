// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Data.Test.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class UnitTestHelper {
        public static bool EnglishBuildAndOS {
            get {
                bool englishBuild = String.Equals(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, "en",
                    StringComparison.OrdinalIgnoreCase);
                bool englishOS = String.Equals(CultureInfo.CurrentCulture.TwoLetterISOLanguageName, "en",
                    StringComparison.OrdinalIgnoreCase);
                return englishBuild && englishOS;
            }
        }

        /// <summary>
        /// Helper method to assert a string item appears in a list of strings
        /// </summary>
        /// <param name="list">actual list to search</param>
        /// <param name="expected">expected string</param>
        public static void AssertListContains(IEnumerable<string> list, string expected)
        {
            foreach (string s in list)
            {
                if (s.Equals(expected))
                {
                    return;
                }
            }
            string result = "Expected <" + expected + "> in list, but actual list was:";
            foreach (string s in list)
                result += ("\r\n" + s);
            Assert.Fail(result);
        }

        /// <summary>
        /// Helper method to assert a string item appears in a list of strings
        /// </summary>
        /// <param name="list">actual list to search</param>
        /// <param name="expected">expected string</param>
        public static void AssertListContains(IEnumerable<ValidationResult> list, string expected)
        {
            foreach (ValidationResult v in list)
            {
                string s = v.ErrorMessage;
                if (s.Equals(expected))
                {
                    return;
                }
            }
            string result = "Expected <" + expected + "> in list, but actual list was:";
            foreach (ValidationResult v in list)
                result += ("\r\n" + v.ErrorMessage);
            Assert.Fail(result);
        }

        /// <summary>
        /// Helper method to assert at least an item satisfying the expected condition appear in the given list
        /// </summary>
        /// <typeparam name="T">type of items in list</typeparam>
        /// <param name="list">actual list to search</param>
        /// <param name="expected">filter condition for the search</param>
        public static void AssertListContains<T>(IEnumerable<T> list, Func<T, bool> expected)
        {
            if (list.Where(expected).Any())
            {
                return;
            }

            // list does not contain any items that satisfy the expected predicate
            Assert.Fail();
        }

#if !SILVERLIGHT
        /// <summary>
        /// Write the list of errors to console
        /// </summary>
        /// <param name="listName">descriptive name of the list</param>
        /// <param name="errors">error list</param>
        public static void LogErrorListContents(string listName, IEnumerable<System.Web.Ria.Data.OperationError> errors)
        {
            Console.WriteLine(string.Format("Contents of error list {0}:", listName));
            if (errors == null)
            {
                Console.WriteLine("<null>");
                return;
            }
            Console.WriteLine(string.Format("Count: {0}", errors.Count()));
            foreach (System.Web.Ria.Data.OperationError error in errors)
            {
                Console.WriteLine(string.Format(
                    "Item: Message={0}, ErrorCode={1}, StackTrace={2}",
                    error.Message,
                    error.ErrorCode,
                    error.StackTrace));
            }
            Console.WriteLine();
        }
#endif
    }
}
