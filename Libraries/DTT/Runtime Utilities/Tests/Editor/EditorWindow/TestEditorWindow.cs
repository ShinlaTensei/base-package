#if TEST_FRAMEWORK

using System;
using DTT.Utils.Extensions;
using NUnit.Framework;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Tests ellipsis methods as these are only callable from OnGUI methods.
/// </summary>
public class TestEditorWindow : EditorWindow
{
    /// <summary>
    /// Whether the OnGUI method has been run.
    /// </summary>
    private bool _didCallOnGUI = false;

    /// <summary>
    /// Runs all tests once.
    /// </summary>
    private void OnGUI()
    {
        if (!_didCallOnGUI)
        {
            // Ellipsis
            Test_Ellipsis_Null_Value();
            Test_Ellipsis_Added();
            Test_Ellipsis_Not_Added();
            Test_Ellipsis_CharacterCount();
            Test_Ellipsis_Clamp_CharacterCount();

            _didCallOnGUI = true;
        }
    }

    /// <summary>
    /// Tests whether an ellipsis method can handle a null value.
    /// It expects a <see cref="ArgumentNullException"/> if the value is null.
    /// </summary>
    private void Test_Ellipsis_Null_Value()
    {
        // Arrange.
        string value = null;
        char ellipsisChar = ',';

        // Act.
        TestDelegate action = () => value.Ellipsis(5, ellipsisChar, GUI.skin.font);

        // Assert.
        Assert.Catch<ArgumentNullException>(action, "Expected the null value to cause an exception but it didn't.");
    }

    /// <summary>
    /// Tests whether an ellipsis character is added when necessary.
    /// It expects the ellipsis characters to be added when the max width is exceeded.
    /// </summary>
    private void Test_Ellipsis_Added()
    {
        // Arrange.
        string value = "my ellipsis string";
        char ellipsisChar = ',';

        // Act.
        string ellipsis = value.Ellipsis(5, ellipsisChar, GUI.skin.font);

        // Assert.
        Assert.IsTrue(ellipsis[ellipsis.Length - 1] == ellipsisChar,
            "Expected the ellipsis string to end with the character but it didn't.");
    }

    /// <summary>
    /// Tests whether an ellipsis character is not added when unnecessary.
    /// It expects the ellipsis characters not to be added when the max width is not exceeded.
    /// </summary>
    private void Test_Ellipsis_Not_Added()
    {
        // Arrange.
        string value = "my ellipsis string";
        char ellipsisChar = ',';

        // Act.
        string ellipsis = value.Ellipsis(1000, ellipsisChar, GUI.skin.font);

        // Assert.
        Assert.IsFalse(ellipsis[ellipsis.Length - 1] == ellipsisChar,
            $"Expected the ellipsis string {ellipsis} to not end with the character but it didn't.");
    }

    /// <summary>
    /// Tests whether the characters added is equal to the given character count.
    /// It expects the characters added to be equal to the given character count.
    /// </summary>
    private void Test_Ellipsis_CharacterCount()
    {
        // Arrange.
        string value = "my ellipsis string";
        char ellipsisChar = ',';
        int characterCount = 4;

        // Act.
        string ellipsis = value.Ellipsis(5,ellipsisChar, GUI.skin.font, characterCount);

        string end = ellipsis.Substring(ellipsis.Length - characterCount);

        // Assert.
        Assert.IsTrue(end.All(c => c == ellipsisChar),
            $"Expected the amount of characters added to {ellipsis} be equal to {characterCount} count but it wasn't.");
    }

    /// <summary>
    /// Tests whether the character count if is clamped if to high.
    /// It expects the characters added to be equal to the string length if if it was larger or equal to the string length.
    /// </summary>
    private void Test_Ellipsis_Clamp_CharacterCount()
    {
        // Arrange.
        string value = "my ellipsis string";
        char ellipsisChar = ',';
        int characterCount = value.Length + 1;

        // Act.
        string ellipsis = value.Ellipsis(0,ellipsisChar, GUI.skin.font, characterCount);

        // Assert.
        Assert.IsTrue(ellipsis.All(c => c == ellipsisChar),
            $"Expected the character count in {ellipsis} to be clamped but it wasn't.");
    }
}

#endif