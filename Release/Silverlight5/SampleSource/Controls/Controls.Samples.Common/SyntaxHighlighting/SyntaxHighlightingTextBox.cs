// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;

namespace System.Windows.Controls.Samples.SyntaxHighlighting
{
    /// <summary>
    /// A simple text control for displaying syntax highlighted source code.
    /// </summary>
    [TemplatePart(Name = TextBoxName, Type = typeof(RichTextBox))]
    public class SyntaxHighlightingTextBox : Control
    {
        /// <summary>
        /// The name of the text block part.
        /// </summary>
        private const string TextBoxName = "RichTextBox";

        /// <summary>
        /// Shared static color coding system instance.
        /// </summary>
        private static WeakReference _colorizer;

        /// <summary>
        /// Backing field for the text block.
        /// </summary>
        private RichTextBox _richTextBox;

        #region public SourceLanguageType SourceLanguage
        /// <summary>
        /// Gets or sets the source language type.
        /// </summary>
        public SourceLanguageType SourceLanguage
        {
            get { return (SourceLanguageType)GetValue(SourceLanguageProperty); }
            set { SetValue(SourceLanguageProperty, value); }
        }

        /// <summary>
        /// Identifies the SourceLanguage dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceLanguageProperty =
            DependencyProperty.Register(
                "SourceLanguage",
                typeof(SourceLanguageType),
                typeof(SyntaxHighlightingTextBox),
                new PropertyMetadata(SourceLanguageType.CSharp, OnSourceLanguagePropertyChanged));

        /// <summary>
        /// Property change ignore field.
        /// </summary>
        private bool _ignorePropertyChange;

        /// <summary>
        /// SourceLanguageProperty property changed handler.
        /// </summary>
        /// <param name="d">SyntaxHighlightingTextBlock that changed its SourceLanguage.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSourceLanguagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SyntaxHighlightingTextBox source = d as SyntaxHighlightingTextBox;

            if (source._ignorePropertyChange)
            {
                source._ignorePropertyChange = false;
                return;
            }

            SourceLanguageType value = (SourceLanguageType)e.NewValue;
            if (value != SourceLanguageType.CSharp &&
                value != SourceLanguageType.VisualBasic &&
                value != SourceLanguageType.Xaml &&
                value != SourceLanguageType.Xml)
            {
                source._ignorePropertyChange = true;
                d.SetValue(e.Property, e.OldValue);
                throw new ArgumentException("Invalid source language type.");
            }

            if (e.NewValue != e.OldValue)
            {
                source.HighlightContents();
            }
        }
        #endregion public SourceLanguageType SourceLanguage

        #region public string Text
        /// <summary>
        /// Gets or sets the source code to display inside the syntax
        /// highlighting text block.
        /// </summary>
        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Identifies the Text dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(SyntaxHighlightingTextBox),
                new PropertyMetadata(string.Empty, OnTextPropertyChanged));

        /// <summary>
        /// TextProperty property changed handler.
        /// </summary>
        /// <param name="d">SyntaxHighlightingTextBlock that changed its Text.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SyntaxHighlightingTextBox source = d as SyntaxHighlightingTextBox;
            source.HighlightContents();
        }
        #endregion public string Text

        /// <summary>
        /// Initializes a new instance of the SyntaxHighlightingTextBlock
        /// control.
        /// </summary>
        public SyntaxHighlightingTextBox()
        {
            DefaultStyleKey = typeof(SyntaxHighlightingTextBox);
        }

        /// <summary>
        /// Overrides the on apply template method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _richTextBox = GetTemplateChild(TextBoxName) as RichTextBox;
            if (_richTextBox != null && !string.IsNullOrEmpty(Text))
            {
                HighlightContents();
            }
        }

        /// <summary>
        /// Clears and updates the contents.
        /// </summary>
        private void HighlightContents()
        {
            if (_richTextBox != null)
            {
                _richTextBox.Blocks.Clear();

                if (!string.IsNullOrEmpty(Text))
                {
                    RichTextBoxFormatter rtbf = new RichTextBoxFormatter(_richTextBox);

                    CodeColorizer cc;
                    if (_colorizer != null && _colorizer.IsAlive)
                    {
                        cc = (CodeColorizer)_colorizer.Target;
                    }
                    else
                    {
                        cc = new CodeColorizer();
                        _colorizer = new WeakReference(cc);
                    }

                    ILanguage language = CreateLanguageInstance(SourceLanguage);
                    cc.Colorize(Text, language, rtbf, new DefaultStyleSheet());
                }
            }
        }

        /// <summary>
        /// Retrieves the language instance used by the highlighting system.
        /// </summary>
        /// <param name="type">The language type to create.</param>
        /// <returns>Returns a new instance of the language parser.</returns>
        private ILanguage CreateLanguageInstance(SourceLanguageType type)
        {
            switch (type)
            {
                case SourceLanguageType.CSharp:
                    return Languages.CSharp;
                    
                case SourceLanguageType.VisualBasic:
                    return Languages.VbDotNet;

                case SourceLanguageType.Xaml:
                case SourceLanguageType.Xml:
                    return Languages.Xml;

                default:
                    throw new InvalidOperationException("Could not locate the provider.");
            }
        }
    }
}