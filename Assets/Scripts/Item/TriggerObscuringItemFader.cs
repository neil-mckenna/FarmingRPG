using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObscuringItemFader : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        // get the gameobject we have collided with , and then get all teh obscuring componets on it children and then trigger them

        ObscuringItemFader[] obscuringItemFaders = collision.GetComponentsInChildren<ObscuringItemFader>();    

        if(obscuringItemFaders.Length > 0)
        {
            for(int i = 0; i < obscuringItemFaders.Length; i++)
            {
                obscuringItemFaders[i].FadeOut();
            } 
        }    
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
        ObscuringItemFader[] obscuringItemFaders = collision.GetComponentsInChildren<ObscuringItemFader>();    

        if(obscuringItemFaders.Length > 0)
        {
            for(int i = 0; i < obscuringItemFaders.Length; i++)
            {
                obscuringItemFaders[i].FadeIn();
            } 
        } 
        
    }
}
