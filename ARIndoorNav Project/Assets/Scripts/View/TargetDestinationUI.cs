using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TargetDestinationUI : MonoBehaviour
{
    public TMP_Text _DestinationDistance;
    public TMP_Text _DestinationName;

    public TMP_Text _LastWorldMarkerPos, _LastVirtualMarkerPos, _ARCorePos;
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayTargetInformation(string destinationName, string targetDistance)
    {
        _DestinationName.text = destinationName;
        _DestinationDistance.text = targetDistance + " m";
    }

    public void UpdateDistance(string targetDistance)
    {
        _DestinationDistance.text = targetDistance + " m";
    }

    public void DisplayPositionInformation(string lastWorldMarkerPos, string lastVirtualMarkerPos, string arCorePos)
    {
        _LastWorldMarkerPos.text = "Last World Marker: " + lastWorldMarkerPos;
        _LastVirtualMarkerPos.text = "Last Virtual Marker: " + lastVirtualMarkerPos;
        _ARCorePos.text = "ARCore:" + arCorePos;
    }
}
