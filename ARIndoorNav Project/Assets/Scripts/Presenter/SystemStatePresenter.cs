using System;
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
    //Model
    public PoseEstimation _PoseEstimation;

    // View
    public UIMenuSwitchingManager _UIMenuSwitchingManager;
    public MarkerDetectionUI _MarkerDetectionUI;
    public UserMessageUI _UserMessageUI;

    // Other presenter? TODO
    public NavigationPresenter _NavigationPresenter;
    
    public void SendWarningMessage(string warning)
    {
        //TODO: More aggressive message
    }

    public void DisplayUserMessage(string message)
    {
        _UserMessageUI.SendUserMessage(message);
    }

    public void TestMessage()
    {
        Debug.Log("Testing Message");
        _UserMessageUI.TestUserMessage();
    }

    public void DisplayCurrentPosition(string roomName)
    {
        _NavigationPresenter.UpdateLastMarker(roomName);
    }

    /**
     * Called from MarkerDetection when the marker detection finished successfully
     */
    public void HideMarkerDetectionMenu()
    {
        _UIMenuSwitchingManager.OpenNavigationMenu();
    }

    /**
     * Called from MarkerDetection when the OCR service is processing the camera image
     */
    public void ShowLoadingAnimation()
    {
        _MarkerDetectionUI.ShowLoadingAnimation();
    }

    /**
     * Method that starts markter detection when the user confirms the process
     */
    public void ConfirmMarkerTracking()
    {
        _PoseEstimation.RequestNewPosition(PoseEstimation.NewPosReason.Manual);
    }
}
