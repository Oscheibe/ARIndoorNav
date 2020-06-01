using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ARVisuals_Avatar : MonoBehaviour, IARVisuals
{
    public NavigationPresenter _NavigationPresenter;
    public GameObject _Guide;
    public float avatarDistance = 4; // in meters in front of the user


    private NavMeshAgent _GuideNavMeshAgent;
    private RobotInterface _RobotInterface;

    private float nextActionTime = 0.0f;
    private float period = 5f;
    private bool countingDown = false;

    // The end position of the path. 
    private Vector3 currentDestinationPos;
    // Unlike the destination, target is not the end point of the path. It is a point betwen the user and the next corner
    private Vector3 targetPosition;
    // User position so that the jumping avatar can look at them
    private Vector3 userPosition;


    // Start is called before the first frame update
    void Start()
    {
        _GuideNavMeshAgent = _Guide.GetComponent<NavMeshAgent>();
        _RobotInterface = _Guide.GetComponent<RobotInterface>();
    }

    // Update is called once per frame
    void Update()
    {
        // Skip the update if the guide is inactive
        if (_Guide.activeSelf == false)
            return;
        else
            CheckTargetDistance();
    }


    public void ClearARDisplay()
    {
        _Guide.SetActive(false);
    }

    /**
     * Called each update from NavigationPresenter when Navigation updates its information
     * This function will: 
     * (0) Start the navigation new from the user position on when the destination changes
     * 1 Calculate a new position for the avatar to walk towards
     * 2 Give the new position to the NavMesh Agent that will walk there itself 
     */
    public void SendNavigationInformation(NavigationInformation navigationInformation)
    {
        // Only start calculations when the original path is finished
        if (navigationInformation.HasPath() == false)
            return;

        // The AR visuals will only be updated when they are needed again
        _Guide.SetActive(true);

        // Check if the destination has changed
        if (currentDestinationPos != navigationInformation.GetDestinationPos())
        {
            // Teleport the guide to the user
            _Guide.transform.position = navigationInformation.GetCurrentUserPos();
            _GuideNavMeshAgent.Warp(_Guide.transform.position);
            currentDestinationPos = navigationInformation.GetDestinationPos();
        }

        // Calculate and save new target position based on user position
        targetPosition = CalculateTargetPos(navigationInformation.GetCurrentUserPos(), navigationInformation.GetNextCorner());

        userPosition = navigationInformation.GetCurrentUserPos();

        // Set the target position of the nav mesh agent that will now attempt to walk there
        _GuideNavMeshAgent.SetDestination(targetPosition);
    }

    private void CheckTargetDistance()
    {
        if (Vector3.Distance(_Guide.transform.position, targetPosition) <= 0.2)
        {
            _GuideNavMeshAgent.ResetPath();

            // Determine the avatar animation based on time waited
            if (CountDown())
                _RobotInterface.TriggerIdleAnimation();

            else
            {
                //_RobotInterface.LookAtPos(userPosition);
                _RobotInterface.TriggerJumpingAnimation();
            }

        }
        else
        {
            _RobotInterface.TriggerWalkingAnimation();
            ResetCountDown();
        }
    }

    /** 
     * Calculate the a point in a distance from the user towards the next corner
     * This will allow the avatar to always walk 4 meters in front of the user
     * If the next corner is too close, the avatar will wait there for the user 
     */
    private Vector3 CalculateTargetPos(Vector3 userPos, Vector3 nextCorner)
    {
        if(Vector3.Distance(userPos, nextCorner) <= avatarDistance)
        {
            return nextCorner;
        }

        Quaternion targetAngle = Quaternion.LookRotation(nextCorner - userPos);
        var unitVectorForward = targetAngle * Vector3.forward * avatarDistance;
        var resultVector = userPos + unitVectorForward;
        return resultVector;
    }

    private bool CountDown()
    {
        if (countingDown && Time.time < nextActionTime)
        {
            return true;
        }
        else
        {
            countingDown = false;
            return false;
        }
    }

    private void ResetCountDown()
    {
        countingDown = true;
        nextActionTime = Time.time + period;
    }

}
