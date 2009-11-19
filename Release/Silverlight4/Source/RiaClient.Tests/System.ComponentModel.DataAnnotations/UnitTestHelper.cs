// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.ComponentModel.DataAnnotations.Test {
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
        public static void AssertListContains(IEnumerable<string> list, string expected) {
            foreach (string s in list) {
                if (s.Equals(expected)) {
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
        public static void AssertListContains(IEnumerable<ValidationResult> list, string expected) {
            foreach (ValidationResult v in list) {
                string s = v.ErrorMessage;
                if (s.Equals(expected)) {
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
        public static void AssertListContains<T>(IEnumerable<T> list, Func<T, bool> expected) {
            if (list.Where(expected).Any()) {
                return;
            }

            // list does not contain any items that satisfy the expected predicate
            Assert.Fail();
        }
    }
}
