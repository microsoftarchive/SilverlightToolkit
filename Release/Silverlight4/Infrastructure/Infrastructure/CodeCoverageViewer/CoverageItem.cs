// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace Microsoft.CodeCoverage
{
    /// <summary>
    /// An item with associated code coverage information.
    /// </summary>
    public partial class CoverageItem
    {
        /// <summary>
        /// Gets or sets the name of the coverage item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the coverage item.
        /// </summary>
        public CoverageItemType ItemType { get; set; }

        /// <summary>
        /// Gets or sets the parent of the coverate item.
        /// </summary>
        public CoverageItem Parent { get; set; }

        /// <summary>
        /// Gets the list of children for this coverage item.
        /// </summary>
        public IList<CoverageItem> Children { get; private set; }

        /// <summary>
        /// Gets or sets the total number of blocks represented by the item.
        /// </summary>
        public int TotalBlocks { get; set; }

        /// <summary>
        /// Gets or sets the number of covered blocks represented by the item.
        /// </summary>
        public int CoveredBlocks { get; set; }

        /// <summary>
        /// Gets a value indicating whether the item is completely covered by
        /// unit tests.
        /// </summary>
        public bool Covered
        {
            get { return CoveredBlocks == TotalBlocks; }
        }

        /// <summary>
        /// Gets the percentage of covered blocks represented by the item.
        /// </summary>
        public double CoveredBlocksPercentage
        {
            get
            {
                return TotalBlocks != 0 ?
                    (double) CoveredBlocks / (double) TotalBlocks :
                    0.0;
            }
        }

        /// <summary>
        /// Gets the number of uncovered blocks represented by the item.
        /// </summary>
        public int UncoveredBlocks
        {
            get { return TotalBlocks - CoveredBlocks; }
        }

        /// <summary>
        /// Gets the percentage of uncovered blocks represented by the item.
        /// </summary>
        public double UncoveredBlocksPercentage
        {
            get { return 1.0 - CoveredBlocksPercentage; }
        }

        /// <summary>
        /// Gets or sets the name of the file containing the coverage item.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the starting line number of the coverage item in the
        /// file.
        /// </summary>
        public int StartLine { get; set; }

        /// <summary>
        /// Gets or sets the starting column number of the coverage item in the
        /// file.
        /// </summary>
        public int StartColumn { get; set; }

        /// <summary>
        /// Gets or sets the starting line number of the coverage item in the
        /// file.
        /// </summary>
        public int EndLine { get; set; }

        /// <summary>
        /// Gets or sets the starting column number of the coverage item in the
        /// file.
        /// </summary>
        public int EndColumn { get; set; }

        /// <summary>
        /// Initializes a new instance of the CoverageItem class.
        /// </summary>
        public CoverageItem()
        {
            Children = new List<CoverageItem>();
            StartLine = 0;
            StartColumn = 0;
            EndLine = -1;
            EndColumn = -1;
        }

        /// <summary>
        /// Parse a coverage data file into coverage items.
        /// </summary>
        /// <param name="document">XDocument containing the file.</param>
        /// <param name="newPathPrefix">New path prefix for files.</param>
        /// <returns>Top level coverage items.</returns>
        public static IEnumerable<CoverageItem> Parse(XDocument document, string newPathPrefix)
        {
            // Obtain the value of old prefix used in coverage.xml
            string oldpathprefix = document.Root.Attribute("pathprefix").Value;

            // Covert the XML into CoverageItems
            List<CoverageItem> assemblies = new List<CoverageItem>();
            foreach (XElement assemblyElement in document.Root.Elements("assembly"))
            {
                // Get coverate details for an assembly
                CoverageItem assemblyItem = new CoverageItem
                {
                    ItemType = CoverageItemType.Assembly,
                    Name = assemblyElement.Attribute("name").Value
                };
                assemblies.Add(assemblyItem);
                foreach (XElement methodElement in assemblyElement.Elements("method"))
                {
                    // Split the type name into namespace/type
                    string fullName = methodElement.Attribute("class").Value;
                    int partition = fullName.LastIndexOf('.');
                    string namespaceName = (partition < 0) ? "" : fullName.Substring(0, partition);
                    string typeName = fullName.Substring(partition + 1, fullName.Length - partition - 1);

                    // Get coverage details for the namespace
                    CoverageItem namespaceItem = null;
                    foreach (CoverageItem ns in assemblyItem.Children)
                    {
                        if (string.Compare(ns.Name, namespaceName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            namespaceItem = ns;
                            break;
                        }
                    }
                    if (namespaceItem == null)
                    {
                        namespaceItem = new CoverageItem
                        {
                            ItemType = CoverageItemType.Namespace,
                            Name = namespaceName,
                            Parent = assemblyItem
                        };
                        assemblyItem.Children.Add(namespaceItem);
                    }

                    // Get coverage details for the type
                    CoverageItem typeItem = null;
                    foreach (CoverageItem t in namespaceItem.Children)
                    {
                        if (string.Compare(t.Name, typeName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            typeItem = t;
                            break;
                        }
                    }
                    if (typeItem == null)
                    {
                        typeItem = new CoverageItem
                        {
                            ItemType = CoverageItemType.Type,
                            Name = typeName,
                            Parent = namespaceItem
                        };
                        namespaceItem.Children.Add(typeItem);
                    }

                    // Replace the old file path prefix in method with new prefix if not null
                    string tempmethodstr = methodElement.Attribute("file").Value;
                    tempmethodstr = ChangeAbsolutePath(tempmethodstr, oldpathprefix, newPathPrefix);

                    // Get coverage details for the method
                    CoverageItem methodItem = new CoverageItem
                    {
                        ItemType = CoverageItemType.Method,
                        Name = methodElement.Attribute("name").Value,
                        File = tempmethodstr,
                        StartLine = int.Parse(methodElement.Attribute("line").Value, CultureInfo.InvariantCulture),
                        Parent = typeItem
                    };
                    typeItem.Children.Add(methodItem);

                    // Get coverage details for the method's blocks
                    foreach (XElement blockElement in methodElement.Descendants("block"))
                    {
                        // Replace the old file path prefix in block with new prefix if not null
                        string tempblockstr = blockElement.Attribute("file").Value;
                        tempblockstr = ChangeAbsolutePath(tempblockstr, oldpathprefix, newPathPrefix);

                        CoverageItem blockItem = new CoverageItem
                        {
                            ItemType = CoverageItemType.Block,
                            Name = blockElement.Attribute("id").Value,
                            File = tempblockstr,
                            CoveredBlocks = string.Compare(blockElement.Attribute("visited").Value, "1", StringComparison.OrdinalIgnoreCase) == 0 ? 1 : 0,
                            TotalBlocks = 1,
                            StartLine = int.Parse(blockElement.Attribute("startLine").Value, CultureInfo.InvariantCulture),
                            StartColumn = int.Parse(blockElement.Attribute("startColumn").Value, CultureInfo.InvariantCulture),
                            EndLine = int.Parse(blockElement.Attribute("lastLine").Value, CultureInfo.InvariantCulture),
                            EndColumn = int.Parse(blockElement.Attribute("lastColumn").Value, CultureInfo.InvariantCulture),
                            Parent = methodItem
                        };
                        methodItem.Children.Add(blockItem);

                        // Update the coverage statistics
                        assemblyItem.TotalBlocks++;
                        namespaceItem.TotalBlocks++;
                        typeItem.TotalBlocks++;
                        methodItem.TotalBlocks++;
                        if (blockItem.Covered)
                        {
                            assemblyItem.CoveredBlocks++;
                            namespaceItem.CoveredBlocks++;
                            typeItem.CoveredBlocks++;
                            methodItem.CoveredBlocks++;
                        }
                    }
                }

                // Sort all of the lists when an assembly is finished
                List<CoverageItem> namespaces = assemblyItem.Children as List<CoverageItem>;
                namespaces.Sort(SortComparer);
                foreach (CoverageItem namespaceItem in assemblyItem.Children)
                {
                    List<CoverageItem> types = namespaceItem.Children as List<CoverageItem>;
                    types.Sort(SortComparer);

                    foreach (CoverageItem typeItem in namespaceItem.Children)
                    {
                        List<CoverageItem> methods = typeItem.Children as List<CoverageItem>;
                        methods.Sort(SortComparer);
                    }
                }
            }

            assemblies.Sort(SortComparer);
            return assemblies;
        }

        /// <summary>
        /// Compare the names of two coverage items for sorting.
        /// </summary>
        /// <param name="first">First coverage item.</param>
        /// <param name="second">Second coverage item.</param>
        /// <returns>
        /// 0 if the names are equal, -1 if first comes before second, or 1 if
        /// second comes before first.
        /// </returns>
        public static int SortComparer(CoverageItem first, CoverageItem second)
        {
            return string.CompareOrdinal(first.Name, second.Name);
        }

        /// <summary>
        /// Compare the highlight ordering of two coverage items.
        /// </summary>
        /// <param name="first">First coverage item.</param>
        /// <param name="second">Second coverage item.</param>
        /// <returns>
        /// 0 if the items are equal, -1 if first comes before second, or 1 if
        /// second comes before first.
        /// </returns>
        public static int HighlightComparer(CoverageItem first, CoverageItem second)
        {
            // Basic blocks will only overlap by nesting one inside the other.
            // Therefore we want to sort with the largest containing blocks at
            // the front of the list (so they will be "painted" first)

            if (first == null)
            {
                // If one or both are null
                return (second == null) ? 0 : 1;
            }
            else if (first == second)
            {
                // If they're the same object
                return 0;
            }
            else if (first.StartLine <= second.StartLine && first.EndLine >= second.EndLine)
            {
                // If the first contains the second
                return -1;
            }
            else if (second.StartLine <= first.StartLine && second.EndLine >= first.EndLine)
            {
                // If the second contains the first
                return 1;
            }
            else if (first.StartLine != second.StartLine)
            {
                // Otherwise sort them be file position
                return (first.StartLine <= second.StartLine) ? -1 : 1;
            }
            else if (first.StartColumn != second.StartColumn)
            {
                // Or sort them by line position
                return (first.StartColumn <= second.StartColumn) ? -1 : 1;
            }
            else if (first.Covered != second.Covered)
            {
                // Or if they're the same character, make the uncovered one go
                // last so people can see it easier
                return first.Covered ? -1 : 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Replace the old file path prefix in method with new prefix.
        /// </summary>
        /// <param name="fullpath">Full path of the file.</param>
        /// <param name="oldPathPrefix">Old file path prefix.</param>
        /// <param name="newPathPrefix">New file path prefix.</param>
        /// <returns>Replaced file path.</returns>
        private static string ChangeAbsolutePath(string fullpath, string oldPathPrefix, string newPathPrefix)
        {
            if (!(string.IsNullOrEmpty(newPathPrefix) || string.IsNullOrEmpty(fullpath)))
            {
                fullpath = fullpath.Substring(oldPathPrefix.Length + 1, fullpath.Length - oldPathPrefix.Length - 1);
                fullpath = System.IO.Path.Combine(newPathPrefix, fullpath);
            }
            return fullpath;
        }
    }
}