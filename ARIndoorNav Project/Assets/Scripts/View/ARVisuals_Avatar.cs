using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ARVisuals_Avatar : MonoBehaviour, IARVisuals
{
    public NavigationPresenter _NavigationPresenter;
    public GameObject _Guide;
    private NavMeshAgent _GuideNavMeshAgent;
    private RobotInterface _RobotInterface;

    // The end position of the path. 
    private Vector3 currentDestinationPos;
    // Unlike the destination, target is not the end point of the path. It is a point betwen the user and the next corner
    private Vector3 targetPosition;


    // Start is called before the first frame update
    void Start()
    {
        _GuideNavMeshAgent = _Guide.GetComponent<NavMeshAgent>();
        _RobotInterface = _Guide.GetComponent<RobotInterface>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckTargetDistance();

        Debug.Log(Vector3.Distance(_Guide.transform.position, targetPosition));
    }


    public void ClearARDisplay()
    {
        throw new System.NotImplementedException();
    }

    public void SendNavigationInformation(NavigationInformation navigationInformation)
    {
        _GuideNavMeshAgent.SetDestination(navigationInformation.GetDestinationPos());
        if (currentDestinationPos != navigationInformation.GetDestinationPos())
        {
            _RobotInterface.TriggerWalkingAnimation();
        }
        currentDestinationPos = navigationInformation.GetDestinationPos();
        SetNewTarget(navigationInformation.GetNextCorner());
    }

    private void CheckTargetDistance()
    {
        if (Vector3.Distance(_Guide.transform.position, targetPosition) <= 0.2)
        {
            _RobotInterface.TriggerIdleAnimation();
        }
    }

    private void SetNewTarget(Vector3 nextCorner)
    {
        targetPosition = currentDestinationPos;
    }
}
