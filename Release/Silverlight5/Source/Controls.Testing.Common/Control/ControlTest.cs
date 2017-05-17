// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Base class used to author unit tests for controls.
    /// </summary>
    public abstract partial class ControlTest : FrameworkElementTest
    {
        #region FrameworkElements to test
        /// <summary>
        /// Gets a default instance of FrameworkElement (or a derived type) to
        /// test.
        /// </summary>
        public override FrameworkElement DefaultFrameworkElementToTest
        {
            get { return DefaultControlToTest; }
        }

        /// <summary>
        /// Gets instances of FrameworkElement (or derived types) to test.
        /// </summary>
        public override IEnumerable<FrameworkElement> FrameworkElementsToTest
        {
            get { return ControlsToTest.OfType<FrameworkElement>(); }
        }

        /// <summary>
        /// Gets instances of IOverriddenFrameworkElement (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenFrameworkElement> OverriddenFrameworkElementsToTest
        {
            get { return OverriddenControlsToTest.OfType<IOverriddenFrameworkElement>(); }
        }
        #endregion FrameworkElements to test

        #region Controls to test
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public abstract Control DefaultControlToTest { get; }

        /// <summary>
        /// Gets instances of Control (or derived types) to test.
        /// </summary>
        public abstract IEnumerable<Control> ControlsToTest { get; }

        /// <summary>
        /// Gets instances of IOverriddenControl (or derived types) to test.
        /// </summary>
        public abstract IEnumerable<IOverriddenControl> OverriddenControlsToTest { get; }
        #endregion Controls to test

        /// <summary>
        /// Gets the Background dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Control, Brush> BackgroundProperty { get; private set; }

        /// <summary>
        /// Gets the BorderBrush dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Control, Brush> BorderBrushProperty { get; private set; }

        /// <summary>
        /// Gets the BorderThickness dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Control, Thickness> BorderThicknessProperty { get; private set; }

        /// <summary>
        /// Gets the FontFamily dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Control, FontFamily> FontFamilyProperty { get; private set; }

        /// <summary>
        /// Gets the FontSize dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Control, double> FontSizeProperty { get; private set; }

        /// <summary>
        /// Gets the FontStretch dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Control, FontStretch> FontStretchProperty { get; private set; }

        /// <summary>
        /// Gets the FontStyle dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Control, FontStyle> FontStyleProperty { get; private set; }

        /// <summary>
        /// Gets the FontWeight dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Control, FontWeight> FontWeightProperty { get; private set; }

        /// <summary>
        /// Gets the Foreground dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Control, Brush> ForegroundProperty { get; private set; }

        /// <summary>
        /// Gets the HorizontalContentAlignment dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Control, HorizontalAlignment> HorizontalContentAlignmentProperty { get; private set; }

        /// <summary>
        /// Gets the Padding dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Control, Thickness> PaddingProperty { get; private set; }

        /// <summary>
        /// Gets the IsEnabled dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Control, bool> IsEnabledProperty { get; private set; }

        /// <summary>
        /// Gets the VerticalContentAlignment dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Control, VerticalAlignment> VerticalContentAlignmentProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ControlTest class.
        /// </summary>
        protected ControlTest()
        {
            Func<Control> initializer = () => DefaultControlToTest;

            BackgroundProperty = new DependencyPropertyTest<Control, Brush>(this, "Background")
                {
                    Property = Control.BackgroundProperty,
                    Initializer = initializer,
                    DefaultValue = null,
                    OtherValues = new Brush[] { new SolidColorBrush(Colors.Black), new SolidColorBrush(Colors.Red) }
                };
            BorderBrushProperty = new DependencyPropertyTest<Control, Brush>(this, "BorderBrush")
                {
                    Property = Control.BorderBrushProperty,
                    Initializer = () => { Control c = DefaultControlToTest; c.BorderThickness = new Thickness(2.0); return c; },
                    DefaultValue = null,
                    OtherValues = new Brush[] { new SolidColorBrush(Colors.Black), new SolidColorBrush(Colors.Red) }
                };
            BorderThicknessProperty = new DependencyPropertyTest<Control, Thickness>(this, "BorderThickness")
                {
                    Property = Control.BorderThicknessProperty,
                    Initializer = () => { Control c = DefaultControlToTest; c.BorderBrush = new SolidColorBrush(Colors.Black); return c; },
                    DefaultValue = new Thickness(0.0),
                    OtherValues = new Thickness[] { new Thickness(2.0), new Thickness(10.0), new Thickness(0.0), new Thickness(.5), new Thickness(1, 2, 3, 4) },
                    InvalidValues = new Dictionary<Thickness, Type>
                    {
                        { new Thickness(-1), typeof(InvalidOperationException) }
                    }
                };
            FontFamilyProperty = new DependencyPropertyTest<Control, FontFamily>(this, "FontFamily")
                {
                    Property = Control.FontFamilyProperty,
                    Initializer = initializer,
                    DefaultValue = new FontFamily("Portable User Interface"),
                    OtherValues = new FontFamily[] { new FontFamily("Verdana"), new FontFamily("Courier"), new FontFamily("Times New Roman"), new FontFamily("Trebuchet MS") }
                };
            FontSizeProperty = new DependencyPropertyTest<Control, double>(this, "FontSize")
                {
                    Property = Control.FontSizeProperty,
                    Initializer = initializer,
                    DefaultValue = 11.0,
                    OtherValues = new double[] { 14.0, 5.0, 20.0 },
                    InvalidValues = new Dictionary<double, Type>
                    {
                        { 0.0, typeof(ArgumentException) },
                        { -1.0, typeof(ArgumentException) },
                        { double.NaN, typeof(ArgumentException) },
                        { double.PositiveInfinity, typeof(ArgumentException) },
                        { double.NegativeInfinity, typeof(ArgumentException) },
                    }
                };
            FontStretchProperty = new DependencyPropertyTest<Control, FontStretch>(this, "FontStretch")
                {
                    Property = Control.FontStretchProperty,
                    Initializer = initializer,
                    DefaultValue = FontStretches.Normal,
                    OtherValues = new FontStretch[] { FontStretches.Condensed, FontStretches.Expanded, FontStretches.UltraExpanded }
                };
            FontStyleProperty = new DependencyPropertyTest<Control, FontStyle>(this, "FontStyle")
                {
                    Property = Control.FontStyleProperty,
                    Initializer = initializer,
                    DefaultValue = FontStyles.Normal,
                    OtherValues = new FontStyle[] { FontStyles.Italic }
                };
            FontWeightProperty = new DependencyPropertyTest<Control, FontWeight>(this, "FontWeight")
                {
                    Property = Control.FontWeightProperty,
                    Initializer = initializer,
                    DefaultValue = FontWeights.Normal,
                    OtherValues = new FontWeight[] { FontWeights.Bold, FontWeights.Thin, FontWeights.ExtraBold, FontWeights.Black, FontWeights.Light }
                };
            ForegroundProperty = new DependencyPropertyTest<Control, Brush>(this, "Foreground")
                {
                    Property = Control.ForegroundProperty,
                    Initializer = initializer,
                    DefaultValue = new SolidColorBrush(Colors.Black),
                    OtherValues = new Brush[] { new SolidColorBrush(Colors.Red), null, new SolidColorBrush(Colors.Blue) }
                };
            HorizontalContentAlignmentProperty = new DependencyPropertyTest<Control, HorizontalAlignment>(this, "HorizontalContentAlignment")
                {
                    Property = Control.HorizontalContentAlignmentProperty,
                    Initializer = initializer,
                    DefaultValue = HorizontalAlignment.Center,
                    OtherValues = new HorizontalAlignment[] { HorizontalAlignment.Right, HorizontalAlignment.Center, HorizontalAlignment.Stretch },
                    InvalidValues = new Dictionary<HorizontalAlignment, Type>
                    {
                        { (HorizontalAlignment)(-1), typeof(OverflowException) },
                        { (HorizontalAlignment)4, typeof(ArgumentException) },
                        { (HorizontalAlignment)10, typeof(ArgumentException) },
                        { (HorizontalAlignment)27, typeof(ArgumentException) },
                        { (HorizontalAlignment)int.MaxValue, typeof(OverflowException) },
                        { (HorizontalAlignment)int.MinValue, typeof(OverflowException) }
                    }
                };
            IsEnabledProperty = new DependencyPropertyTest<Control, bool>(this, "IsEnabled")
                {
                    Property = Control.IsEnabledProperty,
                    Initializer = initializer,
                    DefaultValue = true,
                    OtherValues = new bool[] { false }
                };
            PaddingProperty = new DependencyPropertyTest<Control, Thickness>(this, "Padding")
                {
                    Property = Control.PaddingProperty,
                    Initializer = initializer,
                    DefaultValue = new Thickness(0.0),
                    OtherValues = new Thickness[] { new Thickness(2.0), new Thickness(10.0), new Thickness(.5), new Thickness(1, 2, 3, 4) },
                    InvalidValues = new Dictionary<Thickness, Type>
                    {
                        { new Thickness(-1), typeof(InvalidOperationException) }
                    }
                };
            VerticalContentAlignmentProperty = new DependencyPropertyTest<Control, VerticalAlignment>(this, "VerticalContentAlignment")
                {
                    Property = Control.VerticalContentAlignmentProperty,
                    Initializer = initializer,
                    DefaultValue = VerticalAlignment.Center,
                    OtherValues = new VerticalAlignment[] { VerticalAlignment.Top, VerticalAlignment.Bottom, VerticalAlignment.Stretch },
                    InvalidValues = new Dictionary<VerticalAlignment, Type>
                    {
                        { (VerticalAlignment)(-1), typeof(OverflowException) },
                        { (VerticalAlignment)4, typeof(ArgumentException) },
                        { (VerticalAlignment)10, typeof(ArgumentException) },
                        { (VerticalAlignment)27, typeof(ArgumentException) },
                        { (VerticalAlignment)int.MaxValue, typeof(OverflowException) },
                        { (VerticalAlignment)int.MinValue, typeof(OverflowException) }
                    }
                };
        }

        /// <summary>
        /// Get the dependency property tests.
        /// </summary>
        /// <returns>The dependency property tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            // Get the base Control dependency property tests
            IList<DependencyPropertyTestMethod> tests = TagInherited(base.GetDependencyPropertyTests());

            // BackgroundProperty tests
            tests.Add(BackgroundProperty.BindingTest);
            tests.Add(BackgroundProperty.CheckDefaultValueTest);
            tests.Add(BackgroundProperty.ChangeClrSetterTest);
            tests.Add(BackgroundProperty.ChangeSetValueTest);
            tests.Add(BackgroundProperty.SetNullTest);
            tests.Add(BackgroundProperty.ClearValueResetsDefaultTest);
            tests.Add(BackgroundProperty.CanBeStyledTest);
            tests.Add(BackgroundProperty.TemplateBindTest);
            tests.Add(BackgroundProperty.SetXamlAttributeTest);
            tests.Add(BackgroundProperty.SetXamlElementTest);

            // BorderBrushProperty tests
            tests.Add(BorderBrushProperty.BindingTest);
            tests.Add(BorderBrushProperty.CheckDefaultValueTest);
            tests.Add(BorderBrushProperty.ChangeClrSetterTest);
            tests.Add(BorderBrushProperty.ChangeSetValueTest);
            tests.Add(BorderBrushProperty.SetNullTest);
            tests.Add(BorderBrushProperty.ClearValueResetsDefaultTest);
            tests.Add(BorderBrushProperty.CanBeStyledTest);
            tests.Add(BorderBrushProperty.TemplateBindTest);
            tests.Add(BorderBrushProperty.SetXamlAttributeTest);
            tests.Add(BorderBrushProperty.SetXamlElementTest);

            // BorderThicknessProperty tests
            tests.Add(BorderThicknessProperty.BindingTest);
            tests.Add(BorderThicknessProperty.CheckDefaultValueTest);
            tests.Add(BorderThicknessProperty.ChangeClrSetterTest);
            tests.Add(BorderThicknessProperty.ChangeSetValueTest);
            tests.Add(BorderThicknessProperty.SetXamlAttributeTest);
            tests.Add(BorderThicknessProperty.SetXamlElementTest);

            // BorderThicknessProperty tests
            tests.Add(BorderThicknessProperty.BindingTest);
            tests.Add(BorderThicknessProperty.CheckDefaultValueTest);
            tests.Add(BorderThicknessProperty.ChangeClrSetterTest);
            tests.Add(BorderThicknessProperty.ChangeSetValueTest);
            tests.Add(BorderThicknessProperty.ClearValueResetsDefaultTest);
            tests.Add(BorderThicknessProperty.InvalidValueFailsTest.Bug("Core property is not validated"));
            tests.Add(BorderThicknessProperty.InvalidValueIsIgnoredTest.Bug("Core property is not validated"));
            tests.Add(BorderThicknessProperty.InvalidValueDoesNotChangeVisualStateTest);
            tests.Add(BorderThicknessProperty.CanBeStyledTest);
            tests.Add(BorderThicknessProperty.TemplateBindTest);
            tests.Add(BorderThicknessProperty.SetXamlAttributeTest);
            tests.Add(BorderThicknessProperty.SetXamlElementTest);

            // FontFamilyProperty tests
            tests.Add(FontFamilyProperty.CheckDefaultValueTest);
            tests.Add(FontFamilyProperty.ChangeClrSetterTest);
            tests.Add(FontFamilyProperty.ChangeSetValueTest);
            // Bug: Getting a null FontFamily throws a NullRef exception, but
            // leaving this test in even with a Bug attribute is throwing an
            // ExecutionEngineException.  We're also having a problem template
            // binding a FontFamily which seems related.
            // tests.Add(FontFamilyProperty.SetNullTest);
            // tests.Add(FontFamilyProperty.Binding);
            // tests.Add(FontFamilyProperty.TemplateBindTest);
            tests.Add(FontFamilyProperty.ClearValueResetsDefaultTest);
            tests.Add(FontFamilyProperty.CanBeStyledTest);
            tests.Add(FontFamilyProperty.SetXamlAttributeTest);
            tests.Add(FontFamilyProperty.SetXamlElementTest);

            // FontSizeProperty tests
            tests.Add(FontSizeProperty.BindingTest);
            tests.Add(FontSizeProperty.CheckDefaultValueTest);
            tests.Add(FontSizeProperty.ChangeClrSetterTest);
            tests.Add(FontSizeProperty.ChangeSetValueTest);
            tests.Add(FontSizeProperty.ClearValueResetsDefaultTest);
            tests.Add(FontSizeProperty.InvalidValueFailsTest.Bug("Core property is not validated"));
            tests.Add(FontSizeProperty.InvalidValueIsIgnoredTest.Bug("Core property is not validated"));
            tests.Add(FontSizeProperty.InvalidValueDoesNotChangeVisualStateTest);
            tests.Add(FontSizeProperty.CanBeStyledTest);
            tests.Add(FontSizeProperty.TemplateBindTest);
            tests.Add(FontSizeProperty.SetXamlAttributeTest);
            tests.Add(FontSizeProperty.SetXamlElementTest);

            // FontStretchProperty tests
            tests.Add(FontStretchProperty.BindingTest);
            tests.Add(FontStretchProperty.CheckDefaultValueTest);
            tests.Add(FontStretchProperty.ChangeClrSetterTest);
            tests.Add(FontStretchProperty.ChangeSetValueTest);
            tests.Add(FontStretchProperty.ClearValueResetsDefaultTest);
            tests.Add(FontStretchProperty.CanBeStyledTest);
            tests.Add(FontStretchProperty.SetXamlAttributeTest);
            tests.Add(FontStretchProperty.SetXamlElementTest);

            // FontStyleProperty tests
            tests.Add(FontStyleProperty.BindingTest);
            tests.Add(FontStyleProperty.CheckDefaultValueTest);
            tests.Add(FontStyleProperty.ChangeClrSetterTest);
            tests.Add(FontStyleProperty.ChangeSetValueTest);
            tests.Add(FontStyleProperty.ClearValueResetsDefaultTest);
            tests.Add(FontStyleProperty.CanBeStyledTest);
            tests.Add(FontStyleProperty.SetXamlAttributeTest);
            tests.Add(FontStyleProperty.SetXamlElementTest);

            // FontWeightProperty tests
            tests.Add(FontWeightProperty.BindingTest);
            tests.Add(FontWeightProperty.CheckDefaultValueTest);
            tests.Add(FontWeightProperty.ChangeClrSetterTest);
            tests.Add(FontWeightProperty.ChangeSetValueTest);
            tests.Add(FontWeightProperty.ClearValueResetsDefaultTest);
            tests.Add(FontWeightProperty.CanBeStyledTest);
            tests.Add(FontWeightProperty.SetXamlAttributeTest);
            tests.Add(FontWeightProperty.SetXamlElementTest);

            // ForegroundProperty tests
            tests.Add(ForegroundProperty.BindingTest);
            tests.Add(ForegroundProperty.CheckDefaultValueTest);
            tests.Add(ForegroundProperty.ChangeClrSetterTest);
            tests.Add(ForegroundProperty.ChangeSetValueTest);
            tests.Add(ForegroundProperty.SetNullTest);
            tests.Add(ForegroundProperty.ClearValueResetsDefaultTest);
            tests.Add(ForegroundProperty.CanBeStyledTest);
            tests.Add(ForegroundProperty.TemplateBindTest);
            tests.Add(ForegroundProperty.SetXamlAttributeTest);
            tests.Add(ForegroundProperty.SetXamlElementTest);

            // HorizontalContentAlignmentProperty tests
            tests.Add(HorizontalContentAlignmentProperty.BindingTest);
            tests.Add(HorizontalContentAlignmentProperty.CheckDefaultValueTest);
            tests.Add(HorizontalContentAlignmentProperty.ChangeClrSetterTest);
            tests.Add(HorizontalContentAlignmentProperty.ChangeSetValueTest);
            tests.Add(HorizontalContentAlignmentProperty.ClearValueResetsDefaultTest);
            tests.Add(HorizontalContentAlignmentProperty.InvalidValueFailsTest.Bug("Core property is not validated"));
            tests.Add(HorizontalContentAlignmentProperty.InvalidValueIsIgnoredTest.Bug("Core property is not validated"));
            tests.Add(HorizontalContentAlignmentProperty.InvalidValueDoesNotChangeVisualStateTest);
            tests.Add(HorizontalContentAlignmentProperty.CanBeStyledTest);
            tests.Add(HorizontalContentAlignmentProperty.TemplateBindTest);
            tests.Add(HorizontalContentAlignmentProperty.SetXamlAttributeTest);
            tests.Add(HorizontalContentAlignmentProperty.SetXamlElementTest);

            // IsEnabledProperty tests
            tests.Add(IsEnabledProperty.BindingTest);
            tests.Add(IsEnabledProperty.CheckDefaultValueTest);
            tests.Add(IsEnabledProperty.ChangeClrSetterTest);
            tests.Add(IsEnabledProperty.ChangeSetValueTest);
            tests.Add(IsEnabledProperty.ClearValueResetsDefaultTest);
            tests.Add(IsEnabledProperty.CanBeStyledTest);
            tests.Add(IsEnabledProperty.TemplateBindTest);
            tests.Add(IsEnabledProperty.SetXamlAttributeTest);
            tests.Add(IsEnabledProperty.SetXamlElementTest);

            // PaddingProperty tests
            tests.Add(PaddingProperty.BindingTest);
            tests.Add(PaddingProperty.CheckDefaultValueTest);
            tests.Add(PaddingProperty.ChangeClrSetterTest);
            tests.Add(PaddingProperty.ChangeSetValueTest);
            tests.Add(PaddingProperty.ClearValueResetsDefaultTest);
            tests.Add(PaddingProperty.InvalidValueFailsTest.Bug("Core property is not validated"));
            tests.Add(PaddingProperty.InvalidValueIsIgnoredTest.Bug("Core property is not validated"));
            tests.Add(PaddingProperty.InvalidValueDoesNotChangeVisualStateTest);
            tests.Add(PaddingProperty.CanBeStyledTest);
            tests.Add(PaddingProperty.TemplateBindTest);
            tests.Add(PaddingProperty.SetXamlAttributeTest);
            tests.Add(PaddingProperty.SetXamlElementTest);

            // VerticalContentAlignmentProperty tests
            tests.Add(VerticalContentAlignmentProperty.BindingTest);
            tests.Add(VerticalContentAlignmentProperty.CheckDefaultValueTest);
            tests.Add(VerticalContentAlignmentProperty.ChangeClrSetterTest);
            tests.Add(VerticalContentAlignmentProperty.ChangeSetValueTest);
            tests.Add(VerticalContentAlignmentProperty.ClearValueResetsDefaultTest);
            tests.Add(VerticalContentAlignmentProperty.InvalidValueFailsTest.Bug("Core property is not validated"));
            tests.Add(VerticalContentAlignmentProperty.InvalidValueIsIgnoredTest.Bug("Core property is not validated"));
            tests.Add(VerticalContentAlignmentProperty.InvalidValueDoesNotChangeVisualStateTest);
            tests.Add(VerticalContentAlignmentProperty.CanBeStyledTest);
            tests.Add(VerticalContentAlignmentProperty.TemplateBindTest);
            tests.Add(VerticalContentAlignmentProperty.SetXamlAttributeTest);
            tests.Add(VerticalContentAlignmentProperty.SetXamlElementTest);

            return tests;
        }
    }
}