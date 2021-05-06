using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCursor : MonoBehaviour
{
    private Canvas canvas;
    private Grid grid;
    private Camera mainCamera;

    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite redCursorSprite = null;

    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }

    private int _itemUseGridRadius = 0;
    public int ItemUseGridRadius { get => _itemUseGridRadius; set => _itemUseGridRadius = value; }

    private ItemType _selectedItemType;
    public ItemType SelectedItemType { get => _selectedItemType; set => _selectedItemType = value; }

    private bool _cursorIsEnabled = true;
    public bool CursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; }

    private void OnEnable() 
    {
        EventHandler.AfterSceneLoadedEvent += SceneLoad;
    }

    private void OnDisable() 
    {
        EventHandler.AfterSceneLoadedEvent -= SceneLoad;
    }

    private void Start() 
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }

    private void Update() 
    {
        
        if(CursorIsEnabled)
        {
            DisplayCursor();
        }
    }

    private Vector3Int DisplayCursor()
    {
        if(grid != null)
        {
            // get the position for cursor
            Vector3Int gridPosition = GetGridPositionForCursor();

            // Get grid position for player
            Vector3Int playerGridPosition = GetGridPositionForPlayer();

            // Set Cursor Sprite
            SetCursorValidity(gridPosition, playerGridPosition);

            // Get rect transform position for cursor
            cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);

            return gridPosition;
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    private void SceneLoad()
    {
        grid = GameObject.FindObjectOfType<Grid>();
        
    }

    private void SetCursorValidity(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        SetCursorToValid();

        // check item use radius is valid
        if(Mathf.Abs(cursorGridPosition.x - playerGridPosition.x) > ItemUseGridRadius 
        || Mathf.Abs(cursorGridPosition.y - playerGridPosition.y) > ItemUseGridRadius)
        {
            SetCursorToValid();
            return;
        }

        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if(itemDetails == null)
        {
            SetCursorToInvalid();
            return;
        }

        // get grid property details at cursor position
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        if(gridPropertyDetails != null)
        {
            // Determine cursor validity based on inventory item selected and grid property details
            switch(itemDetails.itemType)
            {
                case ItemType.Seed:
                    if(!IsCursorValidForSeed(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;
                
                case ItemType.Commodity:
                    if(!IsCursorValidForSeed(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;

                case ItemType.none:
                    break;
                case ItemType.count:
                    break;
                default:
                    break;
            }
        }
        else
        {
            SetCursorToInvalid();
            return;
        }
    }

    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
    }

    private void SetCursorToInvalid()
    {
        cursorImage.sprite = redCursorSprite;
        CursorPositionIsValid = false;
    }

    private bool IsCursorValidForSeed(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    private bool IsCursorValidForCommodity(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        CursorIsEnabled = true;
    }

    public void DisableCursor()
    {
        cursorImage.color = Color.clear;
        CursorIsEnabled = false;
    }

    public Vector3Int GetGridPositionForCursor()
    {
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        
        return grid.WorldToCell(worldPos);
    }

    public Vector3Int GetGridPositionForPlayer()
    {
        return grid.WorldToCell(Player.Instance.transform.position);
    }

    public Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {
        Vector3 gridWorldPosition = grid.CellToWorld(gridPosition);
        Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition);
        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, canvas);
    }




    
}
