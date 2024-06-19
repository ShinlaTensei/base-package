#if TEST_FRAMEWORK

using DTT.Utils.Extensions;
using NUnit.Framework;
using System;
using System.Collections;
using DTT.Utils.Workflow;
using UnityEditor;
using UnityEngine.TestTools;

namespace DTT.Utils.Tests
{
    /// <summary>
    /// Tests the <see cref="StringExtensions"/> class.
    /// </summary>
    public class Test_StringExtensions
    {
        #region Tests
        /// <summary>
        /// Tests whether the <see cref="StringExtensions"/> correctly adds spaces
        /// before capitals of a string. It expects a result of valid input to have spaces 
        /// added before capitals.
        /// </summary>
        [Test]
        public void Test_AddSpacesBeforeCapitals_Valid()
        {
            string content = "BBlaBlaB";

            string result = content.AddSpacesBeforeCapitals();

            Assert.AreEqual("B Bla BlaB", result,
                "Expected spaces to be added before capitals but there wheren't.");
        }

        /// <summary>
        /// Tests whether the <see cref="StringExtensions"/> correctly adds spaces
        /// before capitals of a string. It expects that if the input is invalid the
        /// input value will be returned.
        /// </summary>
        [Test]
        public void Test_AddSpacesBeforeCapitals_InValid()
        {
            string content = string.Empty;

            string emptyResult = content.AddSpacesBeforeCapitals();
            string nullResult = StringExtensions.AddSpacesBeforeCapitals(null);

            Assert.IsEmpty(emptyResult,
                "Expected the empty result to be empty but it wasn't.");

            Assert.IsNull(nullResult,
                "Expected the null result to be null but it wasn't.");
        }

        /// <summary>
        /// Tests whether the <see cref="StringExtensions"/> correctly adds spaces
        /// before capitals of a string. It expects that if the input is 2 capitals the
        /// there will be no space added.
        /// </summary>
        [Test]
        public void Test_AddSpacesBeforeCapitals_TwoCapitals()
        {
            string content = "UI";

            string result = content.AddSpacesBeforeCapitals();

            Assert.IsFalse(result.Contains(" "));
        }

        /// <summary>
        /// Tests whether the <see cref="StringExtensions"/> correctly converts
        /// a constant style to a readable style. It expects a result of valid input 
        /// to be converted from a constant style to a readable style.
        /// </summary>
        [Test]
        public void Test_ConvertConstStyleToReadable_Valid()
        {
            string content = "THIS_IS_CONTENT";

            string result = content.FromAllCapsToReadableFormat();

            Assert.AreEqual("This Is Content", result,
                "Expected the constant style to be readable but it wasn't.");
        }

        /// <summary>
        ///  Tests whether the <see cref="StringExtensions"/> correctly converts
        /// a constant style to a readable style. It expects that if the input is 
        /// invalid the input value will be returned.
        /// </summary>
        [Test]
        public void Test_ConvertConstStyleToReadable_InValid()
        {
            string content = string.Empty;

            string emptyResult = content.FromAllCapsToReadableFormat();
            string nullResult = StringExtensions.FromAllCapsToReadableFormat(null);

            Assert.IsEmpty(emptyResult,
                "Expected the empty result to be empty but it wasn't.");

            Assert.IsNull(nullResult,
                "Expected the null result to be null but it wasn't.");
        }

        /// <summary>
        /// Tests whether the <see cref="StringExtensions"/> can convert
        /// a string with content in readable format to all caps format. It expects
        /// the string to convert correctly to all caps.
        /// </summary>
        [Test]
        public void Test_FromReadableFormatToAllCaps_Valid()
        {
            string actual = "My Content".FromReadableFormatToAllCaps();
            string expected = "MY_CONTENT";

            Assert.AreEqual(expected, actual, "Expected the string to be all caps but it wasn't.");
        }

        /// <summary>
        /// Tests whether the <see cref="StringExtensions"/> can correctly
        /// handle invalid string input when converting from a readable format to
        /// all caps. It expects the output to be null when the input is null.
        /// </summary>
        [Test]
        public void Test_FromReadableFormatToAllCaps_InValid()
        {
            string content = null;
            string actual = content.FromReadableFormatToAllCaps();

            Assert.IsNull(actual, "Expected the string to be null but it wasn't.");
        }

        /// <summary>
        /// Tests whether tags can be properly stripped from a string.
        /// It expects text that is null to cause a <see cref="ArgumentNullException"/>.
        /// </summary>
        [Test]
        public void Test_StripHtmlTags_Null_Text()
        {
            // Arrange.
            string text = null;
            
            // Act.
            TestDelegate action = () => text.StripHtmlTags();
            
            // Assert.
            Assert.Catch<ArgumentNullException>(action, "Expected the null text to cause an exception but it didn't.");
        }
        
