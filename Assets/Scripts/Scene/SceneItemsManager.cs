using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GenerateGUID))]
public class SceneItemsManager : SingletonMonobehaviour<SceneItemsManager>, ISaveable
{
    private Transform parentItem;
    [SerializeField] private GameObject itemPrefab = null;

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID{ get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get{ return _gameObjectSave;} set{ _gameObjectSave = value; }}

    private void AfterSceneLoad()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    protected override void Awake() 
    {
        base.Awake();

        ISaveableUniqueID = new GenerateGUID().GUID;
        GameObjectSave = new GameObjectSave();
    }

    ///<summary>
    /// Destroy items currently in the scene
    ///</summary>
    private void DestroySceneItems()
    {
        // Get all items in scene
        Item[] itemInScene = GameObject.FindObjectsOfType<Item>();

        // loop through scene item and destroy them
        for(int i = itemInScene.Length - 1; i > -1; i--)
        {
            Destroy(itemInScene[i].gameObject);
        }
    }

    public void InstantiateSceneItem(int itemCode, Vector3 itemPosition)
    {
        GameObject itemGameObject = Instantiate(itemPrefab, itemPosition, Quaternion.identity, parentItem);
        Item item = itemGameObject.GetComponent<Item>();
        item.Init(itemCode);
    }

    public void InstantiateSceneItems(List<SceneItem> sceneItemsList)
    {
        GameObject itemGameObject;

        foreach(SceneItem sceneItem in sceneItemsList)
        {
            itemGameObject = Instantiate(
                itemPrefab,
                new Vector3(
                    sceneItem.position.x,
                    sceneItem.position.y,
                    sceneItem.position.z),
                Quaternion.identity,
                parentItem);
            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = sceneItem.itemCode;
            item.name = sceneItem.itemName;
        }
    }

    // Events ---------------------------------------------------------

    private void OnEnable() 
    {
        ISaveableRegister();
        EventHandler.AfterSceneLoadedEvent += AfterSceneLoad;
    }

    private void OnDisable() 
    {
        ISaveableDeregister();
        EventHandler.AfterSceneLoadedEvent -= AfterSceneLoad;
    }

    // Interface methods ---------------------------------------------------

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);

    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void ISaveableStoreScene(string sceneName)
    {
        // Remove old scene save for gameObject if exists
        GameObjectSave.sceneData.Remove(sceneName);

        // Get all items in the scene
        List<SceneItem> sceneItemList = new List<SceneItem>();
        Item[] itemInScene = FindObjectsOfType<Item>();

        // Loop through all scene items
        foreach(Item item in itemInScene)
        {
            SceneItem sceneItem = new SceneItem();
            sceneItem.itemCode = item.ItemCode;
            sceneItem.position = new Vector3Serializable(
                item.transform.position.x,
                item.transform.position.y,
                item.transform.position.z);
            sceneItem.itemName = item.name;

            // Add scene item to list
            sceneItemList.Add(sceneItem);    
        }

        // create list scene items dictionary in scene save and add to it
        SceneSave sceneSave = new SceneSave();
        sceneSave.listSceneItemDictionary = new Dictionary<string, List<SceneItem>>();
        sceneSave.listSceneItemDictionary.Add("sceneItemList", sceneItemList);

        // Add scene save to list
        GameObjectSave.sceneData.Add(sceneName, sceneSave);

    }

    public void ISaveableRestoreScene(string sceneName)
    {
        if(GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if(sceneSave.listSceneItemDictionary != null && sceneSave.listSceneItemDictionary.TryGetValue("sceneItemList", out List<SceneItem> sceneItemList))
            {
                // scene list item founds - destroy items in scene
                DestroySceneItems();

                // now instantiate the list of scene items
                InstantiateSceneItems(sceneItemList);
            }
        }
    }

    
}
