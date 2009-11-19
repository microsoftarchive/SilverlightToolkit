// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using AdvancedDataGridView;

namespace Microsoft.CodeCoverage
{
    /// <summary>
    /// Code coverage viewer.
    /// </summary>
    public partial class Viewer : Form
    {
        /// <summary>
        /// Gets or sets the path of the coverage data being viewed.
        /// </summary>
        private string CoverageDataPath { get; set; }

        /// <summary>
        /// Gets or sets the currently selected coverage file.
        /// </summary>
        private string SelectedFile { get; set; }

        /// <summary>
        /// Gets or sets the currently selected coverage item.
        /// </summary>
        private CoverageItem SelectedItem { get; set; }

        /// <summary>
        /// Gets or sets the coverage data in the current file.
        /// </summary>
        private XDocument CoverageData { get; set; }

        /// <summary>
        /// Gets or sets a mapping of file names to the coverage highlighting in
        /// that file.
        /// </summary>
        private Dictionary<string, List<CoverageItem>> Highlighting { get; set; }

        /// <summary>
        /// Initializes a new instance of the Viewer class.
        /// </summary>
        public Viewer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Open a new coverage data file.
        /// </summary>
        /// <param name="sender">Menu item.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMenuOpenClick(object sender, EventArgs e)
        {
            if (_dlgOpen.ShowDialog() == DialogResult.OK)
            {
                LoadCoverageData(_dlgOpen.FileName, null);
            }
        }

        /// <summary>
        /// Exit the application.
        /// </summary>
        /// <param name="sender">Menu item.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMenuExitClick(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Load code coverage data.
        /// </summary>
        /// <param name="path">Path to the coverage data.</param>
        /// <param name="newPathPrefix">New path prefix for files.</param>
        public void LoadCoverageData(string path, string newPathPrefix)
        {
            // Clear any existing values
            _grdCoverage.Nodes.Clear();
            SelectedFile = null;
            _lblFile.Text = null;
            _txtSource.Text = null;

            // Load the coverage data
            CoverageDataPath = path;
            CoverageData = XDocument.Load(path);
            IEnumerable<CoverageItem> data = CoverageItem.Parse(CoverageData, newPathPrefix);
            Highlighting = new Dictionary<string, List<CoverageItem>>();

            // Populate the grid
            foreach (CoverageItem assemblyItem in data)
            {
                TreeGridNode assemblyNode = CreateCoverageNode(null, assemblyItem);
                foreach (CoverageItem namespaceItem in assemblyItem.Children)
                {
                    TreeGridNode namespaceNode = CreateCoverageNode(assemblyNode, namespaceItem);
                    foreach (CoverageItem typeItem in namespaceItem.Children)
                    {
                        TreeGridNode typeNode = CreateCoverageNode(namespaceNode, typeItem);
                        foreach (CoverageItem methodItem in typeItem.Children)
                        {
                            CreateCoverageNode(typeNode, methodItem);
                            foreach (CoverageItem blockItem in methodItem.Children)
                            {
                                List<CoverageItem> intervals = null;
                                if (!Highlighting.TryGetValue(blockItem.File, out intervals))
                                {
                                    intervals = new List<CoverageItem>();
                                    Highlighting[blockItem.File] = intervals;
                                }
                                intervals.Add(blockItem);
                            }
                        }
                        typeNode.Collapse();
                    }
                }
            }

            // Sort the highlights (see HighlightComparer for more details)
            foreach (List<CoverageItem> list in Highlighting.Values)
            {
                list.Sort(CoverageItem.HighlightComparer);
            }
        }

        /// <summary>
        /// Create a TreeGridNode for a coverage item.
        /// </summary>
        /// <param name="parent">Parent of the tree node.</param>
        /// <param name="item">Coverage item.</param>
        /// <returns>TreeGridNode for the coverage item.</returns>
        private TreeGridNode CreateCoverageNode(TreeGridNode parent, CoverageItem item)
        {
            TreeGridNode node = new TreeGridNode { Tag = item };
            ((parent != null) ? parent.Nodes : _grdCoverage.Nodes).Add(node);
            node.SetValues(item.Name, item.UncoveredBlocks, item.UncoveredBlocksPercentage, item.CoveredBlocks, item.CoveredBlocksPercentage, null);
            switch (item.ItemType)
            {
                case CoverageItemType.Assembly:
                    node.ImageIndex = 0;
                    node.Expand();
                    break;
                case CoverageItemType.Namespace:
                    node.ImageIndex = 1;
                    node.Expand();
                    break;
                case CoverageItemType.Type:
                    node.ImageIndex = 2;
                    node.Expand();
                    break;
                case CoverageItemType.Method:
                    node.ImageIndex = 3;
                    break;
            }
            return node;
        }

        /// <summary>
        /// View the coverage data for a specific item.
        /// </summary>
        /// <param name="sender">DataGrid cell.</param>
        /// <param name="e">Event arguments.</param>
        private void OnCoverageCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Get the node for the clicked row
            TreeGridNode node = _grdCoverage.GetNodeForRow(e.RowIndex);
            if (node != null)
            {
                // Get the associated coverage item
                CoverageItem item = node.Tag as CoverageItem;
                if (item != null)
                {
                    // Walk down the children until we have a file
                    while (string.IsNullOrEmpty(item.File) && item.Children.Count > 0)
                    {
                        item = item.Children[0];
                    }

                    // Change to the file if we can
                    if (!string.IsNullOrEmpty(item.File))
                    {
                        ViewCoverageItem(item);
                    }
                }
            }
        }

