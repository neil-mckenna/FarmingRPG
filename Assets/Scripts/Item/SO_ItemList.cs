
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_ItemList", menuName = "Scriptable Objects/Item/Item List", order = 0)]

public class SO_ItemList : ScriptableObject
{
    [SerializeField]
    public List<ItemDetails> itemDetails;

    
}
