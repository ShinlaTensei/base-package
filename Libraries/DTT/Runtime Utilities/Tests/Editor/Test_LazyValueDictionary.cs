#if TEST_FRAMEWORK

using DTT.Utils.Exceptions;
using DTT.Utils.Optimization;
using NUnit.Framework;

namespace DTT.Utils.Tests
{
    /// <summary>
    /// Tests the <see cref="LazyValueDictionary{T}"/> class.
    /// </summary>
    public class Test_LazyValueDictionary
    {
        private const float VALID_CACHE_PROPERTY_VALUE = 15f;

        /// <summary>
        /// A test class to test the <see cref="LazyValueDictionary{T}"/> on null values.
        /// </summary>
        private class NullNameFloatCache : LazyValueDictionary<string, float>
        {
            public float NullNameFloat => base[nameof(NullNameFloat)];

            public NullNameFloatCache() => Add(null, () => 0.0f);
        }

        /// <summary>
        /// A test class to test the <see cref="LazyValueDictionary{T}"/> on null values.
        /// </summary>
        private class NullConstructorFloatCache : LazyValueDictionary<string, float>
        {
            public float NullConstructorFloat => base[nameof(NullConstructorFloat)];

            public NullConstructorFloatCache() => Add(nameof(NullConstructorFloat), null);
        }

        /// <summary>
        /// A test class to test the <see cref="LazyValueDictionary{T}"/> on valid values.
        /// </summary>
        private class ValidConstructorFloatCache : LazyValueDictionary<string, float>
        {
            public float FloatProperty => base[nameof(FloatProperty)];

            public ValidConstructorFloatCache() => Add(nameof(FloatProperty), () => VALID_CACHE_PROPERTY_VALUE);
        }

        #region Tests
        /// <summary>
        /// Tests whether the <see cref="LazyValueDictionary{T}"/> can handle the
        /// addition of property names with a null value. It expects a 
        /// <see cref="LazyDictionaryException"/> to be thrown when a 
        /// property name with a null value is added.
        /// </summary>
        [Test]
        public void Test_Add_Null_NameOfProperty()
        {
            TestDelegate action = () =>
            {
                NullNameFloatCache cache = new NullNameFloatCache();
            };

            Assert.Catch<LazyDictionaryException>(action, "Expected are TypeSafeCacheException to be thrown but it wasn't.");
        }

        /// <summary>
        /// Tests whether the <see cref="LazyValueDictionary{T}"/> can handle the
        /// addition of constructors with a null value. It expects a 
        /// <see cref="LazyDictionaryException"/> to be thrown when a 
        /// constructor with a null value is added.
        /// </summary>
        [Test]
        public void Test_Add_Null_Constructor()
        {
            TestDelegate action = () =>
            {
                NullConstructorFloatCache cache = new NullConstructorFloatCache();
            };

            Assert.Catch<LazyDictionaryException>(action, "Expected are TypeSafeCacheException to be thrown but it wasn't.");
        }

        /// <summary>
        /// Tests whether the <see cref="LazyValueDictionary{T}"/> class works with
        /// valid construction. It expects a valid value to be returned
        /// without any exceptions being thrown.
        /// </summary>
        [Test]
        public void Test_Add_Correct_Item()
        {
            // Arrange.
            ValidConstructorFloatCache cache = new ValidConstructorFloatCache();

            // Act.
            float value = cache.FloatProperty;

            // Assert.
            Assert.AreEqual(value, VALID_CACHE_PROPERTY_VALUE, "Expected the cached value to be returned but it wasn't.");
        }
        #endregion
    }
}

#endif