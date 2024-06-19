#if TEST_FRAMEWORK

using DTT.Utils.Extensions;
using NUnit.Framework;
using System;
using UnityEngine;

namespace DTT.Utils.Tests
{
    /// <summary>
    /// Tests the <see cref="RichTextExtensions"/> class.
    /// </summary>
    public class Test_RichTextExtensions
    {
        /// <summary>
        /// Fails the test if the application is currently not in edit mode.
        /// </summary>
        public void Setup()
        {
            if (!Application.isEditor)
                Assert.Fail("Can't test richt text outside of editor mode.");
        }

        /// <summary>
        /// Tests whether the <see cref="RichTextHelper.Bold(string)"/> 
        /// actually wraps the elements around.
        /// Expects the returned string to have the bold elements at the beginning and end.
        /// </summary>
        [Test]
        public void Test_Bold_CorrectSemantics()
        {
            string input = "x";

            string output = input.Bold();
            string expectedOutput = $"<b>{input}</b>";

            Assert.AreEqual(expectedOutput, output, "Output doesn't match expectations.");
        }

        /// <summary>
        /// Tests whether the <see cref="RichTextHelper.Italics(string)"/> 
        /// actually wraps the elements around.
        /// Expects the returned string to have the italics elements at the beginning and end.
        /// </summary>
        [Test]
        public void Test_Italics_CorrectSemantics()
        {
            string input = "x";

            string output = input.Italics();
            string expectedOutput = $"<i>{input}</i>";

            Assert.AreEqual(expectedOutput, output, "Output doesn't match expectations.");
        }

        /// <summary>
        /// Tests whether the <see cref="RichTextHelper.Size(string, int)"/> 
        /// actually wraps the elements around.
        /// Expects the returned string to have the size elements at the beginning and end.
        /// </summary>
        [Test]
        public void Test_Size_CorrectSemantics()
        {
            string input = "x";
            int size = 5;

            string output = input.Size(size);
            string expctedOutput = $"<size={size}>{input}</size>";

            Assert.AreEqual(expctedOutput, output, "Output doesn't match expectations.");
        }

        /// <summary>
        /// Tests whether the <see cref="RichTextHelper.Color(string, Color)"/> 
        /// actually wraps the elements around.
        /// Expects the returned string to have the color elements at the beginning and end.
        /// </summary>
        [Test]
        public void Test_Color_CorrectSemantics()
        {
            string input = "x";
            Color color = Color.white;

            string output = input.Color(color);
            string expectedOutput = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{input}</color>";

            Assert.AreEqual(expectedOutput, output, "Output doesn't match expectations.");
        }

        /// <summary>
        /// Tests whether the <see cref="RichTextHelper.Color(string, Color)"/> 
        /// actually returns the correct hex color.
        /// Expects the returned string to have the correct hex value in the color element.
        /// </summary>
        [Test]
        public void Test_Color_CorrectHexValue()
        {
            string input = "x";
            Color color = Color.red;

            string output = input.Color(color);
            string expectedOutput = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{input}</color>";

            Assert.AreEqual(expectedOutput, output, "Output didn't match expectations.");
        }

        /// <summary>
        /// Tests whether the <see cref="RichTextHelper.Color(string, string)"/> 
        /// actually returns the correct hex color.
        /// Expects the returned string to have the correct hex value in the color element.
        /// </summary>
        [Test]
        public void Test_ColorString_CorrectHexValue()
        {
            string input = "x";

            string output1 = input.Color("ff00ff");
            string output2 = input.Color("#ffff00");
            string expectedOutput1 = $"<color=#ff00ff>{input}</color>";
            string expectedOutput2 = $"<color=#ffff00>{input}</color>";

            Assert.AreEqual(expectedOutput1, output1, "Output didn't match expectations.");
            Assert.AreEqual(expectedOutput2, output2, "Output didn't match expectations.");
        }

        /// <summary>
        /// Tests whether the <see cref="RichTextHelper.Color(string, string)"/> 
        /// actually throws exceptions with incorrect values.
        /// Expects <see cref="ArgumentException"/> exceptions to be thrown with incorrect values.
        /// </summary>
        [Test]
        public void Test_ColorString_IncorrectHexValue([Values("Invalid hex", "#ff00ff00", "-#00ffff", "#1", "1")] string hexColor)
        {
            // Assert.
            Assert.Throws<ArgumentException>(() => "x".Color(hexColor));
        }

        /// <summary>
        /// Tests whether the <see cref="RichTextHelper.Color(string, RichTextHelper.ColorType)"/> 
        /// actually returns the correct color name.
        /// Expects the returned string to have the correct color name in the color element.
        /// </summary>
        [Test]
        public void Test_Color_CorrectColorTypeValue()
        {
            string input = "x";
            Array enumValues = Enum.GetValues(typeof(RichTextColor));
            for (int i = 0; i < enumValues.Length; i++)
            {
                RichTextColor color = (RichTextColor)enumValues.GetValue(i);

                string output = input.Color(color);
                string expectedOutput = $"<color={color}>{input}</color>";

                Assert.AreEqual(expectedOutput, output, "Output didn't match expectations.");
            }
        }
    }
}

#endif