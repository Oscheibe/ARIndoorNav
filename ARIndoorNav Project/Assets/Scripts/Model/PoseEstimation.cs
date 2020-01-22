using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseEstimation : MonoBehaviour
{
    public Transform _ARCoreOriginTransform;
    public Transform _ARCoreFPSTransform;
    public MarkerDetection _MarkerDetection;
    public NavigationPresenter _NavigationPresenter;

    public Transform _TestWorldMarker;
    public Transform _TestVirtualMarker;

    private string lastWorldMarkerPos, lastVirtualMarkerPos, lastWorldMarkerRot, lastVirtualMarkerRot;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //TestUpdateLastMarkerPosition();

        // Check if 
        _NavigationPresenter.DisplayPositionInformation(
            "pos: " + lastWorldMarkerPos + " rot: " + lastWorldMarkerRot,
            "pos: " + lastVirtualMarkerPos + " rot: " + lastVirtualMarkerRot,
            "pos: " + _ARCoreOriginTransform.position.ToString() + " rot: " + _ARCoreOriginTransform.rotation.eulerAngles.ToString(),
            "pos: " + _ARCoreFPSTransform.position.ToString() + " rot: " + _ARCoreFPSTransform.rotation.eulerAngles.ToString());
    }

    /**
        Method to gather positional data to combine it and calculate the most likely user position
        ARCore is constantly updating the position in the background 
        This method is used to counteract drift by scanning marker
     */
    public void ReportMarkerPose(Transform virtualMarkerTransform, Pose worldMarkerPose)
    {
        lastWorldMarkerPos = worldMarkerPose.position.ToString();
        lastVirtualMarkerPos = virtualMarkerTransform.position.ToString();
        lastWorldMarkerRot = worldMarkerPose.rotation.eulerAngles.ToString();
        lastVirtualMarkerRot = virtualMarkerTransform.rotation.eulerAngles.ToString();

        _MarkerDetection.StopDetection();

        var arPosBeforeRotation = _ARCoreFPSTransform.position;
        UpdateLastMarkerRotation(virtualMarkerTransform, worldMarkerPose);
        var arPosRotationOffset = _ARCoreFPSTransform.position - arPosBeforeRotation;
        Debug.Log("Position Offset: " + arPosRotationOffset);

        UpdateLastMarkerPosition(virtualMarkerTransform.position, worldMarkerPose.position, arPosRotationOffset);
    }

    private void UpdateLastMarkerPosition(Vector3 virtualMarkerPosition, Vector3 worldMarkerPosition, Vector3 arPosRotationOffset)
    {
        var originPosition = _ARCoreOriginTransform.position;
        var fpsPos = _ARCoreFPSTransform.position;

        Vector3 targetPosDelta;
        targetPosDelta = virtualMarkerPosition - worldMarkerPosition + arPosRotationOffset;
        _ARCoreOriginTransform.position += targetPosDelta;

        Debug.Log(("Virtual POS: " + virtualMarkerPosition));
        Debug.Log("World POS: " + worldMarkerPosition);

        Debug.Log("Origin BEFORE POS: " + originPosition);
        Debug.Log("POS Delta: " + targetPosDelta);
        Debug.Log("Origin AFTER POS: " + _ARCoreOriginTransform.position);
    }

    private void UpdateLastMarkerRotation(Transform virtualMarkerTransform, Pose worldMarkerPose)
    {
        var originRotation = _ARCoreOriginTransform.rotation;
        var worldMarkerRotation = worldMarkerPose.rotation;
        var virtualMarkerRotation = virtualMarkerTransform.rotation;
        Quaternion targetRotDelta;

        targetRotDelta = virtualMarkerRotation  * Quaternion.Inverse(worldMarkerRotation);
        _ARCoreOriginTransform.rotation *= targetRotDelta;

        Debug.Log("Virtual ROT: " + virtualMarkerRotation.eulerAngles);
        Debug.Log("World ROT: " + worldMarkerPose.rotation.eulerAngles);

        Debug.Log("Origin BEFORE ROT: " + originRotation.eulerAngles);
        Debug.Log("ROT Delta: " + targetRotDelta.eulerAngles);
        Debug.Log("Origin AFTER ROT: " + _ARCoreOriginTransform.rotation.eulerAngles);
    }



    public void ReportMarkerRotation(Transform virtualMarkerPosition, Pose worldMarkerPose)
    {
        _MarkerDetection.StopDetection();
        //UpdateLastMarkerRotation(virtualMarkerPosition, worldMarkerPose);
    }

    /**
        This method is called when the accumulated tracking error exceed a limit
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

    private void UpdateLastMarkerRotationEuler(Transform virtualMarkerTransform, Pose worldMarkerPose)
    {
        var originRotation = _ARCoreOriginTransform.rotation;
        var worldMarkerRotation = worldMarkerPose.rotation;
        var virtualMarkerRotation = virtualMarkerTransform.rotation;


        var targetRotX = 0f; // virtualMarkerRotation.eulerAngles.x - _ARCoreOriginTransform.rotation.eulerAngles.x - worldMarkerRotation.eulerAngles.x + _ARCoreOriginTransform.rotation.eulerAngles.x;
        var targetRotY = 0f; //virtualMarkerRotation.eulerAngles.y - originRotation.eulerAngles.y - worldMarkerRotation.eulerAngles.y + originRotation.eulerAngles.y;
        targetRotY = originRotation.eulerAngles.y - virtualMarkerRotation.eulerAngles.y - worldMarkerRotation.eulerAngles.y + 180;

        var targetRotZ = 0f;// virtualMarkerRotation.eulerAngles.z - _ARCoreOriginTransform.rotation.eulerAngles.z - worldMarkerRotation.eulerAngles.z + _ARCoreOriginTransform.rotation.z;

        if (targetRotX < 0) targetRotX += 360;
        if (targetRotX >= 360) targetRotX -= 360;
        if (targetRotY < 0) targetRotX += 360;
        if (targetRotY >= 360) targetRotX -= 360;
        if (targetRotZ < 0) targetRotX += 360;
        if (targetRotZ >= 360) targetRotX -= 360;

        Quaternion targetRotDelta;
        targetRotDelta = Quaternion.Euler(targetRotX, targetRotY, targetRotZ);
        _ARCoreOriginTransform.rotation = targetRotDelta;
    }


}



