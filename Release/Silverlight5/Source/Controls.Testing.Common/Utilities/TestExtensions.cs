// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Extension methods used to unit test controls and layout containers.
    /// </summary>
    public static partial class TestExtensions
    {
        /// <summary>
        /// Create a reference to the element that will be added to the testing
        /// surface and then removed when the reference is disposed.
        /// </summary>
        /// <param name="element">Element create the reference for.</param>
        /// <param name="test">Currently executing test class.</param>
        /// <returns>LiveReference to track the element.</returns>
        public static LiveReference CreateLiveReference(this UIElement element, TestBase test)
        {
            return new LiveReference(test, element);
        }

        /// <summary>
        /// Get the custom attributes of a specified member.
        /// </summary>
        /// <typeparam name="T">Type of the attribute.</typeparam>
        /// <param name="member">Member to check for attributes.</param>
        /// <returns>The custom attributes of a specified member.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By design")]
        public static IEnumerable<T> GetAttributes<T>(this MemberInfo member)
            where T : Attribute
        {
            // Note that we're explicitly ignoring any inherited attributes
            // because it's how the tools check for template, etc., attributes
            // so they can be replaced by those on a derived class.
            return (member == null) ?
                Enumerable.Empty<T>() :
                member.GetCustomAttributes(typeof(T), false).Cast<T>();
        }

        /// <summary>
        /// Get the template parts for a given type.
        /// </summary>
        /// <param name="type">Type to check for attributes.</param>
        /// <returns>
        /// Dictionary mapping the names of template parts to their types.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only types should have template parts.")]
        public static IDictionary<string, Type> GetTemplateParts(this Type type)
        {
            return GetAttributes<TemplatePartAttribute>(type).ToDictionary(a => a.Name, a => a.Type);
        }

        /// <summary>
        /// Get the visual states for a given type.
        /// </summary>
        /// <param name="type">Type to check for attributes.</param>
        /// <returns>
        /// Dictionary mapping visual state names to their groups.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only types should have visual states.")]
        public static IDictionary<string, string> GetVisualStates(this Type type)
        {
            return GetAttributes<TemplateVisualStateAttribute>(type).ToDictionary(a => a.Name, a => a.GroupName);
        }

        /// <summary>
        /// Get the style typed properties for a given type.
        /// </summary>
        /// <param name="type">Type to check for attributes.</param>
        /// <returns>
        /// Dictionary mapping style typed property names to the type of their
        /// style.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only types should have style typed properties.")]
        public static IDictionary<string, Type> GetStyleTypedProperties(this Type type)
        {
            return GetAttributes<StyleTypedPropertyAttribute>(type).ToDictionary(a => a.Property, a => a.StyleTargetType);
        }

        /// <summary>
        /// Assert that a string is null or empty.
        /// </summary>
        /// <param name="value">Value of the string.</param>
        public static void AssertIsNullOrEmpty(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                throw new AssertFailedException(string.Format(
                    CultureInfo.InvariantCulture,
                    "String is not null or empty.  Actual:{1}.",
                    value));
            }
        }

        /// <summary>
        /// Assert that the actual sequence matches the expected sequence.
        /// </summary>
        /// <typeparam name="T">Type of the sequences.</typeparam>
        /// <param name="expected">The expected sequence.</param>
        /// <param name="actual">The actual sequence.</param>
        /// <returns>
        /// A value indicating whether the sequences are equal.
        /// </returns>
        public static bool AreSequencesEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            if ((expected == null && actual != null) || (expected != null && actual == null))
            {
                return false;
            }
            else
            {
                using (IEnumerator<T> expectedEnumerator = expected.GetEnumerator())
                using (IEnumerator<T> actualEnumerator = actual.GetEnumerator())
                {
                    bool hasExpected;
                    bool hasActual;
                    do
                    {
                        hasExpected = expectedEnumerator.MoveNext();
                        hasActual = actualEnumerator.MoveNext();

                        if (hasExpected && hasActual && !object.Equals(expectedEnumerator.Current, actualEnumerator.Current))
                        {
                            return false;
                        }
                    }
                    while (hasExpected && hasActual);
                    
                    if (hasActual || hasExpected)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Assert that the actual sequence matches the expected sequence.
        /// </summary>
        /// <typeparam name="T">Type of the sequences.</typeparam>
        /// <param name="expected">The expected sequence.</param>
        /// <param name="actual">The actual sequence.</param>
        /// <param name="message">Assertion message.</param>
        /// <param name="args">Assertion message arguments.</param>
        public static void AssertSequencesEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, string message, params object[] args)
        {
            AssertSequencesEqual(expected, actual, e => e.ToString(), message, args);
        }

        /// <summary>
        /// Assert that the actual sequence matches the expected sequence.
        /// </summary>
        /// <typeparam name="T">Type of the sequences.</typeparam>
        /// <param name="expected">The expected sequence.</param>
        /// <param name="actual">The actual sequence.</param>
        /// <param name="elementToString">
        /// Function to convert an element of the sequence to a string.
        /// </param>
        /// <param name="message">Assertion message.</param>
        /// <param name="args">Assertion message arguments.</param>
        public static void AssertSequencesEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, Func<T, string> elementToString, string message, params object[] args)
        {
            if (!AreSequencesEqual(expected, actual))
            {
                message = string.Format(
                    CultureInfo.CurrentCulture,
                    "Expected: {0}, Actual: {1}.  {2}",
                    SequenceToString(expected, elementToString),
                    SequenceToString(actual, elementToString),
                    string.Format(CultureInfo.CurrentCulture, message, args));
                throw new AssertFailedException(message);
            }
        }

        /// <summary>
        /// Convert a sequence of elements to a string.
        /// </summary>
        /// <typeparam name="T">Type of elements in the sequence.</typeparam>
        /// <param name="sequence">The sequence of elements.</param>
        /// <param name="elementToString">
        /// Function to convert an element of the sequence to a string.
        /// </param>
        /// <returns>A string representation of the sequence.</returns>
        private static string SequenceToString<T>(IEnumerable<T> sequence, Func<T, string> elementToString)
        {
            if (sequence == null)
            {
                return "(null)";
            }

            StringBuilder builder = new StringBuilder();
            builder.Append("{");
            bool first = true;

            foreach (T element in sequence)
            {
                if (!first)
                {
                    builder.Append(", ");
                }
                first = false;

                builder.Append(elementToString(element));
            }

            builder.Append("}");
            return builder.ToString();
        }

        /// <summary>
        /// Determines if the specified brushes are equal.
        /// </summary>
        /// <param name="first">The first brush to compare.</param>
        /// <param name="second">The second brush to compare.</param>
        /// <returns>True if the brushes are equal, false otherwise.</returns>
        public static bool AreBrushesEqual(Brush first, Brush second)
        {
            // If the default comparison is true, that's good enough.
            if (object.Equals(first, second))
            {
                return true;
            }

            // Do a field by field comparison if they're not the same reference
            SolidColorBrush firstSolidColorBrush = first as SolidColorBrush;
            if (firstSolidColorBrush != null)
            {
                SolidColorBrush secondSolidColorBrush = second as SolidColorBrush;
                if (secondSolidColorBrush != null)
                {
                    return object.Equals(firstSolidColorBrush.Color, secondSolidColorBrush.Color) &&
                        firstSolidColorBrush.Opacity == secondSolidColorBrush.Opacity;
                }
            }
            else
            {
                LinearGradientBrush firstLinearGradientBrush = first as LinearGradientBrush;
                if (firstLinearGradientBrush != null)
                {
                    LinearGradientBrush secondLinearGradientBrush = second as LinearGradientBrush;
                    if (secondLinearGradientBrush != null)
                    {
                        if (object.Equals(firstLinearGradientBrush.StartPoint, secondLinearGradientBrush.StartPoint) &&
                            object.Equals(firstLinearGradientBrush.EndPoint, secondLinearGradientBrush.EndPoint) &&
                            firstLinearGradientBrush.ColorInterpolationMode == secondLinearGradientBrush.ColorInterpolationMode  &&
                            firstLinearGradientBrush.MappingMode == secondLinearGradientBrush.MappingMode  &&
                            firstLinearGradientBrush.Opacity == secondLinearGradientBrush.Opacity &&
                            firstLinearGradientBrush.SpreadMethod == secondLinearGradientBrush.SpreadMethod &&
                            firstLinearGradientBrush.GradientStops.Count == secondLinearGradientBrush.GradientStops.Count)
                        {
                            for (int i = 0; i < firstLinearGradientBrush.GradientStops.Count; i++)
                            {
                                GradientStop firstStop = firstLinearGradientBrush.GradientStops[i];
                                GradientStop secondStop = secondLinearGradientBrush.GradientStops[i];
                                if (firstStop.Offset != secondStop.Offset)
                                {
                                    return false;
                                }
                                if (!object.Equals(firstStop.Color, secondStop.Color))
                                {
                                    return false;
                                }
                            }

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Verifies that the specified brushes are equal.
        /// </summary>
        /// <param name="expected">
        /// The first brush to compare.  This is the brush the unit test
        /// expects.
        /// </param>
        /// <param name="actual">
        /// The second brush to compare.  This is the brush the unit test
        /// produced.
        /// </param>
        public static void AssertBrushesAreEqual(Brush expected, Brush actual)
        {
            AssertBrushesAreEqual(expected, actual, null, null);
        }

        /// <summary>
        /// Verifies that the specified brushes are equal.
        /// </summary>
        /// <param name="expected">
        /// The first brush to compare.  This is the brush the unit test
        /// expects.
        /// </param>
        /// <param name="actual">
        /// The second brush to compare.  This is the brush the unit test
        /// produced.
        /// </param>
        /// <param name="message">Exception message.</param>
        /// <param name="arguments">Exception message arguments.</param>
        public static void AssertBrushesAreEqual(Brush expected, Brush actual, string message, params object[] arguments)
        {
            if (!AreBrushesEqual(expected, actual))
            {
                if (!string.IsNullOrEmpty(message))
                {
                    message = string.Format(CultureInfo.InvariantCulture, "  " + message, arguments);
                }
                throw new AssertFailedException(string.Format(
                    CultureInfo.InvariantCulture,
                    "Brushes are not equal.  Expected:{0}.  Actual:{1}.{2}",
                    expected,
                    actual,
                    message));
            }
        }

        /// <summary>
        /// Returns the visual tree ancestors of an element.
        /// </summary>
        /// <param name="element">The descendent of the ancestors.</param>
        /// <returns>The visual tree ancestors of the element.</returns>
        public static IEnumerable<FrameworkElement> GetVisualAncestors(this FrameworkElement element)
        {
            if (element == null)
            {
                yield break;
            }

            DependencyObject parent = null;
            parent = VisualTreeHelper.GetParent(element);
            while (parent != null)
            {
                FrameworkElement frameworkElementParent = parent as FrameworkElement;
                if (frameworkElementParent != null)
                {
                    yield return frameworkElementParent;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
        }

        /// <summary>
        /// Use VisualTreeHelper to enumerate the descendants of a dependency
        /// object.
        /// </summary>
        /// <param name="parent">Dependency object.</param>
        /// <returns>Sequence of the object's visual children.</returns>
        public static IEnumerable<DependencyObject> GetVisualDescendents(this DependencyObject parent)
        {
            if (parent == null)
            {
                yield break;
            }

            // Use a queue to recurse over the descendants
            Queue<DependencyObject> remaining = new Queue<DependencyObject>();
            remaining.Enqueue(parent);
            while (remaining.Count > 0)
            {
                DependencyObject obj = remaining.Dequeue();

                // We included the parent in the queue to initialize the
                // recursion, but we shouldn't return it
                if (obj != parent)
                {
                    yield return obj;
                }

                // Add the children of the object to the queue
                int count = VisualTreeHelper.GetChildrenCount(obj);
                for (int i = 0; i < count; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child != null)
                    {
                        remaining.Enqueue(child);
                    }
                }
            }
        }

        /// <summary>
        /// Use VisualTreeHelper to find a named descendent of a dependency
        /// object.
        /// </summary>
        /// <param name="parent">Dependency object.</param>
        /// <param name="name">Name of the visual child.</param>
        /// <returns>Desired visual child.</returns>
        /// <remarks>
        /// Note: This only returns the first visual child with the desired
        /// name.  It's possible to have multiple children with the same names
        /// in different namescopes.
        /// </remarks>
        public static DependencyObject GetVisualChild(this DependencyObject parent, string name)
        {
            foreach (DependencyObject obj in parent.GetVisualDescendents())
            {
                if (obj == null)
                {
                    continue;
                }

                if (string.CompareOrdinal((string) obj.GetValue(FrameworkElement.NameProperty), name) == 0)
                {
                    return obj;
                }
            }

            return null;
        }

        /// <summary>
        /// This helper method is designed to find the first child element that
        /// has a Type of type, using VisualTreeHelper.
        /// </summary>
        /// <param name="root">Root is the UI element root.</param>
        /// <param name="type">
        /// Type is the specified type that the method needs to find from the
        /// root's children and return.
        /// </param>
        /// <returns>The child element that is type of "type".</returns>
        public static FrameworkElement GetChildrenByType(this FrameworkElement root, Type type)
        {
            if (root == null)
            {
                return null;
            }
            FrameworkElement element = root;
            if (element.GetType() == type)
            {
                return element;
            }
            else
            {
                int childCount = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < childCount; i++)
                {
                    FrameworkElement child = GetChildrenByType((FrameworkElement)VisualTreeHelper.GetChild(root, i), type);
                    if (child != null)
                    {
                        return child;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// A method to judge if two double values are close enough, that is, differ in less than 5 percent.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <returns>Either true or false.</returns>
        public static bool AreClose(double expected, double actual)
        {
            if (Math.Abs(expected - actual) / expected * 100 < 5)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove all instances of a specific test from the list of tests.
        /// </summary>
        /// <param name="tests">List of tests.</param>
        /// <param name="test">Type of tests to remove.</param>
        public static void RemoveTests(this ICollection<DependencyPropertyTestMethod> tests, DependencyPropertyTestMethod test)
        {
            Assert.IsNotNull(tests, "List of tests should not be null!");
            Assert.IsNotNull(test, "Test should not be null!");

            // Get the tests to remove
            DependencyPropertyTestMethod[] remove =
                (from t in tests
                 where t.Method == test.Method &&
                    string.Compare(t.PropertyName, test.PropertyName, StringComparison.Ordinal) == 0
                 select t).ToArray();
            
            // Remove the tests
            foreach (DependencyPropertyTestMethod t in remove)
            {
                tests.Remove(t);
            }
        }
    }
}