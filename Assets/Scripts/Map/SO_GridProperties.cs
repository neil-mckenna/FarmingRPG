using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "so_GridProperties", menuName = "Scriptable Objects/Grid Properties", order = 0)]
public class SO_GridProperties : ScriptableObject 
{
    public SceneName sceneName;
    public int gridWidth;
    public int gridHeight;
    public int originX;
    public int originY;

    [SerializeField]
    public List<GridProperty> gridPropertyList;


    
}

