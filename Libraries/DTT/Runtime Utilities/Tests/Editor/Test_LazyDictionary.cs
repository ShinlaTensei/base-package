#if TEST_FRAMEWORK

using DTT.Utils.Exceptions;
using DTT.Utils.Optimization;
using NUnit.Framework;
using System;

namespace DTT.Utils.Tests
{
    /// <summary>
    /// Tests the <see cref="LazyDictionary{T}"/> class.
    /// </summary>
    public class Test_LazyDictionary
    {
        /// <summary>
        /// A constructable safe cache with a failing retrieval of a cached item.
        /// </summary>
        private class FailingCache : LazyDictionary<string, string>
        {
            public string MyProperty => base[nameof(MyProperty)];
        }

        /// <summary>
        /// A constructable safe cached that only returns one cached value.
        /// </summary>
        private class SucceedingCache : LazyDictionary<string, string>
        {
            public string MyProperty => base[nameof(MyProperty)];

            public const string PROPERTY_VALUE = "My constructed string.";

            public SucceedingCache()
            {
                Add(nameof(MyProperty), () => PROPERTY_VALUE);
            }
        }

        [Test]
        public void Test_Add_Null_PropertyName()
        {
            // Arrange.
            LazyDictionary<string, string> cache = new LazyDictionary<string, string>();

            // Act.
            TestDelegate action = () => cache.Add(null, () => "My constructed string.");

            // Assert.
            Assert.Catch<ArgumentNullException>(action, "Expected the null property name to cause an exception but it didn't.");
        }

        [Test]
        public void Test_Add_Null_Constructor()
        {
            // Arrange.
            LazyDictionary<string, string> cache = new LazyDictionary<string, string>();

            // Act.
            TestDelegate action = () => cache.Add("propertyName", null);

            // Assert.
            Assert.Catch<ArgumentNullException>(action, "Expected the null constructor to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether property retrieval can be done properly and with feedback.
        /// It expects a <see cref="LazyDictionaryException"/> to be thrown if the retrieval failed.
        /// </summary>
        [Test]
        public void Test_Implementation_PropertyRetrieval_Fail()
        {
            // Arrange.
            FailingCache cache = new FailingCache();

            // Act.
            TestDelegate action = () =>
            {
                string value = cache.MyProperty;
            };

            // Assert.
            Assert.Catch<LazyDictionaryException>(action, "Expected the unimplemented retrieval of cached item to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether property retrieval can be done properly and with feedback.
        /// It expects a cached value to be returned if the retrieval is properly.
        /// </summary>
        [Test]
        public void Test_Implementation_PropertyRetrieval_Succes()
        {
            // Arrange.
            SucceedingCache cache = new SucceedingCache();

            // Act.
            string value = cache.MyProperty;

            // Assert.
            Assert.AreEqual(SucceedingCache.PROPERTY_VALUE, value, "Expected the value to be retrieved from the cache.");
        }
    }
}

#endif