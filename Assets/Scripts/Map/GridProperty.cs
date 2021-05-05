using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridProperty
{
    public GridCoord gridCoord;
    public GridBoolProperty gridBoolProperty;
    public bool gridBoolValue;

    public GridProperty(GridCoord gridCoord, GridBoolProperty gridBoolProperty, bool gridBoolValue)
    {
        this.gridCoord = gridCoord;
        this.gridBoolProperty = gridBoolProperty;
        this.gridBoolValue = gridBoolValue;

    }

}
