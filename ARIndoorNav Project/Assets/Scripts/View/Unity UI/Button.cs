using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/**
    This button class is only here to controll Marker Detection behaviour manually
*/
public class Button : MonoBehaviour, IPointerClickHandler
{
    public MarkerDetection _MarkerDetection;

    public void OnPointerClick(PointerEventData eventData)
    {
        _MarkerDetection.StartDetection();
    }
}
