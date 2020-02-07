using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class PoseEstimation : MonoBehaviour
{
    public Transform _ARCoreOriginTransform;
    public Transform _ARCoreFPSTransform;

    public MarkerDetection _MarkerDetection;
    public NavigationPresenter _NavigationPresenter;
    public SystemStatePresenter _SystemStatePresenter;
    public Navigation _Navigation;
    public TrackingErrorHandling _TrackingErrorHandling; 

    private float rotationDegree = 0.5f;
    private int currentFloor = -1;

    public enum NewPosReason
    {
        Manual = 0,
        TooMuchError = 1,
        EnteredStairs = 2,
        EnteredElevator = 3,
    }

    void Update() 
    {
        // Reporting the current position for error evaluation
        _TrackingErrorHandling.ReportCurrentUserPosition(_ARCoreFPSTransform.position);
    }


    /**
        Method to gather positional data to combine it and calculate the most likely user position
        ARCore is constantly updating the position in the background 
        This method is used to counteract drift by scanning marker
     */
    public void ReportMarkerPose(Transform virtualMarkerTransform, Pose worldMarkerPose)
    {
        var arPosBefore = _ARCoreFPSTransform.position;
        UpdateUserRotation(virtualMarkerTransform, worldMarkerPose);
        var arPosAfter = _ARCoreFPSTransform.position;

        CorrectRotationOffset(arPosBefore, arPosAfter);
        UpdateUserPosition(virtualMarkerTransform.position, worldMarkerPose.position);

        _Navigation.WarpNavMeshAgent(_ARCoreFPSTransform.position);
        _SystemStatePresenter.HideMarkerDetectionMenu();
    }

    /**
     * changes the current floor based on marker detection results
     */
    public void ReportCurrentFloor(int currentFloor)
    {
        this.currentFloor = currentFloor;
    }

    private void UpdateUserPosition(Vector3 virtualMarkerPosition, Vector3 worldMarkerPosition)
    {
        var originPosition = _ARCoreOriginTransform.position;
        var fpsPos = _ARCoreFPSTransform.position;

        Vector3 targetPosDelta;
        targetPosDelta = virtualMarkerPosition - worldMarkerPosition;
        _ARCoreOriginTransform.position += targetPosDelta;
    }

    private void UpdateUserRotation(Transform virtualMarkerTransform, Pose worldMarkerPose)
    {
        var originRotation = _ARCoreOriginTransform.rotation;
        var worldMarkerRotation = worldMarkerPose.rotation;
        var virtualMarkerRotation = virtualMarkerTransform.rotation;
        Quaternion targetRotDelta;

        targetRotDelta = virtualMarkerRotation * Quaternion.Inverse(worldMarkerRotation);
        _ARCoreOriginTransform.rotation *= targetRotDelta;
    }

    private void CorrectRotationOffset(Vector3 arPosBefore, Vector3 arPosAfter)
    {
        var posOffset = arPosAfter - arPosBefore;
        _ARCoreOriginTransform.position -= posOffset;

        Debug.Log("Position Offset: " + posOffset);
        _SystemStatePresenter.DisplayUserMessage("Position Offset: " + posOffset);
    }

    /**
        This method is called when the accumulated tracking error exceed a limit OR
        When it is called manually
        Tracking will be stopped and a user prompt initiated that asks the user to scan a marker
     */
    public void RequestNewPosition(NewPosReason reason)
    {
        var destinationFloor = _Navigation.GetDestinationFloor();
        
        switch (reason)
        {
            case NewPosReason.Manual:
                _MarkerDetection.StartDetection();
                break;
            case NewPosReason.TooMuchError:
                //TODO
                break;
            case NewPosReason.EnteredStairs:
                _NavigationPresenter.SendTakeStairsMessage(currentFloor, destinationFloor);
                break;
            case NewPosReason.EnteredElevator:
                _NavigationPresenter.SendTakeElevatorMessage(currentFloor, destinationFloor);
                break;
            default:
                break;
        }
    }

    /**
     * The absolute position of the user relative to the unity origin is
     * the combined position of the ARCore origin and the relative movement of the user
     * 
     * Returns the Unity world coordinates of the user
     */
    public Vector3 GetUserPosition()
    {
        return _ARCoreFPSTransform.position;
    }

    public Quaternion GetUserRotation()
    {
        return _ARCoreFPSTransform.rotation;
    }

    /**
     * Method to manually adjust the rotation of the user position
     * Rotates the user clockwise
     */
    public void RotateClockwise()
    {
        var arPosBefore = _ARCoreOriginTransform.position;
        _ARCoreOriginTransform.transform.rotation *= Quaternion.Euler(0, rotationDegree, 0);
        var arPosAfter = _ARCoreOriginTransform.position;

        CorrectRotationOffset(arPosBefore, arPosAfter);
    }

    /**
     * Method to manually adjust the rotation of the user position
     * Rotates the user clockwise
     */
    public void RotateCounterClockwise()
    {
        var arPosBefore = _ARCoreOriginTransform.position;
        _ARCoreOriginTransform.transform.rotation *= Quaternion.Euler(0, -rotationDegree, 0);
        var arPosAfter = _ARCoreOriginTransform.position;

        CorrectRotationOffset(arPosBefore, arPosAfter);
    }

    /**
     * method to reset the state of ARCore
     * used when the user has to relocate themselves because of an error or when they change floor
     */
    private void ResetARCore()
    {
        //TODO
        
    }

}



