using System;
using UnityEngine;

public class AStarNode : IComparable<AStarNode>
{
    public Vector2Int gridPosition;
    /// <summary>
    /// Distance from the starting node
    /// </summary>
    public int GCost { get; set; }
    /// <summary>
    /// Distance from the finishing node
    /// </summary>
    public int HCost { get; set; }
    public AStarNode parentNode;

    public AStarNode(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
        parentNode = null;
    }

    public int FCost { get { return GCost + HCost; } }

    public int CompareTo(AStarNode nodeToCompare)
    {
        //Compare will be less than 0 if this instance FCost is less than nodeToCompare.FCost
        //Compare will be greater than 0 if this instance FCost is greater than nodeToCompare.FCost
        //Compare will be == 0 if the values are the same

        int compare = FCost.CompareTo(nodeToCompare.FCost);

        if (compare == 0)
        {
            compare = HCost.CompareTo(nodeToCompare.HCost);
        }

        return compare;
    }
}
