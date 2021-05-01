using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryBar : MonoBehaviour
{
    [SerializeField] private Sprite blank16x16sprite = null;
    [SerializeField] private UIInventorySlot[] inventorySlots = null;
    public GameObject inventoryBarDraggedItem;
    [HideInInspector] public GameObject inventoryTextBoxGameObject;


    private RectTransform rectTransform;

    private bool _isInventoryBarPositionBottom = true;

    public bool isInventoryBarPositionBottom { get => _isInventoryBarPositionBottom; set => _isInventoryBarPositionBottom = value; }

    private void Awake() 
    {

        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable() 
    {
        EventHandler.InventoryUpdatedEvent += InventoryUpdated;
        
    }

    private void OnDisable() 
    {
        EventHandler.InventoryUpdatedEvent -= InventoryUpdated;
    }


    private void LateUpdate() 
    {
        // Switch inventory bar position depending on player position
        SwitchInventoryBarPosition();         
    }

    

    private void InventoryUpdated(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if(inventoryLocation == InventoryLocation.player)
        {
            ClearInventorySlots();

            if(inventorySlots.Length > 0 && inventoryList.Count > 0)
            {
                // loop through inventory slots and update witha  corresponding inventory list item
                for(int i = 0; i < inventorySlots.Length; i++)
                {
                    if(i < inventoryList.Count)
                    {
                        int itemCode = inventoryList[i].itemCode;

                        // ItemDetails = InventoryManager.Instance.itemList.itemdetails.Find(x => x.itemCode == itemCode)
                        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

                        if(itemDetails != null)
                        {
                            // add images and details to the item slot
                            inventorySlots[i].inventorySlotImage.sprite = itemDetails.itemSprite;
                            inventorySlots[i].textMeshProUGUI.text = inventoryList[i].itemQuantity.ToString();
                            inventorySlots[i].itemDetails = itemDetails;
                            inventorySlots[i].itemQuantity = inventoryList[i].itemQuantity;
                            SetHighlightedInventorySlots(i);

                        }

                    }
                    else
                    {
                        break;
                    }
                }

            }

        }

    }


    private void SwitchInventoryBarPosition()
    {
        Vector3 playerViewportPosition = Player.Instance.GetPlayerViewportPosition();

        if(playerViewportPosition.y > 0.3f && isInventoryBarPositionBottom == false)
        {
            // transform.position = new Vector3(transform.position.x, 7.5f, 0f); // this was changed to control the recttransform see below
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 2.5f);

            isInventoryBarPositionBottom = true; 

        }
        else if(playerViewportPosition.y <= 0.3f && isInventoryBarPositionBottom == true)
        {
            // move to top 
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -2.5f);

            isInventoryBarPositionBottom = false;

        }
    }

    

    public void SetHighlightedInventorySlots()
    {
        if(inventorySlots.Length > 0)
        {
            for(int i = 0; i < inventorySlots.Length; i++)
            {
                SetHighlightedInventorySlots(i);
            }    
        }
    }

    // overloaded 
    public void SetHighlightedInventorySlots(int itemPosition)
    {
        if(inventorySlots.Length > 0 && inventorySlots[itemPosition].itemDetails != null)
        {
            if(inventorySlots[itemPosition].isSelected)
            {
                inventorySlots[itemPosition].inventorySlotHightlight.color = new Color(1f, 1f, 1f , 1f);

                // Update inventory to show item as selected
                InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, inventorySlots[itemPosition].itemDetails.itemCode);
            }
        }
    }

    private void ClearInventorySlots()
    {
        if(inventorySlots.Length > 0)
        {
            // loop through inventory slots
            for(int i = 0; i < inventorySlots.Length; i++)
            {
                inventorySlots[i].inventorySlotImage.sprite = blank16x16sprite;
                inventorySlots[i].textMeshProUGUI.text = "";
                inventorySlots[i].itemDetails = null;
                inventorySlots[i].itemQuantity = 0;

                SetHighlightedInventorySlots(i);
            }
        }
    }

    

    public void ClearHighlightOnInventorySlots()
    {
        if(inventorySlots.Length > 0)
        {
            // loop through inventory slotsand clear the highlighting
            for(int i = 0; i < inventorySlots.Length; i++)
            {
                if(inventorySlots[i].isSelected)
                {
                    inventorySlots[i].isSelected = false;
                    inventorySlots[i].inventorySlotHightlight.color = new Color(0f, 0f, 0f, 0f);
                    // Update inventory to show item is not selected
                    InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);
                }
            }
        }
    }

    


}
