#if TEST_FRAMEWORK

using DTT.Utils.Workflow;
using NUnit.Framework;
using System;

namespace DTT.Utils.Tests
{
    /// <summary>
    /// Tests the <see cref="Unguaranteed{T}"/> class.
    /// </summary>
    public class Test_Unguaranteed
    {
        /// <summary>
        /// Tests whether the value can be accessed if it hasn't been created.
        /// It expects a <see cref="InvalidOperationException"/> to be trown 
        /// if the value hasn't been created.
        /// </summary>
        [Test]
        public void Test_Value_Acces_Not_Created()
        {
            // Arrange.
            Unguaranteed<object> obj = new Unguaranteed<object>(() => null);

            // Act.
            TestDelegate action = () =>
            {
                object value = obj.Value;
            };

            // Assert.
            Assert.Catch<InvalidOperationException>(action,
                "Expected an invalid operation exception because the value wasn't created but there wasn't.");
        }

        /// <summary>
        /// Tests whether the value can be accessed if it hasn't been created.
        /// It expects a <see cref="InvalidOperationException"/> to be thrown 
        /// if the value hasn't been created.
        /// </summary>
        [Test]
        public void Test_Value_Acces_Created()
        {
            // Arrange.
            Unguaranteed<string> obj = new Unguaranteed<string>(() => string.Empty);

            // Act.
            string value = obj.Value;

            // Assert.
            Assert.AreEqual(string.Empty, value, "Expected the constructed value to be assigned but it wasn't.");
        }

        /// <summary>
        /// Tests whether the Unguanteed class can be constructed with 
        /// an constructor that is null. It expects a <see cref="ArgumentNullException"/> to be thrown.
        /// </summary>
        [Test]
        public void Test_Creation_Null_Constructor()
        {
            // Arrange.
            TestDelegate action = () =>
            {
                // Act.
                Unguaranteed<string> obj = new Unguaranteed<string>(null);
            };

            // Assert.
            Assert.Catch<ArgumentNullException>(action,
                "Expected the creation of the value with a null constructor to cause an exception but it didn't.");
        }
    }
}

#endif