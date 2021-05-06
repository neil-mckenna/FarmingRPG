using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperMethods
{
    ///<summary>
    /// Gets components of type T at box with center point and size and angle. returns true if at least one found and the found compots are retur
    /// returned in the list
    ///</summary>

    public static bool GetComponentsAtBoxLocation<T>(out List<T> listComponentsAtBoxPosition, Vector2 point, Vector2 size, float angle)
    {
        bool found = false;
        List<T> componentList = new List<T>();

        Collider2D[] collider2DArray = Physics2D.OverlapBoxAll(point, size, angle);

        // Loop through all colliders to get an object of type T
        for(int i = 0; i < collider2DArray.Length; i++)
        {
            T tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();
            if(tComponent != null)
            {
                found = true;
                componentList.Add(tComponent);
            }
            else
            {
                tComponent = collider2DArray[i].gameObject.GetComponentInChildren<T>();
                if(tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                }
            }
        }

        listComponentsAtBoxPosition = componentList;

        return found;
    }


}