        /// <summary>
        /// View the coverage data for an item.
        /// </summary>
        /// <param name="item">Coverage item to view.</param>
        private void ViewCoverageItem(CoverageItem item)
        {
            if (SelectedItem == item)
            {
                return;
            }
            SelectedItem = item;

            // Only change the source tab if the file is different
            if (string.Compare(SelectedFile, item.File, StringComparison.OrdinalIgnoreCase) != 0)
            {
                SelectedFile = item.File;
                _lblFile.Text = Path.GetFileName(item.File);
                _txtSource.Text = File.ReadAllText(item.File);
                _cmbIRMethods.Items.Clear();

                // Highlight the file
                List<CoverageItem> intervals;
                if (Highlighting.TryGetValue(item.File, out intervals))
                {
                    foreach (CoverageItem interval in intervals)
                    {
                        Color highlight = GetHighlightColor(interval.Covered);
                        for (int line = interval.StartLine; line <= interval.EndLine; line++)
                        {
                            int start = (line == interval.StartLine) ?
                                interval.StartColumn :
                                0;
                            int end = (line == interval.EndLine) ?
                                interval.EndColumn :
                                -1;
                            HighlightLine(_txtSource, line, start, end, highlight);
                        }
                    }
                }

                // Fill the LIR drop down
                CoverageItem typeItem = item.Parent;
                foreach (CoverageItem methodItem in typeItem.Children)
                {
                    _cmbIRMethods.Items.Add(methodItem);
                }
            }

            // Scroll to the desired line in the source tab (though we move 5
            // lines up in the file since the method lines start at the first
            // instruction instead of the declaration)
            _txtSource.Select(_txtSource.GetFirstCharIndexFromLine(Math.Max(0, item.StartLine - 5)), 0);
            _txtSource.ScrollToCaret();

            // Select the item in the LIR drop down
            _cmbIRMethods.SelectedItem = SelectedItem;
        }

        /// <summary>
        /// Reload the LIR when the selected index is changed.
        /// </summary>
        /// <param name="sender">Combo box.</param>
        /// <param name="e">Event arguments.</param>
        private void OnIRMethodsSelectedIndexChanged(object sender, EventArgs e)
        {
            _txtIR.Text = "";

            // Get the selected item
            CoverageItem methodItem = _cmbIRMethods.SelectedItem as CoverageItem;
            if (methodItem == null)
            {
                return;
            }

            // Get the IR instructions
            string fullTypeName = methodItem.Parent.Parent.Name + '.' + methodItem.Parent.Name;
            XElement[] instructions =
                CoverageData
                .Descendants("method")
                .Where(m =>
                    string.Compare(m.Attribute("name").Value, methodItem.Name, StringComparison.OrdinalIgnoreCase) == 0 &&
                    string.Compare(m.Attribute("class").Value, fullTypeName, StringComparison.OrdinalIgnoreCase) == 0)
                .Descendants("instruction")
                .ToArray();

            // Build up the coverage display
            StringBuilder source = new StringBuilder();
            foreach (XElement instruction in instructions)
            {
                source.AppendLine(instruction.Value);
            }
            _txtIR.Text = source.ToString();

            // Highlight the coverage
            Dictionary<string, bool> coverage = methodItem.Children.ToDictionary(i => i.Name, i => i.Covered);
            for (int i = 0; i < instructions.Length; i++)
            {
                bool covered;
                string block = instructions[i].Attribute("block").Value;
                if (coverage.TryGetValue(block, out covered))
                {
                    HighlightLine(_txtIR, i, 0, -1, GetHighlightColor(covered));
                }
            }
        }

        /// <summary>
        /// Get the highlight color for coverage data.
        /// </summary>
        /// <param name="covered">Desired highlight color.</param>
        /// <returns>Highlight color.</returns>
        private static Color GetHighlightColor(bool covered)
        {
            return covered ?
                Color.PowderBlue :
                Color.LightCoral;
        }

        /// <summary>
        /// Highlight a sequence of text.
        /// </summary>
        /// <param name="textbox">Textbox to highlight.</param>
        /// <param name="start">Start of the highlight.</param>
        /// <param name="end">End of the highlight.</param>
        /// <param name="color">Color of the highlight.</param>
        private static void HighlightText(RichTextBox textbox, int start, int end, Color color)
        {
            textbox.Select(start, end - start + 1);
            textbox.SelectionBackColor = color;
            textbox.DeselectAll();
        }

        /// <summary>
        /// Highlight a line of text.
        /// </summary>
        /// <param name="textbox">Textbox to highlight.</param>
        /// <param name="line">Index of the line to highlight.</param>
        /// <param name="start">Starting column on the highlight.</param>
        /// <param name="end">Ending column of the highlight.</param>
        /// <param name="color">Color of the highlight.</param>
        private static void HighlightLine(RichTextBox textbox, int line, int start, int end, Color color)
        {
            // Get the start and end of the line
            string text = textbox.Text;
            int lineStart = textbox.GetFirstCharIndexFromLine(line - 1);
            start = lineStart + Math.Max(0, start - 1);
            end = (end < 0) ?
                textbox.GetFirstCharIndexFromLine(line) - 1 :
                lineStart + Math.Max(0, end - 1);
            
            // Trim whitespace from each end
            while (start < end)
            {
                if (char.IsWhiteSpace(text[start]))
                {
                    start++;
                }
                else if (char.IsWhiteSpace(text[end]))
                {
                    end--;
                }
                else
                {
                    break;
                }
            }

            // Set the background color
            HighlightText(textbox, start, end, color);
        }
    }
}