using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class PoseEstimation : MonoBehaviour
{
    public COMPoseTracking _ARPositionTracking;
    public MarkerDetection _MarkerDetection;
    public Navigation _Navigation;
    public TrackingErrorHandling _TrackingErrorHandling;
    public int currentFloor = 3;

    private float rotationDegree = 2.5f;


    public enum NewPosReason
    {
        Manual = 0,
        TooMuchError = 1,
        EnteredStairs = 2,
        EnteredElevator = 3,
    }

    void Start()
    {
        _TrackingErrorHandling.AnnouncePositionJump(_ARPositionTracking.GetUnityPosition());
    }

    void Update()
    {
        // Reporting the current position for error evaluation
        _TrackingErrorHandling.ReportCurrentUserPosition(_ARPositionTracking.GetUnityPosition());
    }


    /**
        Method to gather positional data to combine it and calculate the most likely user position
        ARCore is constantly updating the position in the background 
        This method is used to counteract drift by scanning marker
     */
    public void ReportMarkerPose(Transform virtualMarkerTransform, Pose worldMarkerPose)
    {
        var arPosBefore = _ARPositionTracking.GetUnityPosition();
        UpdateUserRotation(virtualMarkerTransform, worldMarkerPose);
        var arPosAfter = _ARPositionTracking.GetUnityPosition();

        CorrectRotationOffset(arPosBefore, arPosAfter);
        UpdateUserPosition(virtualMarkerTransform.position, worldMarkerPose.position);

        _Navigation.ReportUserPosJump(_ARPositionTracking.GetUnityPosition());
    }

    /**
     * changes the current floor based on marker detection results
     */
    public void ReportCurrentFloor(int currentFloor)
    {
        Debug.Log("Changed current floor to: " + currentFloor);
        this.currentFloor = currentFloor;
    }

    private void UpdateUserPosition(Vector3 virtualMarkerPosition, Vector3 worldMarkerPosition)
    {
        var originPosition = _ARPositionTracking.GetOriginPosition();
        var fpsPos = _ARPositionTracking.GetUnityPosition();

        Vector3 targetPosDelta;
        targetPosDelta = virtualMarkerPosition - worldMarkerPosition;

        var newPosition = _ARPositionTracking.GetOriginPosition() + targetPosDelta;
        _TrackingErrorHandling.AnnouncePositionJump(newPosition);

        _ARPositionTracking.SetOriginPosition(newPosition);
    }

    private void UpdateUserRotation(Transform virtualMarkerTransform, Pose worldMarkerPose)
    {
        var originRotation = _ARPositionTracking.GetOriginRotation();
        var worldMarkerRotation = worldMarkerPose.rotation;
        var virtualMarkerRotation = virtualMarkerTransform.rotation;
        Quaternion targetRotDelta;

        targetRotDelta = virtualMarkerRotation * Quaternion.Inverse(worldMarkerRotation);
        var newRotation = originRotation *= targetRotDelta;
        _ARPositionTracking.SetOriginRotation(newRotation);
    }

    private void CorrectRotationOffset(Vector3 arPosBefore, Vector3 arPosAfter)
    {
        var posOffset = arPosAfter - arPosBefore;
        var newPosition = _ARPositionTracking.GetOriginPosition() - posOffset;
        _TrackingErrorHandling.AnnouncePositionJump(newPosition);

        _ARPositionTracking.SetOriginPosition(newPosition);
    }

    /**
    * This method is called when the accumulated tracking error exceed a limit OR
    * When it is called manually
    * Tracking will be stopped and a user prompt initiated that asks the user to scan a marker
    * 1 Save current Transform data of the user
    * 2 Send a snapshot of the camera image to an OCR service
    * 3 Display a loading animation while waiting for a response
    * 4a Receive response and calculate user position
    * 4b Handle response timeout (TODO)
    * (5 optional) Initiate manual rotation adjustment
    * 6 Send confirmation message to user
    */
    public void RequestNewPosition(NewPosReason reason)
    {

        switch (reason)
        {
            case NewPosReason.Manual:
                _MarkerDetection.StartDetection();
                break;
            case NewPosReason.TooMuchError:
                //TODO
                break;
            case NewPosReason.EnteredStairs:
                // TODO
                break;
            case NewPosReason.EnteredElevator:
                // TODO
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
        return _ARPositionTracking.GetUnityPosition();
    }

    public Quaternion GetUserRotation()
    {
        return _ARPositionTracking.GetUnityRotation();
    }

    public int GetCurrentFloor()
    {
        return currentFloor;
    }

    /**
     * Method to manually adjust the rotation of the user position
     * Rotates the user clockwise
     */
    public void RotateClockwise()
    {
        _ARPositionTracking.RotateAroundOrigin(rotationDegree); // positive for clockwise rotation
    }

    /**
     * Method to manually adjust the rotation of the user position
     * Rotates the user clockwise
     */
    public void RotateCounterClockwise()
    {
        _ARPositionTracking.RotateAroundOrigin(-rotationDegree); // negative for counter-clockwise rotation
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



