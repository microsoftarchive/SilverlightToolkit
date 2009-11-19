// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace System.ComponentModel.Composition.Packaging
{
    [TestClass]
    public class PackageTests
    {
        private Uri GetDummyUri()
        {
            return new Uri("PackageTestUri", UriKind.Relative);
        }

        [TestMethod]
        public void Constructor_Valid()
        {
            var package = new Package(GetDummyUri(), new Assembly[0]);

            Assert.AreEqual(GetDummyUri(), package.Uri);
            Assert.AreEqual(0, package.Assemblies.Count());
        }

        [TestMethod]
        public void Constructor_NullUri_ShouldThrow()
        {
            ExceptionAssert.ThrowsArgumentNull("packageUri", () =>
                new Package(null, new Assembly[0]));
        }

        [TestMethod]
        public void Constructor_NullAssemblyList_ShouldThrow()
        {
            ExceptionAssert.ThrowsArgumentNull("assemblies", () =>
                new Package(GetDummyUri(), null));
        }

        [TestMethod]
        public void DownloadPackageAsync_NullUri_ShouldThrow()
        {
            ExceptionAssert.ThrowsArgumentNull("packageUri", () =>
                Package.DownloadPackageAsync(null, null));
        }

        // Don't have an easy way to mock WebClient so there aren't any other
        // tests targetting the DownloadPackageAsync method.
    }
}
