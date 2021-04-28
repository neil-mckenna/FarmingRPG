using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonobehaviour<InventoryManager>
{
    private Dictionary<int, ItemDetails> itemDetailsDictionary;

    [SerializeField] private SO_ItemList itemList = null;

    protected override void Awake()
    {
        base.Awake();

        // Create item details dictionary
        CreateItemsDetailsDictionary(); 
    }

    /// <summary>
    ///   Populates the itemDetailsDictionary from a scriptable object item list 
    /// </summary>
    private void CreateItemsDetailsDictionary()
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();


        

        foreach(ItemDetails itemDetails in itemList.itemDetails)
        {
            itemDetailsDictionary.Add(itemDetails.itemCode, itemDetails);


        } 

    }

    /// <summary>
    ///  Returns the itemDetails (from the SO_Itemlist) for teh itemcode, or null if the item code does not exist
    /// </summary>

    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails itemDetails;


        if(itemDetailsDictionary.TryGetValue(itemCode, out itemDetails))
        {
            return itemDetails;
        }
        else
        {
            return null;
        }


    } 




    
}
