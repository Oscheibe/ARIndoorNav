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
    public ARVisuals_ArrowLine _DefaultARVisuals;
    public ARVisuals_BendingWords _BendingWords;
    public ARVisuals_HapticFeedback _HapticFeedback;
    public ARVisuals_Avatar _Avatar;

    public TargetDestinationUI _TargetDestinationUI;
    public UserMessageUI _UserMessageUI;
    public UIMenuSwitchingManager _UIMenuSwitchingManager;

    private List<IARVisuals> _ActiveARVisuals = new List<IARVisuals>();


    void Start()
    {
        AddARVisuals((IARVisuals) _DefaultARVisuals);
        //AddARVisuals((IARVisuals) _BendingWords);
        //AddARVisuals((IARVisuals) _HapticFeedback);
        AddARVisuals((IARVisuals) _Avatar);
    }

    /**
     * Method to change the currently active AR visual display
     * I'd rather just change the public variable, but unity doesn't allow for serialized interfaces
     * (And I don't know enough about them to understand why you shouldn't have them)
     */
    public void AddARVisuals(IARVisuals newARVisuals)
    {
        _ActiveARVisuals.Add(newARVisuals);
    }

    public void UpdateDestination(Room destination)
    {
        _Navigation.UpdateDestination(destination);
    }

    public void DisplayNavigationInformation(NavigationInformation navigationInformation)
    {
        _TargetDestinationUI.DisplayTargetInformation(navigationInformation.GetDestinationName(), navigationInformation.GetTotalDistance());
        UpdateAllVisuals(navigationInformation);
    }

    public void UpdateNavigationInformation(NavigationInformation navigationInformation)
    {
        _TargetDestinationUI.UpdateDistance(navigationInformation.GetTotalDistance());
        UpdateAllVisuals(navigationInformation);
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
        _TargetDestinationUI.ResetTargetInformation();
        ClearPathDisplay();
    }

    /**
     * Only clears current AR UI elements, without reseting any values
     */
    public void ClearPathDisplay()
    {
        foreach (var visual in _ActiveARVisuals)
        {
            visual.ClearARDisplay();
        }
        //_ActiveARVisuals.ClearARDisplay();
        //_BendingWords.ClearARDisplay();
    }

    /**
     * Method to continue a paused navigation
     */
    public void ContinueNavigation()
    {
        _Navigation.ContinueNavigation();
    }

    /**
     * Function to clean up visuals updates
     */
    private void UpdateAllVisuals(NavigationInformation navigationInformation)
    {
        foreach (var visual in _ActiveARVisuals)
        {
            //Debug.Log("Updating: " + visual);
            visual.SendNavigationInformation(navigationInformation);
        }
        //_ActiveARVisuals.SendNavigationInformation(navigationInformation);
        //_BendingWords.SendNavigationInformation(navigationInformation);
    }


}
