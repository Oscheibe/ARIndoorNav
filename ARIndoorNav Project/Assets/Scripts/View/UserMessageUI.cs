using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UserMessageUI : MonoBehaviour
{
    public SystemInformationHeaderController _SystemInformationHeaderController;
    public UIMenuSwitchingManager _UIMenuSwitchingManager;

    public GameObject _destinationReachedText;
    public GameObject _walkStairsText;
    public GameObject _takeElevatorText;

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

    private void ShowStairsText(string floorDifference, string relativeMovement, string destinationFloor)
    {
        _takeElevatorText.SetActive(false);
        _walkStairsText.SetActive(true);
        // String example: "Please take the stairs down 2 floors to floor 1"
        _walkStairsText.GetComponent<TMP_Text>().text = "Please take the stairs " + relativeMovement + " " + floorDifference + " floors to floor " + destinationFloor;
    }


    private void ShowElevatorText(string floorDifference, string relativeMovement, string destinationFloor)
    {
        _walkStairsText.SetActive(false);
        _takeElevatorText.SetActive(true);
        // String example: "Please take the elevator down to floor 1";
        _takeElevatorText.GetComponent<TMP_Text>().text = "Please take the elevator " + relativeMovement + " to floor " + destinationFloor;
    }

    public void DisplayObstacleMessage(int currentFloor, int destinationFloor, PoseEstimation.NewPosReason obstacle)
    {
        _UIMenuSwitchingManager.OpenConfirmScreen();

        var floorDifference = (destinationFloor - currentFloor);
        string relativeMovement = "";
        if(floorDifference > 0)
            relativeMovement = "up";
        else 
            relativeMovement = "down";

        if(obstacle == PoseEstimation.NewPosReason.EnteredStairs)
        {
            ShowStairsText(Mathf.Abs(floorDifference).ToString(), relativeMovement, destinationFloor.ToString());
        }
        else
            ShowElevatorText(Mathf.Abs(floorDifference).ToString(), relativeMovement, destinationFloor.ToString());
    }




}
