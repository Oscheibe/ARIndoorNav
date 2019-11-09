using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GoogleARCore;

/**
    This script is used for the marker detection
    It uses AugmentedImages to scan for marker in the real world
    If a marker was found, it's relative position to it's virtual counterpart is sent to PoseEstimation
 */
public class MarkerDetection : MonoBehaviour
{
    public PoseEstimation _PoseEstimation;
    public MarkerDatabase _MarkerDatabase;

    private bool _isTracking = false;
    private List<DetectedPlane> _detectedPlanes = new List<DetectedPlane>();
    private List<AugmentedImage> _detectedImages = new List<AugmentedImage>();
    private List<Transform> _markerPositionList = new List<Transform>();
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
        _isTracking = true;
    }

    /**
        Stops the marker detection with ARCore AugmentedImages
        Is called by PoseEstimation
     */
    public void StopDetection()
    {
        _isTracking = false;
    }

    /**
       Method that uses ARCores AugmentedImage tracking capabilities
       Is called once per frame when the _isTracking bool is true 
     */
    private void DetectMarker()
    {
        AugmentedImage detectedMarker;
        Transform detectedMarkerPosition = null;
        // ARCore needs to be in a tracking state for it to find a marker
        if (Session.Status != SessionStatus.Tracking) return;

        // Override _detectedPlanes with all tracked Planes
        Session.GetTrackables<DetectedPlane>(_detectedPlanes, TrackableQueryFilter.All);
        // Override _detectedImages with all tracked Augmented Images
        Session.GetTrackables(_detectedImages, TrackableQueryFilter.Updated);

        if (_detectedImages.Count == 1)
        {
            detectedMarker = _detectedImages[0];
            if (detectedMarker.TrackingState == TrackingState.Tracking &&
                detectedMarker.TrackingMethod == AugmentedImageTrackingMethod.FullTracking)
            {
                detectedMarkerPosition = _MarkerDatabase.RequestMarkerPosition(detectedMarker.Name);
            }
        }

        //TODO median of detected marker
        if (detectedMarkerPosition != null)
        {
            _markerPositionList.Add(detectedMarkerPosition);
        }

        if(_markerPositionList.Count >= _MaxTrackingCount)
        {
            Transform calculatedMarkerPosition = HelperFunctions.CalculateAverageTransfrom(_markerPositionList);
            _PoseEstimation.ReportPosition(calculatedMarkerPosition);
        }
    }


}
