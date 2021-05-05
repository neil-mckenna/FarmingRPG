using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneSave
{
    // string key is an identifier name we choose for this list
    public Dictionary<string, List<SceneItem>> listSceneItemDictionary;
}
