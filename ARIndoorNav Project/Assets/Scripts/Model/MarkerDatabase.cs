using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MarkerDatabase : MonoBehaviour
{

    public RoomDatabase _RoomDatabase;

    private List<Room> roomList;

    // Start is called before the first frame update
    void Start()
    {
        roomList = _RoomDatabase.GetRoomList();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Transform RequestMarkerPosition(string markerName)
    {
        Transform position = null;
        foreach (var room in roomList)
        {
            if(room.Name == markerName) 
                position = room.Location;

        }
        return position;
    }

    public Room ContainsRoom(List<string> potentialMarkerList)
    {
        Room result = null;
        //TODO efficient search

        return result;
    }
}
