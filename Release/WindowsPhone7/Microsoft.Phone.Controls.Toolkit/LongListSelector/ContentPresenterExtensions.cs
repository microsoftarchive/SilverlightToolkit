// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// This class utilizes the Tag property to attach data to ContentPresenters
    /// </summary>
    internal static class ContentPresenterExtensions
    {
        private class ExtraData
        {
            public int ResolvedIndex;
            public double LastDesiredHeight;
        }

        public static void GetExtraData(this ContentPresenter cp, out int resolvedIndex, out double lastDesiredHeight)
        {
            ExtraData extraData = cp.Tag as ExtraData;
            if (extraData != null)
            {
                resolvedIndex = extraData.ResolvedIndex;
                lastDesiredHeight = extraData.LastDesiredHeight;
            }
            else
            {
                resolvedIndex = -1;
                lastDesiredHeight = 0;
            }
        }

        public static void SetExtraData(this ContentPresenter cp, int resolvedIndex, double lastDesiredHeight)
        {
            ExtraData extraData = cp.Tag as ExtraData;
            if (extraData == null)
            {
                cp.Tag = extraData = new ExtraData();                
            }
            extraData.ResolvedIndex = resolvedIndex;
            extraData.LastDesiredHeight = lastDesiredHeight;
        }
    }
}