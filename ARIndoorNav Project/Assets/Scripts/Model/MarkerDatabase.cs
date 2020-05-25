using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MarkerDatabase : MonoBehaviour
{

    public RoomDatabase _RoomDatabase;

    private List<GameObject> markerList;

    // private string missingRoomsJSON = ""; USED FOR DEBUGGING IN LINE 82

    // Start is called before the first frame update
    void Awake()
    {
        markerList = new List<GameObject>(GameObject.FindGameObjectsWithTag("VirtualMarker"));//_RoomDatabase.GetRoomList();
    }

    public Transform RequestMarkerPosition(string markerName)
    {
        Predicate<GameObject> markerFinder = (GameObject marker) => { return marker.name == markerName; };
        var markerGO = markerList.Find(markerFinder);

        if (markerGO == null)
            return null;
        else
            return markerGO.transform;

        //return markerGO ? null : markerGO.transform;
    }

    /**
     * 1 Go through each result from OCR
     * 2 Go through each character from OCR to determin if it contains a number
     * 3 Check if a string that contains a number colorates with a room number
     * 4 Add each result to the result list
     * 5 If there is no match return 0 else return the list with at least 1 room
     * Needs to be adjusted later, MakerDatabase should not return any rooms!
     */
    public List<Room> ContainsRoom(List<string> potentialMarkerStringsList)
    {
        List<Room> resultList = new List<Room>();
        foreach (string text in potentialMarkerStringsList)
        {
            bool containsNumber = false;
            foreach (char character in text)
            {
                if (Char.IsDigit(character))
                    containsNumber = true;
            }
            if (containsNumber)
            {
                var resultRoom = _RoomDatabase.GetRoom(text.Replace(" ", ""));
                if (resultRoom != null)
                    resultList.Add(resultRoom);
            }
        }
        if (resultList.Count == 0)
            return null;
        else
            return resultList;
    }

    /**
     * Method to validate if every marker has an entry in the database
     */
    public bool ValidateMarkerList(List<Room> roomList)
    {
        bool matched1to1 = true;
        string markerName = "";
        Predicate<Room> roomFinder = (Room room) => { return room.Name == markerName; };

        foreach (var marker in markerList)
        {
            markerName = marker.name;
            if (roomList.Find(roomFinder) == null)
            {
                matched1to1 = false;
                Debug.Log("No room for marker: " + markerName);
                //missingRoomsJSON += "{\"RoomName\": \"" + markerName + "\",\n\"Description\": \"\"\n},";
            }
        }
        //Debug.Log(missingRoomsJSON);
        return matched1to1;
    }
}
