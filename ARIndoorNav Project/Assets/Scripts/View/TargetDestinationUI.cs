using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDestinationUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayTargetInformation(string destinationName, string targetDistance)
    {
        Debug.Log(destinationName + " (" + targetDistance + "m away)");
    }
}
