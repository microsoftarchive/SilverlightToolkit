// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// A derived AutoCompleteBox class that exposes template parts to enable 
    /// testing.
    /// </summary>
    public class OverriddenAutoCompleteBox : AutoCompleteBox
    {
        /// <summary>
        /// Gets or sets the TextBox template part.
        /// </summary>
        public TextBox TextBox { get; set; }

        /// <summary>
        /// Initializes a new instance of the OverriddenAutoCompleteBox type.
        /// </summary>
        public OverriddenAutoCompleteBox()
        {
            FormatValueActions = new OverriddenMethod<object>();
            DropDownClosedActions = new OverriddenMethod<RoutedPropertyChangedEventArgs<bool>>();
            DropDownClosingActions = new OverriddenMethod<RoutedPropertyChangingEventArgs<bool>>();
            DropDownOpenedActions = new OverriddenMethod<RoutedPropertyChangedEventArgs<bool>>();
            DropDownOpeningActions = new OverriddenMethod<RoutedPropertyChangingEventArgs<bool>>();
            PopulatedActions = new OverriddenMethod<PopulatedEventArgs>();
            PopulatingActions = new OverriddenMethod<PopulatingEventArgs>();
            SelectionChangedActions = new OverriddenMethod<SelectionChangedEventArgs>();
            TextChangedActions = new OverriddenMethod<RoutedEventArgs>();
            GetSelectionAdapterActions = new OverriddenMethod();
        }

        /// <summary>
        /// Overrides the OnApplyTemplate method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            TextBox = GetTemplateChild("Text") as TextBox;
        }

        /// <summary>
        /// Gets test actions for the FormatValue method.
        /// </summary>
        public OverriddenMethod<object> FormatValueActions { get; private set; }

        /// <summary>
        /// Performs test actions for the FormatValue method.
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <returns>Formatted value.</returns>
        protected override string FormatValue(object value)
        {
            FormatValueActions.DoPreTest(value);
            string result = base.FormatValue(value);
            FormatValueActions.DoTest(value);
            return result;
        }

        /// <summary>
        /// Gets test actions for the OnDropDownClosed method.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for testing")]
        public OverriddenMethod<RoutedPropertyChangedEventArgs<bool>> DropDownClosedActions { get; private set; }

        /// <summary>
        /// Performs test actions for the OnDropDownClosed method.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnDropDownClosed(RoutedPropertyChangedEventArgs<bool> e)
        {
            DropDownClosedActions.DoPreTest(e);
            base.OnDropDownClosed(e);
            DropDownClosedActions.DoTest(e);
        }

        /// <summary>
        /// Gets test actions for the OnDropDownClosing method.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for testing")]
        public OverriddenMethod<RoutedPropertyChangingEventArgs<bool>> DropDownClosingActions { get; private set; }

        /// <summary>
        /// Performs test actions for the OnDropDownClosing method.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnDropDownClosing(RoutedPropertyChangingEventArgs<bool> e)
        {
            DropDownClosingActions.DoPreTest(e);
            base.OnDropDownClosing(e);
            DropDownClosingActions.DoTest(e);
        }

        /// <summary>
        /// Gets test actions for the OnDropDownOpened method.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for testing")]
        public OverriddenMethod<RoutedPropertyChangedEventArgs<bool>> DropDownOpenedActions { get; private set; }

        /// <summary>
        /// Performs test actions for the OnDropDownOpened method.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnDropDownOpened(RoutedPropertyChangedEventArgs<bool> e)
        {
            DropDownOpenedActions.DoPreTest(e);
            base.OnDropDownOpened(e);
            DropDownOpenedActions.DoTest(e);
        }

        /// <summary>
        /// Gets test actions for the OnDropDownOpening method.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for testing")]
        public OverriddenMethod<RoutedPropertyChangingEventArgs<bool>> DropDownOpeningActions { get; private set; }

        /// <summary>
        /// Performs test actions for the OnDropDownOpening method.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnDropDownOpening(RoutedPropertyChangingEventArgs<bool> e)
        {
            DropDownOpeningActions.DoPreTest(e);
            base.OnDropDownOpening(e);
            DropDownOpeningActions.DoTest(e);
        }

        /// <summary>
        /// Gets test actions for the OnPopulated method.
        /// </summary>
        public OverriddenMethod<PopulatedEventArgs> PopulatedActions { get; private set; }

        /// <summary>
        /// Performs test actions for the OnPopulated method.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnPopulated(PopulatedEventArgs e)
        {
            PopulatedActions.DoPreTest(e);
            base.OnPopulated(e);
            PopulatedActions.DoTest(e);
        }

        /// <summary>
        /// Gets test actions for the OnPopulating method.
        /// </summary>
        public OverriddenMethod<PopulatingEventArgs> PopulatingActions { get; private set; }

        /// <summary>
        /// Performs test actions for the OnPopulating method.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnPopulating(PopulatingEventArgs e)
        {
            PopulatingActions.DoPreTest(e);
            base.OnPopulating(e);
            PopulatingActions.DoTest(e);
        }

        /// <summary>
        /// Gets test actions for the OnSelectionChanged method.
        /// </summary>
        public OverriddenMethod<SelectionChangedEventArgs> SelectionChangedActions { get; private set; }

        /// <summary>
        /// Performs test actions for the OnSelectionChanged method.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            SelectionChangedActions.DoPreTest(e);
            base.OnSelectionChanged(e);
            SelectionChangedActions.DoTest(e);
        }

        /// <summary>
        /// Gets test actions for the OnTextChanged method.
        /// </summary>
        public OverriddenMethod<RoutedEventArgs> TextChangedActions { get; private set; }

        /// <summary>
        /// Performs test actions for the OnTextChanged method.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnTextChanged(RoutedEventArgs e)
        {
            TextChangedActions.DoPreTest(e);
            base.OnTextChanged(e);
            TextChangedActions.DoTest(e);
        }

        /// <summary>
        /// Gets test actions for the GetSelectionAdapter method.
        /// </summary>
        public OverriddenMethod GetSelectionAdapterActions { get; private set; }

        /// <summary>
        /// Overrides the SelectionAdapter template part get method to inject 
        /// the OverriddenSelectionAdapter type for Selector objects.
        /// </summary>
        /// <returns>Returns a new ISelectionAdapter instance or null.</returns>
        protected override ISelectionAdapter GetSelectionAdapterPart()
        {
            GetSelectionAdapterActions.DoPreTest();
            Selector selector = GetTemplateChild("Selector") as Selector;
            ISelectionAdapter result = selector != null ? new OverriddenSelectionAdapter(selector) : base.GetSelectionAdapterPart();
            GetSelectionAdapterActions.DoTest();
            return result;
        }
    }
}