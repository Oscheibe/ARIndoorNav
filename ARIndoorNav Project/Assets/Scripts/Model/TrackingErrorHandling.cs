using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingErrorHandling : MonoBehaviour
{
    public SystemStatePresenter _SystemStatePresenter;
    public PoseEstimation _PoseEstimation;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /**
        Method that accumulates the tracking state of ARCore and analyses it for drift
        If the drift is too high, a marker needs to be scanned.
        
     */
    public void ReportTrackingState(string state, float time)
    {
        // TODO
        
        _SystemStatePresenter.DisplayUserMessage(state);
    }

    private void ReportTrackingState()
    {
        // if.... _UserMessagesPresenter.SendWarningMessage("TODO");
        
    }

}
