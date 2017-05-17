// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// Adapter for DockPanel.
    /// </summary>
    internal class ExpanderParentAdapter : ParentAdapter
    {
        /// <summary>
        /// Determines whether expander allows parenting.
        /// </summary>
        private bool _canParent = true;

        /// <summary>
        /// Determines whether an item may be dragged inside this panel.
        /// </summary>
        /// <param name="parent">The panel.</param>
        /// <param name="childType">The child.</param>
        /// <returns>True if the item may be parented.</returns>
        public override bool CanParent(ModelItem parent, Type childType)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            if (childType == null)
            {
                throw new ArgumentNullException("childType");
            }

            return _canParent;
        }

        /// <summary>
        /// The act of parenting a child to this panel.
        /// </summary>
        /// <param name="parent">The panel.</param>
        /// <param name="child">The child being parented.</param>
        public override void Parent(ModelItem parent, ModelItem child)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }

            child.Properties[PlatformTypes.FrameworkElement.MarginProperty].ClearValue();
            child.Properties[PlatformTypes.FrameworkElement.HorizontalAlignmentProperty].ClearValue();
            child.Properties[PlatformTypes.FrameworkElement.VerticalAlignmentProperty].ClearValue();

            parent.Content.SetValue(child);
        }

        /// <summary>
        /// Redirect the parent action.
        /// </summary>
        /// <param name="parent">The item being dragged in.</param>
        /// <param name="childType">The type of the item being dragged.</param>
        /// <returns>The new parent for the child.</returns>
        public override ModelItem RedirectParent(ModelItem parent, Type childType)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            if (childType == null)
            {
                throw new ArgumentNullException("childType");
            }

            if (parent.Content != null && parent.Content.IsSet)
            {
                // if Expander has a content and if the content is a panel or a contentcontrol, let the content act as the parent
                ModelItem content = parent.Content.Value;
                if (content != null &&
                    (content.IsItemOfType(PlatformTypes.Panel.TypeId) ||
                     content.IsItemOfType(PlatformTypes.ContentControl.TypeId)))
                {
                    return content;
                }
                else
                {
                    _canParent = false;
                    return parent; // Expander has a content already but its neither Panel nor ContentControl.
                }
            }

            // Expander doesnt have any content and now Expander itself acts as the parent
            return base.RedirectParent(parent, childType);
        }

        /// <summary>
        /// Called when removing.
        /// </summary>
        /// <param name="currentParent">The current parent.</param>
        /// <param name="newParent">The new parent.</param>
        /// <param name="child">The child being re-parented.</param>
        public override void RemoveParent(ModelItem currentParent, ModelItem newParent, ModelItem child)
        {
            if (currentParent == null)
            {
                throw new ArgumentNullException("currentParent");
            }
            if (newParent == null)
            {
                throw new ArgumentNullException("newParent");
            }
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }
            if (currentParent.Content.Value == child)
            {
                currentParent.Content.ClearValue();
            }
            else
            {
                if (currentParent.IsItemOfType(PlatformTypes.HeaderedContentControl.TypeId) &&
                    currentParent.Properties[PlatformTypes.HeaderedContentControl.HeaderProperty].Value == child)
                {
                    currentParent.Properties[PlatformTypes.HeaderedContentControl.HeaderProperty].ClearValue(); // clear the header property.
                }
            }
        }
    }
}
