using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ItemCodeDescriptionAttribute))]
public class ItemCodeDescriptionDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // change the returned property height to be double the cater for the addional item code description that we will draw
        return EditorGUI.GetPropertyHeight(property) * 2;

    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
    {
        // Using BeginProperty/ EndProperty on teh parent property means that the prefab ovveride logic works on the entire property
        
        EditorGUI.BeginProperty(position, label, property);

        if(property.propertyType == SerializedPropertyType.Integer)
        {
            EditorGUI.BeginChangeCheck(); // start the check for change values

            // Draw item code
            var newValue = EditorGUI.IntField(new Rect(position.x, position.y, position.width, position.height/2),label, property.intValue);

            // Draw Item Description
            EditorGUI.LabelField(new Rect(position.x, position.y + position.height/2, position.width, position.height/2),"Item Description", GetItemDescription(property.intValue));

            // if item code valua has change then set value to new value
            if(EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }

        }

        EditorGUI.EndProperty();

    }

    private string GetItemDescription(int itemCode)
    {
        SO_ItemList sO_ItemList;

        sO_ItemList = AssetDatabase.LoadAssetAtPath("Assets/Scriptable Objects Assets/Item/so_ItemList.asset", typeof(SO_ItemList)) as SO_ItemList;

        List<ItemDetails> itemDetailsList = sO_ItemList.itemDetails;

        ItemDetails itemDetail = itemDetailsList.Find(x => x.itemCode == itemCode);

        if(itemDetail != null)
        {
            return itemDetail.itemDescription;
        }
        else
        {
            return "";

        }    



    }

    
}
