// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace PhoneToolkitSample.Samples
{
    public partial class HubTileSample : PhoneApplicationPage
    {
        public HubTileSample()
        {
            InitializeComponent();
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as Pivot).SelectedIndex == 1)
            {
                HubTileService.UnfreezeGroup("Places");
                HubTileService.FreezeGroup("Food");
            }
            else
            {
                HubTileService.UnfreezeGroup("Food");
                HubTileService.FreezeGroup("Places");
            }
        }
    }
}