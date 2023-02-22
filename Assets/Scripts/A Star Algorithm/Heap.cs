using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame.AStarPathfinding
{
    /// <summary>
    /// This generic class makes the implementation for a binary heap possible
    /// and is used in the <seealso cref="AStar"/> class, so the A* algorithm is more efficient.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Heap<T> where T : IHeapItem<T>
    {
        private readonly T[] items;

        private int currentItemCount;

        /// <summary>
        /// The count of the items that are currently on the heap.
        /// </summary>
        public int Count { get { return currentItemCount; } }

        /// <summary>
        /// Creates a new array of the instance T with the passed int,
        /// because an array is difficult to resize on runtime
        /// we need to know how many nodes are gonna be on the heap,
        /// and because the <seealso cref="AStar"/> class generetas a path on a grid,
        /// we can multiply the grid height and the grid width to know the maximum number of
        /// nodes that can be on the grid and therefore on the heap.
        /// </summary>
        /// <param name="maxHeapSize">The max size of the heap, multiply the grid width for the 
        /// grid heigth to know the maximum posible size for this heap.</param>
        public Heap(int maxHeapSize)
        {
            items = new T[maxHeapSize];
        }

        /// <summary>
        /// Adds new items to the heap
        /// </summary>
        /// <param name="item">The new item to add</param>
        public void Add(T item)
        {
            item.HeapIndex = currentItemCount;
            items[currentItemCount] = item;
            SortItemsUp(item);
            currentItemCount++;
        }

        /// <summary>
        /// Update an item if another item with a lower cost was found.
        /// </summary>
        /// <param name="item"></param>
        public void UpdateItem(T item)
        {
            SortItemsUp(item);
        }

        /// <summary>
        /// Check if the item is in the heap
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if the item is in the heap, false otherwise</returns>
        public bool Contains(T item)
        {
            return Equals(items[item.HeapIndex], item);
        }

        /// <summary>
        /// Removes the first item of the array
        /// </summary>
        /// <returns>Returns the first item of the array</returns>
        public T RemoveFirst()
        {
            // Retrieve the first item of the array
            T firstItem = items[0];

            // Reduce the item count
            currentItemCount--;

            // Reset the first item on the array
            items[0] = items[currentItemCount];
            items[0].HeapIndex = 0;

            SortItemsDown(items[0]);

            return firstItem;
        }

        /// <summary>
        /// Sort the items on the heap,
        /// from lowest to highest priority.
        /// If the child node has a higher priority than it's parent,
        /// the nodes will swape place.
        /// </summary>
        /// <param name="item"></param>
        private void SortItemsDown(T item)
        {
            while (true)
            {
                // Calculate the index of the child node on the left
                int childIndexLeft = item.HeapIndex * 2 + 1;

                // Calculate the index of the child node on the right
                int childIndexRight = item.HeapIndex * 2 + 2;
                int swapIndex = 0;

                // Check if this parent node has a children
                if (childIndexLeft < currentItemCount)
                {
                    swapIndex = childIndexLeft;

                    // Also check if this node has a child on the rigth
                    if (childIndexRight < currentItemCount)
                    {
                        // Check which children has a higher priority
                        if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                        {
                            swapIndex = childIndexRight;
                        }
                    }

                    // Check if the parent has a higher priority, if not swap it with the child with the
                    // highest priority.
                    if (item.CompareTo(items[swapIndex]) < 0)
                    {
                        SwapItems(item, items[swapIndex]);
                    }
                    else
                        // The parent has a higher priority so just return.
                        return;
                }
                else
                    // If the parent does not have children, then it is in the correct position
                    // just return.
                    return;
            }
        }

        /// <summary>
        /// Sort the items on the heap,
        /// from lowest to highest priority.
        /// If the child node has a higher priority than it's parent,
        /// the nodes will swape place.
        /// </summary>
        /// <param name="item"></param>
        private void SortItemsUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;

            while (true)
            {
                T parentItem = items[parentIndex];

                // The compareTO method will return 1 if the node to compare has higher priority
                // 0 is it has the same priority
                // and -1 if it has lower priority
                if (item.CompareTo(parentItem) > 0)
                {
                    SwapItems(item, parentItem);
                }
                else
                    // break out of the loop once we child node is no longer has a
                    // higher priority than it's parent
                    break;
                // else just keep recalculating the priority until we break out of the loop
                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        /// <summary>
        /// Swaps the items depending on the priority of each one
        /// </summary>
        /// <param name="itemA"></param>
        /// <param name="itemB"></param>
        private void SwapItems(T itemA, T itemB)
        {
            // Swap the itemA und itemB
            items[itemA.HeapIndex] = itemB;
            items[itemB.HeapIndex] = itemA;

            // Swap also the indexes
            (itemB.HeapIndex, itemA.HeapIndex) = (itemA.HeapIndex, itemB.HeapIndex);
        }
    }

    public interface IHeapItem<T> : IComparable<T>
    {
        /// <summary>
        /// The index of this item on the heap
        /// </summary>
        public int HeapIndex { get; set; }
    }
}