#if TEST_FRAMEWORK

using NUnit.Framework;
using System;
using DTT.Utils.Workflow;

namespace DTT.Utils.Tests
{
    public class Test_StringUtility 
    {
        /// <summary>
        /// Tests whether a random insecure string can be generated.
        /// It expects an <see cref="ArgumentOutOfRangeException"/> if the length is smaller than 0.
        /// </summary>
        [Test]
        public void Test_RandomInsecure_OutOfRange_Length()
        {
            // Arrange.
            int length = -1;
            
            // Act.
            TestDelegate action = () => StringUtility.RandomInsecure(length);
            
            // Assert.
            Assert.Catch<ArgumentOutOfRangeException>(action,
                "Expected the out of range length to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether a random insecure string can be generated.
        /// It expects the resulting string's length to be equal to the input length.
        /// </summary>
        [Test]
        public void Test_RandomInsecure_Length()
        {
            // Arrange.
            int length = 5;
            
            // Act.
            string result = StringUtility.RandomInsecure(length);
            
            // Assert.
            Assert.AreEqual(length, result.Length, "Expected the resulting string to be of proper length but it wasn't.");
        }
        
        /// <summary>
        /// Tests whether a random insecure string can be generated using a seed.
        /// It expects the resulting string's length to be equal to the input length.
        /// </summary>
        [Test]
        public void Test_RandomInsecure_Seed_Length()
        {
            // Arrange.
            int length = 5;
            int seed = 289;
            
            // Act.
            string result = StringUtility.RandomInsecure(length, seed);
            
            // Assert.
            Assert.AreEqual(length, result.Length, "Expected the resulting string to be of proper length but it wasn't.");
        }
    }
}

#endif