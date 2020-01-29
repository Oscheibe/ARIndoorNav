using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SystemInformationHeaderController : MonoBehaviour
{
    public float _DisplayTime = 6.0f;
    public UIMenuSwitchingManager _UIMenuSwitchingManager;
    public TMP_Text systemInformationText;
    
    private bool isDisplaying = false;

    public void DisplaySystemInformation(string text)
    {
        if (isDisplaying)
        {
            systemInformationText.text = "- " + text + "\n" + systemInformationText.text;
        }
        else
        {
            systemInformationText.text = "- " + text;
            _UIMenuSwitchingManager.OpenSystemInformationHeader();
            isDisplaying = true;
            Invoke("CloseSysInfo", _DisplayTime);
        }

    }

    private void CloseSysInfo()
    {
        isDisplaying = false;
        _UIMenuSwitchingManager.CloseSystemInformationHeader();
    }
}
