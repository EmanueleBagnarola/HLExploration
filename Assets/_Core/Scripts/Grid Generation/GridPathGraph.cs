using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Grid/PathGraph", fileName = "NewGridPathGraph")]
public class GridPathGraph : ScriptableObject
{
    [Serializable]
    public class Node
    {
        public Vector2Int position;
        public List<Vector2Int> connectedTo = new List<Vector2Int>();
    }

    public List<Node> nodes = new List<Node>();

    public Node GetNodeAt(Vector2Int pos) => nodes.Find(n => n.position == pos);
}