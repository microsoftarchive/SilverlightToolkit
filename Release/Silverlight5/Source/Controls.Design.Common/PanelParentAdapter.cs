// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// ParentAdapter for Panels.
    /// </summary>
    internal class PanelParentAdapter : ParentAdapter
    {
        /// <summary>
        /// Identifier of the target panel.
        /// </summary>
        private TypeIdentifier _targetPanelType;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PanelParentAdapter() : this(PlatformTypes.Panel.TypeId) { }

        /// <summary>
        /// Constructor of PanelParentAdapter.
        /// </summary>
        /// <param name="targetPanelType">The TypeIdentifier of the Panel.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Copied from Cider.")]
        public PanelParentAdapter(TypeIdentifier targetPanelType)
        {
            _targetPanelType = targetPanelType;
        }

        /// <summary>
        /// Gets the set of properties specific to the container
        /// that should be reset when you parent to a different container type.
        /// </summary>
        protected virtual IEnumerable<PropertyIdentifier> ContainerSpecificProperties
        {
            get
            {
                yield break;
            }
        }

        /// <summary>
        /// Gets the set of properties to clear before your placement adapter gets executed.
        /// </summary>
        protected virtual IEnumerable<PropertyIdentifier> PropertiesToClearBeforeAdd
        {
            get
            {
                yield return PlatformTypes.FrameworkElement.MarginProperty;
                yield return PlatformTypes.FrameworkElement.HorizontalAlignmentProperty;
                yield return PlatformTypes.FrameworkElement.VerticalAlignmentProperty;
            }
        }

        /// <summary>
        /// Gets the set of properties to clear.
        /// </summary>
        protected virtual IEnumerable<PropertyIdentifier> PropertiesToClearBeforeRemove
        {
            get
            {
                yield return PlatformTypes.FrameworkElement.MarginProperty;
            }
        }

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

            return parent.IsItemOfType(_targetPanelType);
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
            OnBeforeAdd(parent, child);
            parent.Properties["Children"].Collection.Add(child);
        }

        /// <summary>
        /// Called before add.
        /// </summary>
        /// <param name="parent">The panel.</param>
        /// <param name="child">The child being parented.</param>
        protected virtual void OnBeforeAdd(ModelItem parent, ModelItem child)
        {
            foreach (PropertyIdentifier prop in PropertiesToClearBeforeAdd)
            {
                ModelProperty property = child.Properties.Find(prop);
                if (property != null)
                {
                    property.ClearValue();
                }
            }
        }

        /// <summary>
        /// Called before remove.
        /// </summary>
        /// <param name="currentParent">The current parent.</param>
        /// <param name="newParent">The new parent.</param>
        /// <param name="child">The child being re-parented.</param>
        protected virtual void OnBeforeRemove(ModelItem currentParent, ModelItem newParent, ModelItem child)
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

            // Remove canvas properties if our new parent is not the same kind of panel
            if (!newParent.IsItemOfType(_targetPanelType))
            {
                foreach (PropertyIdentifier prop in ContainerSpecificProperties)
                {
                    ModelProperty property = child.Properties.Find(prop);
                    if (property != null)
                    {
                        property.ClearValue();
                    }
                }
            }
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

            OnBeforeRemove(currentParent, newParent, child);
            currentParent.Properties["Children"].Collection.Remove(child);
        }
    }
}
