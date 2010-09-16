using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Microsoft.Phone.Controls.Toolkit")]
[assembly: AssemblyDescription("Windows Phone Toolkit")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Microsoft Corporation")]
[assembly: AssemblyProduct("Microsoft® Windows Phone")]
[assembly: AssemblyCopyright("© Microsoft Corporation.  All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("c3074b8e-8b70-4c3a-8a8b-e0cc41a244ae")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: CLSCompliant(false)] // IApplicationBar is not CLS-compliant, but its use matches the type of the platform's PhoneApplicationPage.ApplicationBar property
[assembly: NeutralResourcesLanguage("en-US")]

[assembly: XmlnsPrefix("clr-namespace:Microsoft.Phone.Controls;assembly=Microsoft.Phone.Controls.Toolkit", "toolkit")]
[assembly: XmlnsDefinition("clr-namespace:Microsoft.Phone.Controls;assembly=Microsoft.Phone.Controls.Toolkit", "Microsoft.Phone.Controls")]
[assembly: XmlnsPrefix("clr-namespace:Microsoft.Phone.Controls.Primitives;assembly=Microsoft.Phone.Controls.Toolkit", "toolkitPrimitives")]
[assembly: XmlnsDefinition("clr-namespace:Microsoft.Phone.Controls.Primitives;assembly=Microsoft.Phone.Controls.Toolkit", "Microsoft.Phone.Controls.Primitives")]
