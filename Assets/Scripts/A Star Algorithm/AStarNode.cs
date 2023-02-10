using System;
using UnityEngine;

public class AStarNode : IHeapItem<AStarNode>
{
    public Vector2Int gridPosition;
    int heapIndex;
    
    public int HeapIndex { get { return heapIndex; } set { heapIndex = value; } }

    /// <summary>
    /// Distance from the starting node
    /// </summary>
    public int GCost { get; set; }
    /// <summary>
    /// Distance from the finishing node
    /// </summary>
    public int HCost { get; set; }
    public AStarNode parentNode;
    
    /// <summary>
    /// The total cost of this node
    /// </summary>
    public int FCost { get { return GCost + HCost; } }

    public AStarNode(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
        parentNode = null;
    }

    public int CompareTo(AStarNode nodeToCompare)
    {
        // With Heaps implemented we want this method
        // to return 1 if the current Node has a higher priority
        // than the nodeToCompare.
        // The CompareTo method returns 1 if the integer is higher, with heaps is reverse
        // we want 1 if it's lower, because a higher priority means the FCost is lower,
        // therefore the item must be higher in the binary heap (because in binary heaps the item
        // with a lower number is at the top of the other items with higher numbers).
        // That's why here we return -compare.

        int compare = FCost.CompareTo(nodeToCompare.FCost);

        if (compare == 0)
            compare = HCost.CompareTo(nodeToCompare.HCost);

        return -compare;
    }
}
