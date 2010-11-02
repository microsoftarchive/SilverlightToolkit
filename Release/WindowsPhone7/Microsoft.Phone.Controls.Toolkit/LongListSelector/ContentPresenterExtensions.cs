// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
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
            public LinkedListNode<ContentPresenter> Node;
            public int ResolvedIndex;
        }

        public static void GetExtraData(this ContentPresenter cp, out LinkedListNode<ContentPresenter> node, out int resolvedIndex)
        {
            ExtraData extraData = cp.Tag as ExtraData;
            if (extraData != null)
            {
                node = extraData.Node;
                resolvedIndex = extraData.ResolvedIndex;
            }
            else
            {
                node = null;
                resolvedIndex = -1;
            }
        }

        public static void SetExtraData(this ContentPresenter cp, LinkedListNode<ContentPresenter> node, int resolvedIndex)
        {
            ExtraData extraData = cp.Tag as ExtraData;
            if (extraData == null)
            {
                cp.Tag = extraData = new ExtraData();
            }
            extraData.Node = node;
            extraData.ResolvedIndex = resolvedIndex;
        }
    }
}