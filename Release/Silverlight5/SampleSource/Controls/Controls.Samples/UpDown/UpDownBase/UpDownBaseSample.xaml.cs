// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

[assembly: SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Scope = "member", Target = "System.Windows.Controls.Samples.UpDownBaseSample+SUD.#.ctor()")]
[assembly: SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Scope = "member", Target = "System.Windows.Controls.Samples.UpDownBaseSample+TUD.#.ctor()")]
[assembly: SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Scope = "member", Target = "System.Windows.Controls.Samples.UpDownBaseSample+NUD.#.ctor()")]

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// UpDownBase sample page.
    /// </summary>
    [Sample("UpDownBase", DifficultyLevel.Basic, "UpDownBase")]
    public partial class UpDownBaseSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the UpDownBaseSample class.
        /// </summary>
        public UpDownBaseSample()
        {
            InitializeComponent();

            // create instance of the test non generic sub classes
            spNUD.Children.Add(new NUD());
            spTUD.Children.Add(new TUD());
            spSUD.Children.Add(new SUD());

            btnChange.Click += (s, a) => CreateUDB();
        }

        /// <summary>
        /// This function does following three things:
        /// 1. Remove existing interactive UpDownBase control if it exists;
        /// 2. Create a new instance of selected UpDownBase subclass type;
        /// 3. Add the newly created UpDownBase control to visual tree.
        /// </summary>
        private void CreateUDB()
        {
            if (udb != null)
            {
                Debug.Assert(udb == sp.Children[0], "udb should be sp's only child!");
                sp.Children.Remove(udb);
            }

            switch (lbType.SelectedIndex)
            {
                case 0:
                    udb = new NUD();
                    ConfigUDB((UpDownBase<int>)udb);
                    break;
                case 1:
                    udb = new TUD();
                    ConfigUDB((UpDownBase<DateTime>)udb);
                    break;
                case 2:
                    udb = new SUD();
                    ConfigUDB((UpDownBase<string>)udb);
                    break;
            }

            sp.Children.Add(udb);
        }

        /// <summary>
        /// Configure a UpDownBase&lt;T&gt; control.
        /// </summary>
        /// <typeparam name="T">Type parameter.</typeparam>
        /// <param name="udbt">UpDownBase&lt;T&gt; control.</param>
        private void ConfigUDB<T>(UpDownBase<T> udbt)
        {
            Debug.Assert(udbt != null, "udb should not be null!");

            udbt.Value = ParseUtility.ReadValue(tbValue, udbt.Value);
            ////udbt.IsCyclic = (bool)cbIsCyclic.IsChecked;
            udbt.IsEditable = (bool)cbIsEditable.IsChecked;
            udbt.IsEnabled = (bool)cbIsEnabled.IsChecked;
        }

        /// <summary>
        /// The interactive UpDownBase control.
        /// </summary>
        private UpDownBase udb;

        #region Non-generic test classes inheriting from UpDownBase<T>
        /// <summary>
        /// Non-generic test class inheriting from UpDownBase&lt;int&gt;.
        /// </summary>
        /// <remarks>
        /// Value is of type int, in between 0 and 10.
        /// IsCyclic decides whether Value will wrap around 0 and 10.
        /// OnIncrement and OnDecrement change Value by 1.
        /// </remarks>
        [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
        [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
        [TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
        [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]

        [TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
        [TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]

        [TemplatePart(Name = "TextName", Type = typeof(TextBox))]
        [TemplatePart(Name = "SpinnerName", Type = typeof(Spinner))]
        [StyleTypedProperty(Property = "SpinnerStyle", StyleTargetType = typeof(Spinner))]
        internal class NUD : UpDownBase<int>
        {
            /// <summary>
            /// Private variable to replace IsCyclic property for now removed from UpDownBase&lt;T&gt;.
            /// </summary>
            private bool IsCyclic = true;

            /// <summary>
            /// Internal constructor.
            /// </summary>
            internal NUD()
                : base()
            {
                Value = 0;
            }

            /// <summary>
            /// Called by OnSpin when the spin direction is SpinDirection.Increase.
            /// </summary>
            protected override void OnIncrement()
            {
                Value = IsCyclic ? (Value + 1) % 10 : Math.Min(10, Value + 1);
            }

            /// <summary>
            /// Called by OnSpin when the spin direction is SpinDirection.Increase.
            /// </summary>
            protected override void OnDecrement()
            {
                Value = IsCyclic ? (Value - 1) % 10 : Math.Max(0, Value - 1);
            }

            /// <summary>
            /// Called by ApplyValue to parse user input.
            /// </summary>
            /// <param name="text">User input.</param>
            /// <returns>Value parsed from user input.</returns>
            protected override int ParseValue(string text)
            {
                return int.Parse(text, CultureInfo.CurrentCulture);
            }

            /// <summary>
            /// Called to render Value for Text template part to display.
            /// </summary>
            /// <returns>Formatted Value.</returns>
            protected override string FormatValue()
            {
                return Value.ToString(CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Non-generic test class inheriting from UpDownBase&lt;DateTime&gt;.
        /// </summary>
        /// <remarks>
        /// Value is of type DateTime. 
        /// IsCyclic decides whether Value will wrap around DateTime.MinValue and DateTime.MaxValue.
        /// OnIncrement and OnDecrement change Value by one hour.
        /// </remarks>
        [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
        [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
        [TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
        [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]

        [TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
        [TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]

        [TemplatePart(Name = "TextName", Type = typeof(TextBox))]
        [TemplatePart(Name = "SpinnerName", Type = typeof(Spinner))]
        [StyleTypedProperty(Property = "SpinnerStyle", StyleTargetType = typeof(Spinner))]
        internal class TUD : UpDownBase<DateTime>
        {
            /// <summary>
            /// Private variable to replace IsCyclic property for now removed from UpDownBase&lt;T&gt;.
            /// </summary>
            private bool IsCyclic = true;

            /// <summary>
            /// Internal constructor.
            /// </summary>
            internal TUD()
                : base()
            {
                Value = DateTime.Now;
            }

            /// <summary>
            /// Called by OnSpin when the spin direction is SpinDirection.Increase.
            /// </summary>
            protected override void OnIncrement()
            {
                try
                {
                    Value = Value.AddHours(1);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Value = IsCyclic ? DateTime.MinValue : DateTime.MaxValue;
                }
            }

            /// <summary>
            /// Called by OnSpin when the spin direction is SpinDirection.Increase.
            /// </summary>
            protected override void OnDecrement()
            {
                try
                {
                    Value = Value.AddHours(-1);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Value = IsCyclic ? DateTime.MaxValue : DateTime.MinValue;
                }
            }

            /// <summary>
            /// Called by ApplyValue to parse user input.
            /// </summary>
            /// <param name="text">User input.</param>
            /// <returns>Value parsed from user input.</returns>
            protected override DateTime ParseValue(string text)
            {
                return DateTime.Parse(text, CultureInfo.CurrentCulture);
            }

            /// <summary>
            /// Called to render Value for Text template part to display.
            /// </summary>
            /// <returns>Formatted Value.</returns>
            protected override string FormatValue()
            {
                return Value.ToShortTimeString();
            }
        }

        /// <summary>
        /// Non-generic class for UpDownBase&lt;T&gt; testing only.
        /// </summary>
        /// <remarks>
        /// Value is of type string.
        /// OnIncrement and OnDecrement change string length by 1 character.
        /// IsCyclic decides whether Value length will cycle through 0 to 10.
        /// </remarks>
        [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
        [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
        [TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
        [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]

        [TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
        [TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]

        [TemplatePart(Name = "TextName", Type = typeof(TextBox))]
        [TemplatePart(Name = "SpinnerName", Type = typeof(Spinner))]
        [StyleTypedProperty(Property = "SpinnerStyle", StyleTargetType = typeof(Spinner))]
        internal class SUD : UpDownBase<string>
        {
            /// <summary>
            /// Private variable to replace IsCyclic property for now removed from UpDownBase&lt;T&gt;.
            /// </summary>
            private bool IsCyclic = true;

            /// <summary>
            /// Internal constructor.
            /// </summary>
            internal SUD()
                : base()
            {
                Value = "0";
            }

            /// <summary>
            /// Called by OnSpin when the spin direction is SpinDirection.Increase.
            /// </summary>
            protected override void OnIncrement()
            {
                string value = string.IsNullOrEmpty(Value) ? "0" : Value;
                if (value.Length >= 10)
                {
                    if (IsCyclic)
                    {
                        value = "0";
                    }
                }
                else
                {
                    value = value + value.Length.ToString(CultureInfo.CurrentCulture);
                }
                Value = value;
            }

            /// <summary>
            /// Called by OnSpin when the spin direction is SpinDirection.Increase.
            /// </summary>
            protected override void OnDecrement()
            {
                string value = string.IsNullOrEmpty(Value) ? "0" : Value;
                if (value.Length <= 1)
                {
                    if (IsCyclic)
                    {
                        value = "0123456789";
                    }
                }
                else
                {
                    value = value.Substring(0, value.Length - 1);
                }
                Value = value;
            }

            /// <summary>
            /// Called by ApplyValue to parse user input.
            /// </summary>
            /// <param name="text">User input.</param>
            /// <returns>Value parsed from user input.</returns>
            protected override string ParseValue(string text)
            {
                if (string.IsNullOrEmpty(text))
                {
                    text = "0";
                }

                if (text.Length > 10)
                {
                    text = text.Substring(0, 10);
                }

                return text;
            }

            /// <summary>
            /// Called to render Value for Text template part to display.
            /// </summary>
            /// <returns>Formatted Value.</returns>
            protected override string FormatValue()
            {
                return string.IsNullOrEmpty(Value) ? "0" : Value;
            }
        }
        #endregion 
    }
}
