// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Data.Design
{
    internal class DataGridLengthEditorGrid : Grid
    {
        private enum DataGridLengthUnitType
        {
            Auto = 0,
            Pixel = 1,
            SizeToCells = 2,
            SizeToHeader = 3,
        }

        private TextBox _textBox;
        private ComboBox _comboBox;
        // Default this to 100 like Winforms
        private double _lastValidPixelWidth = 100;
        private bool _loadingValue;

        public DataGridLengthEditorGrid()
        {
            this.ColumnDefinitions.Add(new ColumnDefinition());
            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.Width = GridLength.Auto;
            this.ColumnDefinitions.Add(columnDefinition);

            _textBox = new TextBox();
            Binding binding = new Binding("Text");
            binding.Mode = BindingMode.TwoWay;
            _textBox.SetBinding(TextBox.TextProperty, binding);
            _textBox.KeyDown += new KeyEventHandler(TextBox_KeyDown);
            _textBox.DataContext = this;
            this.Children.Add(_textBox);

            _comboBox = new ComboBox();
            _comboBox.SetValue(Grid.ColumnProperty, 1);
            // Unfortunately, there's no built in way to tell the ComboBox to be as wide as the widest item
            _comboBox.MinWidth = 95; 
            _comboBox.SelectionChanged += new SelectionChangedEventHandler(ComboBox_SelectionChanged);
            DataGridLengthUnitType[] items = 
            {
                DataGridLengthUnitType.Auto,
                DataGridLengthUnitType.Pixel,
                DataGridLengthUnitType.SizeToCells,
                DataGridLengthUnitType.SizeToHeader
            };
            _comboBox.ItemsSource = items;
            this.Children.Add(_comboBox);
        }

        #region Dependency Properties

        /// <summary>
        /// Text is bound to the TextBox's Text property.  By doing this, we know when the
        /// TextBox's Text changes and we can commit in a similar fashion (focus lost) as any
        /// other bound TextBox
        /// </summary>
        public string Text
        {
            get
            {
                return (string)this.GetValue(DataGridLengthEditorGrid.TextProperty);
            }
            set
            {
                this.SetValue(DataGridLengthEditorGrid.TextProperty, value);
            }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(DataGridLengthEditorGrid),
                new PropertyMetadata(OnTextPropertyChanged));

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridLengthEditorGrid editorGrid = (DataGridLengthEditorGrid)d;
            if (!editorGrid._loadingValue)
            {
                string newText = (string)e.NewValue;
                // We only want to update the value when the user has typed something in
                if (String.IsNullOrEmpty(newText))
                {
                    editorGrid._textBox.Text = editorGrid._lastValidPixelWidth.ToString(CultureInfo.InvariantCulture);
                    return;
                }
                editorGrid.SetLengthValue();
            }
            else
            {
                // Even though we're just loading the Value, we still need to cache 
                // the lastValidPixelWidth because it may have been set through xaml
                double pixels;
                if (double.TryParse(editorGrid._textBox.Text, out pixels))
                {
                    editorGrid._lastValidPixelWidth = pixels;
                }
            }
        }

        /// <summary>
        /// Value is a DataGridLength that is bound to the Value that we are given by Blend.
        /// Wrapping this property gives us the changed notifications we need
        /// </summary>
        public SSWC.DataGridLength Value
        {
            get
            {
                return (SSWC.DataGridLength)this.GetValue(DataGridLengthEditorGrid.ValueProperty);
            }
            set
            {
                this.SetValue(DataGridLengthEditorGrid.ValueProperty, value);
            }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(SSWC.DataGridLength),
                typeof(DataGridLengthEditorGrid),
                new PropertyMetadata(OnValuePropertyChanged));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridLengthEditorGrid editorGrid = (DataGridLengthEditorGrid)d;
            if (!editorGrid.AreHandlersSuspended())
            {
                editorGrid.LoadLengthValue((SSWC.DataGridLength)e.NewValue);
            }
        }

        #endregion Dependency Properties

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this._loadingValue)
            {
                SetLengthValue();
            }
        }

        private void DisableTextBox()
        {
            this.Text = String.Empty;
            _textBox.IsEnabled = false;
        }

        private void LoadLengthValue(SSWC.DataGridLength dataGridLength)
        {
            this._loadingValue = true;
            try
            {
                if (dataGridLength.IsAuto)
                {
                    _comboBox.SelectedItem = DataGridLengthUnitType.Auto;
                    DisableTextBox();
                }
                else if (dataGridLength.IsSizeToCells)
                {
                    _comboBox.SelectedItem = DataGridLengthUnitType.SizeToCells;
                    DisableTextBox();
                }
                else if (dataGridLength.IsSizeToHeader)
                {
                    _comboBox.SelectedItem = DataGridLengthUnitType.SizeToHeader;
                    DisableTextBox();
                }
                else
                {
                    _comboBox.SelectedItem = DataGridLengthUnitType.Pixel;
                    this.Text = dataGridLength.Value.ToString(CultureInfo.InvariantCulture);
                    _textBox.IsEnabled = true;
                }
            }
            finally
            {
                this._loadingValue = false;
            }
        }

        private void SetLengthValue()
        {
            DataGridLengthUnitType unitType = (DataGridLengthUnitType)_comboBox.SelectedItem;
            switch (unitType)
            {
                case DataGridLengthUnitType.Auto:
                    // To get around grow only in the designer, set the width to a small value
                    this.SetValueNoCallback(DataGridLengthEditorGrid.ValueProperty, new SSWC.DataGridLength(1));
                    this.Value = SSWC.DataGridLength.Auto;
                    break;
                case DataGridLengthUnitType.SizeToCells:
                    // To get around grow only in the designer, set the width to a small value
                    this.SetValueNoCallback(DataGridLengthEditorGrid.ValueProperty, new SSWC.DataGridLength(1));
                    this.Value = SSWC.DataGridLength.SizeToCells;
                    break;
                case DataGridLengthUnitType.SizeToHeader:
                    // To get around grow only in the designer, set the width to a small value
                    this.SetValueNoCallback(DataGridLengthEditorGrid.ValueProperty, new SSWC.DataGridLength(1));
                    this.Value = SSWC.DataGridLength.SizeToHeader;
                    break;
                default:
                    // Treat it as a Pixel length
                    double pixels;
                    if (double.TryParse(_textBox.Text, out pixels))
                    {
                        // Store the last good pixel value
                        _lastValidPixelWidth = pixels;
                        this.Value = new SSWC.DataGridLength(pixels);
                    }
                    else
                    {
                        // The user entered something bad so revert to the last good value
                        _textBox.Text = _lastValidPixelWidth.ToString(CultureInfo.InvariantCulture);
                        // The Text binding does not update the Value in this case so set it explicitly
                        this.Value = new SSWC.DataGridLength(_lastValidPixelWidth);
                    }
                    break;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // We need to force an update when the Enter key is pressed
            if (e.Key == Key.Enter)
            {
                BindingExpression bindingExpression = _textBox.GetBindingExpression(TextBox.TextProperty);
                Debug.Assert(bindingExpression != null);
                bindingExpression.UpdateSource();
            }
        }
    }
}
