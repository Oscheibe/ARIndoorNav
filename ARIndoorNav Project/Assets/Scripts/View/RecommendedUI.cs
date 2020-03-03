using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecommendedUI : MonoBehaviour
{
    public NavigationPresenter _NavigationPresenter;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //TODO
    public void SendRecommendedRoomList(List<Room> recommendedRoomList)
    {
        foreach (var room in recommendedRoomList)
        {
            //Debug.Log(room.Name + " at: " + room.Location.position);
        }
    }

    public void ChooseDestination(Room destination)
    {
        //_RoomListPresenter.UpdateDestination(destination);
    }
}
