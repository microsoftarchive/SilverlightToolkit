// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;

namespace System.Windows.Controls.VisualStudio.Design
{
    /// <summary>
    /// Controls how controls are added to the TabControl. 
    /// In particular redirects the addition of things that aren't TabItems 
    /// to the active TabItem rather than the TabControl itself.
    /// </summary>
    internal class TabControlParentAdapter : ParentAdapter
    {
        /// <summary>
        /// Local state.
        /// </summary>
        private bool _canParent = true;

        /// <summary>
        /// Determine whether the given control can be parented into a TabControl.
        /// </summary>
        /// <param name="parent">A ModelItem representing the parent.</param>
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
        /// Parent the given control into the TabControl.
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

            // Clear existing property values that we don't want to apply to the new container.
            child.Properties[MyPlatformTypes.FrameworkElement.MarginProperty].ClearValue();
            child.Properties[MyPlatformTypes.FrameworkElement.HorizontalAlignmentProperty].ClearValue();
            child.Properties[MyPlatformTypes.FrameworkElement.VerticalAlignmentProperty].ClearValue();

            bool childIsTabItem = child.IsItemOfType(MyPlatformTypes.TabItem.TypeId);

            // We always accept parenting in TabControl if there is not active focused Task.
            // If the control being pasted is not TabItem and TabControl is empty (doesnt have any TabItem(s) in it)
            // then we inject a TabItem with Grid in TabControl and paste the control under consideration in TabItem.
            // We inject a TabItem also in cases when active TabItem is not capable of parenting concerned control
            if (!childIsTabItem)
            {
                // Based on evaluation done in RedirectParent(), 
                // if any control other than TabItem is being parented to Control in this Parent() call
                // then we need to add a new Tabitem with Grid in it

                try
                {
                    ModelItem newTabItem = ModelFactory.CreateItem(parent.Context, MyPlatformTypes.TabItem.TypeId, CreateOptions.InitializeDefaults);
                    parent.Content.Collection.Add(newTabItem);

                    // activate the newly added tabItem
                    TabItemDesignModeValueProvider.SetDesignTimeIsSelected(newTabItem, true);

                    int index = parent.Content.Collection.Count - 1;
                    // Find a suitable parent for control to be pasted.
                    // Since we always accept parenting for TabControl when there is no active focused task,
                    // we better make sure this works.
                    AdapterService adapterService = parent.Context.Services.GetService<AdapterService>();
                    if (adapterService != null)
                    {
                        ModelItem targetParent = FindSuitableParent(adapterService, parent, child.ItemType, index);
                        if (targetParent != null)
                        {
                            ParentAdapter parentAdapter = adapterService.GetAdapter<ParentAdapter>(targetParent.ItemType);
                            parentAdapter.Parent(targetParent, child);
                        }
                        else
                        {
                            Debug.Assert(targetParent != null, "Parenting failed!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Parenting Failed", ex.InnerException);
                }
            }
            else
            {
                // child being added is a TabItem;                
                child.Properties[MyPlatformTypes.TabItem.IsSelectedProperty].ClearValue();
                parent.Content.Collection.Add(child);
                TabItemDesignModeValueProvider.SetDesignTimeIsSelected(child, true);       // Activate the newly added tabItem         
            }
        }

        /// <summary>
        /// If the control being added is TabItem then add it as sibling of existing TabItems;
        /// otherwise redirect the add to the active TabItem.
        /// </summary>
        /// <param name="parent">The parent item.</param>
        /// <param name="childType">The type of child item.</param>
        /// <returns>A redirected parent.</returns>
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

            // if the control being pasted is tabItem then add it as sibling of existing tabItems;
            // else redirect it to active tabItem.
            if (ModelFactory.ResolveType(parent.Context, MyPlatformTypes.TabItem.TypeId).IsAssignableFrom(childType))
            {
                return parent; // TabControl acts as parent of this tabItem being parented.
            }
            else
            {
                _canParent = false;

                if (parent.Context != null)
                {
                    Tool tool = parent.Context.Items.GetValue<Tool>();
                    if (tool != null && tool.FocusedTask == null)
                    {
                        if (parent.Content != null && parent.Content.Collection.Count > 0)
                        {
                            // TabControl has tabItem(s) in it
                            int index = TabControlDesignModeValueProvider.GetDesignTimeSelectedIndex(parent); // get hold of active tab
                            if (index == -1)
                            {
                                // TabControl has tabItems, but no selection. Select first TabItem.
                                TabControlDesignModeValueProvider.SetDesignTimeSelectedIndex(parent, 0); // select first tabItem
                                index = 0;
                            }

                            // Check if the active TabItem is capable of parenting the control. 
                            // If not, we need to inject a TabItem with Grid in it,
                            // and this new tabItem will take care of parenting the concerned control(childType).

                            AdapterService adapterService = parent.Context.Services.GetService<AdapterService>();
                            if (adapterService != null)
                            {
                                ModelItem targetParent = FindSuitableParent(adapterService, parent, childType, index);
                                if (targetParent != null)
                                {
                                    return targetParent;
                                }
                                else
                                {
                                    _canParent = true;
                                    return parent; // return TabControl. If the active tab is not capable of parenting the new control, we'll inject another tabItem with Grid in TabControl
                                }
                            }
                        }
                        else
                        {
                            if (parent.Content.Collection.Count == 0)
                            {
                                // tabControl is empty. Inject a TabItem with Grid in it
                                _canParent = true;
                                return parent; // return TabControl
                            }
                        }
                    }
                    else
                    {
                        // Task is going on
                        if (parent.Content != null && parent.Content.Collection.Count > 0)
                        {
                            int index = TabControlDesignModeValueProvider.GetDesignTimeSelectedIndex(parent);
                            if (index == -1)
                            {
                                // TabControl has tabItems, but no selection ..so select
                                TabControlDesignModeValueProvider.SetDesignTimeSelectedIndex(parent, 0);
                                index = 0;
                            }
                            return parent.Content.Collection[index];
                        }
                        else
                        {
                            // TabControl has no TabItems in it. Cannot parent
                            return parent;
                        }
                    }
                }
            }
            return base.RedirectParent(parent, childType);
        }

