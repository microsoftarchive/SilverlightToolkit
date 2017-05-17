// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media;

namespace System.Windows.Controls.Samples.SyntaxHighlighting
{
    // CONSIDER: background worker to improve performance, with all UI
    // generation at the end.

    /// <summary>
    /// A formatter for source that uses a RichTextBox.
    /// </summary>
    internal class RichTextBoxFormatter : IFormatter
    {
        /// <summary>
        /// Backing field for the text box instance.
        /// </summary>
        private RichTextBox _textBox;

        /// <summary>
        /// Paragraph backing field.
        /// </summary>
        private Paragraph _para;

        /// <summary>
        /// Initializes a new instance of the XAML inline formatter which will
        /// store the contents of the syntax highlighting results into the
        /// rich text box instance.
        /// </summary>
        /// <param name="richTextBox">The rich text box instance.</param>
        public RichTextBoxFormatter(RichTextBox richTextBox)
            : this()
        {
            _textBox = richTextBox;
            _para = new Paragraph();
            _textBox.Blocks.Add(_para);
        }

        /// <summary>
        /// Initializes a new instance of the RichTextBoxFormatter class.
        /// </summary>
        protected RichTextBoxFormatter()
        {
        }

        /// <summary>
        /// Writes to the underlying rich text control's paragraph.
        /// </summary>
        /// <param name="parsedSourceCode">The parsed source code string.</param>
        /// <param name="scopes">Set of scopes identifying sections of the
        /// parsed code, in order.</param>
        /// <param name="styleSheet">The style sheet to use for coloring.</param>
        public void Write(string parsedSourceCode, IList<Scope> scopes, IStyleSheet styleSheet)
        {
            List<TextInsertion> styleInsertions = new List<TextInsertion>();

            int offset = 0;
            bool lastScopeWasComment = false;
            bool scopeIsXmlAttribute = false;
            bool lastScopeWasXmlAttributeQuote = false;
            bool insertXmlSpaceIfNeeded = false;

            foreach (Scope scope in scopes)
            {
                scopeIsXmlAttribute = scope != null && scope.Name == "XML Attribute";

                string t = parsedSourceCode.Substring(scope.Index, scope.Length);
                offset = scope.Index + scope.Length;
                if (!string.IsNullOrEmpty(t))
                {
                    Inline run = new Run { Text = t.Replace("\r", string.Empty) };
                    if (scope != null && styleSheet.Styles.Contains(scope.Name))
                    {
                        Style style = styleSheet.Styles[scope.Name];
                        run.Foreground = new SolidColorBrush(style.Foreground);
                    }

                    // XML language parser does not offer scopes for spaces,
                    // these conditional checks ensure decent spacing.
                    if (scopeIsXmlAttribute)
                    {
                        insertXmlSpaceIfNeeded = true;
                    }
                    if (scope.Name == "XML Delimiter")
                    {
                        insertXmlSpaceIfNeeded = false;
                    }

                    // Insert a space in special XML scope sets when needed.
                    if (insertXmlSpaceIfNeeded)
                    {
                        _para.Inlines.Add(new Run { Text = " " });
                        insertXmlSpaceIfNeeded = false;
                    }

                    if (!insertXmlSpaceIfNeeded && scope.Name == "XML Name")
                    {
                        insertXmlSpaceIfNeeded = true;
                    }

                    // Add the inlines to the paragraph
                    _para.Inlines.Add(run);

                    lastScopeWasComment = (scope != null && scope.Name == "Comment");
                    lastScopeWasXmlAttributeQuote = scope != null && scope.Name == "XML Attribute Quotes";

                    if (insertXmlSpaceIfNeeded && scope.Name == "XML Delimiter")
                    {
                        insertXmlSpaceIfNeeded = false;
                    }
                }
            }
            string left = parsedSourceCode
                .Substring(offset)
                .Replace("\r\n", "\n")
                .Replace("\r", "\n");
            if (!string.IsNullOrEmpty(left))
            {
                for (int i = left.IndexOf("\n"); i >= 0; i = left.IndexOf("\n"))
                {
                    if (i > 0)
                    {
                        Inline tby = new Run { Text = left.Substring(0, i) };
                        _para.Inlines.Add(tby);
                    }

                    left = left.Substring(i + 1);
                    if (lastScopeWasComment)
                    {
                        lastScopeWasComment = false;
                    }
                    else
                    {
                        _para.Inlines.Add(new LineBreak());
                    }
                }

                if (!string.IsNullOrEmpty(left))
                {
                    Inline nrun = new Run { Text = left };
                    _para.Inlines.Add(nrun);
                }
            }

            if (_textBox != null)
            {
                _textBox.Selection.Select(_textBox.ContentStart, _textBox.ContentStart);
            }
        }

        /// <summary>
        /// Gets a set of style properties for the scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="styleInsertions">The style insertions instance.</param>
        private static void GetStyleInsertionsForCapturedStyle(Scope scope, ICollection<TextInsertion> styleInsertions)
        {
            styleInsertions.Add(new TextInsertion
            {
                Index = scope.Index,
                Scope = scope
            });

            foreach (Scope childScope in scope.Children)
            {
                GetStyleInsertionsForCapturedStyle(childScope, styleInsertions);
            }

            styleInsertions.Add(new TextInsertion
            {
                Index = scope.Index + scope.Length,
            });
        }
    }
}