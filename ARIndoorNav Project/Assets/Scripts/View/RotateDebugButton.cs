using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateDebugButton : MonoBehaviour, IPointerClickHandler
{
    public Transform _LastMarkerPositionTransform;
    public bool direction = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(direction) _LastMarkerPositionTransform.Rotate(new Vector3(0,1,0), 2f);
        else _LastMarkerPositionTransform.Rotate(new Vector3(0,1,0), -2f);
    }
}
