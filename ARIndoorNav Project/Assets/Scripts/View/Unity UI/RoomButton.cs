using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomButton : MonoBehaviour
{
    public RoomListUI _RoomListUI;
    
    public TMP_Text _RoomNameText;
    public TMP_Text _RoomInformationText;
    public TMP_Text _DistanceToRoomText;

    private string allText;

    
    private Room room;
    
    public void InitializeButton(Room room, string distanceToRoom)
    {
        this.room = room;
        _RoomNameText.text = room.Name;
        _RoomInformationText.text = room.Description;
        _DistanceToRoomText.text = distanceToRoom;

        var oldName = _RoomNameText.text;
        var newName = _RoomNameText.text.Replace(".", "").Replace("/",""); // removing the . and / for easy searchability
        
        allText = oldName + " " + newName + " " + _RoomInformationText.text + " ";
    }

    public void UpdateDistanceToRoom()
    {
        _DistanceToRoomText.text = "(" + room.DistanceToUser.ToString("0.00") + " m)";
    }

    public void OnClick()
    {
        _RoomListUI.ChooseDestination(room);
    }

    // Returns true if the room botton contains the text
    public bool ContainsText(string text)
    {
        return allText.ToLower().Contains(text.ToLower());
    }

    public float GetDistance()
    {
        return room.DistanceToUser;
    }

    public string GetName()
    {
        return room.Name;
    }
}
