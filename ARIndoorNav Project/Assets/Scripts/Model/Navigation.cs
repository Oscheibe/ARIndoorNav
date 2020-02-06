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
    public PoseEstimation _PoseEstimation;

    public Transform _Floor3;
    public Transform _Floor2;
    public Transform _Floor1;
    public Transform _Floor0;
    
    public float _goalReachedDistance = 1.0f; // In meters

    private Room destination;
    private Vector3 destinationPos; // Destination position with a y value of the _GroundFloor
    private int lastDestinationFloor = -1;

    private int stairsMask = 8;
    private int elevatorMask = 16;

    // Sends periodic updates of the current navigation state
    void Update()
    {
        if (destination == null || _NavMeshAgent == null) return;
        var currentDistance = CalculateDistance();
        _NavigationPresenter.UpdateNavigationInformation(currentDistance, GetPath());
        if (currentDistance < _goalReachedDistance)
        {

            StopNavigation();
            _NavigationPresenter.ReachedDestination();
        }
        ProcessCurrentArea();
    }

    /** 
        Sets the destination of the NavMesh agent and updates the local variable _destination that 
        stores more information than the position
    */
    public void UpdateDestination(Room destination)
    {
        this.destination = destination;
        lastDestinationFloor = destination.Floor;
        // Setting the destination height to the ground level
        destinationPos = new Vector3(destination.Location.position.x, _Floor3.position.y, destination.Location.position.z);

        _NavMeshAgent.SetDestination(destinationPos);
        _NavigationPresenter.DisplayNavigationInformation(this.destination.Name, CalculateDistance(), GetPath());
    }

    /**
     * Used to update the destination of the Agent after a warp
     */
    private void UpdateDestination()
    {
        if (destination == null)
        {
            Debug.Log("Updating Destination impossible: no destination set");
            return;
        }
        _NavMeshAgent.SetDestination(destinationPos);
        _NavigationPresenter.DisplayNavigationInformation(destination.Name, CalculateDistance(), GetPath());
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
        return Vector3.Distance(_NavMeshAgent.transform.position, destination.Location.position);
    }

    /**
     * Sets destination to null which ends all destination related calculations.
     * Used when the user has reached their goal
     */
    public void StopNavigation()
    {
        _NavMeshAgent.ResetPath();
        _NavigationPresenter.ClearPathDisplay();
        destination = null;
    }

    /**
     * Only returns a value when a path is set. The NavMesh mask is a bitset representing the current area
     * 8 = Stairs / 16 = Elevator
     */
    private void ProcessCurrentArea()
    {
        NavMeshHit navMeshHit;
        int currentMask = -1;
        if (!_NavMeshAgent.SamplePathPosition(NavMesh.AllAreas, 0f, out navMeshHit))
        {
            currentMask = navMeshHit.mask;
        }
        if(currentMask == stairsMask)
        {

            StopNavigation();
            _PoseEstimation.RequestNewPosition(PoseEstimation.NewPosReason.EnteredStairs);
        }
        else if (currentMask == elevatorMask)
        {
            StopNavigation();
            _PoseEstimation.RequestNewPosition(PoseEstimation.NewPosReason.EnteredElevator);
        }
    }

    /**
     * returns the floor number of the last current destination
     */
    public int GetDestinationFloor()
    {
        return lastDestinationFloor;
    }
}
