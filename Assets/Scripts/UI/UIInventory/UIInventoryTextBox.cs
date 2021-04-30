using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIInventoryTextBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textmeshTop1 = null;
    [SerializeField] private TextMeshProUGUI textmeshTop2 = null;
    [SerializeField] private TextMeshProUGUI textmeshTop3 = null;
    [SerializeField] private TextMeshProUGUI textmeshBottom1 = null;
    [SerializeField] private TextMeshProUGUI textmeshBottom2 = null;
    [SerializeField] private TextMeshProUGUI textmeshBottom3 = null;

    public void SetTextboxText(string textTop1, string textTop2, string textTop3, string textBot1, string textBot2, string textBot3)
    {
        textmeshTop1.text = textTop1;
        textmeshTop2.text = textTop2;
        textmeshTop3.text = textTop3;
        textmeshBottom1.text = textBot1;
        textmeshBottom2.text = textBot2;
        textmeshBottom3.text = textBot3;




    }




    
}
