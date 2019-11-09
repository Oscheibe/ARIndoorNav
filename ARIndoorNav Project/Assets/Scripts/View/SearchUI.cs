using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchUI : MonoBehaviour
{
    public RoomPresenter _RoomPresenter;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //TODO
    public void SendRoomList(List<Room> roomList)
    {
        Debug.Log("Rooms: ");
        foreach (var room in roomList)
        {
            Debug.Log(room.Name + " at: " + room.Location.position);
        }
    }

    //TODO
    public void ChooseDestination(Room destination)
    {

        _RoomPresenter.UpdateDestination(destination);
    }

}
