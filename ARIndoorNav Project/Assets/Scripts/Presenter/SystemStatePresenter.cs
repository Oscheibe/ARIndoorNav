using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
    Script to relay high-iportance messages to the user. 
    Used for when the tracking state has degenerated too much and needs the user to react
 */
public class SystemStatePresenter : MonoBehaviour
{
    public PoseEstimation _PoseEstimation;
    public UserMessageUI _UserMessageUI;
    
    public void SendWarningMessage(string warning)
    {
        //TODO: More aggressive message
    }

    public void DisplayUserMessage(string message)
    {
        _UserMessageUI.SendUserMessage(message);
    }

    public void RequestMarkerTracking()
    {
        _PoseEstimation.RequestNewPosition();
    }

    public void TestMessage()
    {
        _UserMessageUI.TestUserMessage();
    }

    public void IndicateDetectedWall()
    {
        _PoseEstimation.RequestWallInformationUpdates();
    }
}
