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

    // Sends periodic updates of the current navigation state
    void Update()
    {
        if (_destination == null || _NavMeshAgent == null) return;
        _NavigationPresenter.UpdateNavigationInformation(CalculateDistance().ToString(), GetPath());
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
     * Used to update the destination of the Agent after a warp
     */
    private void UpdateDestination()
    {
        if(_destination == null)
        {
            Debug.Log("Updating Destination impossible: no destination set");
            return;
        }
        _NavMeshAgent.SetDestination(_destination.Location.position);
        _NavigationPresenter.DisplayNavigationInformation(_destination.Name, CalculateDistance(), GetPath());
    }

    /**
        Warps the NavMesh agent to the Vector3 position
        Returns true if successfull.
        This Method is used after the PoseEstimation has updated to user position to warp
        an eventually stuck NavMesh agent out of its stuck position
    */
    public bool WarpNavMeshAgent(Vector3 warpPosition)
    {
        var hasWarped = _NavMeshAgent.Warp(warpPosition);
        UpdateDestination();
        return hasWarped;
    }

    /**
        NavMesh constantly updates the path based on the user and destination position.
        The path that was updated during the current frame can be accessed here
     */
    private Vector3[] GetPath()
    {
        return _NavMeshAgent.path.corners;
    }

    /**
     * Returns the Vector3 of the next corner in the path.
     * If the corner count == 1, then the next corner is the destination
     */
    public Vector3 GetNextCorner()
    {
        return _NavMeshAgent.path.corners[0];
    }

    /**
        Returns the distance between the users next simulated position and the origin vector
    */
    public string GetDistanceToUser(Vector3 origin)
    {
        return Vector3.Distance(_NavMeshAgent.nextPosition, origin).ToString();
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
