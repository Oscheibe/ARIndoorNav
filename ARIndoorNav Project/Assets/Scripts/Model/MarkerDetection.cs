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

    public Text _TrackingStateText;

    public bool _isTracking = false;
    private List<DetectedPlane> _detectedPlanes = new List<DetectedPlane>();
    private List<AugmentedImage> _detectedImages = new List<AugmentedImage>();
    private List<Pose> _WorldMarkerPositionList = new List<Pose>();
    private readonly int _MaxTrackingCount = 10;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Continuosly tracks marker after the StartTracking() method is called
    // Only stops when the StopTracking method is called
    void Update()
    {
        if (_isTracking) DetectMarker();
    }

    /**
        Starts the marker detection with ARCore AugmentedImages
        Is called by PoseEstimation
     */
    public void StartDetection()
    {
        Debug.Log("Started marker detection");
        _isTracking = true;
        _TrackingStateText.text = "Is Tracking";
    }

    /**
        Stops the marker detection with ARCore AugmentedImages
        Is called by PoseEstimation
     */
    public void StopDetection()
    {
        Debug.Log("Stopped marker detection");
        _isTracking = false;
        _TrackingStateText.text = "Tracking stopped";
    }

    /**
       Method that uses ARCores AugmentedImage tracking capabilities
       Is called once per frame when the _isTracking bool is true 
     */
    private void DetectMarker()
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
                detectedVirtualMarkerPosition = _MarkerDatabase.RequestMarkerPosition(detectedWorldMarker.Name);
                //_WorldMarkerPositionList.Add(detectedWorldMarker.CenterPose);
                if(detectedVirtualMarkerPosition == null)
                {
                    Debug.Log("No matching marker found for Markername: " + detectedWorldMarker.Name);
                }
                else
                {
                     _PoseEstimation.ReportMarkerPosition(detectedVirtualMarkerPosition, detectedWorldMarker.CenterPose);
                }
            }
        }
        
        
        if (_WorldMarkerPositionList.Count >= _MaxTrackingCount)
        {
            //Pose averageWorldMarkerPosition = HelperFunctions.CalculateAveragePose(_WorldMarkerPositionList);
            //_PoseEstimation.ReportMarkerPosition(detectedVirtualMarkerPosition, );
            //_WorldMarkerPositionList.Clear();
        }
    }


}
