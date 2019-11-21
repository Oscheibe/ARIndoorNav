using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListUI : MonoBehaviour
{
    public GameObject _buttonTemplate;
    public RoomListPresenter _RoomListPresenter;

    private List<GameObject> buttonGameobjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateDistanceToUser();
    }

    public void SendRoomList(List<Room> roomList)
    {
        ClearButtonList();
        foreach (var room in roomList)
        {
            GenButtons(room);
        }
    }

    public void ChooseDestination(Room destination)
    {
        _RoomListPresenter.UpdateDestination(destination);
    }

    private void GenButtons(Room room)
    {
        GameObject button = Instantiate(_buttonTemplate) as GameObject;
        button.SetActive(true);
        button.GetComponent<RoomButton>().InitializeButton(room, "Distance TBD");
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

    /**
        Updates all the DistanceToUser values within the database
    */
    private void UpdateDistanceToUser()
    {
        foreach (var button in buttonGameobjects)
        {
            button.GetComponent<RoomButton>().UpdateDistanceToRoom();
        }
    }
}
