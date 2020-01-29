using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
    Sends navigation information to the UI.
    Information include: AR Element position/ Room name, number, distance, etc.
 */
public class NavigationPresenter : MonoBehaviour
{
    public ARVisuals _ARVisuals;
    public TargetDestinationUI _TargetDestinationUI;
    public PoseEstimation _PoseEstimation;

    private string _currentDestination;
    
    public void DisplayNavigationInformation(string destinationName, float destinationDistance, Vector3[] path)
    {
        _currentDestination = destinationName;
        _TargetDestinationUI.DisplayTargetInformation(_currentDestination, destinationDistance.ToString());
        _ARVisuals.SendNavigationPath(path);
    }

    public void UpdateNavigationInformation(string destinationDistance, Vector3[] path)
    {
        _TargetDestinationUI.UpdateDistance(destinationDistance);
        _ARVisuals.SendNavigationPath(path);
    }

    public void UpdateLastMarker(string markerName)
    {
        _TargetDestinationUI.UpdateLastMarker(markerName);
    }

    /**
     * Method that starts the following steps:
     * 1 Save current Transform data of the user
     * 2 Send a snapshot of the camera image to an OCR service
     * 3 Display a loading animation while waiting for a response
     * 4a Receive response and calculate user position
     * 4b Handle response timeout (TODO)
     * (5 optional) Initiate manual rotation adjustment
     * 6 Send confirmation message to user
     */
    public void InitiateMarkerDetection()
    {
        _PoseEstimation.RequestNewPosition();
    }
}
