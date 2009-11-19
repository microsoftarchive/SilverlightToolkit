// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Packaging
{
    /// <summary>
    ///     This type is dependent on the Package class, which is currently an experimental API
    ///     added in this assembly. Which makes this API also an experimental API and will likely
    ///     change if it ever ships as part of Silverlight.
    /// </summary>
    public class PackageCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        private object _lockObject = new object();
        private Dictionary<Uri, Package> _packages = new Dictionary<Uri, Package>();
        private Dictionary<string, object> _loadedAssemblies = new Dictionary<string, object>();
        private List<ComposablePartDefinition> _parts = new List<ComposablePartDefinition>();
        private IQueryable<ComposablePartDefinition> _partsQuery;
        private volatile bool _isDisposed = false;

        /// <summary>
        ///     Construct a PackageCatlaog object.
        /// </summary>
        public PackageCatalog()
        {
            this._partsQuery = this._parts.AsQueryable();
        }

        /// <summary>
        ///     Adds a Package to the catalog. It will ensure that the same Package added more than
        ///     once will not cause duplication in the catalog. It will also ensure that the same
        ///     assembly appearing in multiple packages will not cause duplication in the catalog.
        /// </summary>
        /// <param name="package">
        ///     Package obtained by constructing a <see cref="Package" /> object or 
        ///     calling <see cref="Package.DownloadPackageAsync" />. 
        /// </param>
        public void AddPackage(Package package)
        {
            this.ThrowIfDisposed();

            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            List<AssemblyCatalog> addedCatalogs = new List<AssemblyCatalog>();
            ComposablePartDefinition[] addedDefinitions;

            lock (this._lockObject)
            {
                if (this._packages.ContainsKey(package.Uri))
                {
                    // Nothing to do because the package has already been added.
                    return;
                }

                foreach (Assembly assembly in package.Assemblies)
                {
                    if (!this._loadedAssemblies.ContainsKey(assembly.FullName))
                    {
                        addedCatalogs.Add(new AssemblyCatalog(assembly));
                    }
                }
            }

            addedDefinitions = addedCatalogs.SelectMany(asmCat => asmCat.Parts).ToArray<ComposablePartDefinition>();

            if (addedDefinitions.Length == 0)
            {
                // If the package doesn't contain any added definitions then simply add it to the 
                // list of known packages and then return
                lock (this._lockObject)
                {
                    if (!this._packages.ContainsKey(package.Uri))
                    {
                        this._packages.Add(package.Uri, package);
                    }
                }

                return;
            }

            // Need to raise the changing event inside an AtomicComposition to allow listeners
            // to contribute state change based on whether or not the changes to the catalog
            // are completed.
            using (var atomicComposition = new AtomicComposition())
            {
                var changingArgs = new ComposablePartCatalogChangeEventArgs(addedDefinitions, Enumerable.Empty<ComposablePartDefinition>(), atomicComposition);
                this.OnChanging(changingArgs); // throws ChangedRejectedException if these changes break the composition.

                lock (this._lockObject)
                {
                    if (this._packages.ContainsKey(package.Uri))
                    {
                        // Someone beat us to it so return and don't complete the AtomicComosition
                        return; 
                    }

                    this._packages.Add(package.Uri, package);

                    foreach (var catalog in addedCatalogs)
                    {
                        if (!this._loadedAssemblies.ContainsKey(catalog.Assembly.FullName))
                        {
                            this._loadedAssemblies.Add(catalog.Assembly.FullName, null);
                            this._parts.AddRange(catalog.Parts);
                        }
                    }
                }

                atomicComposition.Complete();
            }

            var changedArgs = new ComposablePartCatalogChangeEventArgs(addedDefinitions, Enumerable.Empty<ComposablePartDefinition>(), null);
            this.OnChanged(changedArgs);
        }

        /// <summary>
        ///     List of packages already contained in this catalog.
        /// </summary>
        public IEnumerable<Package> Packages
        {
            get
            {
                lock (this._lockObject)
                {
                    return this._packages.Values.ToArray();
                }
            }
        }

        /// <summary>
        ///     Gets the union of all the part definitions for all the packages that have
        ///     been added to this catalog.
        /// </summary>
        /// <value>
        ///     A <see cref="IQueryable{T}"/> of <see cref="ComposablePartDefinition"/> objects of the 
        ///     <see cref="PackageCatalog"/>.
        /// </value>
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get 
            {
                this.ThrowIfDisposed();

                lock (this._lockObject)
                {
                    return this._partsQuery;
                }
            }
        }

        /// <summary>
        /// Notify when the contents of the Catalog has changed.
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;

        /// <summary>
        /// Notify when the contents of the Catalog has changing.
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

        /// <summary>
        ///     Raises the <see cref="INotifyComposablePartCatalogChanged.Changed"/> event.
        /// </summary>
        /// <param name="e">
        ///     An <see cref="ComposablePartCatalogChangeEventArgs"/> containing the data for the event.
        /// </param>
        protected virtual void OnChanged(ComposablePartCatalogChangeEventArgs e)
        {
            EventHandler<ComposablePartCatalogChangeEventArgs> changedEvent = this.Changed;
            if (changedEvent != null)
            {
                changedEvent(this, e);
            }
        }

        /// <summary>
        ///     Raises the <see cref="INotifyComposablePartCatalogChanged.Changing"/> event.
        /// </summary>
        /// <param name="e">
        ///     An <see cref="ComposablePartCatalogChangeEventArgs"/> containing the data for the event.
        /// </param>
        protected virtual void OnChanging(ComposablePartCatalogChangeEventArgs e)
        {
            EventHandler<ComposablePartCatalogChangeEventArgs> changingEvent = this.Changing;
            if (changingEvent != null)
            {
                changingEvent(this, e);
            }
        }

        /// <summary>
        ///     Disposes the PackageCatalog.
        /// </summary>
        /// <param name="disposing">true if diposing; false if finalizing.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (!this._isDisposed)
                    {
                        lock (this._lockObject)
                        {
                            if (!this._isDisposed)
                            {
                                this._isDisposed = true;
                                this._packages = null;
                                this._loadedAssemblies = null;
                                this._parts = null;
                                this._partsQuery = null;
                            }
                        }
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        private void ThrowIfDisposed()
        {
            if (this._isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString()); 
            }
        }
    }
}
