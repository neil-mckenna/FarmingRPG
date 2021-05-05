using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridCoord 
{
    public int x;
    public int y;

    public GridCoord(int p1, int p2)
    {
        x = p1;
        y = p2;
    }

    public static explicit operator Vector2(GridCoord gridCoord)
    {
        return new Vector2((float)gridCoord.x, (float)gridCoord.y);
    }

    public static explicit operator Vector2Int(GridCoord gridCoord)
    {
        return new Vector2Int(gridCoord.x, gridCoord.y);
    }

    public static explicit operator Vector3(GridCoord gridCoord)
    {
        return new Vector3((float)gridCoord.x, (float)gridCoord.y, 0f);
    }

    public static explicit operator Vector3Int(GridCoord gridCoord)
    {
        return new Vector3Int(gridCoord.x, gridCoord.y, 0);
    }

}
