using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SwitchConfineBoundingShape : MonoBehaviour
{
    
    private void OnEnable() 
    {
        EventHandler.AfterSceneLoadedEvent += SwitchBoundingShape;
    }

    private void OnDisable() 
    {
        EventHandler.AfterSceneLoadedEvent -= SwitchBoundingShape;
        
    }


    /// <summary>
    ///   Switch the collider that cinemachine uses to define teh edges of the screen
    ///</summary>
    private void SwitchBoundingShape()
    {
        // Get the polygon collider on the 'boundsconfiner' gameobject which is used by cinemachine to prevent teh camera going beyong the screen edges
        PolygonCollider2D polygonCollider2D = GameObject.FindGameObjectWithTag(Tags.BoundsConfiner).GetComponent<PolygonCollider2D>();

        CinemachineConfiner cinemachineConfiner = GetComponent<CinemachineConfiner>();

        cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;

        // since teh confiner bounds have changed need to call this to clear the cache.
        cinemachineConfiner.InvalidatePathCache();
    }




}
