using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SimpleJSON;

public class RoomDatabase : MonoBehaviour
{
    public RoomListPresenter _RoomListPresenter;
    public Navigation _Navigation;
    public MarkerDatabase _MarkerDatabase;

    private List<Room> roomList = new List<Room>();
    public string roomListFilePath = "Rooms/RoomList";


    // Awake is called before Start
    void Start()
    {
        InitiateDatabase();
        var validatedList = _MarkerDatabase.ValidateMarkerList(roomList);
        if(validatedList)
            Debug.Log("Validated room database successfully");
        _RoomListPresenter.UpdateRoomList(roomList);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDistanceToUser();
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
        TextAsset roomListJSON = Resources.Load(roomListFilePath) as TextAsset;
        var roomListNode = JSON.Parse(roomListJSON.ToString());

        var roomArray = roomListNode.AsArray;
        foreach (var roomPair in roomArray)
        {
            var roomName = roomPair.Value["RoomName"];
            var roomDescription = roomPair.Value["Description"];
            if (roomName == null || roomDescription == null)
            {
                Debug.Log("Error Calculating Room KeyValuePair: " + roomPair);
                continue;
            }

            var floorNumber = GetFloorNumber(roomName);

            var roomPosition = GetRoomPosition(roomName);
            if (roomPosition == null)
            {
                Debug.Log("No Room Found: " + roomName);
                continue; // If there is no room with that name, skip that entry
            }

            var newRoom = new Room(roomName, roomDescription, floorNumber, roomPosition);
            roomList.Add(newRoom);
            //Debug.Log("Room: " + roomName + " Floor: " + floorNumber);
        }
    }

    /**
        Updates all the DistanceToUser values within the database
    */
    private void UpdateDistanceToUser()
    {
        foreach (var room in roomList)
        {
            room.DistanceToUser = _Navigation.GetUnityDistanceToUser(room);
        }
    }

    /**
     * Returns the first number from the room name.
     * Stairs should not be used as a target because of it
     */
    private int GetFloorNumber(string roomName)
    {
        int floorNumber = -1;
        foreach (var ch in roomName.ToCharArray())
        {
            if (char.IsNumber(ch))
            {
                floorNumber = (int)char.GetNumericValue(ch);
                break;
            }
        }
        // It should never happen, but who knows
        if (floorNumber > 3)
            return -1;
        return floorNumber;
    }

    /**
    Searches the child components of the parent objects of all rooms within unity 
    and returns a Transform of found object. Else returns null
     */
    public Transform GetRoomPosition(string roomName)
    {
        return _MarkerDatabase.RequestMarkerPosition(roomName);
    }

}
