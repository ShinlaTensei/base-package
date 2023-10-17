using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Base.Helper
{
    public static class ListExtensionMethods
    {
        // /// <summary> Shuffle the list in place using the Fisher-Yates method. </summary>
        // /// <typeparam name="T"> Type of List </typeparam>
        // /// <param name="list"> List to shuffle </param>
        // public static void Shuffle<T>(this IList<T> list)
        // {
        //     Random rng = new Random();
        //     int n = list.Count;
        //     while (n > 1)
        //     {
        //         n--;
        //         int k = rng.Next(n + 1);
        //         (list[k], list[n]) = (list[n], list[k]);
        //     }
        // }

        public static void AddIfNotContains<T>(this IList<T> list, T value)
        {
            if (!list.Contains(value))
            {
                list.Add(value);
            }
        }

        public static void AddRangeIfNotContains<T>(this List<T> list, IList<T> values)
        {
            list.AddRange(values.Where(x => !list.Contains(x)));
        }

        /// <summary> Return a random item from the list. Sampling with replacement.  </summary>
        /// <typeparam name="T"> Type of List </typeparam>
        /// <param name="list"> List to get random item from </param>
        /// <returns> The randomly selected item </returns>
        public static T GetRandom<T>(this IList<T> list)
        {
            if (list.Count == 0)
                throw new System.IndexOutOfRangeException("Cannot select a random item from an empty list");
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        /// <summary> Removes a random item from the list, returning that item. Sampling without replacement. </summary>
        /// <typeparam name="T"> Type of List </typeparam>
        /// <param name="list"> List to remove random from </param>
        /// <returns> The randomly selected item </returns>
        public static T RemoveRandom<T>(this IList<T> list)
        {
            if (list.Count == 0)
                throw new System.IndexOutOfRangeException("Cannot remove a random item from an empty list");
            int index = UnityEngine.Random.Range(0, list.Count);
            T item = list[index];
            list.RemoveAt(index);
            return item;
        }

        /// <summary> Return a collection of random items from a list. </summary>
        /// <typeparam name="T"> Type of list. </typeparam>
        /// <param name="list"> List to get random items from. </param>
        /// <param name="size"> Size of how many the new list should contain </param>
        /// <param name="duplicates"> Whether duplicate entries are allowed. </param>
        /// <returns> A new list with randomly selected entries </returns>
        public static List<T> GetRandomCollection<T>(this IList<T> list, int size = -1, bool duplicates = false)
        {
            IList<T> pool = (duplicates) ? list : new List<T>(list);
            List<T> newList = new List<T>();

            if (size < 0 || size > pool.Count) size = pool.Count;
            for (int i = 0; i < size; i++)
            {
                if (duplicates)
                    newList.Add(pool.GetRandom());
                else
                    newList.Add(pool.RemoveRandom());
            }

            return newList;
        }


        public static T FindRandomElement<T>(this IList<T> list, Predicate<T> match)
        {
            if (match == null) throw new ArgumentNullException();
            //var random = new Random();
            int index = 0;
            do
            {
                index = Random.Range(0, list.Count);
            } while (!match(list[index]));

            return list[index];
        }

        public static int FindRandomIndex<T>(this IList<T> list, Predicate<T> match)
        {
            if (match == null) throw new ArgumentNullException();
            int index = -1;

            List<int> listIndexUnchecked = new List<int>();

            for (int i = 0; i < list.Count; ++i)
            {
                listIndexUnchecked.Add(i);
            }
            
            do
            {
                if (index >= 0) listIndexUnchecked.Remove(index);
                index = listIndexUnchecked.GetRandom();
                
            } while (!match(list[index]));

            return index;
        }

        /// <summary> Returns a collection of random items from a list. The items taken are removed from the original list. </summary>
        /// <typeparam name="T"> Type of list. </typeparam>
        /// <param name="list"> List to get random items from. </param>
        /// <param name="size"> Size of how many the new list should contain </param>
        /// <returns> A new list with randomly selected entries </returns>
        public static List<T> RemoveRandomCollection<T>(this IList<T> list, int size = -1)
        {
            List<T> newList = new List<T>();

            if (size < 0 || size > list.Count) size = list.Count;
            for (int i = 0; i < size; i++)
            {
                newList.Add(list.RemoveRandom());
            }

            return newList;
        }

        public static bool IsNullOrEmpty<T>(this List<T> list)
        {
            if (list == null)
            {
                return false;
            }

            if (list.Count < 1 && list.Capacity < 1)
            {
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Add a value to an amount of item in list
        /// </summary>
        /// <param name="value"> Value </param>
        /// <param name="length"> Range to add value (start with index 0) </param>
        /// <typeparam name="T"></typeparam>
        public static void Populate<T>(this List<T> list, T value, int length)
        {
            for (int i = 0; i < length; ++i)
            {
                list.Add(value);
            }
        }

        public static void SetActiveAllMono<T>(this List<T> list, bool isActive, int from = -1, int to = -1) where T : BaseMono
        {
            if (from == -1) from = 0;
            if (to == -1) to = list.Count;

            for (int i = from; i < to; ++i)
            {
                list[i].Active = isActive;
            }
        }
        
        public static void SetActiveAllComponent<T>(this List<T> list, bool isActive, int from = -1, int to = -1) where T : Component
        {
            if (from == -1) from = 0;
            if (to == -1) to = list.Count;

            for (int i = from; i < to; ++i)
            {
                list[i].gameObject.SetActive(isActive);
            }
        }

        public static void DestroyAll<T>(this List<T> list) where T : Component
        {
            for (int i = 0; i < list.Count; ++i)
            {
                T obj = list[i];
                if (obj != null)
                {
                    Object.Destroy(obj.gameObject);
                }
            }
        }
        
        /// <summary>
        /// Swaps values at 'first' index with value at 'second' index.
        /// </summary>
        /// <param name="list">The list to swap values of.</param>
        /// <param name="firstIndex">The first index.</param>
        /// <param name="secondIndex">The second index.</param>
        /// <typeparam name="T">The type of list.</typeparam>
        public static void Swap<T>(this IList<T> list, int firstIndex, int secondIndex)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count < 2)
                throw new ArgumentException("List count should be at least 2 for a swap.");
            
            T firstValue = list[firstIndex];
            
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = firstValue;
        }

        /// <summary>
        /// Shuffles the list using the Fisher-Yates algorithm.
        /// </summary>
        /// <param name="list">The list to shuffle.</param>
        /// <typeparam name="T">The type of list.</typeparam>
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randomIndex = Random.Range(i, list.Count);
                Swap(list, randomIndex, i);
            }
        }
        
        /// <summary>
        /// Shuffles the list using the Fisher-Yates algorithm.
        /// </summary>
        /// <param name="list">The list to shuffle.</param>
        /// <param name="seed">The seed to use for the random shuffle.</param>
        /// <typeparam name="T">The type of list.</typeparam>
        public static void Shuffle<T>(this IList<T> list, int seed)
        {
            var state = Random.state;
            Random.InitState(seed);
            
            Shuffle(list);

            Random.state = state;
        }

        /// <summary>
        /// Moves all items of a list to the left.
        /// </summary>
        /// <param name="list">The list to rotate.</param>
        /// <param name="count">The amount of times to move to the left.</param>
        /// <typeparam name="T">The type of list.</typeparam>
        public static void RotateLeft<T>(this IList<T> list, int count = 1)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count < 2)
                return;

            for (int current = 0; current < count; current++)
            {
                T first = list[0];
                list.RemoveAt(0);
                list.Add(first);
            }
        }

        /// <summary>
        /// Moves all items of a list to the right.
        /// </summary>
        /// <param name="list">The list to rotate.</param>
        /// <param name="count">The amount of times to move to the right.</param>
        /// <typeparam name="T">The type of list.</typeparam>
        public static void RotateRight<T>(this IList<T> list, int count = 1)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count < 2)
                return;

            int lastIndex = list.Count - 1;
            for (int current = 0; current < count; current++)
            {
                T last = list[lastIndex];
                list.RemoveAt(lastIndex);
                list.Insert(0, last);
            }
        }

        /// <summary>
        /// Removes null entries from a list.
        /// </summary>
        /// <typeparam name="T">The type of list.</typeparam>
        /// <param name="list">The list to remove null entries from.</param>
        public static void RemoveNullEntries<T>(this IList<T> list) where T : class
        {
            for (int i = list.Count - 1; i >= 0; i--)
                if (Equals(list[i], null))
                    list.RemoveAt(i);
        }

        /// <summary>
        /// Removes default values from a list.
        /// </summary>
        /// <typeparam name="T">The type of list.</typeparam>
        /// <param name="list">The list to remove default values from.</param>
        public static void RemoveDefaultValues<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
                if (Equals(default(T), list[i]))
                    list.RemoveAt(i);
        }

        /// <summary>
        /// Returns whether an index is inside the bounds of the list.
        /// </summary>
        /// <typeparam name="T">The type of list to check the bounds of.</typeparam>
        /// <param name="list">The list to check the bounds of.</param>
        /// <param name="index">The index to check.</param>
        /// <returns>Whether the index is inside the bounds.</returns>
        public static bool HasIndex<T>(this IList<T> list, int index) => index.InRange(0, list.Count - 1);
    }
}