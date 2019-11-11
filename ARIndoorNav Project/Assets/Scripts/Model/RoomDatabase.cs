using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomDatabase : MonoBehaviour
{
    public GameObject _RoomParentGameObject;
    public RoomPresenter _RoomPresenter;

    private List<Room> _RoomList = new List<Room>();
    private string testDatabaseEntries =
        "3.215;Test3215 V2\n" +
        "3.216;Test3216\n" +
        "3.217;Test3217\n" +
        "3.218;Test3218\n" +
        "3.219;Test3219\n" +
        "3.220;Test3220\n" +
        "3.222;Test3222\n" +
        "3.222a;Test3222a";

    // Start is called before the first frame update
    void Start()
    {
        InitiateDatabase();
        _RoomPresenter.UpdateRoomList(_RoomList);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /**
        Searches the child components of the parent objects of all rooms within unity 
        and returns a Transform of found object. Else returns null
     */
    public Transform GetRoomPosition(string roomName)
    {
        return GameObject.Find(_RoomParentGameObject.name + "/" + roomName).transform;
    }

    /**
        Searches a list of all rooms for the location of a marker using its name
        The marker name has to be the same as the room name
        If no room is found, null is returned.
     */
    public Room GetRoom(string roomName)
    {
        Room room = null;

        Predicate<Room> roomFinder = (Room r) => { return r.Name == roomName; };
        room = _RoomList.Find(roomFinder);

        return room;
    }

    /**
        Reads a string that contains all room information and generates a List<Room>
        The values of each room are separated by ';' and each individual room by '\n'
     */
    private void InitiateDatabase()
    {
        string[] testEntryList = testDatabaseEntries.Split('\n');
        foreach (var entry in testEntryList)
        {
            var roomName = entry.Split(';')[0];
            var roomDescription = entry.Split(';')[1];
            
            if (roomName == null || roomDescription == null) continue;
            var newRoom = new Room(roomName, GetRoomPosition(roomName), roomDescription);
            _RoomList.Add(newRoom);
            
        }
    }

}
