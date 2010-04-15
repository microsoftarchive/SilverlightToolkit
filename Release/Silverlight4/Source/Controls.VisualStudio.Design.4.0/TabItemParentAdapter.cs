// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;

namespace System.Windows.Controls.VisualStudio.Design
{
    /// <summary>
    /// Controls how controls are added to TabItem.
    /// </summary>
    internal class TabItemParentAdapter : ParentAdapter
    {
        /// <summary>
        /// State field.
        /// </summary>
        private bool _canParent = true;

        /// <summary>
        /// Determine whether the given control can be parented into a TabItem.
        /// </summary>
        /// <param name="parent">ModelItem representing the parent.</param>
        /// <param name="childType">The type of child item.</param>
        /// <returns>true if the specified parent can accept a child of the specified type; otherwise, false.</returns>
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
        /// Parent the given control into the TabItem.
        /// </summary>
        /// <param name="parent">The new parent item for child.</param>
        /// <param name="child">The child item.</param>
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

            child.Properties[MyPlatformTypes.FrameworkElement.MarginProperty].ClearValue();
            child.Properties[MyPlatformTypes.FrameworkElement.HorizontalAlignmentProperty].ClearValue();
            child.Properties[MyPlatformTypes.FrameworkElement.VerticalAlignmentProperty].ClearValue();

            parent.Content.SetValue(child);
        }

        /// <summary>
        /// Determine how the control is parented into the TabItem.
        /// If the control is a TabItem then parent it to the TabControl rather 
        /// than the TabItem (think select, copy, paste a TabItem).
        /// If the TabItem has content and the content can accept children 
        /// then parent into that and so on.
        /// </summary>
        /// <param name="parent">The parent item.</param>
        /// <param name="childType">The type of child item.</param>
        /// <returns>Redirected parent.</returns>
        public override ModelItem RedirectParent(ModelItem parent, Type childType)
        {
            _canParent = true;

            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            if (childType == null)
            {
                throw new ArgumentNullException("childType");
            }

            // if the tabItem is not activated and/or not within TabControl, 
            // redirect to its parent by returning ourselves.
            if (parent.IsItemOfType(MyPlatformTypes.TabItem.TypeId)
                && parent.Parent != null
                && (!parent.Parent.IsItemOfType(MyPlatformTypes.TabControl.TypeId) || !TabItemDesignModeValueProvider.GetDesignTimeIsSelected(parent)))
            {
                _canParent = false;
                return parent;
            }

            // if element being parented is tabItem then add it as sibling of 
            // existing tabItems inside TabControl - redirect to parent;
            // else parent this control in TabItem's content, if its Panel.       
            if (ModelFactory.ResolveType(parent.Context, MyPlatformTypes.TabItem.TypeId).IsAssignableFrom(childType))
            {
                _canParent = false;
                return parent;
            }

            if (parent.Content != null && parent.Content.IsSet)
            {
                // if TabItem has a content and if the content is a panel or a contentcontrol, 
                // let the content act as the parent.
                ModelItem content = parent.Content.Value;
                if (content != null)
                {
                    ViewItem contentView = content.View;
                    if (content.View != null)
                    {
                        // if the content is not visible but the tabItem is selected, 
                        // Update this TabItems Layout.
                        if (!contentView.IsVisible && TabItemDesignModeValueProvider.GetDesignTimeIsSelected(parent))
                        {
                            parent.View.UpdateLayout();
                        }

                        if (contentView.IsVisible &&
                            (content.IsItemOfType(MyPlatformTypes.Panel.TypeId) || content.IsItemOfType(MyPlatformTypes.ContentControl.TypeId)))
                        {
                            return content;
                        }
                    }
                    else
                    {
                        // TabItem has a content already but the content cannot accept parenting right now
                        // so let tabItem's parent take care of parenting.
                        _canParent = false;
                        return parent;
                    }
                }
            }

            // TabItem doesnt have any content and now tabItem itself acts as the parent.
            return base.RedirectParent(parent, childType);
        }

        /// <summary>
        /// Remove the given control from this TabItem.
        /// </summary>
        /// <param name="currentParent">The item that is currently the parent of child.</param>
        /// <param name="newParent">The item that will become the new parent of child.</param>
        /// <param name="child">The child item.</param>
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

            currentParent.Content.ClearValue();
        }
    }
}