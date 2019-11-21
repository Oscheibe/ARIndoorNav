using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
    Sends navigation information to the UI.
    Information include: AR Element position/ Room name, number, distance, etc.
 */
public class NavigationPresenter : MonoBehaviour
{
    public ARVisuals _ARVisuals;
    public TargetDestinationUI _TargetDestinationUI;

    private string _currentDestination;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayNavigationInformation(string destinationName, float destinationDistance, Vector3[] path)
    {
        _currentDestination = destinationName;
        _TargetDestinationUI.DisplayTargetInformation(_currentDestination, destinationDistance.ToString());
        _ARVisuals.SendNaviagtionPath(path);
    }

    public void UpdateNavigationInformation(string destinationDistance, Vector3[] path)
    {
        _TargetDestinationUI.UpdateDistance(destinationDistance);
        _ARVisuals.SendNaviagtionPath(path);
    }

}
