// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Markup;

[assembly: AssemblyTitle("Microsoft.Phone.Controls.Toolkit")]
[assembly: AssemblyDescription("Windows Phone Toolkit")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Microsoft Corporation")]
[assembly: AssemblyProduct("Microsoft® Windows Phone")]
[assembly: AssemblyCopyright("© Microsoft Corporation.  All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("c3074b8e-8b70-4c3a-8a8b-e0cc41a244ae")]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: CLSCompliant(false)] // IApplicationBar is not CLS-compliant, but its use matches the type of the platform's PhoneApplicationPage.ApplicationBar property
[assembly: NeutralResourcesLanguage("en-US")]

[assembly: XmlnsPrefix("clr-namespace:Microsoft.Phone.Controls;assembly=Microsoft.Phone.Controls.Toolkit", "toolkit")]
[assembly: XmlnsDefinition("clr-namespace:Microsoft.Phone.Controls;assembly=Microsoft.Phone.Controls.Toolkit", "Microsoft.Phone.Controls")]
[assembly: XmlnsPrefix("clr-namespace:Microsoft.Phone.Controls.Primitives;assembly=Microsoft.Phone.Controls.Toolkit", "toolkitPrimitives")]
[assembly: XmlnsDefinition("clr-namespace:Microsoft.Phone.Controls.Primitives;assembly=Microsoft.Phone.Controls.Toolkit", "Microsoft.Phone.Controls.Primitives")]
