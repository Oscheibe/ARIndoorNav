using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseEstimation : MonoBehaviour
{
    public GameObject _LastUserPosition;
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
    public void ReportPosition(Transform reportedPosition)
    {
        Transform calculatedPosition;
        //TODO: Save positions for future analysis and do some calculations with them
        calculatedPosition = reportedPosition;

        _MarkerDetection.StopDetection();
        UpdateLastUserPosition(calculatedPosition);
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

    private void UpdateLastUserPosition(Transform reportedPosition)
    {
        //TODO: change _userPosition
        _LastUserPosition.transform.Rotate(new Vector3(1,1,1));
    }

}
