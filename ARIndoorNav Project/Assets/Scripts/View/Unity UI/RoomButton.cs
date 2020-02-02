using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomButton : MonoBehaviour
{
    public RoomListUI _RoomListUI;
    
    public Text _RoomNameText;
    public Text _RoomInformationText;
    public TMP_Text _DistanceToRoomText;
    public Image _RoomImage;

    private string allText;

    
    private Room room;
    
    public void InitializeButton(Room room, string distanceToRoom)
    {
        this.room = room;
        _RoomNameText.text = room.Name;
        _RoomInformationText.text = room.Description;
        _DistanceToRoomText.text = distanceToRoom;
        if(room.Image != null) _RoomImage = room.Image;

        allText = _RoomNameText.text + " " + _RoomInformationText.text;
    }

    public void UpdateDistanceToRoom()
    {
        _DistanceToRoomText.text = room.DistanceToUser + " m";
    }

    public void OnClick()
    {
        _RoomListUI.ChooseDestination(room);
    }

    // Returns true if the room botton contains the text
    public bool ContainsText(string text)
    {
        return allText.Contains(text);
    }
}
