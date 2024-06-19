#if TEST_FRAMEWORK

using System.Collections.ObjectModel;
using NUnit.Framework;
using System.Linq;
using DTT.Utils.Workflow;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DTT.Utils.Tests
{
    /// <summary>
    /// Tests the <see cref="GridBase{T}"/> class.
    /// </summary>
    public class Test_GridBase 
    {
        /// <summary>
        /// Expects creation of the grid to set the grid default values.
        /// </summary>
        [Test]
        public void Test_Creation_Default_Values()
        {
            // Arrange.
            GridBase<int> grid;

            // Act.
            grid = new GridBase<int>();
            
            // Assert.
            Assert.AreEqual(grid.Columns, GridBase<int>.MIN_ROW_OR_COLUMN_SIZE);
            Assert.AreEqual(grid.Rows, GridBase<int>.MIN_ROW_OR_COLUMN_SIZE);
            Assert.NotNull(grid.Values);
        }
        
        /// <summary>
        /// Expects the grid with custom values to be set accordingly.
        /// </summary>
        [Test]
        public void Test_Creation_Custom_Values()
        {
            // Arrange.
            GridBase<int> grid;
            int columns = Random.Range(GridBase<int>.MIN_ROW_OR_COLUMN_SIZE, 1000);
            int rows = Random.Range(GridBase<int>.MIN_ROW_OR_COLUMN_SIZE, 1000);

            // Act.
            grid = new GridBase<int>(columns, rows);
            
            // Assert.
            Assert.AreEqual(grid.Columns, columns);
            Assert.AreEqual(grid.Rows, rows);
            Assert.NotNull(grid.Values);
        }

        /// <summary>
        /// Expects the populate method to set the grid values.
        /// </summary>
        [Test]
        public void Test_Populate_One_Dimension()
        {
            // Arrange.
            GridBase<int> grid = new GridBase<int>(3, 3);
            int[] numbers = new int[]
            {
                8, 5, 3,
                1, 5, 3,
                0, 5, 2
            };
            
            // Act.
            grid.Populate(numbers);
            bool condition = Enumerable.SequenceEqual(numbers, grid.Values);

            // Assert.
            Assert.IsTrue(condition, "Expected the grid to be populated with numbers but it wasn't.");
        }

        /// <summary>
        /// Expects the populate method to populate the grid and resize it
        /// by default if the array is multi dimensional.
        /// </summary>
        [Test]
        public void Test_Populate_Two_Dimensions()
        {
            // Arrange.
            GridBase<int> grid = new GridBase<int>(2, 2);
            int[,] numbers = new int[,]
            {
                { 0, 1, 5 },
                { 8, 1, 3 },
                { 0, 3, 1 }
            };
            
            // Act.
            grid.Populate(numbers);
            bool condition = Enumerable.SequenceEqual(numbers.Cast<int>().ToArray(), grid.Values);

            // Assert.
            Assert.IsTrue(condition, "Expected the grid to be populated with numbers but it wasn't.");
        }

        /// <summary>
        /// Expects the correct array index to be returned based on the grid coordinates.
        /// </summary>
        [Test]
        public void Test_GetIndex()
        {
            // Arrange.
            int x = 2;
            int y = 1;
            GridBase<int> grid = new GridBase<int>(new int[,]
            {
                { 0, 1, 5 },
                { 8, 1, 3 },
                { 0, 3, 1 }
            });
            
            // Act.
            int index = grid.GetIndex(x, y);
            int expected = 5;
            
            // Assert.
            Assert.AreEqual(expected, index, "Expected the correct index to be returned but it wasn't.");
        }

        /// <summary>
        /// Expects a valid value to be returned based on the grid coordinates.
        /// </summary>
        [Test]
        public void Test_GetValue_In_Bounds()
        {
            // Arrange.
            int x = 1;
            int y = 1;
            int lookup = 15;
            
            GridBase<int> grid = new GridBase<int>(new int[,]
            {
                { 0, 1, 5 },
                { 8, lookup, 3 },
                { 0, 3, 1 }
            });
            
            // Act.
            int value = grid.GetValue(x, y);

            // Assert.
            Assert.AreEqual(lookup, value, "Expected the correct value to be found in the grid but it wasn't.");
        }
        
        /// <summary>
        /// Expects a default value to be returned if the grid coordinates are out of bounds.
        /// </summary>
        [Test]
        public void Test_GetValue_Out_Of_Bounds()
        {
            // Arrange.
            int x = 1;
            int y = 5;

            GridBase<int> grid = new GridBase<int>(new int[,]
            {
                { 0, 1, 5 },
                { 8, 15, 3 },
                { 0, 3, 1 }
            });
            
            // Act.
            int value = grid.GetValue(x, y);
            int expected = default;

            // Assert.
            Assert.AreEqual(expected, value, "Expected the default value to be returned but it wasn't.");
        }

        /// <summary>
        /// Expects the value of a grid coordinate to be set properly if the coordinates are valid.
        /// </summary>
        [Test]
        public void Test_SetValue_In_Bounds()
        {
            // Arrange.
            int x = 1;
            int y = 1;
            int newValue = 12;
            
            GridBase<int> grid = new GridBase<int>(new int[,]
            {
                { 0, 1, 5 },
                { 8, 15, 3 },
                { 0, 3, 1 }
            });
            
            // Act.
            grid.SetValue(x,y, newValue);
            int value = grid.GetValue(x, y);

            // Assert.
            Assert.AreEqual(newValue, value, "Expected the new value to be found in the grid but it wasn't.");
        }
        
        /// <summary>
        /// Expects no values to be changed in the grid if the grid coordinates for setting
        /// a value are out of bounds.
        /// </summary>
        [Test]
        public void Test_SetValue_Out_Of_Bounds()
        {
            // Arrange.
            int x = 1;
            int y = 5;

            GridBase<int> grid = new GridBase<int>(new int[,]
            {
                { 0, 1, 5 },
                { 8, 15, 3 },
                { 0, 3, 1 }
            });
            
            // Act.
            ReadOnlyCollection<int> valuesBefore = grid.Values;
            grid.SetValue(x,y, 12);
            ReadOnlyCollection<int> valuesAfter = grid.Values;

            bool condition = Enumerable.SequenceEqual(valuesBefore, valuesAfter);

            // Assert.
            Assert.IsTrue(condition, "Expected the values not to be changed but they were.");
        }

        /// <summary>
        /// Expects values of two coordinates to be swapped properly if both coordinates
        /// are within bounds.
        /// </summary>
        [Test]
        public void Test_SwapValues()
        {
            // Arrange.
            Vector2Int first = new Vector2Int(2, 1);
            Vector2Int second = new Vector2Int(2, 2);
            GridBase<int> grid = new GridBase<int>(new int[,]
            {
                { 0, 1, 5 },
                { 8, 15, 3 },
                { 0, 3, 1 }
            });
            
            // Act.
            int firstValue = grid.GetValue(first);
            int secondValue = grid.GetValue(second);
            grid.SwapValues(first, second);
            
            // Assert.
            Assert.AreEqual(firstValue, grid.GetValue(second), "Expected the first value to be at the second coordinates but it wasn't.");
            Assert.AreEqual(secondValue, grid.GetValue(first), "Expected the second value to be at the first coordinates but it wasn't.");
        }
    }
}

#endif