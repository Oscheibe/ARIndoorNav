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
    public Navigation _Navigation;
    public UserMessageUI _UserMessageUI;
    public UIMenuSwitchingManager _UIMenuSwitchingManager;

    private string _currentDestination;

    public void DisplayNavigationInformation(string destinationName, float destinationDistance, Vector3[] path)
    {
        _currentDestination = destinationName;
        _TargetDestinationUI.DisplayTargetInformation(_currentDestination, destinationDistance);
        _ARVisuals.SendNavigationPath(path);
    }

    public void UpdateNavigationInformation(float destinationDistance, Vector3[] path)
    {
        _TargetDestinationUI.UpdateDistance(destinationDistance);
        _ARVisuals.SendNavigationPath(path);
    }

    public void UpdateLastMarker(string markerName)
    {
        _TargetDestinationUI.UpdateLastMarker(markerName);
    }

    /**
     * Called when the user is within 1 meter of the destination
     * Displays a message to the user
     */
    public void ReachedDestination()
    {
        _UserMessageUI.ShowDestinationReachedText();
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
        _PoseEstimation.RequestNewPosition(PoseEstimation.NewPosReason.Manual);
    }

    public Vector3 GetNextPathPosition()
    {
        return _Navigation.GetNextCorner();
    }

    /**
     * Method to adjust the user rotation manually
     */
    public void RotatePathToRight()
    {
        _PoseEstimation.RotateCounterClockwise();
    }

    /**
     * Method to adjust the user rotation manually
     */
    public void RotatePathToLeft()
    {
        _PoseEstimation.RotateClockwise();
    }

    /**
     * Called when the users enters a "Stairs" area.
     * The user must walk the stairs and then re-locate
     */
    public void SendTakeStairsMessage(int currentFloor, int destinationFloor)
    {
        _UIMenuSwitchingManager.OpenConfirmScreen();
        _UserMessageUI.ShowStairsText(currentFloor.ToString(), destinationFloor.ToString());
    }

    /**
     * Called when the users enters a "Elevator" area.
     * The user must walk the elevator and then re-locate
     */
    public void SendTakeElevatorMessage(int currentFloor, int destinationFloor)
    {
        _UIMenuSwitchingManager.OpenConfirmScreen();
        _UserMessageUI.ShowElevatorText(currentFloor.ToString(), destinationFloor.ToString());
    }
    /**
     * Resets the destination information and clears AR elements AR elements
     */
    public void ResetPathDisplay()
    {
        _ARVisuals.StopARDisplay();
        _TargetDestinationUI.ResetTargetInformation();
    }

    /**
     * Only clears current AR UI elements, without reseting any values
     */
    public void ClearPathDisplay()
    {
        _ARVisuals.StopARDisplay();
    }

    /**
     * Method to continue a paused navigation
     */
    public void ContinueNavigation()
    {
        _Navigation.ContinueNavigation();
    }
}
