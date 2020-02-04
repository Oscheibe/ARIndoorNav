using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UserMessageUI : MonoBehaviour
{
    public SystemInformationHeaderController _SystemInformationHeaderController;

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

    public void ShowStairsText(string currentFloor, string destinationFloor)
    {
        _takeElevatorText.SetActive(false);
        _walkStairsText.SetActive(true);
        _walkStairsText.GetComponent<TMP_Text>().text = "Please take the stairs to floor: " + destinationFloor;
    }


    public void ShowElevatorText(string currentFloor, string destinationFloor)
    {
        _walkStairsText.SetActive(false);
        _takeElevatorText.SetActive(true);
        _takeElevatorText.GetComponent<TMP_Text>().text = "Please take the elevator to floor: " + destinationFloor;
    }






}
