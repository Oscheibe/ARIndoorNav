using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListUI : MonoBehaviour
{
    public GameObject _buttonTemplate;
    public NavigationPresenter _NavigationPresenter;

    private List<GameObject> buttonGameobjects = new List<GameObject>();

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
        SortListByName();
    }

    public void ChooseDestination(Room destination)
    {
        _NavigationPresenter.UpdateDestination(destination);
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

    /**
     * Go through each room in the list and hides the button if they don't contain the text
     * No consideration for performance
     */
    public void SearchRoom(string text)
    {
        foreach (var button in buttonGameobjects)
        {
            if (button.GetComponent<RoomButton>().ContainsText(text) == false)
                button.SetActive(false);
            else
                button.SetActive(true);
        }
    }

    /**
     * Makes all buttons visible again
     */
    public void ResetAllButton()
    {
        foreach (var button in buttonGameobjects)
        {
            button.SetActive(true);
        }
    }

    /**
     * Sorts the room buttons by changing their order as siblings
     * Can be used to order them by their distance
     */
    public void SortList(System.Comparison<GameObject> comparison)
    {
        buttonGameobjects.Sort(comparison);
        foreach (var button in buttonGameobjects)
        {
            /**
             * Set as last so that the next element in the list comes after the first one;
             * Sorted list: 1, 2, 3, 4 
             * {} -> {1} -> {1,2} -> {1,2,3}
             */

            button.transform.SetAsLastSibling();
        }
    }

    /**
     * Method to sort the room list by their distance
     * Uses the SortByDistanceComparator to sort the list
     */
    public void SortListByDistance()
    {
        SortList(SortByDistanceComparator);
    }

    /**
     * Method to sort the room list by their distance
     * Uses the SortByDistanceComparator to sort the list
     */
    public void SortListByName()
    {
        SortList(SortByNameComparator);
    }

    /**
     * Compare method to compare the rooms of each button by distance
     */
    private int SortByDistanceComparator(GameObject roomButton1, GameObject roomButton2)
    {
        float distance1 = roomButton1.GetComponent<RoomButton>().GetDistance();
        float distance2 = roomButton2.GetComponent<RoomButton>().GetDistance();

        return distance1.CompareTo(distance2);
    }

    /**
     * Compare method to compare the rooms of each button by name
     */
    private int SortByNameComparator(GameObject roomButton1, GameObject roomButton2)
    {
        string name1 = roomButton1.GetComponent<RoomButton>().GetName();
        string name2 = roomButton2.GetComponent<RoomButton>().GetName();
        return name1.CompareTo(name2);
    }

}
