#if TEST_FRAMEWORK

using NUnit.Framework;
using System;
using DTT.Utils.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.Utils.Tests
{
    /// <summary>
    /// Tests the <see cref="RectTransformExtensions"/> class.
    /// </summary>
    public class Test_RectTransformExtensions
    {
        /// <summary>
        /// The rectangle transform used for the tests.
        /// </summary>
        private RectTransform _rectTransform;

        /// <summary>
        /// Creates a UI image and saves the rectangle transform for testing.
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            EditorApplication.ExecuteMenuItem("GameObject/UI/Image");
            _rectTransform = (RectTransform) GameObject.FindObjectOfType<Image>().transform;
        }

        /// <summary>
        /// Tests whether the SetAnchor method can properly set the anchor values.
        /// It expects the xMin, xMax, yMin and yMax values to be set.
        /// </summary>
        [Test]
        public void Test_SetAnchor_Values()
        {
            // Arrange.
            float xMin = 0.5f;
            float xMax = 0.5f;
            float yMin = 0f;
            float yMax = 0f;

            // Act
            _rectTransform.SetAnchor(xMin, xMax, yMin, yMax);

            // Assert.
            Assert.AreEqual(_rectTransform.anchorMin.x, xMin,
                "Expected the minimal x anchor value to be set but it wasn't.");
            Assert.AreEqual(_rectTransform.anchorMax.x, xMax,
                "Expected the maximum x anchor value to be set but it wasn't.");
            Assert.AreEqual(_rectTransform.anchorMin.y, yMin,
                "Expected the minimal y anchor value to be set but it wasn't.");
            Assert.AreEqual(_rectTransform.anchorMax.y, yMax,
                "Expected the maximum y anchor value to be set but it wasn't.");
        }

        /// <summary>
        /// Tests whether the SetAnchor method can handle a null transform being given to it.
        /// It expects an <see cref="ArgumentNullException"/> to be thrown if the transform is null.
        /// </summary>
        [Test]
        public void Test_SetAnchor_Values_Null_Transform()
        {
            // Arrange.
            RectTransform transform = null;

            // Act.
            TestDelegate action = () => RectTransformExtensions.SetAnchor(transform, 0, 0, 0, 0);

            // Assert.
            Assert.Catch<ArgumentNullException>(action,
                "Expected the null transform to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether the SetAnchor method using a setting can handle a null transform being given to it.
        /// It expects an <see cref="ArgumentNullException"/> to be thrown if the transform is null.
        /// </summary>
        [Test]
        public void Test_SetAnchor_RectAnchor_Setting_Null_Transform()
        {
            // Arrange.
            RectTransform transform = null;

            // Act.
            TestDelegate action = () => RectTransformExtensions.SetAnchor(transform, RectAnchor.TOP_LEFT, true, true);

            // Assert.
            Assert.Catch<ArgumentNullException>(action,
                "Expected the null transform to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether the SetAnchor method using a RectAnchor setting will properly set the values.
        /// It expects the values set to correspond with the setting given to the method.
        /// </summary>
        [TestCase(RectAnchor.TOP_LEFT, 0f, 0f, 1f, 1f)]
        [TestCase(RectAnchor.TOP_CENTER, 0.5f, 0.5f, 1f, 1f)]
        [TestCase(RectAnchor.TOP_RIGHT, 1f, 1f, 1f, 1f)]
        [TestCase(RectAnchor.MIDDLE_LEFT, 0f, 0f, 0.5f, 0.5f)]
        [TestCase(RectAnchor.MIDDLE_CENTER, 0.5f, 0.5f, 0.5f, 0.5f)]
        [TestCase(RectAnchor.MIDDLE_RIGHT, 1f, 1f, 0.5f, 0.5f)]
        [TestCase(RectAnchor.BOTTOM_LEFT, 0f, 0f, 0f, 0f)]
        [TestCase(RectAnchor.BOTTOM_CENTER, 0.5f, 0.5f, 0f, 0f)]
        [TestCase(RectAnchor.BOTTOM_RIGHT, 1f, 1f, 0f, 0f)]
        [TestCase(RectAnchor.STRETCH_TOP, 0f, 1f, 1f, 1f)]
        [TestCase(RectAnchor.STRETCH_MIDDLE, 0f, 1f, 0.5f, 0.5f)]
        [TestCase(RectAnchor.STRETCH_BOTTOM, 0f, 1f, 0f, 0f)]
        [TestCase(RectAnchor.STRETCH_LEFT, 0f, 0f, 0f, 1f)]
        [TestCase(RectAnchor.STRETCH_CENTER, 0.5f, 0.5f, 0f, 1f)]
        [TestCase(RectAnchor.STRETCH_RIGHT, 1f, 1f, 0f, 1f)]
        [TestCase(RectAnchor.STRETCH_FULL, 0f, 1f, 0f, 1f)]
        [Test]
        public void Test_SetAnchor_RectAnchor_Setting(RectAnchor setting, float xMin, float xMax, float yMin,
            float yMax)
        {
            // Act.
            _rectTransform.SetAnchor(setting, false, false);

            // Assert.
            Assert.AreEqual(_rectTransform.anchorMin.x, xMin,
                "Expected the minimal x anchor value to be set but it wasn't.");
            Assert.AreEqual(_rectTransform.anchorMax.x, xMax,
                "Expected the maximum x anchor value to be set but it wasn't.");
            Assert.AreEqual(_rectTransform.anchorMin.y, yMin,
                "Expected the minimal y anchor value to be set but it wasn't.");
            Assert.AreEqual(_rectTransform.anchorMax.y, yMax,
                "Expected the maximum y anchor value to be set but it wasn't.");
        }

        /// <summary>
        /// Tests whether the SetAnchor method can set the pivot if the 'alsoSetPivot' flag is set to true.
        /// It expects the pivot values set to correspond with the setting given to the method.
        /// </summary>
        [TestCase(RectAnchor.TOP_LEFT, 0f, 1f)]
        [TestCase(RectAnchor.TOP_CENTER, 0.5f, 1f)]
        [TestCase(RectAnchor.TOP_RIGHT, 1f, 1f)]
        [TestCase(RectAnchor.MIDDLE_LEFT, 0f, 0.5f)]
        [TestCase(RectAnchor.MIDDLE_CENTER, 0.5f, 0.5f)]
        [TestCase(RectAnchor.MIDDLE_RIGHT, 1f, 0.5f)]
        [TestCase(RectAnchor.BOTTOM_LEFT, 0f, 0f)]
        [TestCase(RectAnchor.BOTTOM_CENTER, 0.5f, 0f)]
        [TestCase(RectAnchor.BOTTOM_RIGHT, 1f, 0f)]
        [TestCase(RectAnchor.STRETCH_TOP, 0.5f, 1f)]
        [TestCase(RectAnchor.STRETCH_MIDDLE, 0.5f, 0.5f)]
        [TestCase(RectAnchor.STRETCH_BOTTOM, 0.5f, 0f)]
        [TestCase(RectAnchor.STRETCH_LEFT, 0f, 0.5f)]
        [TestCase(RectAnchor.STRETCH_CENTER, 0.5f, 0.5f)]
        [TestCase(RectAnchor.STRETCH_RIGHT, 1f, 0.5f)]
        [TestCase(RectAnchor.STRETCH_FULL, 0.5f, 0.5f)]
        [Test]
        public void Test_SetAnchor_RectAnchor_Setting_Pivot(RectAnchor setting, float x, float y)
        {
            // Act.
            _rectTransform.SetAnchor(setting, true, false);

            // Assert.
            Assert.AreEqual(_rectTransform.pivot.x, x, "Expected the pivot x value to be set but it wasn't.");
            Assert.AreEqual(_rectTransform.pivot.y, y, "Expected the pivot y value to be set but it wasn't.");
        }

        /// <summary>
        /// Tests whether the SetAnchor method can properly set the position if the 'alsoSetPosition' flag is set.
        /// It expects the anchored position of the transform to be reset to (0,0). 
        /// </summary>
        [Test]
        public void Test_SetAnchor_RectAnchor_Setting_Position()
        {
            // Arrange.
            RectAnchor setting = RectAnchor.TOP_LEFT;

            // Act.
            _rectTransform.SetAnchor(setting, true, true);

            // Assert.
            Assert.Zero(_rectTransform.anchoredPosition.x,
                "Expected the anchored position x value to be set but it wasn't.");
            Assert.Zero(_rectTransform.anchoredPosition.x,
                "Expected the anchored position y value to be set but it wasn't.");
        }

        /// <summary>
        /// Tests whether the retrieval of a world rect value is done properly.
        /// It expects a null transform to cause a <see cref="ArgumentNullException"/>.
        /// </summary>
        [Test]
        public void Test_GetWorldRect_Null_Transform()
        {
            // Arrange.
            RectTransform transform = null;

            // Act.
            TestDelegate action = () => transform.GetWorldRect();

            // Assert.
            Assert.Catch<ArgumentNullException>(action,
                "Expected the null transform to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether the retrieval of a world rect value is done properly.
        /// It expects a valid transform value to return a valid world rectangle value. 
        /// </summary>
        [Test]
        public void Test_GetWorldRect_Valid_Transform()
        {
            // Arrange.
            Vector3[] corners = new Vector3[4];
            _rectTransform.GetWorldCorners(corners);

            Vector3 bottomLeft = corners[0];

            Vector2 size = new Vector2(
                _rectTransform.lossyScale.x * _rectTransform.rect.size.x,
                _rectTransform.lossyScale.y * _rectTransform.rect.size.y);

            // Act.
            Rect rect = _rectTransform.GetWorldRect();

            // Assert.
            Assert.AreEqual(rect.position, (Vector2) bottomLeft, "Expected the position to be correct but it wasn't.");
            Assert.AreEqual(rect.size, size, "Expected the size to be correct but it wasn't.");
        }

        /// <summary>
        /// Tests whether retrieval of a rectangle transform is done properly.
        /// It expects a null component to cause a <see cref="ArgumentNullException"/>.
        /// </summary>
        [Test]
        public void Test_GetRectTransform_Component_Null_Component()
        {
            // Arrange.
            Component component = null;

            // Act.
            TestDelegate action = () => RectTransformExtensions.GetRectTransform(component);

            // Assert.
            Assert.Catch<ArgumentNullException>(action,
                "Expected the null component to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether retrieval of a rectangle transform is done properly.
        /// It expects a null game object to cause a <see cref="ArgumentNullException"/>.
        /// </summary>
        [Test]
        public void Test_GetRectTransform_GameObject_Null_GameObject()
        {
            // Arrange.
            GameObject gameObject = null;

            // Act.
            TestDelegate action = () => RectTransformExtensions.GetRectTransform(gameObject);

            // Assert.
            Assert.Catch<ArgumentNullException>(action,
                "Expected the null game object to cause an exception but it didn't.");
        }

        /// <summary>
        /// Tests whether retrieval of a rectangle transform is done properly.
        /// It expects a null value to be returned if the game object is not part of the ui.
        /// </summary>
        [Test]
        public void Test_GetRectTransform_Non_UI_Element()
        {
            // Arrange.
            GameObject gameObject = new GameObject("Test_GameObject");

            // Act.
            RectTransform transform = gameObject.GetRectTransform();

            // Assert.
            Assert.IsNull(transform, "Expected the rect transform from a non-ui element to be null but it wasn't.");
        }

        /// <summary>
        /// Tests whether retrieval of a rectangle transform is done properly.
        /// It expects a valid value to be returned if the component is part of the ui.
        /// </summary>
        [Test]
        public void Test_GetRectTransform_UI_Element()
        {
            // Arrange.
            Component component = _rectTransform.GetRectTransform();

            // Act.
            RectTransform transform = component.GetRectTransform();

            // Assert.
            Assert.IsNotNull(transform, "Expected the rect transform from a non-ui element not to be null but it was.");
        }

        /// <summary>
        /// Destroys the rectangle transform object used for testing.
        /// </summary>
        [OneTimeTearDown]
        public void OneTimeTearDown() => GameObject.DestroyImmediate(_rectTransform.gameObject);
    }
}

#endif