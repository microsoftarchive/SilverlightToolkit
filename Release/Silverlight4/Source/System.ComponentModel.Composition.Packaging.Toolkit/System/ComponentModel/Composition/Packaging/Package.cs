// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Resources;
using System.Xml;
using System.ComponentModel;

namespace System.ComponentModel.Composition.Packaging
{
    /// <summary>
    ///     This is a prototype API that we have proposed to the Silverlight team to represent a Package (aka XAP)
    ///     so that we can work on some Catalogs that specifically target Silverlight packages. It is an
    ///     experimental API and will change if it ever ships as part of Silverlight.
    /// </summary>
    public class Package
    {
        /// <summary>
        ///     Constructs package object.
        /// </summary>
        /// <param name="packageUri">Uri of the package</param>
        /// <param name="assemblies">Set of assemblies included in this package</param>
        public Package(Uri packageUri, IEnumerable<Assembly> assemblies)
        {
            if (packageUri == null)
            {
                throw new ArgumentNullException("packageUri");
            }

            if (assemblies == null)
            {
                throw new ArgumentNullException("assemblies");
            }

            this.Uri = packageUri;
            this.Assemblies = assemblies;
        }

        /// <summary>
        ///     Uri of the package.
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        ///     Set of assemblies included in this package
        /// </summary>
        public IEnumerable<Assembly> Assemblies { get; private set; }

        private static Package _current;
        /// <summary>
        ///     Retrieves a Package object for the set of assemblies loaded during the initial
        ///     application XAP load. Depends on the Deployment.Current property being setup and
        ///     so can only be accessed after the Application object has be completely constructed.
        /// </summary>
        public static Package Current
        {
            get
            {
                if (_current == null)
                {
                    var assemblies = new List<Assembly>();

                    // While this may seem like somewhat of a hack, walking the AssemblyParts in the active 
                    // deployment object is the only way to get the list of assemblies loaded by the initial XAP. 
                    foreach (AssemblyPart ap in Deployment.Current.Parts)
                    {
                        StreamResourceInfo sri = Application.GetResourceStream(new Uri(ap.Source, UriKind.Relative));
                        if (sri != null)
                        {
                            // Keep in mind that calling Load on an assembly that is already loaded will
                            // be a no-op and simply return the already loaded assembly object.
                            Assembly assembly = ap.Load(sri.Stream);
                            assemblies.Add(assembly);
                        }
                    }

                    _current = new Package(new Uri(string.Empty, UriKind.Relative), assemblies);
                }

                return _current;
            }
        }
        
        /// <summary>
        ///     Downloads a secondary XAP and loads those assemblies into the AppDomain. Currently only supports
        ///     a limited set of scenarios, namely only loads the list of AssemblyParts found in the AppManifest.xaml
        ///     file of the secondary XAP.
        ///     
        ///     List of known issues:
        ///      - Transparent Platform Extensions (TPE) aren't loaded supported.
        ///      - Resources in the XAP directly are not supported, any needed resources need to be embedded in the assembly.
        ///      - Versioning not supported. Currently if Silverlight finds another assembly with the same name but different
        ///         version it will not load the new assembly. It purely uses the assembly name to identify already loaded assemblies.
        /// 
        /// </summary>
        /// <param name="packageUri">Uri the the xap file to be downloaded, needs to be </param>
        /// <param name="packageDownloadCompleted">callback for when the package downloading completes</param>
        public static void DownloadPackageAsync(Uri packageUri, Action<AsyncCompletedEventArgs, Package> packageDownloadCompleted)
        {
            if (packageUri == null)
            {
                throw new ArgumentNullException("packageUri");
            }

            var client = new WebClient();

            client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);

            client.OpenReadAsync(packageUri, new Tuple<Uri, Action<AsyncCompletedEventArgs, Package>>(packageUri, packageDownloadCompleted));
        }

        private static void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            var tuple = (Tuple<Uri, Action<AsyncCompletedEventArgs, Package>>)e.UserState;

            var uri = tuple.Item1;
            var callback = tuple.Item2;

            Package package = null;

            if (e.Error == null && !e.Cancelled)
            {
                IEnumerable<Assembly> assemblies = LoadPackagedAssemblies(e.Result);
                package = new Package(uri, assemblies);
            }

            if (callback != null)
            {
                callback(new AsyncCompletedEventArgs(e.Error, e.Cancelled, null), package);
            }
        }
      
        private static IEnumerable<Assembly> LoadPackagedAssemblies(Stream packageStream)
        {
            List<Assembly> assemblies = new List<Assembly>();
            StreamResourceInfo packageStreamInfo = new StreamResourceInfo(packageStream, null);

            IEnumerable<AssemblyPart> parts = GetDeploymentParts(packageStreamInfo);

            foreach (AssemblyPart ap in parts)
            {
                StreamResourceInfo sri = Application.GetResourceStream(
                    packageStreamInfo, new Uri(ap.Source, UriKind.Relative));

                assemblies.Add(ap.Load(sri.Stream));
            }
            return assemblies;
        }

        /// <summary>
        ///     Only reads AssemblyParts and does not support external parts (aka Platform Extensions or TPEs).
        /// </summary>
        private static IEnumerable<AssemblyPart> GetDeploymentParts(StreamResourceInfo xapStreamInfo)
        {
            Uri manifestUri = new Uri("AppManifest.xaml", UriKind.Relative);
            StreamResourceInfo manifestStreamInfo = Application.GetResourceStream(xapStreamInfo, manifestUri);
            List<AssemblyPart> assemblyParts = new List<AssemblyPart>();

            // The code assumes the following format in AppManifest.xaml
            //<Deployment ... >
            //  <Deployment.Parts>
            //    <AssemblyPart x:Name="A" Source="A.dll" />
            //    <AssemblyPart x:Name="B" Source="B.dll" />
            //      ...
            //    <AssemblyPart x:Name="Z" Source="Z.dll" />
            //  </Deployment.Parts>
            //</Deployment>
            if (manifestStreamInfo != null)
            {
                Stream manifestStream = manifestStreamInfo.Stream;
                using (XmlReader reader = XmlReader.Create(manifestStream))
                {
                    if (reader.ReadToFollowing("AssemblyPart"))
                    {
                        do
                        {
                            string source = reader.GetAttribute("Source");

                            if (source != null)
                            {
                                assemblyParts.Add(new AssemblyPart() { Source = source });
                            }
                        }
                        while (reader.ReadToNextSibling("AssemblyPart"));
                    }
                }
            }

            return assemblyParts;
        }
    }
}
