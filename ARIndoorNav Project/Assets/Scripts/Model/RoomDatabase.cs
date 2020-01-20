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
        "3.221;nA\n"+
        "3.222;Test3222\n" +
        "3.222a;Test3222a\n"+
        "3.200;nA\n"+
        "3.201;nA\n"+
        "3.202;nA\n"+
        "3.203;nA\n"+
        "3.204;nA\n"+
        "3.205;nA\n"+
        "3.206;nA\n"+
        "3.207;nA\n"+
        "3.208;nA\n"+
        "3.209;nA\n"+
        "3.210;nA\n"+
        "3.211;nA\n"+
        "3.212;nA\n"+
        "3.213;nA\n"+
        "3.214;nA\n"+
        // SEE TOP
        "3.223;nA\n"+
        "3.224;nA\n"+
        "3.225;nA\n"+
        "3.226;nA\n"+
        "3.227;nA\n"+
        "3.228;nA\n"+
        "3.229;nA\n"+
        "3.230;nA\n"+
        "3.231;nA\n"+
        "3.232;nA\n"+
        "3.233;nA\n"+
        "3.234;nA\n"+
        "3.235;nA\n"+
        "3.236;nA\n"+
        "3.237;nA\n"+
        "3.238;nA\n"+
        "3.239;nA\n"+
        "3.240;nA\n"+
        "3.241;nA\n"+
        "3.242;nA\n"+
        "3.243;nA\n"+
        "3.244;nA\n"+
        "3.245;nA\n"+
        "3.246;nA\n"+
        "3.247;nA\n"+
        "3.248;nA\n"+
        // TESTING MORE ROOMS SOON
        "3.119;nA\n"+
        "3.101;nA"; // LAST STRING WITHOUT '\n'

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
        var roomGO = GameObject.Find(_RoomParentGameObject.name + "/" + roomName);
        if(roomGO == null) 
        {
            return null;
        }
        else 
        {
            return roomGO.transform;
        }
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
            var roomPosition = GetRoomPosition(roomName);
            if(roomPosition == null) continue; // If there is no room with that name, skip that entry

            var newRoom = new Room(roomName, roomPosition, roomDescription);
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
