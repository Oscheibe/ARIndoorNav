using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonListButton : MonoBehaviour
{
    public Text myText;
    public Text destinationText;
   
    public void SetText(string textString)
    {
        myText.text = textString;
    }

    public void OnClick()
    {
        destinationText.text = myText.text;
    }
}
