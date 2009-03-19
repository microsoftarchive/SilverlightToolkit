//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace System.Windows.Controls
{
    /// <summary>
    /// Utility class that contains static methods for DataForm validation purposes.
    /// </summary>
    internal static class ValidationUtil
    {
        /// <summary>
        /// Creates a new Binding that is a shallow copy of the source Binding.
        /// </summary>
        /// <param name="source">The Binding to copy.</param>
        /// <returns>The copied Binding.</returns>
        public static Binding CopyBinding(Binding source)
        {
            Binding copy = new Binding();

            if (source == null)
            {
                return copy;
            }

            copy.Converter = source.Converter;
            copy.ConverterCulture = source.ConverterCulture;
            copy.ConverterParameter = source.ConverterParameter;
            copy.Mode = source.Mode;
            copy.NotifyOnValidationError = source.NotifyOnValidationError;
            copy.Path = source.Path;
            copy.Source = source.Source;
            copy.UpdateSourceTrigger = source.UpdateSourceTrigger;
            copy.ValidatesOnExceptions = source.ValidatesOnExceptions;

            return copy;
        }

        /// <summary>
        /// Returns whether or not a DependencyObject has errors.
        /// </summary>
        /// <param name="element">The element to test.</param>
        /// <returns>Whether or not it has errors.</returns>
        public static bool ElementHasErrors(DependencyObject element)
        {
            if (element == null)
            {
                return false;
            }

            if (Validation.GetHasError(element))
            {
                return true;
            }
            else
            {
                int childrenCount = VisualTreeHelper.GetChildrenCount(element);

                for (int i = 0; i < childrenCount; i++)
                {
                    DependencyObject childElement = VisualTreeHelper.GetChild(element, i);

                    DataForm childDataForm = childElement as DataForm;

                    // If we've found a child DataForm, validate it instead of continuing.
                    if (childDataForm != null)
                    {
                        childDataForm.ValidateItem();

                        if (!childDataForm.IsItemValid)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (ElementHasErrors(childElement))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the list of binding expressions for the given element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The list of binding expressions.</returns>
        public static IList<BindingExpression> GetBindingExpressionsForElement(FrameworkElement element)
        {
            List<BindingExpression> bindingExpressions = new List<BindingExpression>();

            if (element == null)
            {
                return bindingExpressions;
            }

            List<DependencyProperty> dependencyProperties = GetDependencyPropertiesForElement(element);
            Debug.Assert(dependencyProperties != null, "GetDependencyPropertiesForElement() should never return null.");

            foreach (DependencyProperty dependencyProperty in dependencyProperties)
            {
                if (dependencyProperty != null)
                {
                    BindingExpression bindingExpression = element.GetBindingExpression(dependencyProperty);

                    if (bindingExpression != null &&
                        bindingExpression.ParentBinding != null &&
                        bindingExpression.ParentBinding.Path != null &&
                        !string.IsNullOrEmpty(bindingExpression.ParentBinding.Path.Path) &&
                        bindingExpression.ParentBinding.Mode == BindingMode.TwoWay)
                    {
                        bindingExpressions.Add(bindingExpression);
                    }
                }
            }

            int childrenCount = VisualTreeHelper.GetChildrenCount(element);

            for (int i = 0; i < childrenCount; i++)
            {
                FrameworkElement childElement = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                // Stop if we've found a child DataForm.
                if (childElement != null && childElement.GetType() != typeof(DataForm))
                {
                    bindingExpressions.AddRange(GetBindingExpressionsForElement(childElement));
                }
            }

            return bindingExpressions;
        }

        /// <summary>
        /// Gets the list of dependency properties for the given element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The list of dependency properties.</returns>
        public static List<DependencyProperty> GetDependencyPropertiesForElement(FrameworkElement element)
        {
            List<DependencyProperty> dependencyProperties = new List<DependencyProperty>();

            if (element == null)
            {
                return dependencyProperties;
            }

            bool isBlocklisted =
                element is Panel || element is Button || element is Image || element is ScrollViewer || element is TextBlock ||
                element is Border || element is Shape || element is ContentPresenter || element is RangeBase;

            if (!isBlocklisted)
            {
                FieldInfo[] fields = element.GetType().GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

                if (fields != null)
                {
                    foreach (FieldInfo field in fields)
                    {
                        if (field.FieldType == typeof(DependencyProperty))
                        {
                            dependencyProperties.Add((DependencyProperty)field.GetValue(null));
                        }
                    }
                }
            }

            return dependencyProperties;
        }

        /// <summary>
        /// Updates the source on the bindings for a given element.
        /// </summary>
        /// <param name="element">The element.</param>
        public static void UpdateSourceOnElementBindings(FrameworkElement element)
        {
            List<DependencyProperty> dependencyProperties = GetDependencyPropertiesForElement(element);
            Debug.Assert(dependencyProperties != null, "GetDependencyPropertiesForElement() should never return null.");

            foreach (DependencyProperty dependencyProperty in dependencyProperties)
            {
                if (dependencyProperty != null)
                {
                    BindingExpression bindingExpression = element.GetBindingExpression(dependencyProperty);

                    if (bindingExpression != null)
                    {
                        bindingExpression.UpdateSource();
                    }
                }
            }
        }
    }
}
