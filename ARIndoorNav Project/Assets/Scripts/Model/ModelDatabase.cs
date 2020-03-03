using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelDatabase : MonoBehaviour
{
    public Transform _Floor3Plane;
    public Transform _Floor2Plane;
    public Transform _Floor1Plane;
    public Transform _Floor0Plane;


    // Start is called before the first frame update
    void Start()
    {

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
