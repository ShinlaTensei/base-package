#if TEST_FRAMEWORK

using DTT.Utils.Exceptions;
using DTT.Utils.Workflow;
using NUnit.Framework;
using System;
using System.IO;

namespace DTT.Utils.Tests
{
    /// <summary>
    /// Tests the <see cref="PathUtility"/> class.
    /// </summary>
    public class Test_PathUtility
    {
        #region Tests
        /// <summary>
        /// Tests whether the directory name retrieval at an index is correct.
        /// It excepts that if valid input is given 
        /// </summary>
        [Test]
        public void Test_GetPathElementAt_Null_Input()
        {
            // Arrange.
            string path = null;
            int index = 0;

            // Act.
            TestDelegate action = () => PathUtility.GetPathElementAt(path, index);

            // Assert.
            Assert.Catch<ArgumentNullException>(action, "Expected the null path to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether the directory name retrieval at an index is correct.
        /// It excepts that if valid input is given 
        /// </summary>
        [Test]
        public void Test_SplitPath_ValidInput()
        {
            // Arrange.
            string path = "Folder1/Folder2/Folder3";
            int index = 0;

            // Act.
            string value = PathUtility.GetPathElementAt(path, index);

            // Assert.
            Assert.AreEqual("Folder1", value, "Expected the value to be the first folder but it wasn't.");
        }

        /// <summary>
        /// Tests whether a path can be split up correctly.
        /// It expects an empty string to be returned if the index is out of bounds.
        /// </summary>
        [Test]
        public void Test_SplitPath_InValidInput()
        {
            // Arrange.
            string path = "Folder1/Folder2/Folder3";
            int index = 3;

            // Act.
            string value = PathUtility.GetPathElementAt(path, index);

            // Assert.
            Assert.AreEqual(string.Empty, value, "Expected the value to be the first folder but it wasn't.");
        }


        /// <summary>
        /// Tests whether the path utility can properly check whether a directory
        /// name is part of a path. It expects a corresponding directory name
        /// to be part of the path.
        /// </summary>
        [Test]
        public void Test_ContainsDirectory_ValidInput()
        {
            // Arrange.
            string path = "Folder1/Folder2/Folder3";
            string argument = "Folder3";

            // Act.
            bool condition = PathUtility.ContainsDirectory(path, argument);

            // Assert.
            Assert.IsTrue(condition, "Expected the folder to be part of the directory path but it wasn't.");
        }
        
        /// <summary>
        /// Tests whether the path utility can properly check whether a directory
        /// name is part of a path. It expects a path that is null to cause a
        /// <see cref="ArgumentNullException"/>.
        /// </summary>
        [Test]
        public void Test_ContainsDirectory_Null_Path()
        {
            // Arrange.
            string path = null;
            string argument = "Folder3";

            // Act.
            TestDelegate action = () => PathUtility.ContainsDirectory(path, argument);

            // Assert.
            Assert.Catch<ArgumentNullException>(action, "Expected the null path to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether the path utility can properly check whether a directory
        /// name is part of a path. It expects a corresponding directory name
        /// to be part of the path.
        /// </summary>
        [Test]
        public void Test_ContainsDirectory_Null_DirectoryName()
        {
            // Arrange.
            string path = "Folder1/Folder2/Folder3";
            string directoryName = null;

            // Act.
            TestDelegate action = () => PathUtility.ContainsDirectory(path, directoryName);

            // Assert.
            Assert.Catch<ArgumentNullException>(action, "Expected the null directoryName to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether the path utility can properly check whether a directory
        /// name is part of a path. It expects an invalid directory name
        /// not to be part of the path.
        [Test]
        public void Test_ContainsDirectory_InValidInput()
        {
            // Arrange.
            string path = "Folder1/Folder2/Folder3";
            string argument = "Folder4";

            // Act.
            bool condition = PathUtility.ContainsDirectory(path, argument);

            // Assert.
            Assert.IsFalse(condition, "Expected the invalid directory name not to be part of the path but it was.");
        }

        /// <summary>
        /// Tests whether the path utility can ensure a directory its existance.
        /// It expects an invalid directory path to cause an <see cref="NullOrEmptyException"/>.
        /// </summary>
        [Test]
        public void Test_EnsureDirectoryExistence_InvalidPath()
        {
            // Arrange.
            string directoryPath = null;

            // Act.
            TestDelegate action = () => PathUtility.EnsureDirectoryExistence(directoryPath);

            // Assert.
            Assert.Catch<NullOrEmptyException>(action, "Expected the null directory path to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether the path utility can ensure a directory its existance.
        /// It expects a non-existing directory to be created.
        /// </summary>
        [Test]
        public void Test_EnsureDirectoryExistence_Doesnt_Exists()
        {
            // Arrange. 
            string directoryPath = "Assets/UCPTesting";

            // Act.
            PathUtility.EnsureDirectoryExistence(directoryPath);

            // Assert.
            Assert.IsTrue(Directory.Exists(directoryPath), "Expected the directory to exist after ensurance but it didn't.");

            // Cleanup.
            Directory.Delete(directoryPath);
        }

        /// <summary>
        /// Tests whether the path utility can convert a full path to an asset path.
        /// It expects an invalid path value to cause an <see cref="NullOrEmptyException"/>.
        /// </summary>
        [Test]
        public void Test_ToAssetPath_Null_Path()
        {
            // Arrange.
            string path = null;

            // Act.
            TestDelegate action = () => path.ToAssetPath();

            // Arrange.
            Assert.Catch<NullOrEmptyException>(action, "Expected the null path to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether the path utility can convert a full path to an asset path.
        /// It expects a valid full path to be converted to an asset path.
        /// </summary>
        [Test]
        public void Test_ToAssetPath_In_Data_Path()
        {
            // Arrange.
            string path = Path.GetFullPath("Assets/Scripts");

            // Act.
            string assetPath = path.ToAssetPath();

            // Arrange.
            Assert.AreEqual("Assets/Scripts", assetPath, "Expected the full path to change to an asset path but it didn't.");
        }

        /// <summary>
        /// Tests whether the path utility can convert a full path to an asset path.
        /// It expects a non-full path to be returned as the same value.
        /// </summary>
        [Test]
        public void Test_ToAssetPath_Not_In_Data_Path()
        {
            // Arrange.
            string path = "Assets/Scripts";

            // Act.
            string assetPath = path.ToAssetPath();

            // Arrange.
            Assert.AreEqual(path, assetPath, "Expected the path to stay the same but it didn't.");
        }

        /// <summary>
        /// Tests whether the path utility can convert a full path to an asset path.
        /// It expects a path with backslashes to be converted to forward slashes.
        [Test]
        public void Test_ToAssetPath_BackwardSlashes()
        {
            // Arrange.
            string path = Path.GetFullPath("Assets\\Scripts");

            // Act.
            string assetPath = path.ToAssetPath();

            // Arrange.
            Assert.AreEqual("Assets/Scripts", assetPath, "Expected the full path to change to an asset path but it didn't.");
        }

        /// <summary>
        /// Tests whether the path utility can check whether a path is a package path.
        /// It expects an invalid value to cause an <see cref="NullOrEmptyException"/>.
        [Test]
        public void Test_IsPackagePath_Invalid_Value()
        {
            // Arrange.
            string path = null;

            // Act.
            TestDelegate action = () => path.IsPackagePath();

            // Arrange.
            Assert.Catch<NullOrEmptyException>(action, "Expected the null path to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether the path utility can check whether a path is a package path.
        /// It expects a path that is not a package path to return false.
        [Test]
        public void Test_IsPackagePath_Incorrect_Value()
        {
            // Arrange.
            string path = "Assets/SomeFolder";

            // Act.
            bool condition =  path.IsPackagePath();

            // Assert.
            Assert.IsFalse(condition, "Expected the path not to be a package path but it was.");
        }

        /// <summary>
        /// Tests whether the path utility can check whether a path is a package path.
        /// It expects a path that is a package path to return true.
        [Test]
        public void Test_IsPackagePath_Correct_Value()
        {
            // Arrange.
            string path = "Packages/SomeFolder";

            // Act.
            bool condition =  path.IsPackagePath();

            // Assert.
            Assert.IsTrue(condition, "Expected the path to be a package path but it wasn't.");
        }
        #endregion
    }
}

#endif