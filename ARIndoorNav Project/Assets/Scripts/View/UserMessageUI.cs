using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UserMessageUI : MonoBehaviour
{
    public SystemInformationHeaderController _SystemInformationHeaderController;

    public GameObject _destinationReachedText;

    private float displayTime = 2.0f;

    public void SendUserMessage(string message)
    {
        _SystemInformationHeaderController.DisplaySystemInformation(message);
    }

    public void TestUserMessage()
    {
        _SystemInformationHeaderController.DisplaySystemInformation("Test Message System");
    }

    public void ShowDestinationReachedText()
    {
        _destinationReachedText.SetActive(true);
        Invoke("HideDestinationReachedText", displayTime);
    }
    
    private void HideDestinationReachedText()
    {
        _destinationReachedText.SetActive(false);
    }
}
