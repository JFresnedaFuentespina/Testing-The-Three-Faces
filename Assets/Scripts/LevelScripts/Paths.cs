using System.IO;
using UnityEngine;

[System.Serializable]
public class Paths
{
    public Vector2Int from;
    public Vector2Int to;

    public Paths(Vector2Int from, Vector2Int to)
    {
        this.from = from;
        this.to = to;
    }
}