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
    private bool isPaused = false;

    private int stairsMask = 8;
    private int elevatorMask = 16;

    // Sends periodic updates of the current navigation state
    void Update()
    {
        if (destination == null || _NavMeshAgent == null || isPaused == true) return;
        var currentDistance = GetDistanceToUser(destination);
        _NavigationPresenter.UpdateNavigationInformation(_NavMeshAgent.remainingDistance, GetPath());

        if (currentDistance < _goalReachedDistance)
        {

            StopNavigation();
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
        var floorTransform = GetFloorTransform(destination.Floor);
        if (floorTransform == null)
        {
            Debug.Log("NO FLOOR FOUND AT ROOM: " + destination.Name);
        }
        destinationPos = new Vector3(destination.Location.position.x, floorTransform.position.y, destination.Location.position.z);

        _NavMeshAgent.SetDestination(destinationPos);
        _NavigationPresenter.DisplayNavigationInformation(this.destination.Name, GetDistanceToUser(destination), GetPath());
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
        _NavigationPresenter.DisplayNavigationInformation(destination.Name, GetDistanceToUser(destination), GetPath());
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
        Returns the distance between the user and the destination coordinate with its Y value on the floor
    */
    public float GetDistanceToUser(Room room)
    {
        var floorY = GetFloorTransform(room.Floor).position.y;
        var roomPosition = room.Location.position;
        var destinationPosition = new Vector3(room.Location.position.x, floorY, room.Location.position.z);

        return Vector3.Distance(_PoseEstimation.GetUserPosition(), destinationPosition);
    }

    private float CalculateDistance(Transform startPosition, Transform endPosition)
    {
        return Vector3.Distance(startPosition.position, endPosition.position);
    }

    /**
     * Sets destination to null which ends all destination related calculations.
     * Used when the user has reached their goal
     */
    public void StopNavigation()
    {
        _NavMeshAgent.ResetPath();
        //_NavMeshAgent.
        _NavigationPresenter.ResetPathDisplay();
        _NavigationPresenter.ReachedDestination();
        destination = null;
    }

    /** 
     * only paused the navigation calculations
     * the current destination is not lost, but the user needs to reposition themselves
     */
    private void PauseNavigation()
    {
        isPaused = true;
        _NavigationPresenter.ClearPathDisplay();
        //_NavMeshAgent.isStopped = true;
        //_NavMeshAgent.ActivateCurrentOffMeshLink(false);
        _NavMeshAgent.enabled = false;
        // Deactivate agent
    }

    /** 
     * only paused the navigation calculations
     * the current destination is not lost, but the user needs to reposition themselves
     */
    public void ContinueNavigation()
    {
        isPaused = false;
        _NavMeshAgent.enabled = true;
        UpdateDestination();
        // Reset Destination
        // Activate agent
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
        if (currentMask == stairsMask)
        {

            PauseNavigation();
            _PoseEstimation.RequestNewPosition(PoseEstimation.NewPosReason.EnteredStairs);
        }
        else if (currentMask == elevatorMask)
        {
            PauseNavigation();
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

    /**
     * returns the floor transform of floor 0 to 3
     */
    private Transform GetFloorTransform(int floorNumber)
    {
        if (floorNumber < 0 || floorNumber > 3)
            return null;

        // No breaks needed when return is called
        switch (floorNumber)
        {
            case 0:
                return _Floor0;

            case 1:
                return _Floor1;

            case 2:
                return _Floor2;

            case 3:
                return _Floor3;

            default:
                return null;
        }

    }
}
