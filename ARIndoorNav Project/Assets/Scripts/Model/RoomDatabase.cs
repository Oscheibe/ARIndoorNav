using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomDatabase : MonoBehaviour
{
    public GameObject _RoomParentGameObject;
    public RoomListPresenter _RoomListPresenter;
    public Navigation _Navigation;

    private List<Room> roomList = new List<Room>();
    private string testDatabaseEntries =
        "3.215;Test3215 V2\n" +
        "3.216;Test3216\n" +
        "3.217;Test3217\n" +
        "3.218;Test3218\n" +
        "3.219;Test3219\n" +
        "3.220;Test3220\n" +
        "3.222;Test3222\n" +
        "3.222a;Test3222a\n" + 
        "Treppenhaus; Um die Ecke";

    // Awake is called before Start
    void Awake()
    {
        InitiateDatabase();
        _RoomListPresenter.UpdateRoomList(roomList);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDistanceToUser();
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
        room = roomList.Find(roomFinder);

        return room;
    }

    // Return the initiated room list
    public List<Room> GetRoomList()
    {
        return roomList;
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
            roomList.Add(newRoom);
            
        }
    }

    /**
        Updates all the DistanceToUser values within the database
    */
    private void UpdateDistanceToUser()
    {
        foreach (var room in roomList)
        {
            room.DistanceToUser = _Navigation.GetDistanceToUser(room.Location.position);
        }
    }

}
