using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonobehaviour<GridPropertiesManager>, ISaveable
{
    public Grid grid;
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary;
    [SerializeField] private SO_GridProperties[] so_gridPropertiesArray = null;

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
    }

    protected void OnDisable() 
    {
        ISaveableDeregister();

        EventHandler.AfterSceneLoadedEvent -= AfterSceneLoaded;        
    }

    private void AfterSceneLoaded()
    {
        // get grid
        grid = GameObject.FindObjectOfType<Grid>();
    }

    private void Start() 
    {
        InitialiseGridProperties();
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
        Debug.Log("SetGridPropertyDetails  top was called");
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

}
