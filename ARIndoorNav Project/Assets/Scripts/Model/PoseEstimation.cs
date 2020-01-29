using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseEstimation : MonoBehaviour
{
    public Transform _ARCoreOriginTransform;
    public Transform _ARCoreFPSTransform;

    public MarkerDetection _MarkerDetection;
    public NavigationPresenter _NavigationPresenter;
    public SystemStatePresenter _SystemStatePresenter;
    public Navigation _Navigation;

    /**
        Method to gather positional data to combine it and calculate the most likely user position
        ARCore is constantly updating the position in the background 
        This method is used to counteract drift by scanning marker
     */
    public void ReportMarkerPose(Transform virtualMarkerTransform, Pose worldMarkerPose)
    {
        var arPosBefore = _ARCoreFPSTransform.position;
        UpdateLastMarkerRotation(virtualMarkerTransform, worldMarkerPose);
        var arPosAfter = _ARCoreFPSTransform.position;

        CorrectRotationOffset(arPosBefore, arPosAfter);
        UpdateLastMarkerPosition(virtualMarkerTransform.position, worldMarkerPose.position);

        _Navigation.WarpNavMeshAgent(_ARCoreFPSTransform.position);
        _SystemStatePresenter.HideMarkerDetectionMenu();
    }

    private void UpdateLastMarkerPosition(Vector3 virtualMarkerPosition, Vector3 worldMarkerPosition)
    {
        var originPosition = _ARCoreOriginTransform.position;
        var fpsPos = _ARCoreFPSTransform.position;

        Vector3 targetPosDelta;
        targetPosDelta = virtualMarkerPosition - worldMarkerPosition;
        _ARCoreOriginTransform.position += targetPosDelta;
    }

    private void UpdateLastMarkerRotation(Transform virtualMarkerTransform, Pose worldMarkerPose)
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
    public void RequestNewPosition()
    {
        _MarkerDetection.StartDetection();
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
     * The rotation of the origin position of ARCore
     * It needs to be added to every result that ARCore has when calculating rotation
     */
    public Quaternion GetARCoreRotationOffset()
    {
        return Quaternion.Inverse(_ARCoreOriginTransform.rotation) * _ARCoreFPSTransform.rotation;
    }

}



