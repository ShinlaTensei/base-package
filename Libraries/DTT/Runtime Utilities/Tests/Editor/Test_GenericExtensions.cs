#if TEST_FRAMEWORK

using DTT.Utils.Extensions;
using NUnit.Framework;
using System;

namespace DTT.Utils.Tests
{
    /// <summary>
    /// Tests the <see cref="GenericExtensions"/> class.
    /// </summary>
    public class Test_GenericExtensions
    {
        /// <summary>
        /// Tests whether a faulty value can be caught with an exception.
        /// It expects an exception to be thrown if the value is equal to the given faulty one.
        /// </summary>
        [Test]
        public void Test_ThrowIf_Faulty()
        {
            // Arrange.
            int value = -1;

            // Act.
            TestDelegate action = () => value.ThrowIf(-1);

            // Assert.
            Assert.Catch<InvalidOperationException>(action, "Expected the faulty value to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether a faulty value can be caught with an exception.
        /// It expects a custom exception to be thrown if the value is equal to the given faulty one
        /// and a custom exception is specified.
        /// </summary>
        [Test]
        public void Test_ThrowIf_Faulty_Custom_Exception()
        {
            // Arrange.
            int value = -1;

            // Act.
            TestDelegate action = () => value.ThrowIf(-1, new NotImplementedException());

            // Assert.
            Assert.Catch<NotImplementedException>(action, "Expected the custom exception to be thrown but it wasn't.");
        }

        /// <summary>
        /// Tests whether a faulty value can be caught with an exception.
        /// It expects no exception to be thrown if the value isn't equal to the given faulty one.
        /// </summary>
        [Test]
        public void Test_ThrowIf_Correct()
        {
            // Arrange.
            int value = 0;

            // Act.
            TestDelegate action = () => value.ThrowIf(-1);

            // Assert.
            Assert.DoesNotThrow(action, "Expected the faulty value to not cause an exception but it did.");
        }

        /// <summary>
        /// Tests whether a faulty value can be replaced with a default.
        /// It expects a replacement to be returned if the value is equal to the given faulty one.
        /// </summary>
        [Test]
        public void Test_ReplaceIf_Faulty()
        {
            // Arrange.
            int value = -1;

            // Act.
            int replacement = value.ReplaceIf(-1);

            // Assert.
            Assert.AreEqual(default(int), replacement, "Expected the value to be replaced by the default but it didn't.");
        }

        /// <summary>
        /// Tests whether a faulty value can be replaced with a default.
        /// It expects a replacement to be returned if the value is equal to the given faulty one.
        /// </summary>
        [Test]
        public void Test_ReplaceIf_Faulty_Custom_Default()
        {
            // Arrange.
            int value = -1;
            int customDefault = 5;

            // Act.
            int replacement = value.ReplaceIf(-1, customDefault);

            // Assert.
            Assert.AreEqual(customDefault, replacement, "Expected the custom default value to be returned but it wasn't.");
        }

        /// <summary>
        /// Tests whether a faulty value can be replaced with a default.
        /// It expects the original value to be returned if the value is not equal to the given faulty one.
        /// </summary>
        [Test]
        public void Test_ReplaceIf_Correct()
        {
            // Arrange.
            int value = 2;

            // Act.
            int replacement = value.ReplaceIf(-1);

            // Assert.
            Assert.AreEqual(value, replacement, "Expected the value to not be replaced but it was.");
        }
    }
}

#endif