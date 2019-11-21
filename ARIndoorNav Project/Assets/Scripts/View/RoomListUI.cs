using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListUI : MonoBehaviour
{
    public GameObject _buttonTemplate;

    private List<GameObject> buttonGameobjects = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SendRoomList(List<Room> roomList)
    {
        ClearButtonList();
        foreach (var room in roomList)
        {
            GenButtons(room);
        }
    }


    private void GenButtons(Room room)
    {
        GameObject button = Instantiate(_buttonTemplate) as GameObject;
        button.SetActive(true);
        //button.GetComponent<RoomButton>().SetText(buttonName);
        button.name = room.Name;
        button.transform.SetParent(_buttonTemplate.transform.parent, false); // set false so that button doesn't position themselves in worldspace. Makes it more dynamic
        buttonGameobjects.Add(button);
    }

    private void ClearButtonList()
    {
        foreach (var button in buttonGameobjects)
        {
            GameObject.Destroy(button);
        }
        buttonGameobjects.Clear();
    }
}
