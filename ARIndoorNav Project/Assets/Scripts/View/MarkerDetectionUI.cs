using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerDetectionUI : MonoBehaviour
{
    public SystemStatePresenter _SystemStatePresenter;
    public TextDetection _TextDetection;
    public MarkerDetection _MarkerDetection;

    public GameObject _loadingAnimationGO;
    public float _loadingTime = 1.0f;

    public string DestinationRoomString { get; private set; }

    public void ShowLoadingAnimation()
    {
        _loadingAnimationGO.SetActive(true);
        Invoke("HideLoadingAnimation", _loadingTime);
    }

    private void HideLoadingAnimation()
    {
        _loadingAnimationGO.SetActive(false);
    }

    /**
     * Called by a button
     * Method to start the marker detection process
     */

    public void StartMarkerDetection()
    {
        Debug.Log($"Pressed Marker detection button: {DestinationRoomString}");
        if (!string.IsNullOrEmpty(DestinationRoomString))
        {
            _MarkerDetection.SaveUserPosition();
            _TextDetection.ReceiveTextList(new List<string> { DestinationRoomString });
        }
        //_SystemStatePresenter.ConfirmMarkerTracking();
    }

    public void SetDestinationRoomString(string input)
    {
        DestinationRoomString = input;
    }
}