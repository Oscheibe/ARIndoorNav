using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingErrorHandling : MonoBehaviour
{
    public SystemStatePresenter _SystemStatePresenter;
    public PoseEstimation _PoseEstimation;

    private Vector3 lastPosition;
    private float maxErrorDistance = 2f; // max offset distance in meter
    private List<Vector3> announcedPositionJumps = new List<Vector3>();

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
        if(distanceToLastPosition >= maxErrorDistance && WasPositionAnnounced(position) == false)
        {
            _SystemStatePresenter.DisplayUserMessage("Detected big jump in user position: " + distanceToLastPosition + "m");
            _SystemStatePresenter.DisplayUserMessage("From: " + lastPosition + " to: " + position);
            Debug.Log("Position jump from: " + lastPosition + " to: " + position);
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
     * check if the new position was announced before
     */
    private bool WasPositionAnnounced(Vector3 position)
    {
        return announcedPositionJumps.Contains(position);
    }
    

}
