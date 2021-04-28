using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        Item item = collision.GetComponent<Item>();

        if(item != null)
        {
            // Get item details from 
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            Debug.Log("Item name " + itemDetails.itemDescription);

        }
        
    }
    


}
