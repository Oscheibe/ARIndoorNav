using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
    Sends navigation information to the UI.
    Information include: AR Element position/ Room name, number, distance, etc.
 */
public class NavigationPresenter : MonoBehaviour
{
    // Model
    public PoseEstimation _PoseEstimation;
    public Navigation _Navigation;

    // View
    public ARVisuals _DefaultARVisuals;
    public TargetDestinationUI _TargetDestinationUI;
    public UserMessageUI _UserMessageUI;
    public UIMenuSwitchingManager _UIMenuSwitchingManager;

    public IARVisuals _ARVisuals;
    private string _currentDestination;


    void Start()
    {
    SetARVisuals(_DefaultARVisuals);
    }

    /**
     * Method to change the currently active AR visual display
     * I'd rather just change the public variable, but unity doesn't allow for serialized interfaces
     * (And I don't know enough about them to understand why you shouldn't have them)
     */
    public void SetARVisuals(IARVisuals newARVisuals)
    {
        _ARVisuals = newARVisuals;
    }

    public void UpdateDestination(Room destination)
    {
        _currentDestination = destination.Name;
        _Navigation.UpdateDestination(destination);
    }

    public void DisplayNavigationInformation(float destinationDistance, Vector3[] path)
    {
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
     * Called when the users enters either an "Elevator" or "Stairs" area.
     * The user must walk up or down the obstacle before resuming navigation!
     */
    public void SendObstacleMessage(int currentFloor, int destinationFloor, PoseEstimation.NewPosReason obstacle)
    {
        _UserMessageUI.DisplayObstacleMessage(currentFloor, destinationFloor, obstacle);
    }

    /**
     * Resets the destination information and clears AR elements AR elements
     */
    public void ResetPathDisplay()
    {
        _ARVisuals.ClearARDisplay();
        _TargetDestinationUI.ResetTargetInformation();
    }

    /**
     * Only clears current AR UI elements, without reseting any values
     */
    public void ClearPathDisplay()
    {
        _ARVisuals.ClearARDisplay();
    }

    /**
     * Method to continue a paused navigation
     */
    public void ContinueNavigation()
    {
        _Navigation.ContinueNavigation();
    }
}
