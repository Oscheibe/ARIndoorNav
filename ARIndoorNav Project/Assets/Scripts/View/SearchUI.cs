using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchUI : MonoBehaviour
{
    public RoomListPresenter _RoomListPresenter;
    public GameObject _TextField;
    public Text _DestinationText;

    private List<GameObject> _roomTextFieldGOList = new List<GameObject>();
    public RectTransform containerRectTrans;

    // Start is called before the first frame update
    void Start()
    {
        //containerRectTrans = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //TODO
    public void SendRoomList(List<Room> roomList)
    {
        // Adding first GameObject because the Y position is relative to the start and not the last added TextField
        float textFieldHeight = 129;//textField.GetComponent<Collider>().bounds.size.y;
        Vector3 position;
        
        float lastY = (containerRectTrans.sizeDelta.y) + transform.position.y;
        foreach (var room in roomList)
        {
            // Create a new RoomTextField that will be stored within a new GameObject
            var newTextField = new RoomTextField(room);
            // Determining the vertical position of the new GameObject
            position = new Vector3(this.transform.position.x, lastY - textFieldHeight, this.transform.position.z);
            lastY -= textFieldHeight;
            // Instantiate the new GameObject from a prefab and the calculated position
            var newTextFieldGO = Instantiate(_TextField, position, Quaternion.identity, transform);
            newTextFieldGO.name = room.Name;
            newTextField.SetRoomData(newTextFieldGO);

            newTextFieldGO.SetActive(true);
            

            _roomTextFieldGOList.Add(newTextFieldGO);
        }
    }

    //TODO
    public void ChooseDestination(Room destination)
    {
        _DestinationText.text = destination.Name;
        _RoomListPresenter.UpdateDestination(destination);
    }




}
