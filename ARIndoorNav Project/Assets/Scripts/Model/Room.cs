using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public string roomName
    {
        get
        {
            return roomName;
        }
        set
        {
            roomName = roomName;
        }
    }

    public Transform roomLocation
    {
        get
        {
            return this.transform;
        }
    }

}