        /// <summary>
        /// Tests whether tags can be properly stripped from a string.
        /// It expects text has html tags to be stripped of them.
        /// </summary>
        [Test]
        public void Test_StripHtmlTags()
        {
            // Arrange.
            string original = "text";
            string tagged = "<p><i><b>" + original + "</b></i></p>";
            
            // Act.
            string stripped = tagged.StripHtmlTags();
            
            // Assert.
            Assert.AreEqual(original, stripped, "Expected all html tags to be stripped from the tagged string.");
        }

        /// <summary>
        /// Tests the Ellipsis method in an OnGUI method on usability.
        /// </summary>
        [UnityTest]
        public IEnumerator Test_Ellipsis_OnGUI()
        {
            // Open the test editor window containing the ellipsis method tests.
            TestEditorWindow window = EditorWindow.GetWindow<TestEditorWindow>("Test Editor Window", true);

            // Wait a frame before closing it.
            yield return null;

            // Close the window now that the test is done.
            window.Close();
        }

        /// <summary>
        /// Tests whether the <see cref="StringExtensions"/> can correctly
        /// handle invalid string input when adding ellipsis characters to a string.
        /// It expects an argument null exception to be thrown when the font is null.
        /// </summary>
        [Test]
        public void Test_Elipsis_InvalidInput_FontNull()
        {
            TestDelegate action = () => string.Empty.Ellipsis(0, null);

            Assert.Catch<ArgumentNullException>(action, "Expected an argument null exception but there was none.");
        }

        /// <summary>
        /// Tests whether the index of an nth appearnce of a string is correctly returned.
        /// It expects a <see cref="ArgumentOutOfRangeException"/> if the given count is smaller than zero.
        /// </summary>
        [Test]
        public void Test_IndexOfNth_Nth_Smaller_Than_Zero()
        {
            // Arrange.
            string @string = "value";
            string value = "val";
            int count = -1;

            // Act.
            TestDelegate action = () => @string.IndexOfNth(value, count);

            // Assert.
            Assert.Catch<ArgumentOutOfRangeException>(action, "Expected the negative count to cause an exception but it didnt't");
        }

        /// <summary>
        /// Tests whether the index of an nth appearnce of a string is correctly returned.
        /// It expects a <see cref="ArgumentNullException"/> if the string value is null.
        /// </summary>
        [Test]
        public void Test_IndexOfNth_String_Null()
        {
            // Arrange.
            string @string = null;
            string value = "val";
            int count = 1;

            // Act.
            TestDelegate action = () => @string.IndexOfNth(value, count);

            // Assert.
            Assert.Catch<ArgumentNullException>(action, "Expected the null string to cause an exception but it didnt't");
        }

        /// <summary>
        /// Tests whether the index of an nth appearnce of a string is correctly returned.
        /// It expects a <see cref="ArgumentNullException"/> if the lookup value is null.
        /// </summary>
        [Test]
        public void Test_IndexOfNth_Value_Null()
        {
            // Arrange.
            string @string = "value";
            string value = null;
            int count = 1;

            // Act.
            TestDelegate action = () => @string.IndexOfNth(value, count);

            // Assert.
            Assert.Catch<ArgumentNullException>(action, "Expected the null value to cause an exception but it didnt't");
        }

        /// <summary>
        /// Tests whether the index of an nth appearnce of a string is correctly returned.
        /// It expects a return value of -1 if nth appearance of the value could not be found.
        /// </summary>
        [Test]
        public void Test_IndexOfNth_Value_Not_Found()
        {
            // Arrange.
            string @string = "value";
            string value = "bad";
            int count = 2;

            // Act.
            int index = @string.IndexOfNth(value, count);

            // Assert.
            Assert.AreEqual(-1, index, "Expected an index of -1 to be returned because of a not found value but it wasn't.");
        }

        /// <summary>
        /// Tests whether the index of an nth appearnce of a string is correctly returned.
        /// It expects a valid index to be returned if the nth appearance of the string was found.
        /// </summary>
        [Test]
        public void Test_IndexOfNth_Found()
        {
            // Arrange.
            string @string = "value_value";
            string value = "val";
            int count = 2;

            // Act.
            int index = @string.IndexOfNth(value, count);

            // Assert.
            Assert.AreEqual(6, index, "Expected an index of 6 to be returned because of a found value but it wasn't.");
        }

