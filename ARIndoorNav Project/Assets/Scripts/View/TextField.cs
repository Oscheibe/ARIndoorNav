using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextField : MonoBehaviour, IPointerClickHandler
{
    public SearchUI _SearchUI;
    public Animator _Animator;

    private Room room;


    public Room Room
    {
        get { return room; }
        set { room = value; }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        _Animator.SetBool("selected", true);
        _SearchUI.ChooseDestination(room);
    }
}
