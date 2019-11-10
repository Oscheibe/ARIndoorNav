using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetDestinationUI : MonoBehaviour
{
    public Text _DestinationDistance;
    
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
        _DestinationDistance.text = targetDistance;
    }
}