        /// <summary>
        /// Tests whether the index of an nth appearnce of a character is correctly returned.
        /// It expects a valid index to be returned if the nth appearance of the character was found.
        /// </summary>
        [Test]
        public void Test_IndexOfNth_Char()
        {
            // Arrange.
            string @string = "value_value";
            char value = 'v';
            int count = 2;

            // Act.
            int index = @string.IndexOfNth(value, count);

            // Assert.
            Assert.AreEqual(6, index, "Expected an index of 6 to be returned because of a found value but it wasn't.");
        }

        /// <summary>
        /// Tests whether a valid guid can be detected.
        /// It expects a <see cref="ArgumentNullException"/> if the value was null.
        /// </summary>
        [Test]
        public void Test_IsValidGuid_Null_String()
        {
            // Arrange.
            string @string = null;

            // Act.
            TestDelegate action = () => @string.IsValidGuid();

            // Assert.
            Assert.Catch<ArgumentNullException>(action, "Expected an exception because of a null string but there wasn't.");
        }

        /// <summary>
        /// Tests whether a valid guid can be detected.
        /// It expects a <see cref="ArgumentNullException"/> if the format was null.
        /// </summary>
        [Test]
        public void Test_IsValidGuid_Null_Format()
        {
            // Arrange.
            string @string = "guid";
            string format = null;

            // Act.
            TestDelegate action = () => @string.IsValidGuid(format);

            // Assert.
            Assert.Catch<ArgumentNullException>(action, "Expected an exception because of a null format but there wasn't.");
        }

        /// <summary>
        /// Tests whether a valid guid can be detected.
        /// It expects a <see cref="ArgumentNullException"/> if the value was null.
        /// </summary>
        [Test]
        public void Test_IsValidGuid_Format_Null_String()
        {
            // Arrange.
            string @string = null;
            string format = "B";

            // Act.
            TestDelegate action = () => @string.IsValidGuid(format);

            // Assert.
            Assert.Catch<ArgumentNullException>(action, "Expected an exception because of a null format but there wasn't.");
        }

        /// <summary>
        /// Tests whether a valid guid can be detected.
        /// It expects true to be returned if the guid is valid.
        /// </summary>
        [Test]
        public void Test_IsValidGuid_Valid()
        {
            // Arrange.
            string @string = new Guid().ToString();

            // Act.
            bool condition = @string.IsValidGuid();

            // Assert.
            Assert.IsTrue(condition, "Expected the string to be a valid guid but it wasn't.");
        }

        /// <summary>
        /// Tests whether a valid guid can be detected.
        /// It expects false to be returned if the guid was not valid.
        /// </summary>
        [Test]
        public void Test_IsValidGuid_InValid()
        {
            // Arrange.
            string @string = "invalid guid.";

            // Act.
            bool condition = @string.IsValidGuid();

            // Assert.
            Assert.IsFalse(condition, "Expected the string not to be a valid guid but it was.");
        }

        /// <summary>
        /// Tests whether a valid guid can be detected.
        /// It expects false to be returned if the guid was not valid.
        /// </summary>
        [Test]
        public void Test_IsValidGuid_Formatted_InValid()
        {
            // Arrange.
            string @string = "invalid guid.";

            // Act.
            bool condition = @string.IsValidGuid("B");

            // Assert.
            Assert.IsFalse(condition, "Expected the string not to be a valid guid but it was.");
        }

        /// <summary>
        /// Tests whether a valid variable name can be detected.
        /// It expects a <see cref="ArgumentNullException"/> if the value was null.
        /// </summary>
        [Test]
        public void Test_IsValidVariableName_Null_String()
        {
            // Arrange.
            string stringValue = null;

            // Act.
            TestDelegate action = () => StringUtility.IsVariableName(stringValue);

            // Assert.
            Assert.Catch<ArgumentNullException>(action, "Expected the null string to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether a valid variable name can be detected.
        /// It expects a true value to be returned if the string was a valid variable name.
        /// </summary>
        [Test]
        public void Test_IsValidVariableName_Correct()
        {
            // Arrange.
            string stringValue = "MyCustomVariableName";

            // Act.
            bool condition = StringUtility.IsVariableName(stringValue);

            // Assert.
            Assert.IsTrue(condition, $"Expected the variable name {stringValue} to be a valid variable name but it wasn't.");
        }

        /// <summary>
        /// Tests whether a valid variable name can be detected.
        /// It expects a false value to be returned if the string wasn't a valid variable name.
        /// </summary>
        [Test]
        public void Test_IsValidVariableName_InCorrect()
        {
            // Arrange.
            string stringValue = "MyCustom/VariableName";

            // Act.
            bool condition = StringUtility.IsVariableName(stringValue);

            // Assert.
            Assert.IsFalse(condition, $"Expected the variable name {stringValue} not to be a valid variable name but it was.");
        }
        #endregion
    }
}

#endif
