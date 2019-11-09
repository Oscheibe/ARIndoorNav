using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MarkerDatabase : MonoBehaviour
{
    public RoomDatabase _RoomDatabase;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public Transform RequestMarkerPosition(string markerName)
    {
        return _RoomDatabase.GetRoomPosition(markerName);
    }
}
