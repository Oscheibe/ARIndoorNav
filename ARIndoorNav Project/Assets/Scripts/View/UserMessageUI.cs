using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UserMessageUI : MonoBehaviour
{
    public SystemInformationHeaderController _SystemInformationHeaderController;
       
    public void SendUserMessage(string message)
    {
        _SystemInformationHeaderController.DisplaySystemInformation(message);
    }

    public void TestUserMessage()
    {
        _SystemInformationHeaderController.DisplaySystemInformation("Test Message System");
    }
}
