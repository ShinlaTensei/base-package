#if TEST_FRAMEWORK

using System;
using DTT.Utils.Extensions;
using NUnit.Framework;

namespace DTT.Utils.Tests
{
    /// <summary>
    /// Tests the <see cref="EnumExtensions"/> class.
    /// </summary>
    public class Test_EnumExtensions
    {
        /// <summary>
        /// An enum type used for testing.
        /// </summary>
        public enum TestEnum
        {
            ValueOne,
            ValueTwo,
            ValueThree
        }

        /// <summary>
        /// An enum type for testing character casting.
        /// </summary>
        public enum CharacterEnum 
        {
            ValueOne = 'o',
            ValueTwo = 't',
            ValueThree = 'r'
        }
        
        /// <summary>
        /// An enum type for testing integer casting.
        /// </summary>
        public enum IntegerEnum : int
        {
            ValueOne = 0,
            ValueTwo = 1,
            ValueThree = 2
        }
        
        /// <summary>
        /// An enum type for testing byte casting.
        /// </summary>
        public enum ByteEnum : byte
        {
            ValueOne = 0,
            ValueTwo = 1,
            ValueThree = 2
        }

        /// <summary>
        /// Tests whether the next value in an enum retrieved is actually the next value.
        /// It expects that if the next value is whithin the bounds of enum values, a value
        /// corresponding to an index + 1 will be returned.
        /// </summary>
        [Test]
        public void Test_Next_Within_Bound()
        {
            // Arrange.
            TestEnum value = TestEnum.ValueOne;

            // Act. 
            TestEnum actual = value.Next();


            // Assert.
            Assert.AreEqual(TestEnum.ValueTwo, actual, "Expected the next value to be returned but it wasn't.");
        }

        /// <summary>
        /// Tests whether the next value in an enum retrieved is actually the next value.
        /// It expects that if the next value is out of bounds of the enum values, it will
        /// loop around and a alue corresponding to an index = 0 will be returned.
        /// </summary>
        [Test]
        public void Test_Next_Out_Of_Bound()
        {
            // Arrange.
            TestEnum value = TestEnum.ValueThree;

            // Act. 
            TestEnum actual = value.Next();


            // Assert.
            Assert.AreEqual(TestEnum.ValueOne, actual, "Expected the next value to loop around but it didn't.");
        }

        /// <summary>
        /// Tests whether the previous value in an enum retrieved is actually the previous value.
        /// It expects that if the previous value is whithin the bounds of enum values, a value
        /// corresponding to an index - 1 will be returned.
        /// </summary>
        [Test]
        public void Test_Previous_Within_Bound()
        {
            // Arrange.
            TestEnum value = TestEnum.ValueThree;

            // Act. 
            TestEnum actual = value.Previous();


            // Assert.
            Assert.AreEqual(TestEnum.ValueTwo, actual, "Expected the previous value to be returned but it wasn't.");
        }

        /// <summary>
        /// Tests whether the previous value in an enum retrieved is actually the previous value.
        /// It expects that if the previous value is out of bounds of the enum values, it will
        /// loop around and a value corresponding to an index = length - 1 will be returned.
        /// </summary>
        [Test]
        public void Test_Previous_Out_Of_Bound()
        {
            // Arrange.
            TestEnum value = TestEnum.ValueOne;

            // Act. 
            TestEnum actual = value.Previous();


            // Assert.
            Assert.AreEqual(TestEnum.ValueThree, actual, "Expected the previous value to loop around but it didn't.");
        }

        /// <summary>
        /// Tests whether an enum can be cast to an character.
        /// It expects that if the enum value is null, an <see cref="ArgumentNullException"/> is thrown.
        /// </summary>
        [Test]
        public void Test_ToChar_Null_Enum()
        {
            // Arrange.
            Enum value = null;
            
            // Act.
            TestDelegate action = () => value.ToChar();
            
            // Assert.
            Assert.Catch<ArgumentNullException>(action, "Expected the null enum to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether an enum can be cast to an character.
        /// It expects that if the type is not assignable, an <see cref="ArgumentException"/> is thrown.
        /// </summary>
        [Test]
        public void Test_ToChar_NotAssignable()
        {
            // Arrange.
            ByteEnum value = ByteEnum.ValueOne;
            
            // Act.
            TestDelegate action = () => value.ToChar();
            
            // Assert.
            Assert.Catch<ArgumentException>(action,
                "Expected the unassignable value to cause an exception but it didn't.");
        }
        
        /// <summary>
        /// Tests whether an enum can be cast to an integer.
        /// It expects that if the enum value is null, an <see cref="ArgumentNullException"/> is thrown.
        /// </summary>
        [Test]
        public void Test_ToInt_Null_Enum()
        {
            // Arrange.
            Enum value = null;
            
            // Act.
            TestDelegate action = () => value.ToInt();
            
            // Assert.
            Assert.Catch<ArgumentNullException>(action, "Expected the null enum to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether an enum can be cast to an integer.
        /// It expects that if the type is not assignable, an <see cref="ArgumentException"/> is thrown.
        /// </summary>
        [Test]
        public void Test_ToInt_NotAssignable()
        {
            // Arrange.
            ByteEnum value = ByteEnum.ValueOne;
            
            // Act.
            TestDelegate action = () => value.ToInt();
            
            // Assert.
            Assert.Catch<ArgumentException>(action,
                "Expected the unassignable value to cause an exception but it didn't.");
        }
        
        /// <summary>
        /// Tests whether an enum can be cast to an character.
        /// It expects that if the type is assignable, the cast is done correctly.
        /// </summary>
        [Test]
        public void Test_ToInt_Assignable()
        {
            // Arrange.
            IntegerEnum value = IntegerEnum.ValueOne;
            
            // Act.
            int actual = value.ToInt();
            int expected = (int) value;

            // Assert.
            Assert.AreEqual(expected, actual, "Expected the result to be casted correctly but it wasn't.");
        }
        
        /// <summary>
        /// Tests whether an enum can be cast to a byte.
        /// It expects that if the enum value is null, an <see cref="ArgumentNullException"/> is thrown.
        /// </summary>
        [Test]
        public void Test_ToByte_Null_Enum()
        {
            // Arrange.
            Enum value = null;
            
            // Act.
            TestDelegate action = () => value.ToByte();
            
            // Assert.
            Assert.Catch<ArgumentNullException>(action, "Expected the null enum to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether an enum can be cast to a byte.
        /// It expects that if the type is not assignable, an <see cref="ArgumentException"/> is thrown.
        /// </summary>
        [Test]
        public void Test_ToByte_NotAssignable()
        {
            // Arrange.
            CharacterEnum value = CharacterEnum.ValueOne;
            
            // Act.
            TestDelegate action = () => value.ToByte();
            
            // Assert.
            Assert.Catch<ArgumentException>(action,
                "Expected the unassignable value to cause an exception but it didn't.");
        }
        
        /// <summary>
        /// Tests whether an enum can be cast to a byte.
        /// It expects that if the type is assignable, the cast is done correctly.
        /// </summary>
        [Test]
        public void Test_ToByte_Assignable()
        {
            // Arrange.
            ByteEnum value = ByteEnum.ValueOne;
            
            // Act.
            byte actual = value.ToByte();
            byte expected = (byte) value;

            // Assert.
            Assert.AreEqual(expected, actual, "Expected the result to be casted correctly but it wasn't.");
        }
    }
}

#endif