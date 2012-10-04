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
            Pivot pivot = sender as Pivot;

            if (pivot == null)
            {
                return;
            }

            switch (pivot.SelectedIndex)
            {
                case 0:
                    HubTileService.UnfreezeGroup("Food");
                    HubTileService.FreezeGroup("Places");
                    HubTileService.FreezeGroup("Sizes");
                    break;

                case 1:
                    HubTileService.FreezeGroup("Food");
                    HubTileService.UnfreezeGroup("Places");
                    HubTileService.FreezeGroup("Sizes");
                    break;

                case 2:
                    HubTileService.FreezeGroup("Food");
                    HubTileService.FreezeGroup("Places");
                    HubTileService.UnfreezeGroup("Sizes");
                    break;
            }
        }
    }
}