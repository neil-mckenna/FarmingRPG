using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Camera mainCamera;
    private Canvas parentCanvas;
    private Transform parentItem;
    private GameObject draggedItem;
    

    public Image inventorySlotHightlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    [SerializeField] private UIInventoryBar inventoryBar = null;
    [SerializeField] private GameObject inventoryTextBoxPrefab = null;
    [HideInInspector] public bool isSelected = false; 
    [HideInInspector] public ItemDetails itemDetails;
    [SerializeField] private GameObject itemPrefab = null;
    [HideInInspector] public int itemQuantity;
    [SerializeField] private int slotNumber = 0;

    private void Awake() 
    {
        
        parentCanvas = GetComponentInParent<Canvas>();
        
    }


    private void Start() 
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
        mainCamera = Camera.main;
        
        
    }

    /// <summary>
    /// Drops the item (if selected ) at teh current mouse position. Called by Drop item
    /// </summary>
    private void DropSelectedItemAtMousePosition()
    {
        if(itemDetails != null && isSelected)
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                -mainCamera.transform.position.z));

            // create item from prefab at mouseposition
            GameObject itemGameObject = Instantiate(itemPrefab, worldPos, Quaternion.identity, parentItem);
            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = itemDetails.itemCode;

            // Remove item from players inventory
            InventoryManager.Instance.RemoveItem(InventoryLocation.player, item.ItemCode);

            if(InventoryManager.Instance.FindItemInInventory(InventoryLocation.player, item.ItemCode) == -1)
            {
                ClearSelectedItem();
            } 

        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(itemDetails != null)
        {
            // disable keyboard
            Player.Instance.DisablePlayerInputAndResetMovement();

            // Instantiate the placeholder at the bar
            draggedItem = Instantiate(inventoryBar.inventoryBarDraggedItem, inventoryBar.transform);

            // Get image for dragged item
            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventorySlotImage.sprite;

            SetSelectedItem();

        }

    }
    public void OnDrag(PointerEventData eventData)
    {
        // move gameObject as dragged item
        if(draggedItem != null)
        {
            draggedItem.transform.position = Input.mousePosition;
        }

    }
    public void OnEndDrag(PointerEventData eventData)
    {
        // Destroy game object as dragged item
        if(draggedItem != null)
        {
            Destroy(draggedItem);

            // if drags ends over inventory bar get item is over and swap them 
            if(eventData.pointerCurrentRaycast.gameObject != null &&
             eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>() != null)
             {
                

                // get the slot number where teh dragged ended
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>().slotNumber;

                // Swap inventory items in inventory list
                InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, slotNumber, toSlotNumber);

                DestroyInventoryTextBox();

                // Clear selected item
                ClearSelectedItem();

             }
             // else attempt to drop item of it can be dropped
             else
             {
                 if(itemDetails.canBeDropped)
                 {
                     DropSelectedItemAtMousePosition();

                 }
             }

             // Enable player input
             Player.Instance.EnablePlayerInput();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Populate text box with item details
        if(itemQuantity != 0)
        {
            // Instantiate
            inventoryBar.inventoryTextBoxGameObject = Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
            inventoryBar.inventoryTextBoxGameObject.transform.SetParent(parentCanvas.transform, false);

            UIInventoryTextBox inventoryTextBox = inventoryBar.inventoryTextBoxGameObject.GetComponent<UIInventoryTextBox>();

            // Set item type description
            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);

            // populate the textbox
            inventoryTextBox.SetTextboxText(itemDetails.itemDescription, itemTypeDescription, "", itemDetails.itemLongDescription, "", "");

            // Set text box position according to the inventory bar position
            if(inventoryBar.isInventoryBarPositionBottom)
            {
                inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                inventoryBar.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 50f, transform.position.z);
            }
            else
            {
                inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                inventoryBar.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y - 50f, transform.position.z);

            }

        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyInventoryTextBox();

    }

    public void DestroyInventoryTextBox()
    {
        if(inventoryBar.inventoryTextBoxGameObject != null)
        {
            Destroy(inventoryBar.inventoryTextBoxGameObject);
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            // if selected then deselected
            if(isSelected == true)
            {
                ClearSelectedItem();
            }
            else
            {
                if(itemQuantity > 0)
                {
                    SetSelectedItem();
                }
            }
        }
    }

    


    /// <summary>
    /// Sets this inventory slot item to be selected
    /// </summary>
    public void SetSelectedItem()
    {
        // Clear currently highlighted items

        inventoryBar.ClearHighlightOnInventorySlots();
        
        isSelected = true;

        inventoryBar.SetHighlightedInventorySlots();

        InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, itemDetails.itemCode);

        if(itemDetails.canBeCarried == true)
        {
            // show player carryin item
            Player.Instance.ShowCarriedItem(itemDetails.itemCode);
        }
        else
        {
            // show player caryying nothing
            Player.Instance.ClearCarriedItem();
        }

    }

    public void ClearSelectedItem()
    {
        // Clear currently highlighted items
        inventoryBar.ClearHighlightOnInventorySlots();

        isSelected = false;

        // set no item select in inventory
        InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);

        // Clear carrying item
        Player.Instance.ClearCarriedItem();
    }

    

    
}
