//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Navigation.UnitTests
{
    [TestClass]
    public class NavigationCacheTests : SilverlightTest
    {
        private Page GeneratePage(int i)
        {
            return new Page() { Title = GenerateTitle(i) };
        }

        private string GenerateTitle(int i)
        {
            return "Page " + i.ToString();
        }

        private string GenerateUri(int i)
        {
            return "/" + i.ToString() + ".xaml";
        }

        private void BulkAddToCache(NavigationCache cache, int startIndex, int numItemsToAdd)
        {
            for (int i = startIndex; i < startIndex + numItemsToAdd; i++)
            {
                cache.AddToCache(GenerateUri(i), GeneratePage(i));
            }
        }

        [TestMethod]
        [Description("The NavigationCache does not exceed its stated size when too many items are added")]
        public void CacheDoesNotExceedSize()
        {
            NavigationCache cache = new NavigationCache(5);

            BulkAddToCache(cache, 0, 20);

            Assert.AreEqual(5, cache.CachePagesSize);
            Assert.AreEqual(5, cache.CacheMRUPagesSize);
        }

        [TestMethod]
        [Description("The NavigationCache should keep all of its contents when the size is increased, and allow adding more items after that")]
        public void CacheSizeIncreasedDoesNotAffectContents()
        {
            NavigationCache cache = new NavigationCache(5);

            // Add items 0-2 to the cache
            BulkAddToCache(cache, 0, 3);

            cache.ChangeCacheSize(10);

            // Check that items 0-2 are still in the cache
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(GenerateTitle(i), cache[GenerateUri(i)].Title);
            }

            BulkAddToCache(cache, 3, 5);

            // Check that items 0-7 are still in the cache - should be possible because cache size was increased
            for (int i = 0; i < 8; i++)
            {
                Assert.AreEqual(GenerateTitle(i), cache[GenerateUri(i)].Title);
            }
        }

        [TestMethod]
        [Description("The NavigationCache should discard the oldest contents first when the cache size is reduced")]
        public void CacheSizeDecreasedRemovesOldestContents()
        {
            NavigationCache cache = new NavigationCache(5);

            BulkAddToCache(cache, 0, 5);

            cache.ChangeCacheSize(2);

            Assert.AreEqual(2, cache.CacheMRUPagesSize);
            Assert.AreEqual(2, cache.CachePagesSize);

            // Check that items 3-4 are the only ones still in the cache
            // 0-2 should have been removed because they were the oldest contents
            for (int i = 0; i <= 2; i++)
            {
                Assert.IsNull(cache[GenerateUri(i)]);
            }
            for (int i = 3; i <= 4; i++)
            {
                Assert.AreEqual(GenerateTitle(i), cache[GenerateUri(i)].Title);
            }
        }

        [TestMethod]
        [Description("The NavigationCache should move an existing entry to the top when it is re-added")]
        public void AddingToCacheWhenAlreadyPresentMovesToTop()
        {
            NavigationCache cache = new NavigationCache(5);

            // Fill up the cache completely
            BulkAddToCache(cache, 0, 5);

            // Re-add an item under Uri 3
            cache.AddToCache(GenerateUri(3), GeneratePage(100));

            // Verify items 0-4 are still present with the correct values
            Assert.AreEqual(GenerateTitle(0), cache[GenerateUri(0)].Title);
            Assert.AreEqual(GenerateTitle(1), cache[GenerateUri(1)].Title);
            Assert.AreEqual(GenerateTitle(2), cache[GenerateUri(2)].Title);
            Assert.AreEqual(GenerateTitle(100), cache[GenerateUri(3)].Title);
            Assert.AreEqual(GenerateTitle(4), cache[GenerateUri(4)].Title);

            // Reduce the cache to only one value, to prove that item 3 was at the top
            cache.ChangeCacheSize(1);

            Assert.AreEqual(GenerateTitle(100), cache[GenerateUri(3)].Title);
        }
    }
}
