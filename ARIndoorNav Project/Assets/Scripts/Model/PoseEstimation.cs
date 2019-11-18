using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseEstimation : MonoBehaviour
{
    public Transform _LastMarkerTransform;
    public MarkerDetection _MarkerDetection;

    public Transform _TestWorldMarker;
    public Transform _TestVirtualMarker;
    public bool _CalculateRotation = true;
    public bool _CalculatePosition = true;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        TestUpdateLastMarkerPosition();
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
        var virtualMarkerPosition = virtualMarkerTransform.position;
        var virtualMarkerRotation = virtualMarkerTransform.rotation;

        var worldMarkerPosition = worldMarkerPose.position - _LastMarkerTransform.position;
        var worldMarkerRotation = worldMarkerPose.rotation * Quaternion.Inverse(_LastMarkerTransform.rotation);

        Vector3 targetPosition;
        Quaternion targetRotation;

        targetPosition = virtualMarkerPosition - _LastMarkerTransform.position - worldMarkerPosition; 
        _LastMarkerTransform.position += targetPosition;

        targetRotation = _LastMarkerTransform.rotation * (Quaternion.Inverse(virtualMarkerRotation) * worldMarkerRotation);
        _LastMarkerTransform.rotation = targetRotation;

    }


    private void TestUpdateLastMarkerPosition()
    {
        var virtualMarkerPosition = _TestVirtualMarker.position;
        var virtualMarkerRotation = _TestVirtualMarker.rotation;

        var worldMarkerPosition = _TestWorldMarker.position - _LastMarkerTransform.position;
        var worldMarkerRotation = _TestWorldMarker.rotation * Quaternion.Inverse(_LastMarkerTransform.rotation);

        Debug.Log(string.Format("\nVirtual / World / Last\nPosition:{0} / {1} / {2}\nRotation:{3} / {4} / {5}",
                                virtualMarkerPosition, worldMarkerPosition, _LastMarkerTransform.position,
                                virtualMarkerRotation, worldMarkerRotation, _LastMarkerTransform.rotation));


        Quaternion targetRotation;
        Vector3 targetPosition;


        if (_CalculatePosition)
        {
            // Calculate the new position of the _arScene center, based on relative distance between unity and ARCore plates
            // Calculation needs to be done after changing the rotation.
            //var targetPosition = (worldMarkerPose.position - virtualMarkerPosition.position) + _LastMarkerPosition.transform.position;
            targetPosition = virtualMarkerPosition - _LastMarkerTransform.position - worldMarkerPosition; // - _LastMarkerTransform.position

            _LastMarkerTransform.position += targetPosition;
            Debug.Log("Adjusted Position: " + targetPosition);
        }
        if (_CalculateRotation)
        {
            // Rotate first because the rotation changes the position of the object.
            // Rotate over the X axis by 90° to account for AugmentedImage detection angles (Might change later, hopefully not)
            targetRotation = _LastMarkerTransform.rotation * (Quaternion.Inverse(virtualMarkerRotation) *
                worldMarkerRotation);
            _LastMarkerTransform.rotation = targetRotation;
            Debug.Log("Adjusted Rotation: " + targetRotation);
        }



    }


}
