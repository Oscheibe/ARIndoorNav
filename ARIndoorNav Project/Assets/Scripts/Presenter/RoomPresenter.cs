using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
    This script provides a searchable list of all rooms and a selevtion of most common destinations to the UI
    After the user chooses a destination, it will be handed over to the Navigation script where its further processed
 */
public class RoomListPresenter : MonoBehaviour
{
    public SearchUI _SearchUI;
    public RecommendedUI _RecommendedUI;
    public Navigation _Navigation;
    public RoomDatabase _RoomDatabase;
    
    // Start is called before the first frame update
    void Start()
    {
        // send lists
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateDestination(Room destination)
    {
        _Navigation.UpdateDestination(destination);
    }

    public void UpdateRoomList(List<Room> roomList)
    {
        _SearchUI.SendRoomList(roomList);
    }
}
