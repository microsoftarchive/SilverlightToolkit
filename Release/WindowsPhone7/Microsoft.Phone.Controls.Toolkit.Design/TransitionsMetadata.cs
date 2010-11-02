// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;

namespace Microsoft.Phone.Controls.Design
{
    internal class TransitionsMetadata : AttributeTableBuilder
    {
        public TransitionsMetadata()
        {
            // Type attributes

            AddCustomAttributes(typeof(ITransition), new DescriptionAttribute("Controls the behavior of transitions."));
            AddCustomAttributes(typeof(ITransition), new ToolboxBrowsableAttribute(false));

            AddCustomAttributes(typeof(NavigationTransition), new DescriptionAttribute("Has TransitionElements for the designer experiences."));
            AddCustomAttributes(typeof(NavigationTransition), new ToolboxBrowsableAttribute(false));

            AddCustomAttributes(typeof(NavigationInTransition), new DescriptionAttribute("Has navigation-in TransitionElements for the designer experiences."));
            AddCustomAttributes(typeof(NavigationInTransition), new ToolboxBrowsableAttribute(false));

            AddCustomAttributes(typeof(NavigationOutTransition), new DescriptionAttribute("Has navigation-out TransitionElements for the designer experiences."));
            AddCustomAttributes(typeof(NavigationOutTransition), new ToolboxBrowsableAttribute(false));

            AddCustomAttributes(typeof(TransitionElement), new DescriptionAttribute("Transition factory for a particular transition family."));
            AddCustomAttributes(typeof(TransitionElement), new ToolboxBrowsableAttribute(false));

            AddCustomAttributes(typeof(TransitionFrame), new DescriptionAttribute("Enables navigation transitions for PhoneApplicationPages."));
            AddCustomAttributes(typeof(TransitionFrame), new ToolboxBrowsableAttribute(false));

            AddCustomAttributes(typeof(TransitionService), new DescriptionAttribute("Provides attached properties for navigation ITransitions."));
            AddCustomAttributes(typeof(TransitionService), new ToolboxBrowsableAttribute(false));

            // Property attributes

            AddCustomAttributes(typeof(NavigationTransition), "Backward", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(NavigationTransition), "Backward", new DescriptionAttribute("Gets or sets the backward NavigationTransition."));
            AddCustomAttributes(typeof(NavigationTransition), "Forward", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(NavigationTransition), "Forward", new DescriptionAttribute("Gets or sets the forward NavigationTransition."));

            AddCustomAttributes(typeof(NavigationInTransition), "Backward", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(NavigationInTransition), "Backward", new DescriptionAttribute("Gets or sets the backward NavigationTransition."));
            AddCustomAttributes(typeof(NavigationInTransition), "Forward", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(NavigationInTransition), "Forward", new DescriptionAttribute("Gets or sets the forward NavigationTransition."));

            AddCustomAttributes(typeof(NavigationOutTransition), "Backward", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(NavigationOutTransition), "Backward", new DescriptionAttribute("Gets or sets the backward NavigationTransition."));
            AddCustomAttributes(typeof(NavigationOutTransition), "Forward", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(NavigationOutTransition), "Forward", new DescriptionAttribute("Gets or sets the forward NavigationTransition."));
        }
    }
}