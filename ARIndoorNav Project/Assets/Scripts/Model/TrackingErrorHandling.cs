using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingErrorHandling : MonoBehaviour
{
    public SystemStatePresenter _SystemStatePresenter;
    public PoseEstimation _PoseEstimation;

    private Vector3 lastPosition;
    private float maxMoveErrorDistance = 2f; // max offset distance in meter
    private float maxAnnouncedErrorDistance = 0.5f;
    private float errorAccumulationTime = 300f; // time in seconds
    private float timeLeft;
    private List<Vector3> announcedPositionJumps = new List<Vector3>();
    private bool isTracking = false; // if the error accumulation is beeing tracked

    void Update()
    {
        DecreaseTimer();
    }

    /**
        Method that accumulates the tracking state of ARCore and analyses it for drift
        If the drift is too high, a marker needs to be scanned.
        
     */
    public void ReportTrackingState(string state, float time)
    {
        //TODO

        _SystemStatePresenter.DisplayUserMessage(state);
    }

    public void ReportCurrentUserPosition(Vector3 position)
    {
        var distanceToLastPosition = Vector3.Distance(position, lastPosition);
        if (distanceToLastPosition >= maxMoveErrorDistance && WasPositionAnnounced(position) == false)
        {
            _SystemStatePresenter.DisplayUserMessage("Detected big jump in user position: " + distanceToLastPosition + "m");
            _SystemStatePresenter.DisplayUserMessage("From: " + lastPosition + " to: " + position);
            Debug.Log("Position jump from: " + lastPosition + " to: " + position);
            AnnounceTooMuchError();
        }

        lastPosition = position;
    }

    private void ReportTrackingState()
    {
        // if.... _UserMessagesPresenter.SendWarningMessage("TODO");

    }

    /**
     * Method to announce an abnormal change in the position of the user 
     * The new position will not be reported to the user and instead be ignored
     * Used when: 
     *  - First start of app
     *  - New marker scanned
     */
    public void AnnouncePositionJump(Vector3 position)
    {
        Debug.Log("Announced position jump: " + position);
        announcedPositionJumps.Add(position);
    }

    /** 
     * check if the new position was announced before within a specified area-size around the announced positions
     */
    private bool WasPositionAnnounced(Vector3 position)
    {
        bool wasAnnounced = false;
        foreach (var announcedPos in announcedPositionJumps)
        {
            if (Vector3.Distance(announcedPos, position) <= maxAnnouncedErrorDistance)
            {
                wasAnnounced = true;
            }
        }
        return wasAnnounced;
    }

    /**
     * Starts a timer for tracking error accumulation
     * further research needs to be made to determine exact relation between time and error accumulation
     */
    private void StartErrorAccumulationTimer()
    {
        timeLeft = errorAccumulationTime;
        isTracking = true;
    }

    /** 
     * Decreases timeLeft by the time delta since the last update
     */
    private void DecreaseTimer()
    {
        // Stop logic when the error accumulation is not beeing tracked
        if(!isTracking) 
            return;

        timeLeft -= Time.deltaTime;
        if(timeLeft < 0)
        {
            AnnounceTooMuchError();
        }
        //TODO: indicate current state of error
    }

    /**
     * When too much error accumulated, the user has to rescan their position
     */
    private void AnnounceTooMuchError()
    {
        _PoseEstimation.RequestNewPosition(PoseEstimation.NewPosReason.TooMuchError);
    }

}
