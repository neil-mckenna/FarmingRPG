using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Camera mainCamera;
    private Transform parentItem;
    private GameObject draggedItem;
    

    public Image inventorySlotHightlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    [SerializeField] private UIInventoryBar inventoryBar = null;
    [HideInInspector] public ItemDetails itemDetails;

    [SerializeField] private GameObject itemPrefab = null;
    [HideInInspector] public int itemQuantity;


    private void Start() 
    {
        mainCamera = Camera.main;
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
        
    }

    /// <summary>
    /// Drops the item (if selected ) at teh current mouse position. Called by Drop item
    /// </summary>
    private void DropSelectedItemAtMousePosition()
    {
        if(itemDetails != null)
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

    

    
}
