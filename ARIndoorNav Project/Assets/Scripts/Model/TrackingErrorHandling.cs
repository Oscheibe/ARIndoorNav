using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingErrorHandling : MonoBehaviour
{
    public SystemStatePresenter _SystemStatePresenter;
    public PoseEstimation _PoseEstimation;

    private Vector3 lastPosition;
    private float maxErrorDistance = 2f; // max offset distance in meter

    /**
        Method that accumulates the tracking state of ARCore and analyses it for drift
        If the drift is too high, a marker needs to be scanned.
        
     */
    public void ReportTrackingState(string state, float time)
    {
        // TODO
        
        _SystemStatePresenter.DisplayUserMessage(state);
    }

    public void ReportCurrentUserPosition(Vector3 position)
    {
        var distanceToLastPosition = Vector3.Distance(position, lastPosition);
        if(distanceToLastPosition >= maxErrorDistance)
        {
            _SystemStatePresenter.DisplayUserMessage("Detected big jump in user position: " + distanceToLastPosition + "m");
            _SystemStatePresenter.DisplayUserMessage("From: " + lastPosition + " to: " + position);
        }

        lastPosition = position;
    }

    private void ReportTrackingState()
    {
        // if.... _UserMessagesPresenter.SendWarningMessage("TODO");
        
    }

}
