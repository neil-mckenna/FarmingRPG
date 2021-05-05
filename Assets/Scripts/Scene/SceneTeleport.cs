using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SceneTeleport : MonoBehaviour
{
    [SerializeField] private SceneName sceneNameGoTo = SceneName.Scene1_Farm;
    [SerializeField] private Vector3 scenePositionGoTo = new Vector3();

    private void OnTriggerEnter2D(Collider2D collision)      
    {
        Player player = collision.GetComponent<Player>();

        if(player != null)
        {
            // Calulate player new position

            float xPosition = Mathf.Approximately(scenePositionGoTo.x, 0f) ? player.transform.position.x : scenePositionGoTo.x;
            float yPosition = Mathf.Approximately(scenePositionGoTo.y, 0f) ? player.transform.position.y : scenePositionGoTo.y;
            float zPosition = 0f;

            // Teleport to new scene
            SceneControllerManager.Instance.FadeAndLoadScene(sceneNameGoTo.ToString(), new Vector3(xPosition, yPosition, zPosition));

        }
        
    }

    


    
}
