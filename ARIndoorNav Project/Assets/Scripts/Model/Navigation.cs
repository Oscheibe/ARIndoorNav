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
    public NavigationPresenter _NavigationPresenter;
    public PoseEstimation _PoseEstimation;
    public ModelDatabase _ModelDatabase;

    public float _goalReachedDistance = 8.0f; // In meters

    private Room destination;
    private Vector3 destinationPos; // Destination position with a y value of the _GroundFloor
    private NavigationInformation navigationInformation;
    private int destinationFloor = -1;
    private bool isPaused = false;

    private int stairsMask = 8;
    private int elevatorMask = 16;

    void Start()
    {
        navigationInformation = new NavigationInformation();
    }

    // Sends periodic updates of the current navigation state
    void Update()
    {
        if (destination == null || _NavMeshAgent == null || isPaused == true) return;

        var nextFloorPath = GetPathToNextFloor();
        var currentDistance = GetPathDistance(GetTotalPath());

        navigationInformation.UpdateNavigationInformation(nextFloorPath, _NavMeshAgent.nextPosition);
        _NavigationPresenter.UpdateNavigationInformation(navigationInformation);

        /**
         * Due to a bug, the first frame of changing the destination will no calculate the correct distance
         * NavMesh will calculate a path after the first update, which means that the first update will result in a 0 distance
         * Using the unity distance will avoid that behavior
         */
        if (GetUnityDistanceToUser(destination) < _goalReachedDistance)
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
        destinationFloor = destination.Floor;
        // Setting the destination height to the ground level
        var floorTransform = _ModelDatabase.GetFloorTransform(destination.Floor);
        if (floorTransform == null)
        {
            Debug.Log("NO FLOOR FOUND AT ROOM: " + destination.Name);
        }
        destinationPos = new Vector3(destination.Location.position.x, floorTransform.position.y, destination.Location.position.z);

        _NavMeshAgent.SetDestination(destinationPos);
        var nextFloorPath = GetPathToNextFloor();
        var totalDistance = GetPathDistance(GetTotalPath());

        navigationInformation.UpdateNavigationInformation(nextFloorPath, _NavMeshAgent.nextPosition);
        navigationInformation.SetDestinationName(destination.Name);
        
        _NavigationPresenter.DisplayNavigationInformation(navigationInformation);
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
        var nextFloorPath = GetPathToNextFloor();
        var totalDistance = GetPathDistance(GetTotalPath());

        navigationInformation.UpdateNavigationInformation(nextFloorPath, _NavMeshAgent.nextPosition);
        _NavigationPresenter.DisplayNavigationInformation(navigationInformation);
    }

    /**
        Warps the NavMesh agent to the Vector3 position
        Returns true if successfull.
        This Method is used after the PoseEstimation has updated the user position to warp
        an eventually stuck NavMesh agent out of its stuck position
    */
    public bool ReportUserJump(Vector3 warpPosition)
    {
        var hasWarped = _NavMeshAgent.Warp(warpPosition);
        UpdateDestination();
        return hasWarped;
    }

    /**
        NavMesh constantly updates the path based on the user and destination position.
        The path that was updated during the current frame can be accessed here
     */
    private Vector3[] GetTotalPath()
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
    public float GetUnityDistanceToUser(Room room)
    {
        var floorY = _ModelDatabase.GetFloorTransform(room.Floor).position.y;
        var roomPosition = room.Location.position;
        var destinationPosition = new Vector3(room.Location.position.x, floorY, room.Location.position.z);

        return Vector3.Distance(_PoseEstimation.GetUserPosition(), destinationPosition);
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

        if (_PoseEstimation.GetCurrentFloor() == destinationFloor)
        {
            return;
        }
        // Check if the navmesh agent is on the mesh
        if (!_NavMeshAgent.SamplePathPosition(NavMesh.AllAreas, 0f, out navMeshHit))
        {
            currentMask = navMeshHit.mask;
        }
        // Check if the user is on stairs
        if (currentMask == stairsMask)
        {
            PauseNavigation();
            var currentFloor = _PoseEstimation.GetCurrentFloor();
            _NavigationPresenter.SendObstacleMessage(currentFloor, destinationFloor, PoseEstimation.NewPosReason.EnteredStairs);
            _PoseEstimation.RequestNewPosition(PoseEstimation.NewPosReason.EnteredStairs);
        }
        // Check if the user is on the elevator
        else if (currentMask == elevatorMask)
        {
            PauseNavigation();
            var currentFloor = _PoseEstimation.GetCurrentFloor();
            _NavigationPresenter.SendObstacleMessage(currentFloor, destinationFloor, PoseEstimation.NewPosReason.EnteredElevator);
            _PoseEstimation.RequestNewPosition(PoseEstimation.NewPosReason.EnteredElevator);
        }
    }

    /**
     * returns the floor number of the last current destination
     */
    public int GetDestinationFloor()
    {
        return destinationFloor;
    }

    /**
     * Calculates the distance between all the points on a path
     */
    private float GetPathDistance(Vector3[] path)
    {
        float result = 0;
        // Iterates over each path corner expect the last and 
        // calculates the distance between each one
        for (int i = 0; i < path.Length - 1; i++)
        {
            result += Vector3.Distance(path[i], path[i + 1]);
        }

        return result;
    }

    /**
     * Returns the path to the next floor-changing corner
     * This method allows for navigation to stairs or elevators without displaying the path beyond them
     * and cleaning up visual clutter this way
     */
    private Vector3[] GetPathToNextFloor()
    {
        var totalPath = GetTotalPath();
        List<Vector3> pathToFloor = new List<Vector3>();
        Vector3 lastCorner = totalPath[0];
        foreach (var corner in totalPath)
        {
            if (corner.y != lastCorner.y)
                break;

            pathToFloor.Add(corner);
            lastCorner = corner;
        }
        return pathToFloor.ToArray();
    }
}
