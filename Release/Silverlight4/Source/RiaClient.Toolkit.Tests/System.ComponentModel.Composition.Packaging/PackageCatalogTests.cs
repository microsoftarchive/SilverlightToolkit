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
    public class PackageCatalogTests
    {
        [Export]
        public class DummyExport // Needed so that there is at least one part in this catalog
        {
        }

        [TestMethod]
        public void Constructor_ValidateTest()
        {
            var packageCatalog = new PackageCatalog();

            Assert.AreEqual(0, packageCatalog.Packages.Count());
            Assert.AreEqual(0, packageCatalog.Parts.Count());
        }

        [TestMethod]
        public void AddPackage_SinglePackage_ShouldBeFine()
        {
            var packageCatalog = new PackageCatalog();

            var package1 = CreateDefaultPackage();

            packageCatalog.AddPackage(package1);

            Assert.AreEqual(1, packageCatalog.Packages.Count());
            Assert.AreEqual(package1, packageCatalog.Packages.First());
            Assert.IsTrue(packageCatalog.Parts.Count() > 0);
        }

        [TestMethod]
        public void AddPackage_MultipleSamePackage_ShouldBeNoop()
        {
            var packageCatalog = new PackageCatalog();

            var package1 = CreateDefaultPackage();
            var package2 = CreateDefaultPackage();

            packageCatalog.AddPackage(package1);

            Assert.AreEqual(1, packageCatalog.Packages.Count());
            Assert.AreEqual(package1, packageCatalog.Packages.First());
            var partCount = packageCatalog.Parts.Count();

            packageCatalog.AddPackage(package2);

            Assert.AreEqual(1, packageCatalog.Packages.Count());
            Assert.AreEqual(package1, packageCatalog.Packages.First());
            Assert.AreEqual(partCount, packageCatalog.Parts.Count(), "Part count should be the same because both packages are the same");

        }

        [TestMethod]
        public void AddPackage_MultipleDifferentPackagesWithSameAssembly_ShouldOnlyAddDifference()
        {
            var packageCatalog = new PackageCatalog();

            var package1 = CreateDefaultPackage();
            var package2 = CreatePackage("UnitTestPackage2", typeof(PackageCatalogTests).Assembly);

            packageCatalog.AddPackage(package1);

            Assert.AreEqual(1, packageCatalog.Packages.Count());
            Assert.AreEqual(package1, packageCatalog.Packages.First());
            var partCount = packageCatalog.Parts.Count();

            packageCatalog.AddPackage(package2);

            Assert.AreEqual(2, packageCatalog.Packages.Count());
            Assert.AreEqual(package1, packageCatalog.Packages.First());
            Assert.AreEqual(package2, packageCatalog.Packages.ElementAt(1));
            Assert.AreEqual(partCount, packageCatalog.Parts.Count(), "Part count should remain same because both packages added same assembly");
        }

        [TestMethod]
        public void AddPackage_SinglePackage_ShouldFireEvents()
        {
            var packageCatalog = new PackageCatalog();

            var package1 = CreateDefaultPackage();

            int changingCount = 0; 
            packageCatalog.Changing += (s, e) =>
                {
                    Assert.IsNotNull(e.AtomicComposition);
                    Assert.IsTrue(e.AddedDefinitions.Count() > 0);
                    Assert.IsTrue(e.RemovedDefinitions.Count() == 0);
                    changingCount++;
                };

            int changedCount = 0;
            packageCatalog.Changed += (s, e) =>
                {
                    Assert.IsNull(e.AtomicComposition);
                    Assert.IsTrue(e.AddedDefinitions.Count() > 0);
                    Assert.IsTrue(e.RemovedDefinitions.Count() == 0);
                    changedCount++;
                };

            packageCatalog.AddPackage(package1);

            Assert.AreEqual(1, packageCatalog.Packages.Count());
            Assert.AreEqual(package1, packageCatalog.Packages.First());
            Assert.IsTrue(packageCatalog.Parts.Count() > 0);

            Assert.IsTrue(changingCount == 1);
            Assert.IsTrue(changedCount == 1);
        }

        [TestMethod]
        public void AddPackage_Disposed_ShouldThrow()
        {
            var packageCatalog = new PackageCatalog();

            packageCatalog.Dispose();

            ExceptionAssert.ThrowsDisposed(() =>
                packageCatalog.AddPackage(CreateDefaultPackage()));
        }

        [TestMethod]
        public void Parts_Disposed_ShouldThrow()
        {
            var packageCatalog = new PackageCatalog();

            packageCatalog.Dispose();

            ExceptionAssert.ThrowsDisposed(() =>
                packageCatalog.Parts.Count());
        }
        
        private Package CreateDefaultPackage()
        {
            return CreatePackage("UnitTestPackage", typeof(PackageCatalogTests).Assembly);
        }

        private Package CreatePackage(string uri, Assembly assembly)
        {
            return new Package(new Uri(uri, UriKind.RelativeOrAbsolute), new Assembly[] { assembly }); 
        }
    }
}
