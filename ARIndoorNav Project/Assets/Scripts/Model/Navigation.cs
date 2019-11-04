using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/**
    The navigation system uses NavMesh to calculate a path.
    The user and their position is model as a GameObject and accessed by NavMesh in the background.
    The destination and its information is chosen by the user. The position is also represented by a GameObject.


 */
public class Navigation : MonoBehaviour
{
    public NavMeshAgent _userPosition;
    public NavMeshSurface _mapModelMesh;

    private Room _destination;
    
    // Start is called before the first frame update
    void Start()
    {
        // Bakes the mesh based on the, in the unity editor defined, navigation values
        _mapModelMesh.BuildNavMesh();
    }

    // No update method needen because NavMesh does all the calulcations in the background
    void Update(){}

    /** 
        Sets the destination of the NavMesh agent and updates the local variable _destination that 
        stores more information than the position
    */
    public void UpdateDestination(Room destination)
    {
        _destination = destination;
        _userPosition.SetDestination(destination.transform.position);
    }

    /**
        NavMesh constantly updates the path based on the user and destination position.
        The path that was updated during the current frame can be accessed here
     */
    public Vector3[] GetPath()
    {
        return _userPosition.path.corners;
    }

}
