using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomTextField
{
    private Room room;

    private RoomTextField() { }

    public RoomTextField(Room room)
    {
        Room = room;
    }

    public Room Room
    {
        get { return room; }
        set { room = value; }
    }

    public void SetRoomData(GameObject textField)
    {
        foreach (Transform child in textField.transform)
        {
            if (child.tag == "PlateText")
                child.gameObject.GetComponentInChildren<TMP_Text>().text = room.Description;

            if (child.tag == "NameText")
                child.gameObject.GetComponentInChildren<TMP_Text>().text = room.Name;
        }

        textField.GetComponent<TextField>().Room = room;
    }

}
