using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class GenerateGUID : MonoBehaviour
{
    [SerializeField]
    private string _gUID = "";
    
    public string GUID { get => _gUID; set => _gUID = value; }

    private void Awake()
    {
        // Only poulate in the editor
        if(!Application.IsPlaying(gameObject))
        {
            // Ensure the object has a guranteed uinique id;
            if(_gUID == "")
            {
                // Assign GUID
                _gUID = System.Guid.NewGuid().ToString();
            }
        }


    
    }
}
