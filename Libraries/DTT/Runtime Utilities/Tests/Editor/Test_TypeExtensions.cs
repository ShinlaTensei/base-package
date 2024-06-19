#if TEST_FRAMEWORK

using DTT.Utils.Extensions;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DTT.Utils.Tests
{
    /// <summary>
    /// Tests the <see cref="TypeExtensions"/> class.
    /// </summary>
    public class Test_TypeExtensions
    {
        #region Tests
        /// <summary>
        /// Tests whether the <see cref="TypeExtensions"/> class can properly
        /// handle invalid input. It expects that when checking whether a type implements
        /// an interface and the input is null, an argument null exception will be thrown.
        /// </summary>
        [Test]
        public void Test_ImplementsInterface_Null()
        {
            TestDelegate nullType = () => TypeExtensions.ImplementsInterface(null, typeof(IEnumerable));
            TestDelegate nullInterface = () => typeof(object).ImplementsInterface(null);

            Assert.Catch<ArgumentNullException>(nullType, "Expected an argument null exception to be thrown but there wasn't.");
            Assert.Catch<ArgumentNullException>(nullInterface, "Expected an argument null exception to be thrown but there wasn't.");
        }

        /// <summary>
        /// Tests whether the <see cref="TypeExtensions"/> class can properly
        /// handle invalid input. It expects that when checking whether a type implements
        /// an interface and the input is invalid, the method will return false.
        /// </summary>
        [Test]
        public void Test_ImplementsInterface_Invalid()
        {
            bool actual = typeof(object).ImplementsInterface(typeof(IEnumerable));
            bool actualTwo = typeof(object).ImplementsInterface(typeof(IEnumerable<>));

            Assert.IsFalse(actual, "Expected the object not to implement the IEnumerable interface but it did.");
            Assert.IsFalse(actualTwo, "Expected the object not to implement the IEnumerable<> interface but it did.");
        }

        /// <summary>
        /// Tests whether the <see cref="TypeExtensions"/> class can properly
        /// handle invalid input. It expects that when checking whether a type implements
        /// an interface and the input is valid, the method will return true.
        /// </summary>
        [Test]
        public void Test_ImplementsInterface_Valid()
        {
            bool actual = typeof(List<>).ImplementsInterface(typeof(IEnumerable));
            bool actualTwo = typeof(string).ImplementsInterface(typeof(IEnumerable<>));

            Assert.IsTrue(actual, "Expected the List to implement the IEnumerable interface but it didn't.");
            Assert.IsTrue(actualTwo, "Expected the string to implement the IEnumerable<> interface but it didn't.");
        }

        /// <summary>
        /// Tests whether the <see cref="TypeExtensions"/> class can properly
        /// handle invalid input. It expects that when checking when an interface type implements
        /// an interface the method will return true.
        /// </summary>
        [Test]
        public void Test_ImplementsInterface_Valid_Interface()
        {
            // Arrange.
            IEnumerable<int> enumerable = Enumerable.Range(0, 5);

            // Act.
            bool condition = enumerable.GetType().ImplementsInterface(typeof(IEnumerable));

            // Assert.
            Assert.IsTrue(condition, "Expected the generic enumerable to implement the enumerable interface but it didn't.");
        }
        #endregion
    }
}

#endif
