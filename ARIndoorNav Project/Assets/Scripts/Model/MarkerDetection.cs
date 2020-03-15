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

    public float _timeoutSeconds = 10f;
    public bool _isTracking = false; // For Debugging purposes: To start manual marker detection
    public bool _isWaiting = false; // If the marker detection has been initialized and is being calculated
    public float _scanningDistance = 0.3f; // The average distance of the user when scanning a sign

    private CameraImageBytes image;
    private Pose userPose;
    private Room markerRoom;


    /**
        Starts the marker detection with ARCore AugmentedImages
        Step 1: Calculate and save the user pose during scanning
        Step 2: Request a OCR result from the connected service
        Step 3: Wait for result and process it when it arrives
        Called by: PoseEstimation
     */
    public void StartDetection()
    {
        SaveUserPosition();
        InitiateOCRDetection();
        WaitForResponse();
    }

    /** 
     * Saves the current user position for calculations done with the OCR image
     */
    private void SaveUserPosition()
    {
        // Calculating the user position, parallel to the x-z axis
        var flatUserRotation = new Quaternion(0,
                                        _PoseEstimation.GetUserRotation().y,
                                        0,
                                        _PoseEstimation.GetUserRotation().w);

        // Getting the current user position relative to where the app was launched
        // This value is constantly tracked by ARCore
        var userPosition = _PoseEstimation.GetUserPosition();

        userPose = new Pose(userPosition, flatUserRotation);
    }

    /**
     * Sends a snapshot of the current camera image and convert it into a JPG image 
     * to be processed by a OCR service.
     * The response can arrive at any time. The default timeout time is 10 seconds
     * The average response time from Google Cloud Vision is 1 second
     */
    private void InitiateOCRDetection()
    {
        image = Frame.CameraImage.AcquireCameraImageBytes();

        // Detection needs to continue until enough cpu resources have been freed
        // If it fails, the user has to start the whole process again
        if (!image.IsAvailable)
        {
            _SystemStatePresenter.DisplayUserMessage("Couldn't access camera image! Please try again.");
        }
        /**
         * The camera image is split into its brightness(Y) channel and the meta data needed for calculations
         * This data is needed to create a Texture2D that can be converted into a JPG
         */
        else
        {
            _TextDetection.DetectText(image.Width, image.Height, image.Y, image.YRowStride);
        }
        image.Release();
    }

    /**
     * Indicate that the app is waiting for an external system.
     * SystemStatePresenter will get notified by this to display an appropriate message to the user
     */
    private void WaitForResponse()
    {
        _isWaiting = true;
        Invoke("TimeOutWaiting", _timeoutSeconds); // Waiting time = 10 seconds
        _SystemStatePresenter.ShowLoadingAnimation();
    }

    /**
     * Receive a list of strings from the OCR that needs to be checked if they contain a room name
     */
    public void ReceiveResponse(List<string> potentialMarkerList)
    {
        _isWaiting = false;
        SaveRoomPosition(potentialMarkerList);
        SendDetectionResults(markerRoom, userPose);
    }

    private void SaveRoomPosition(List<string> potentialMarkerList)
    {
        var resultRoomList = _MarkerDatabase.ContainsRoom(potentialMarkerList);
        foreach (var name in potentialMarkerList)
        {
            Debug.Log("API Result: " + name);
        }
        if (resultRoomList == null)
        {
            _SystemStatePresenter.DisplayUserMessage("No Results Found! Please scan another room plate");
        }

        if (resultRoomList != null)
        {
            //TODO: more than 1 room found
            markerRoom = resultRoomList[0];
            _SystemStatePresenter.DisplayCurrentPosition(resultRoomList[0].Name);
        }
        else
        {
            Debug.Log("ERROR CALCULATING ROOM POSITION: No room or no wall found");
        }
    }

    private void SendDetectionResults(Room markerRoom, Pose userPose)
    {
        _PoseEstimation.ReportMarkerPose(markerRoom.Location, userPose);
        _PoseEstimation.ReportCurrentFloor(markerRoom.Floor);
        _SystemStatePresenter.HideMarkerDetectionMenu();
    }

    private void TimeOutWaiting()
    {
        if (_isTracking)
            _SystemStatePresenter.DisplayUserMessage("OCR service timed out. Please try again");
    }

}
