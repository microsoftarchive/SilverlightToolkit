// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Represents the statistics for a team, division, conference, or the league as a whole.
    /// </summary>
    public class NhlNode
    {
        /// <summary>
        /// Gets or sets a value representing the rank of the node in its level (team, division, etc).
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets a value representing the name of the entity (team, division, etc).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value representing the number of points either for a single team in the case of a leaf, 
        /// or the aggregate sum of the children for a parent node.
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Gets or sets a value representing the number of wins either for a single team in the case of a leaf, 
        /// or the aggregate sum of the children for a parent node.
        /// </summary>
        public int Wins { get; set; }

        /// <summary>
        /// Gets or sets a value representing the number of losses either for a single team in the case of a leaf, 
        /// or the aggregate sum of the children for a parent node.
        /// </summary>
        public int Losses { get; set; }

        /// <summary>
        /// Gets or sets a value representing the number of goals earned in the season either for a single team in the case of a leaf, 
        /// or the aggregate sum of the children for a parent node.
        /// </summary>
        public int GoalsFor { get; set; }

        /// <summary>
        /// Gets or sets a value representing the number of goals against accrued either for a single team in the case of a leaf, 
        /// or the aggregate sum of the children for a parent node.
        /// </summary>
        public int GoalsAgainst { get; set; }

        /// <summary>
        /// Gets or sets a value representing the number of penalty minutes accrued either for a single team in the case of a leaf, 
        /// or the aggregate sum of the children for a parent node.
        /// </summary>
        public int PenaltyMinutes { get; set; }

        /// <summary>
        /// Gets or sets a value representing the children of this division, conference, or league. Empty for teams.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Simplifies samples.")]
        public IList<NhlNode> Children { get; set; }

        /// <summary>
        /// Gets the desired tooltip content.
        /// </summary>
        public string ToolTip
        {
            get
            {
                StringBuilder outStr = new StringBuilder();
                outStr.Append(Name);
                outStr.Append("\nRank: ").Append(Rank);
                outStr.Append("\nPoints: ").Append(Points);
                outStr.Append("\nWins: ").Append(Wins);
                outStr.Append("\nLosses: ").Append(Losses);
                outStr.Append("\nGoals For: ").Append(GoalsFor);
                outStr.Append("\nGoals Against: ").Append(GoalsAgainst);
                outStr.Append("\nPenalty Minutes: ").Append(PenaltyMinutes);

                return outStr.ToString();
            }
        }
    }
}
