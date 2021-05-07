using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonobehaviour<GridPropertiesManager>, ISaveable
{
    private Tilemap groundDecoration1;
    private Tilemap groundDecoration2;

    private Grid grid;
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary;
    [SerializeField] private SO_GridProperties[] so_gridPropertiesArray = null;
    [SerializeField] private Tile[] dugGround = null;
    [SerializeField] private Tile[] wateredGround = null;


    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get{ return _iSaveableUniqueID; } set{ _iSaveableUniqueID = value ; } }
    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set{ _gameObjectSave = value; } }


    // method events -----------------------------------------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        ISaveableRegister();

        EventHandler.AfterSceneLoadedEvent += AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent += AdvanceDay;
    }

    protected void OnDisable() 
    {
        ISaveableDeregister();

        EventHandler.AfterSceneLoadedEvent -= AfterSceneLoaded;  
        EventHandler.AdvanceGameDayEvent -= AdvanceDay;      
    }

    private void AfterSceneLoaded()
    {
        // get grid
        grid = GameObject.FindObjectOfType<Grid>();

        // Get tilemaps (to show prevous dug tiles)
        groundDecoration1 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration1).GetComponent<Tilemap>();
        groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>();
    }

    private void Start() 
    {
        InitialiseGridProperties();
    }

    private void ClearDisplayGroundDecorations()
    {
        // Remove ground decorations
        groundDecoration1.ClearAllTiles();
        groundDecoration2.ClearAllTiles();
    }

    private void ClearDisplayGridPropertyDetails()
    {
        ClearDisplayGroundDecorations();
    }

    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        // Dug
        if(gridPropertyDetails.daysSinceLastDug > -1)
        {
            ConnectDugGround(gridPropertyDetails);
        }
    }

    public void DisplayWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        // watered
        if(gridPropertyDetails.daysSinceWatered > -1)
        {
            ConnectWateredGround(gridPropertyDetails);
        }
    }

    private void ConnectDugGround(GridPropertyDetails gridPropertyDetails)
    {
        // Select tile based on surrounding dug tiles
        Tile dugTile0 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), dugTile0);

        // Set4 tiles if dug surroundingcurrent tile - up down left right now that this central tle has been dug

        GridPropertyDetails adjacentGridPropertyDetails;

        // up y+
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if(adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceLastDug > -1)
        {
            Tile dugTile1 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), dugTile1);
        }

        // down y-
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if(adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceLastDug > -1)
        {
            Tile dugTile2 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), dugTile2);
        }

        // right x+
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if(adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceLastDug > -1)
        {
            Tile dugTile3 = SetDugTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), dugTile3);
        }

        // left x-
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if(adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceLastDug > -1)
        {
            Tile dugTile4 = SetDugTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), dugTile4);
        }
    }

        private Tile SetDugTile(int xGrid, int yGrid)
    {
        // Get whether surrounding tile (up,down, left, right) are dug or not

        bool upDug = IsGridSquareDug(xGrid, yGrid + 1);
        bool downDug = IsGridSquareDug(xGrid, yGrid - 1);
        bool rightDug = IsGridSquareDug(xGrid + 1, yGrid);
        bool leftDug = IsGridSquareDug(xGrid - 1, yGrid);

        #region Set appropiate tile based on whether surrounding tiles are dug or not

        // no surround
        if(!upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[0];
        }
        // down & right
        else if(!upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[1];
        }
        // down, right, left
        else if(!upDug && downDug && rightDug && leftDug)
        {
            return dugGround[2];
        }
        // down left
        else if(!upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[3];
        }
        // down
        else if(!upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[4];
        }
        // up down right
        else if(upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[5];
        }
        // up down right left
        else if(upDug && downDug && rightDug && leftDug)
        {
            return dugGround[6];
        }
        // up down left
        else if(upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[7];
        }
        // up down
        else if(upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[8];
        }
        // up right
        else if(upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[9];
        }
        // up right left
        else if(upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[10];
        }
        // up left
        else if(upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[11];
        }
        // up
        else if(upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[12];
        }
        // right
        else if(!upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[13];
        }
        // right left
        else if(!upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[14];
        }
        // left
        else if(!upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[15];
        }

        return null;

        #endregion Set appropiate tile based on whether surrounding tiles are dug or not
    }

    private bool IsGridSquareDug(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);

        if(gridPropertyDetails == null)
        {
            return false;
        }
        else if(gridPropertyDetails.daysSinceLastDug > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Watered --------------------------------------------------------------------------------------------------------------

    private void ConnectWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        // Select tile based on surrounding tiles

        Tile wateredTile0 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), wateredTile0);

        // Set 4 tiles if water surrunded current tile - up down left right and now that this central has been watered

        GridPropertyDetails adjacentGridPropertyDetails;
        
        // up adjacent
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if(adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > 1)
        {
            Tile wateredTile1 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), wateredTile1); 
        }

        // down adjacent
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if(adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > 1)
        {
            Tile wateredTile2 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), wateredTile2); 
        }

        // left adjacent
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if(adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > 1)
        {
            Tile wateredTile3 = SetWateredTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), wateredTile3); 
        }

        // right adjacent
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if(adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > 1)
        {
            Tile wateredTile4 = SetWateredTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1 , gridPropertyDetails.gridY, 0), wateredTile4); 
        }
    }

    private Tile SetWateredTile(int xGrid, int yGrid)
    {
        // Get whether surrounding tile (up,down,left, and right) are watered or not

        bool upWatered = IsGridSquareWatered(xGrid, yGrid + 1);
        bool downWatered = IsGridSquareWatered(xGrid, yGrid - 1);
        bool rightWatered = IsGridSquareWatered(xGrid + 1, yGrid);
        bool leftWatered = IsGridSquareWatered(xGrid - 1, yGrid);

        #region Set apporiate tile based on whether surrounding tiles are waterd or not

        // alone
        if(!upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[0];
        }
        // down right
        else if(!upWatered && downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[1];
        }
        // down right left
        else if(!upWatered && downWatered && rightWatered && leftWatered)
        {
            return wateredGround[2];
        }
        // down left
        else if(!upWatered && downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[3];
        }
        // down
        else if(!upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[4];
        }
        // up down right
        else if(upWatered && downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[5];
        }
        // up down left right
        else if(upWatered && downWatered && rightWatered && leftWatered)
        {
            return wateredGround[6];
        }
        // up down left
        else if(!upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[7];
        }
        // up down
        else if(!upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[8];
        }
        // up right
        else if(upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[9];
        }
        // up right left
        else if(upWatered && !downWatered && rightWatered && leftWatered)
        {
            return wateredGround[10];
        }
        // up left
        else if(upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[11];
        }
        // up 
        else if(upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[12];
        }
        // right
        else if(!upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[13];
        }
        // right left
        else if(!upWatered && !downWatered && rightWatered && leftWatered)
        {
            return wateredGround[14];
        }
        // left
        else if(!upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[15];
        }

        return null;

        #endregion Set appropiate tile based on whether surrounding tile are watered or not
    }

    private bool IsGridSquareWatered(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);

        if(gridPropertyDetails == null)
        {
            return false;
        }
        else if(gridPropertyDetails.daysSinceWatered > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DisplayGridPropertyDetails()
    {
        // Loop through all grid items
        foreach(KeyValuePair<string, GridPropertyDetails> item in gridPropertyDictionary)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;

            DisplayDugGround(gridPropertyDetails);

            DisplayWateredGround(gridPropertyDetails);
        }
    }



    // intialise -----------------------------------------------------------------------------------------
    public void InitialiseGridProperties()
    {
        // Loop through all gridProperties in the array
        foreach(SO_GridProperties so_GridProperties in so_gridPropertiesArray)
        {
            //Debug.Log("so_grid props " + so_GridProperties);


            // Create dictionary of grid property details
            Dictionary<string, GridPropertyDetails> gridPropertyDictionary = new Dictionary<string, GridPropertyDetails>();

            // Populate grid property dictionary - Iterate through all grid properties in the so gridproperties list
            foreach(GridProperty gridProperty in so_GridProperties.gridPropertyList)
            {
                   

                GridPropertyDetails gridPropertyDetails;

                gridPropertyDetails = GetGridPropertyDetails(gridProperty.gridCoord.x, gridProperty.gridCoord.y, gridPropertyDictionary);

                if(gridPropertyDetails == null)
                {
                    //Debug.Log("gridProperty was null so a new GridPropertyDetails is created");
                    gridPropertyDetails = new GridPropertyDetails();
                }
                if(gridPropertyDetails != null)
                {
                    //Debug.Log("Well done");
                }
                 
                switch(gridProperty.gridBoolProperty)
                {
                    
                    case GridBoolProperty.diggable:
                        gridPropertyDetails.isDiggable = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.canDropItem:
                        gridPropertyDetails.canDropItem = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.canPlaceFurniture:
                        gridPropertyDetails.canPlaceFurniture = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.isPath:
                        gridPropertyDetails.isPath = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.isNPCObstacle:
                        gridPropertyDetails.isNPCObstacle = gridProperty.gridBoolValue;
                        break;

                    default:
                        Debug.Log("entered the switch");
                        break;
                }
                

                SetGridPropertyDetails(gridProperty.gridCoord.x, gridProperty.gridCoord.y, gridPropertyDetails, gridPropertyDictionary);
            }

            // create a scene osave for this gameobject
            SceneSave sceneSave = new SceneSave();

            // Add grid property dictionary to scene data
            sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

            // if starting scene set teh gridPropertyDictionary member variable to the current iteration
            if(so_GridProperties.sceneName.ToString() == SceneControllerManager.Instance.startingSceneName.ToString())
            {
                
                this.gridPropertyDictionary = gridPropertyDictionary;
            }

            // Add sceneSave to game object scene data
            GameObjectSave.sceneData.Add(so_GridProperties.sceneName.ToString(), sceneSave);
        }
    }

    

    // ----------------------------------------------------------------------------------------------------
    // ISaveable interface methods  -----------------------------------------------------------------------
    // ----------------------------------------------------------------------------------------------------

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    //reload
    public void ISaveableRestoreScene(string sceneName)
    {
        // Get sceneSave for scene - it exists since we created it in initilaise
        if(GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            // Get grid property details dictionary - it exists since we created it in initilaise
            if(sceneSave.gridPropertyDetailsDictionary != null)
            {
                gridPropertyDictionary = sceneSave.gridPropertyDetailsDictionary;
            }

            // if grid properties exist
            if(gridPropertyDictionary.Count > 0)
            {
                // grid property details foudn for the current scene destroy existing grond decoration
                ClearDisplayGridPropertyDetails();

                // Instantiate grid properties details for current scene
                DisplayGridPropertyDetails();
            }
        }
    }
    // save
    public void ISaveableStoreScene(string sceneName)
    {
        // Remove sceneSave for scene
        GameObjectSave.sceneData.Remove(sceneName);

        // Create sceneSave for scene
        SceneSave sceneSave = new SceneSave();

        // create & add dict grid property details dictionary
        sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

        // Add scene save to gameObject scene data
        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }

    // ----------------------------------------------------------------------------------------------------
    // Grid Properties detail setter and getter -----------------------------------------------------------
    // ----------------------------------------------------------------------------------------------------

    /// <summary>
    ///     Returns the gridPropetiesDetails at the gridlocation for the supplied dictionary, or null if no properties exist at that location. 
    /// </summary>
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        
        // Construct key from coordinate
        string key = "x" + gridX + "y" + gridY;
        

        GridPropertyDetails gridPropertyDetails; 

        // Check if grid property details exist for coordinate and retrieve
        if(!gridPropertyDictionary.TryGetValue(key, out gridPropertyDetails))
        {
            // if not found            
            return null;
        }
        else
        {
            return gridPropertyDetails;
        }
    }

    // overload get 
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
    {
        
        return GetGridPropertyDetails(gridX, gridY, gridPropertyDictionary);
    }

    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        //Debug.Log("SetGridPropertyDetails  top was called");
        SetGridPropertyDetails(gridX, gridY, gridPropertyDetails, gridPropertyDictionary);
    }

    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        //Debug.Log("SetGridPropertyDetails bottom was called");
        // construct key from coordinate
        string key = "x" + gridX + "y" + gridY;

        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;

        // Set value
        gridPropertyDictionary[key] = gridPropertyDetails;
    }

    public void AdvanceDay(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        // Clear Display all Grid Property Details
        ClearDisplayGridPropertyDetails();

        // Loop through all scenes - by looping through all gridproperties in the array
        foreach(SO_GridProperties so_GridProperties in so_gridPropertiesArray)
        {
            // get gridPropertdetail dictionary scene
            if(GameObjectSave.sceneData.TryGetValue(so_GridProperties.sceneName.ToString(), out SceneSave sceneSave))
            {
                if(sceneSave.gridPropertyDetailsDictionary != null)
                {
                    for(int i  = sceneSave.gridPropertyDetailsDictionary.Count - 1; i >= 0; i--)
                    {
                        KeyValuePair<string, GridPropertyDetails> item = sceneSave.gridPropertyDetailsDictionary.ElementAt(i);

                        GridPropertyDetails gridPropertyDetails = item.Value;

                        #region Update all grid properties to reflect the advance in day

                        // if ground is watered, then clear water
                        if(gridPropertyDetails.daysSinceWatered > -1)
                        {
                            gridPropertyDetails.daysSinceWatered = -1;
                        }

                        // set gridproperty details
                        SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails, sceneSave.gridPropertyDetailsDictionary);

                        # endregion Update all grid properties to reflect the advance in day
                    }
                }
            }
        }

        // Display grid property details to relfect changed value
        DisplayGridPropertyDetails();

    }

}
