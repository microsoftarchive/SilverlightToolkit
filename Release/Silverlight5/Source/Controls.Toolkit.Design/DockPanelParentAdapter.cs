// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSWC = Silverlight::System.Windows.Controls;
using Microsoft.Windows.Design.Metadata;

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// Adapter for DockPanel.
    /// </summary>
    internal class DockPanelParentAdapter : PanelParentAdapter
    {
        /// <summary>
        /// Default constructor for DockPanelParentAdapter.
        /// </summary>
        public DockPanelParentAdapter()
            : base(PlatformTypes.DockPanel.TypeId)
        {
        }

        /// <summary>
        /// Gets the set of container specific properties.
        /// </summary>
        protected override IEnumerable<PropertyIdentifier> ContainerSpecificProperties
        {
            get
            {
                yield return PlatformTypes.DockPanel.DockProperty;
            }
        }
    }
}