        /// <summary>
        /// When redirecting the add operation, find the parent to add the new control to.
        /// </summary>
        /// <param name="adapterService">Adapter service.</param>
        /// <param name="parent">The parent item.</param>
        /// <param name="childType">The type of child item.</param>
        /// <param name="index">Index of the last child of the parent.</param>
        /// <returns>The parent to add the new control to.</returns>
        private static ModelItem FindSuitableParent(AdapterService adapterService, ModelItem parent, Type childType, int index)
        {
            ModelItem suitableParent = null;

            if (adapterService != null)
            {
                ParentAdapter parentAdapter = adapterService.GetAdapter<ParentAdapter>(parent.Content.Collection[index].ItemType);
                Debug.Assert(parentAdapter != null, "Parent Adapter cannot be null");
                List<ModelItem> parentList = new List<ModelItem>();
                ModelItem targetParent = parent.Content.Collection[index];
                ModelItem redirectedParent = parentAdapter.RedirectParent(parent.Content.Collection[index], childType);
                parentList.Add(targetParent);
                parentList.Add(redirectedParent);
                while (redirectedParent != targetParent)
                {
                    targetParent = redirectedParent;
                    parentAdapter = adapterService.GetAdapter<ParentAdapter>(targetParent.ItemType);
                    Debug.Assert(parentAdapter != null, "Parent Adapter for Redirected Parent cannot be null");
                    redirectedParent = parentAdapter.RedirectParent(targetParent, childType);
                    if (parentList.Contains(redirectedParent))
                    {
                        break; // To avoid recursion 
                    }
                    else
                    {
                        parentList.Add(redirectedParent);
                    }
                }

                if (parentAdapter.CanParent(targetParent, childType))
                {
                    suitableParent = targetParent;
                }
            }

            return suitableParent;
        }

        /// <summary>
        /// Remove the given control from this TabControl.
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

            currentParent.Content.Collection.Remove(child);
        }
    }
}