using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GoogleARCore;
using UnityEngine.UI;

/**
    This script is used for the marker detection
    It uses AugmentedImages to scan for marker in the real world
    If a marker was found, it's relative position to it's virtual counterpart is sent to PoseEstimation
 */
public class MarkerDetection : MonoBehaviour
{
    public PoseEstimation _PoseEstimation;
    public MarkerDatabase _MarkerDatabase;
    public SystemStatePresenter _SystemStatePresenter;
    public TextDetection _TextDetection;
    public float _waitingTime = 10f;
    public bool _isTracking = false; // For Debugging purposes: To start manual marker detection
    public bool _isWaiting = false; // If the marker detection has been initialized and is being calculated
    public bool _isIndicatingWall = false;
    public float _scanningDistance = 0.3f; // The average distance of the user when scanning a sign

    private List<DetectedPlane> _detectedPlanes = new List<DetectedPlane>();
    private List<AugmentedImage> _detectedImages = new List<AugmentedImage>();
    private List<Pose> _WorldMarkerPositionList = new List<Pose>();
    private readonly int _MaxTrackingCount = 10;
    private CameraImageBytes image;
    private DetectedPlane detectedPlane = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Continuosly tracks marker after the StartTracking() method is called
    // Only stops when the StopTracking method is called and there is no pending result 
    // indicated by "_isWaiting"
    void Update()
    {
        if (_isTracking && _isWaiting == false)
        {
            DetectMarker();
        }
        if (_isIndicatingWall) DisplayDetectedWallInfo();
    }

    /**
        Method that manages the detection method used (Augmented Images or OCR)
        Is called once per frame when the _isTracking bool is true 
    */
    private void DetectMarker()
    {
        DetectMarkerOCR();
        //DetectMarkerAugmentedImages(); // -> Not in use because it didn't provide sufficient results
    }

    /**
        Starts the marker detection with ARCore AugmentedImages
        Is called by PoseEstimation
     */
    public void StartDetection()
    {
        Debug.Log("Started marker detection");
        _isTracking = true;
        _SystemStatePresenter.DisplayUserMessage("Is Tracking");
    }

    /**
        Stops the marker detection with ARCore AugmentedImages
        Is called by PoseEstimation
     */
    public void StopDetection()
    {
        Debug.Log("Stopped marker detection");
        _isTracking = false;
        _SystemStatePresenter.DisplayUserMessage("Tracking stopped");
    }

    /**
        A function for the TextDetection component.
        It is the dmz of the Google Cloud Vision API to detect text using 
        the ARCore CameraImageByte class 

        1. It announces that a result has been arrived
        2. It checks which value contains a number
    */
    public void SendMarkerList(List<string> potentialMarkerList)
    {
        AnnounceResult();
        StopDetection(); // Should be called by PoseEstimation only
        var resultRoomList = _MarkerDatabase.ContainsRoom(potentialMarkerList);
        foreach (var name in potentialMarkerList)
        {
            Debug.Log("API Result: " + name);
        }
        if(resultRoomList == null)
        {
            _SystemStatePresenter.DisplayUserMessage("No Results Found!");
        }

        if (resultRoomList != null && detectedPlane != null)
        {
            //TODO: more than 1 room found
            CalculateOCRPosition(resultRoomList[0], detectedPlane);
            _SystemStatePresenter.DisplayUserMessage("Setting position to room: " + resultRoomList[0].Name);
        }
        else
        {
            Debug.Log("ERROR CALCULATING ROOM POSITION: No room or no wall found");
        }
    }

    private void CalculateOCRPosition(Room room, DetectedPlane plane)
    {
        Transform virtualMarkerTransform = room.Location;
        Pose worldMarkerPose = new Pose();

        var worldMarkerRotation = _PoseEstimation.GetUserRotation() * new Quaternion(0,0,-1, 0);   //plane.CenterPose.rotation; // * _PoseEstimation.GetARCoreRotationOffset();
        //var userPosAddition = (worldMarkerRotation * new Vector3(1, 1, 1)) * _scanningDistance;
        var worldMarkerPosition = _PoseEstimation.GetUserRotation() * new Vector3(0, 0, -_scanningDistance);

        Debug.Log("Virtual Marker ROTATION:" + virtualMarkerTransform.rotation.eulerAngles);
        Debug.Log("WorldMarker ROTATION: " + worldMarkerRotation.eulerAngles);

        worldMarkerPose = new Pose(worldMarkerPosition, worldMarkerRotation);

        _PoseEstimation.ReportMarkerPose(virtualMarkerTransform, worldMarkerPose);
    }

