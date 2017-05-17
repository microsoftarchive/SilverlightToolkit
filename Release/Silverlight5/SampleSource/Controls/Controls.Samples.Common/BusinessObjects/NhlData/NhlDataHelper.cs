// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Globalization;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Holds helper methods to load or generate test data used in the TreeMap samples.
    /// </summary>
    public static class NhlDataHelper
    {
        /// <summary>
        /// A random number generator for generating test trees.
        /// </summary>
        private static Random _random = new Random();

        /// <summary>
        /// Helper to load a tree of NhlNodes from an XML document.
        /// </summary>
        /// <param name="elements">Collection of XElement objects representing the current level of nodes to be loaded.</param>
        /// <returns>A list of NhlNode objects loaded from their XElement counterparts.</returns>
        public static IList<NhlNode> LoadNhlNodes(IEnumerable<XElement> elements)
        {
            IEnumerable<NhlNode> result = 
                from n in elements
                select new NhlNode()
                {
                    Children = LoadNhlNodes(n.Element("Children").Elements("NhlNode")),
                    GoalsAgainst = int.Parse(n.Element("GoalsAgainst").Value, CultureInfo.InvariantCulture),
                    GoalsFor = int.Parse(n.Element("GoalsFor").Value, CultureInfo.InvariantCulture),
                    Losses = int.Parse(n.Element("Losses").Value, CultureInfo.InvariantCulture),
                    Name = n.Element("Name").Value,
                    PenaltyMinutes = int.Parse(n.Element("PenaltyMinutes").Value, CultureInfo.InvariantCulture),
                    Points = int.Parse(n.Element("Points").Value, CultureInfo.InvariantCulture),
                    Rank = int.Parse(n.Element("Rank").Value, CultureInfo.InvariantCulture),
                    Wins = int.Parse(n.Element("Wins").Value, CultureInfo.InvariantCulture)
                };

            return result.ToList();
        }

        /// <summary>
        /// Loads test data from the default XML data file.
        /// </summary>
        /// <returns>A list of NhlNode objects which can be set as the ItemsSource in the TreeMap.</returns>
        public static IList<NhlNode> LoadDefaultFile()
        {
            IList<NhlNode> result = null;
            NhlNode node = new NhlNode();
            // Stream stream = Application.GetResourceStream(new Uri("/System.Windows.Controls.Samples.BusinessObjects.NhlData.NhlData.xml", UriKind.RelativeOrAbsolute)).Stream
            using (Stream stream = node.GetType().Assembly.GetManifestResourceStream("System.Windows.Controls.Samples.BusinessObjects.NhlData.NhlData.xml"))
            {
                result = LoadNhlNodes(XElement.Load(stream).Elements("NhlNode"));
            }

            return result;
        }

        /// <summary>
        /// Helper to generate a random hierarchy of the specified depth, with the specified number
        /// of children at each level.
        /// </summary>
        /// <param name="depth">Number of levels in the generated tree.</param>
        /// <param name="nodesPerLevel">Number of children of each node.</param>
        /// <returns>The root node of the randomly generated tree.</returns>
        public static NhlNode CreateRandomTree(int depth, int nodesPerLevel)
        {
            NhlNode node = new NhlNode();
            node.Name = "N" + depth;

            if (depth <= 0)
            {
                node.Children = new NhlNode[0];
                node.GoalsAgainst = _random.Next(1, 50);
                node.GoalsFor = _random.Next(1, 50);
                node.Losses = _random.Next(1, 50);
                node.PenaltyMinutes = _random.Next(1, 50);
                node.Points = _random.Next(1, 50);
                node.Rank = _random.Next(1, 50);
                node.Wins = _random.Next(1, 50);
            }
            else
            {
                node.Children = (from index in Enumerable.Range(0, nodesPerLevel)
                                 select CreateRandomTree(depth - 1, nodesPerLevel)).ToArray();
            }

            return node;
        }
    }
}
