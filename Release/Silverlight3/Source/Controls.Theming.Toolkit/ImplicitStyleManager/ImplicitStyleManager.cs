// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Resources;
using System.Xml;

[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Controls.Theming.ImplicitStyleManager", Justification = "Implicit styling is built into WPF.")]

namespace System.Windows.Controls.Theming
{
    /// <summary>
    /// Encapsulates an attached behavior that propagates styles in a framework 
    /// element's associated resource dictionary to its children.  The class 
    /// also provides attached properties that allow the resource dictionary to 
    /// be loaded from an external source.  Hierarchical styling is supported in
    /// a way similar to WPF.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public static class ImplicitStyleManager
    {
        #region private attached bool HasBeenStyled
        /// <summary>
        /// Gets the value of the HasBeenStyled attached property for a specified FrameworkElement.
        /// </summary>
        /// <param name="element">The FrameworkElement from which the property value is read.</param>
        /// <returns>The HasBeenStyled property value for the FrameworkElement.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Type of attached property is FrameworkElement.")]
        private static bool GetHasBeenStyled(FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (bool) element.GetValue(HasBeenStyledProperty);
        }

        /// <summary>
        /// Sets the value of the HasBeenStyled attached property to a specified FrameworkElement.
        /// </summary>
        /// <param name="element">The FrameworkElement to which the attached property is written.</param>
        /// <param name="value">The needed HasBeenStyled value.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Type of attached property is FrameworkElement.")]
        private static void SetHasBeenStyled(FrameworkElement element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(HasBeenStyledProperty, value);
        }

        /// <summary>
        /// Identifies the HasBeenStyled dependency property.
        /// </summary>
        private static readonly DependencyProperty HasBeenStyledProperty =
            DependencyProperty.RegisterAttached(
                "HasBeenStyled",
                typeof(bool),
                typeof(ImplicitStyleManager), 
                null);

        #endregion private attached bool HasBeenStyled

        #region public attached ImplicitStylesApplyMode ApplyMode
        /// <summary>
        /// Gets the value of the ImplicitStyleManager.ApplyMode attached property 
        /// for a specified framework element. 
        /// The value indicates whether the style implicitly applied on descendent
        /// framework elements.
        /// </summary>
        /// <param name="element">The FrameworkElement from which the property value is read.</param>
        /// <returns>The ApplyMode property value for the FrameworkElement.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Type of attached property is FrameworkElement.")]
        public static ImplicitStylesApplyMode GetApplyMode(FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (ImplicitStylesApplyMode) element.GetValue(ApplyModeProperty);
        }

        /// <summary>
        /// Sets the value of the ImplicitStyleManager.ApplyMode attached
        /// property on a specified framework element. 
        /// This value indicates whether the style should be implicitly applied
        /// on descendent framework elements.
        /// </summary>
        /// <param name="element">The FrameworkElement to which the attached 
        /// property is written.</param>
        /// <param name="value">The ApplyMode value.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Type of attached property is FrameworkElement.")]
        public static void SetApplyMode(FrameworkElement element, ImplicitStylesApplyMode value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(ApplyModeProperty, value);
        }

        /// <summary>
        /// Identifies the ApplyMode dependency property.
        /// </summary>
        public static readonly DependencyProperty ApplyModeProperty =
            DependencyProperty.RegisterAttached(
                "ApplyMode",
                typeof(ImplicitStylesApplyMode),
                typeof(ImplicitStyleManager),
                new PropertyMetadata(ImplicitStylesApplyMode.None, OnApplyModePropertyChanged));

        /// <summary>
        /// ApplyModeProperty property changed handler.
        /// </summary>
        /// <param name="dependencyObject">FrameworkElement that changed its 
        /// ApplyMode.</param>
        /// <param name="eventArgs">Event arguments.</param>
        private static void OnApplyModePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            FrameworkElement element = dependencyObject as FrameworkElement;
            if (element == null)
            {
                throw new ArgumentNullException("dependencyObject");
            }

            ImplicitStylesApplyMode oldMode = (ImplicitStylesApplyMode)eventArgs.OldValue;
            ImplicitStylesApplyMode newMode = (ImplicitStylesApplyMode)eventArgs.NewValue;

            ImplicitStyleManager.SetHasBeenStyled(element, false);

            EventHandler eventHandler = ImplicitStyleManager.GetLayoutUpdatedHandler(element);

            // If element is automatically styled (once or always) attach event 
            // handler.
            if ((newMode == ImplicitStylesApplyMode.Auto || newMode == ImplicitStylesApplyMode.OneTime)
                && oldMode == ImplicitStylesApplyMode.None)
            {
                if (eventHandler == null)
                {
                    eventHandler =
                        (sender, args) =>
                        {
                            ImplicitStyleManager.PropagateStyles(element, false);
                        };

                    ImplicitStyleManager.SetLayoutUpdatedHandler(element, eventHandler);
                    element.LayoutUpdated += eventHandler;
                }
            }
            else if ((oldMode == ImplicitStylesApplyMode.Auto || oldMode == ImplicitStylesApplyMode.OneTime)
                && newMode == ImplicitStylesApplyMode.None)
            {
                if (eventHandler != null)
                {
                    element.LayoutUpdated -= eventHandler;
                    ImplicitStyleManager.SetLayoutUpdatedHandler(element, null);
                }
            }
        }
        #endregion public attached ImplicitStylesApplyMode ApplyMode

        #region private attached ResourceDictionary ExternalResourceDictionary
        /// <summary>
        /// Gets the value of the ExternalResourceDictionary attached property 
        /// for a specified FrameworkElement.
        /// </summary>
        /// <param name="element">The FrameworkElement from which the property 
        /// value is read.</param>
        /// <returns>The ExternalResourceDictionary property value for the 
        /// FrameworkElement.</returns>
        private static ResourceDictionary GetExternalResourceDictionary(FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return element.GetValue(ExternalResourceDictionaryProperty) as ResourceDictionary;
        }

        /// <summary>
        /// Sets the value of the ExternalResourceDictionary attached property 
        /// to a specified FrameworkElement.
        /// </summary>
        /// <param name="element">The FrameworkElement to which the attached 
        /// property is written.</param>
        /// <param name="value">The needed ExternalResourceDictionary value.
        /// </param>
        internal static void SetExternalResourceDictionary(FrameworkElement element, ResourceDictionary value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(ExternalResourceDictionaryProperty, value);
        }

        /// <summary>
        /// Identifies the ExternalResourceDictionary dependency property.
        /// </summary>
        private static readonly DependencyProperty ExternalResourceDictionaryProperty =
            DependencyProperty.RegisterAttached(
                "ExternalResourceDictionary",
                typeof(ResourceDictionary),
                typeof(ImplicitStyleManager),
                new PropertyMetadata(null));

        #endregion private attached ResourceDictionary ExternalResourceDictionary

        #region private attached EventHandler LayoutUpdatedHandler
        /// <summary>
        /// Gets the value of the LayoutUpdatedHandler attached property for a 
        /// specified FrameworkElement.
        /// </summary>
        /// <param name="element">The FrameworkElement from which the property 
        /// value is read.</param>
        /// <returns>The LayoutUpdatedHandler property value for the 
        /// FrameworkElement.</returns>
        private static EventHandler GetLayoutUpdatedHandler(FrameworkElement element)
        {
            Debug.Assert(element != null, "element cannot be null.");
            return element.GetValue(LayoutUpdatedHandlerProperty) as EventHandler;
        }

        /// <summary>
        /// Sets the value of the LayoutUpdatedHandler attached property to a 
        /// specified FrameworkElement.
        /// </summary>
        /// <param name="element">The FrameworkElement to which the attached 
        /// property is written.</param>
        /// <param name="value">The needed LayoutUpdatedHandler value.</param>
        private static void SetLayoutUpdatedHandler(FrameworkElement element, EventHandler value)
        {
            Debug.Assert(element != null, "element cannot be null.");
            element.SetValue(LayoutUpdatedHandlerProperty, value);
        }

        /// <summary>
        /// Identifies the LayoutUpdatedHandler dependency property.
        /// </summary>
        private static readonly DependencyProperty LayoutUpdatedHandlerProperty =
            DependencyProperty.RegisterAttached(
                "LayoutUpdatedHandler",
                typeof(EventHandler),
                typeof(ImplicitStyleManager),
                new PropertyMetadata(null));

        #endregion private attached EventHandler LayoutUpdatedHandler

        #region public attached Uri ResourceDictionaryUri
        /// <summary>
        /// Gets the value of the ImplicitStyleManager.ResourceDictionaryUri 
        /// attached property for a specified framework element.
        /// The value indicates the URI address for the resource dictionary that 
        /// is implicitly applied.
        /// </summary>
        /// <param name="element">The FrameworkElement from which the property 
        /// value is read.</param>
        /// <returns>The ResourceDictionaryUri property value for the 
        /// FrameworkElement.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Type of attached property is FrameworkElement.")]
        public static Uri GetResourceDictionaryUri(FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return element.GetValue(ResourceDictionaryUriProperty) as Uri;
        }

        /// <summary>
        /// Sets the value of the ImplicitStyleManager.ResourceDictionaryUri 
        /// attached property for a specified framework element. 
        /// The value indicates the URI address for the resource dictionary 
        /// that is implicitly applied.
        /// </summary>
        /// <param name="element">The FrameworkElement to which the attached 
        /// property is written.</param>
        /// <param name="value">The needed ResourceDictionaryUri value.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Type of attached property is FrameworkElement.")]
        public static void SetResourceDictionaryUri(FrameworkElement element, Uri value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(ResourceDictionaryUriProperty, value);
        }

        /// <summary>
        /// Identifies the ResourceDictionaryUri dependency property.
        /// </summary>
        public static readonly DependencyProperty ResourceDictionaryUriProperty =
            DependencyProperty.RegisterAttached(
                "ResourceDictionaryUri",
                typeof(Uri),
                typeof(ImplicitStyleManager),
                new PropertyMetadata(OnResourceDictionaryUriPropertyChanged));

        /// <summary>
        /// ResourceDictionaryUriProperty property changed handler.
        /// </summary>
        /// <param name="dependencyObject">FrameworkElement that changed its 
        /// ResourceDictionaryUri.</param>
        /// <param name="eventArgs">Event arguments.</param>
        private static void OnResourceDictionaryUriPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            FrameworkElement element = dependencyObject as FrameworkElement;
            if (element != null)
            {
                Uri value = (Uri) eventArgs.NewValue;
                ResourceDictionary resourceDictionary = null;
                if (value != null)
                {
                    try
                    {
                        resourceDictionary = Parse(value);
                        ImplicitStyleManager.SetExternalResourceDictionary(element, resourceDictionary);
                    }
                    catch (Exception)
                    {
                        element.ClearValue(ImplicitStyleManager.ExternalResourceDictionaryProperty);
                        throw;
                    }
                }
                else
                {
                    element.ClearValue(ImplicitStyleManager.ExternalResourceDictionaryProperty);
                }
            }
        }
        #endregion public attached Uri ResourceDictionaryUri

        #region Application Properties

        /// <summary>
        /// Gets or sets a value indicating whether to use application
        /// resources.
        /// </summary>
        private static bool _useApplicationResources = true;

        /// <summary>
        /// Gets or sets a value indicating whether the implicitly applied 
        /// styles are based on values from 
        /// ImplicitStyleManager.ApplicationResourceDictionaryUri.
        /// </summary>
        public static bool UseApplicationResources
        {
            get
            {
                return _useApplicationResources;
            }
            set
            {
                _useApplicationResources = value;
            }
        }

        /// <summary>
        /// A field that stores a resource dictionary uri which is used to 
        /// retrieve resources associated with the application.
        /// </summary>
        private static Uri _applicationResourceDictionaryUri;

        /// <summary>
        /// Gets or sets the ResourceDictionary URI that is implicitly applied 
        /// to all FrameworksElements in the Application.
        /// </summary>
        public static Uri ApplicationResourceDictionaryUri
        {
            set
            {
                if (value != null)
                {
                    try
                    {
                        ApplicationExternalResourceDictionary = Parse(value);
                        _applicationResourceDictionaryUri = value;
                    }
                    catch (Exception)
                    {
                        ApplicationExternalResourceDictionary = null;
                        throw;
                    }
                }
                else
                {
                    ApplicationExternalResourceDictionary = null;
                    _applicationResourceDictionaryUri = null;
                }
            }
            get
            {
                return _applicationResourceDictionaryUri;
            }
        }

        /// <summary>
        /// Gets or sets the external resource dictionary to use for the 
        /// application.
        /// </summary>
        private static ResourceDictionary ApplicationExternalResourceDictionary { get; set; }

        #endregion

        #region private attached Style Style
        /// <summary>
        /// Gets the value of the Style attached property for a specified FrameworkElement.
        /// </summary>
        /// <param name="element">The FrameworkElement from which the property value is read.</param>
        /// <returns>The Style property value for the FrameworkElement.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Type of attached property is FrameworkElement.")]
        private static Style GetStyle(FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return element.GetValue(StyleProperty) as Style;
        }

        /// <summary>
        /// Sets the value of the Style attached property to a specified FrameworkElement.
        /// </summary>
        /// <param name="element">The FrameworkElement to which the attached property is written.</param>
        /// <param name="value">The needed Style value.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Type of attached property is FrameworkElement.")]
        private static void SetStyle(FrameworkElement element, Style value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(StyleProperty, value);
        }

        /// <summary>
        /// Identifies the Style dependency property.
        /// </summary>
        private static readonly DependencyProperty StyleProperty =
            DependencyProperty.RegisterAttached(
                "Style",
                typeof(Style),
                typeof(ImplicitStyleManager),
                new PropertyMetadata(OnStylePropertyChanged));

        /// <summary>
        /// StyleProperty property changed handler.
        /// </summary>
        /// <param name="dependencyObject">FrameworkElement that changed its Style.</param>
        /// <param name="eventArgs">Event arguments.</param>
        private static void OnStylePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            FrameworkElement source = dependencyObject as FrameworkElement;
            if (source == null)
            {
                throw new ArgumentNullException("dependencyObject");
            }
            Style value = eventArgs.NewValue as Style;
            source.Style = value;
        }
        #endregion private attached Style Style

        /// <summary>
        /// Applies implicit styles to an element and its descendents.
        /// </summary>
        /// <param name="element">The element to apply implicit styles to.
        /// </param>
        public static void Apply(FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            PropagateStyles(element, true);
        }

        /// <summary>
        /// Retrieves or creates the merged style dictionary of an element.
        /// </summary>
        /// <param name="styledElement">A styled element.</param>
        /// <returns>The merged dictionary of the element.</returns>
        private static BaseMergedStyleDictionary GetMergedStyleDictionary(FrameworkElement styledElement)
        {
            Debug.Assert(styledElement != null, "styledElement cannot be null.");

            IList<FrameworkElement> ancestorsInclusive = styledElement.GetVisualAncestors().Append(styledElement).Reverse().ToList();

            IEnumerable<FrameworkElement> ancestorsTopToBottom = ancestorsInclusive;

            BaseMergedStyleDictionary initialDictionary = null;
            if (UseApplicationResources)
            {
                initialDictionary = new MergedStyleResourceDictionary(ApplicationExternalResourceDictionary ?? Application.Current.Resources);
            }
            else
            {
                FrameworkElement topLevelElement = ancestorsInclusive[0];
                initialDictionary = new MergedStyleResourceDictionary(ImplicitStyleManager.GetExternalResourceDictionary(topLevelElement) ?? topLevelElement.Resources);

                ancestorsTopToBottom = ancestorsInclusive.Skip(1);
            }

            BaseMergedStyleDictionary styleDictionary =
                ancestorsTopToBottom.Aggregate(
                    initialDictionary,
                    (dictionary, ancestor) => new MergedStyleResourceDictionary(ImplicitStyleManager.GetExternalResourceDictionary(ancestor) ?? ancestor.Resources) { Parent = dictionary });

            return styleDictionary;
        }

        /// <summary>
        /// Retrieves the style key for a given element by first checking to
        /// see if it can retrieve its default style key and then using the
        /// full name of its concrete type.
        /// </summary>
        /// <param name="element">The element to retrieve the style key for.
        /// </param>
        /// <returns>The style key.</returns>
        private static string GetStyleKey(FrameworkElement element)
        {
            Control elementControl = element as Control;
            if (elementControl != null)
            {
                Type type = DefaultStyleKeyRetriever.GetDefaultStyleKey(elementControl) as Type;
                if (type != null)
                {
                    return type.FullName;
                }
            }

            return element.GetType().FullName;
        }

        /// <summary>
        /// Retrieves a style collection from a uri of a resource
        /// dictionary.
        /// </summary>
        /// <param name="uri">The uri of a resource dictionary.</param>
        /// <returns>A style collection containing the styles in the resource
        /// dictionary.</returns>
        private static ResourceDictionary Parse(Uri uri)
        {
            Debug.Assert(uri != null, "uri cannot be null.");

            StreamResourceInfo info = Application.GetResourceStream(uri);
            if (info == null)
            {
                throw new ResourceNotFoundException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.ImplicitStyleManager_ResourceNotFound, uri));
            }
            else
            {
                try
                {
                    return ResourceParser.Parse(info.Stream, true);
                }
                catch (Exception exception)
                {
                    throw new InvalidResourceException(Properties.Resources.InvalidResourceException_UnableToLoadResources, exception);
                }
            }
        }

        /// <summary>
        /// This method propagates the styles in the resources associated with
        /// a framework element to its descendents. This results in a  
        /// style inheritance that mimics WPF's behavior.
        /// </summary>
        /// <param name="element">The element that will have its styles 
        /// propagated to its children.</param>
        /// <param name="recurse">Whether to recurse over styled elements that
        /// are set to OneTime and have already been styled.</param>
        private static void PropagateStyles(FrameworkElement element, bool recurse)
        {
            BaseMergedStyleDictionary initialDictionary = GetMergedStyleDictionary(element);

            // Create stream of elements and their base merged style 
            // dictionaries by traversing the logical tree.
            IEnumerable<Tuple<FrameworkElement, BaseMergedStyleDictionary>> elementsToStyleAndDictionaries =
                FunctionalProgramming.TraverseDepthFirst(
                    new Tuple<FrameworkElement, BaseMergedStyleDictionary>(element, initialDictionary),
                    (elementAndDictionary) => 
                        elementAndDictionary
                            .First
                            .GetLogicalChildrenDepthFirst()
                            .Select(childElement => 
                                new Tuple<FrameworkElement, BaseMergedStyleDictionary>(
                                    childElement, 
                                    new MergedStyleResourceDictionary(
                                        ImplicitStyleManager.GetExternalResourceDictionary(childElement) ?? childElement.Resources,
                                        elementAndDictionary.Second))),
                    (elementAndDictionary) => recurse ||
                        (ImplicitStyleManager.GetApplyMode(elementAndDictionary.First) != ImplicitStylesApplyMode.OneTime ||
                        !ImplicitStyleManager.GetHasBeenStyled(elementAndDictionary.First)));

            foreach (Tuple<FrameworkElement, BaseMergedStyleDictionary> elementToStyleAndDictionary in elementsToStyleAndDictionaries)
            {
                FrameworkElement elementToStyle = elementToStyleAndDictionary.First;
                BaseMergedStyleDictionary styleDictionary = elementToStyleAndDictionary.Second;

                bool styleApplied = false;

                if (elementToStyle.Style == null || GetStyle(elementToStyle) == elementToStyle.Style)
                {
                    Style style = styleDictionary[GetStyleKey(elementToStyle)];
                    if (style != null)
                    {
                        SetStyle(elementToStyle, style);
                        styleApplied = true;
                    }
                }
                if (ImplicitStyleManager.GetApplyMode(elementToStyle) == ImplicitStylesApplyMode.OneTime && (VisualTreeHelper.GetChildrenCount(elementToStyle) > 0 || styleApplied))
                {
                    ImplicitStyleManager.SetHasBeenStyled(elementToStyle, true);
                }
            }
        }
    }
}