    /*
        Deprecated
        A method to start the marker detection with the ARCore Augmented Images functionalities
        Since this method is unreliable when used to glass-covered images (door plates), it cannot be used in this project
    */
    private void DetectMarkerAugmentedImages()
    {
        AugmentedImage detectedWorldMarker = null;
        Transform detectedVirtualMarkerPosition = null;
        // ARCore needs to be in a tracking state for it to find a marker
        if (Session.Status != SessionStatus.Tracking) return;

        // Override _detectedPlanes with all tracked Planes
        Session.GetTrackables<DetectedPlane>(_detectedPlanes, TrackableQueryFilter.All);
        // Override _detectedImages with all tracked Augmented Images
        Session.GetTrackables(_detectedImages, TrackableQueryFilter.Updated);

        if (_detectedImages.Count == 1)
        {
            detectedWorldMarker = _detectedImages[0];
            if (detectedWorldMarker.TrackingState == TrackingState.Tracking &&
                detectedWorldMarker.TrackingMethod == AugmentedImageTrackingMethod.FullTracking)
            {
                Debug.Log("Found 1 Augmented Image: " + _detectedImages[0].Name);

                detectedVirtualMarkerPosition = _MarkerDatabase.RequestMarkerPosition(detectedWorldMarker.Name);
                //_WorldMarkerPositionList.Add(detectedWorldMarker.CenterPose);
                if (detectedVirtualMarkerPosition == null)
                {
                    Debug.Log("No matching marker found for Markername: " + detectedWorldMarker.Name);
                }
                else
                {
                    Debug.Log("Reporting Augmented Image Position");
                    _PoseEstimation.ReportMarkerPose(detectedVirtualMarkerPosition, detectedWorldMarker.CenterPose);
                }
            }
        }

        if (_WorldMarkerPositionList.Count >= _MaxTrackingCount)
        {
            //Pose averageWorldMarkerPosition = HelperFunctions.CalculateAveragePose(_WorldMarkerPositionList);
            //_PoseEstimation.ReportMarkerPosition(detectedVirtualMarkerPosition, );
            //_WorldMarkerPositionList.Clear();
        }
        Debug.Log("End of Augmented Tracking");
    }

    /*
        Method to start the optical character reader script using the Google Vision API
    */
    private void DetectMarkerOCR()
    {
        image = Frame.CameraImage.AcquireCameraImageBytes();
        // Resetting detectedPlane to null before each new calculation because it is a criteria for a newly detected plane
        detectedPlane = null;
        // Detection needs to continue until enough cpu resources have been freed
        if (!image.IsAvailable)
        {
            _SystemStatePresenter.DisplayUserMessage("Couldn't access camera image!");
        }
        /**
         * The camera image can be acquired and used to detect text within it
         * 1. The camera image is split into its brightness(Y) channel and meta data needed for calculations
         * 2. This data is sent to the Text Detection script to be evaluated by an OCR
         * 3. The facing current direction of the wall in front of the User is calculated 
         * 4. _isWaiting bool is set to true until the result comes back
         * 5. The detection can now be stopped. 
         * The status afterwards is "_isWaiting" and not "_isTracking"
         */
        else
        {
            var imageWidth = image.Width;
            var imageHeight = image.Height;
            var imageY = image.Y;
            var imageYRowStride = image.YRowStride;

            _TextDetection.DetectText(imageWidth, imageHeight, imageY, imageYRowStride);

            detectedPlane = GetCenterPlane();

            IndicateWaitingForResult();
        }
    }

    public void IndictateDetectedWall_OnOff()
    {
        _isIndicatingWall = !_isIndicatingWall;
        _SystemStatePresenter.DisplayUserMessage("Wall indicator started/ stopped");
    }

    private void DisplayDetectedWallInfo()
    {
        var foundPlane = GetCenterPlane();
        if (foundPlane == null)
        {
            _SystemStatePresenter.DisplayUserMessage("No wall in front");
        }
        else
        {
            _SystemStatePresenter.DisplayUserMessage("Wall found!");
        }

        /*
        Session.GetTrackables<DetectedPlane>(_detectedPlanes, TrackableQueryFilter.All);
        Debug.Log("\n\n\n");
        foreach (var plane in _detectedPlanes)
        {
            Debug.Log("Plane: " + plane.PlaneType + " / ROT: " + plane.CenterPose.rotation.eulerAngles);
        }
        */
    }

    /**
     * 1 Raycast to middle of screen to search for ARCore planes
     * 2 If the hit is a DetectedPlane, return it
     * 3    else return null
     * 
     */
    private DetectedPlane GetCenterPlane()
    {
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds;

        if (Frame.Raycast(Screen.width / 2, Screen.height / 2, raycastFilter, out hit))
        {
            if ((hit.Trackable is DetectedPlane)) // Might need to check if the hit is on the back of the plane
            {
                return (DetectedPlane)hit.Trackable;
            }
        }
        //Debug.Log("NO WALL DETECTED!");
        return null;
    }

    /*
        Method to indicate that a calculation has been initiated which can take longer
        than 1 update cycle. (Waiting for API response)

    */
    private void IndicateWaitingForResult()
    {
        _isWaiting = true;
        Invoke("TimeOutWaiting", _waitingTime); // Waiting time = 10 seconds
    }

    // Method to indicate that a result has been delivered
    private void AnnounceResult()
    {
        _isWaiting = false;
    }

    private void TimeOutWaiting()
    {
        AnnounceResult();
    }




}
