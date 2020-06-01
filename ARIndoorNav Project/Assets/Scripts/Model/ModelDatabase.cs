using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelDatabase : MonoBehaviour
{
    public Transform _Floor3Plane;
    public Transform _Floor2Plane;
    public Transform _Floor1Plane;
    public Transform _Floor0Plane;

    public GameObject _Floor3GO;
    public GameObject _Floor2GO;
    public GameObject _Floor1GO;
    public GameObject _Floor0GO;



    // Start is called before the first frame update
    void Start()
    {

    }

    public void HideFloorsUntil(int floor)
    {
        /**
         * Hides the floor above and then falls through to the next one.
         * Example: User is in floor 1;
         * 1 case 1: Floor 2 is hidden 
         * 2 case 2: Floor 3 is hidden 
         * 3 Now the only visible floors are Floor 1 and Floor 0
         * (Floor 1 still needs to be visible because the user is standing on that)
         */
        switch (floor)
        {
            case 0:
                _Floor1GO.SetActive(false);
                goto case 1; // Because C# doesn't allow normal fall through ... 
            case 1:
                _Floor2GO.SetActive(false);
                goto case 2; // Because C# doesn't allow normal fall through ... 
            case 2:
                _Floor3GO.SetActive(false);
                break;

            default:
                break;
        }
    }


    /**
     * returns the floor transform of floor 0 to 3
     */
    public Transform GetFloorTransform(int floorNumber)
    {
        if (floorNumber < 0 || floorNumber > 3)
            return null;

        // No breaks needed when return is called
        switch (floorNumber)
        {
            case 0:
                return _Floor0Plane;

            case 1:
                return _Floor1Plane;

            case 2:
                return _Floor2Plane;

            case 3:
                return _Floor3Plane;

            default:
                return null;
        }
    }
}
