using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseEstimation : MonoBehaviour
{
    public Transform _LastMarkerTransform;
    public MarkerDetection _MarkerDetection;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /**
        Method to gather positional data to combine it and calculate the most likely user position
        ARCore is constantly updating the position in the background 
        This method is used to counteract drift by scanning marker
     */
    public void ReportMarkerPosition(Transform virtualMarkerPosition, Pose worldMarkerPose)
    {
        _MarkerDetection.StopDetection();
        UpdateLastMarkerPosition(virtualMarkerPosition, worldMarkerPose);
    }

    /**
        This method is called when the accumulated tracking error exceed a limit
        Tracking will be stopped and a user prompt initiated that asks the user to scan a marker
        
     */
    public void RequestNewPosition()
    {
        _MarkerDetection.StartDetection();
        //TODO: stop tracking and start scanning
    }

    private void UpdateLastMarkerPosition(Transform virtualMarkerTransform, Pose worldMarkerPose)
    {
        // Rotate first because the rotation changes the position of the object.
        // Rotate over the X axis by 90° to account for AugmentedImage detection angles (Might change later, hopefully not)
        var targetRotation = _LastMarkerTransform.rotation * (Quaternion.Inverse(virtualMarkerTransform.rotation) * worldMarkerPose.rotation);
        var targetRotation2 = _LastMarkerTransform.rotation * (Quaternion.Inverse(worldMarkerPose.rotation ) * virtualMarkerTransform.rotation);
        

        //targetRotation = targetRotation * Quaternion.Euler(90, 0, 0);
        if (_LastMarkerTransform.rotation != targetRotation) // Math.Abs(targetPlate.transform.rotation.eulerAngles.y - image.CenterPose.rotation.eulerAngles.y) >= 0.1
        {
            //_LastMarkerTransform.rotation = targetRotation2;
        }

        // Calculate the new position of the _arScene center, based on relative distance between unity and ARCore plates
        // Calculation needs to be done after changing the rotation.
        Debug.Log("LastRotation: " + _LastMarkerTransform.rotation + " // VirtualMarker: " + virtualMarkerTransform.rotation + " // WorldMarker: " + worldMarkerPose.rotation);
        //var targetPosition = (worldMarkerPose.position - virtualMarkerPosition.position) + _LastMarkerPosition.transform.position;
        var targetPosition =  - worldMarkerPose.position + (virtualMarkerTransform.position); // - _LastMarkerTransform.position

        _LastMarkerTransform.position = targetPosition;


        Debug.Log("TargetRotation: " + targetRotation + " // Rotation2: " + targetRotation2);
    }

}
