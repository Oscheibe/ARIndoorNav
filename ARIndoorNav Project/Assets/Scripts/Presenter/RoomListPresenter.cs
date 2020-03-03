using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
    This script provides a searchable list of all rooms and a selevtion of most common destinations to the UI
    After the user chooses a destination, it will be handed over to the Navigation script where its further processed
 */
public class RoomListPresenter : MonoBehaviour
{
    // View
    public RoomListUI _RoomListUI;
    public RecommendedUI _RecommendedUI;

    public void UpdateRoomList(List<Room> roomList)
    {
        _RoomListUI.SendRoomList(roomList);
    }
}
