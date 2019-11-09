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
    public NavMeshAgent _NavMeshAgent;
    public NavMeshSurface _MapModelMesh;
    public NavigationPresenter _NavigationPresenter;

    private Room _destination;

    // Start is called before the first frame update
    void Start()
    {
        // Bakes the mesh based on the, in the unity editor defined, navigation values
        _MapModelMesh.BuildNavMesh();
    }

    // Sends periodic updates of the current navigation state
    void Update()
    {
        if (_destination == null || _NavMeshAgent == null) return;
        _NavigationPresenter.UpdateNavigationInformation(CalculateDistance(), GetPath());
    }

    /** 
        Sets the destination of the NavMesh agent and updates the local variable _destination that 
        stores more information than the position
    */
    public void UpdateDestination(Room destination)
    {
        _destination = destination;
        _NavMeshAgent.SetDestination(destination.Location.position);
        _NavigationPresenter.DisplayNavigationInformation(_destination.Name, CalculateDistance(), GetPath());
    }

    /**
        NavMesh constantly updates the path based on the user and destination position.
        The path that was updated during the current frame can be accessed here
     */
    public Vector3[] GetPath()
    {
        return _NavMeshAgent.path.corners;
    }

    private float CalculateDistance(Transform startPosition, Transform endPosition)
    {
        return Vector3.Distance(startPosition.position, endPosition.position);
    }

    private float CalculateDistance()
    {
        return Vector3.Distance(_NavMeshAgent.transform.position, _destination.Location.position);
    }

}
