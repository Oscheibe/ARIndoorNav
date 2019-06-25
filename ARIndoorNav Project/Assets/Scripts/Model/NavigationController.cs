using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using GoogleARCore;

public class NavigationController : MonoBehaviour
{

    public Transform _destination;
    public NavMeshSurface surface;
    public NavMeshAgent _navMeshAgent;
    public GameObject cornerObject;
    public float cornerIndictaorScale = 1f;
    public float rotationSpeed = 3f;

    //private DetectedPlane floorPlane;
    private GameObject cornerObjectInstance = null;
    private readonly List<DetectedPlane> _detectedPlanes = new List<DetectedPlane>();

    void Awake()
    {
        if (_navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent component is not atached to " + gameObject.name);
            throw new MissingComponentException();
        }
        else
        {
            //SetDestination(_destination.transform.position);
        }
    }

    /* The NavMesh path is made out of a list of corner positions.
     * The 1st corner is the agent itself, the 2nd is the next corner.
     * This 2nd corner is used to show display the way towards the destination.
     */
    public void DrawNextPathCorner(List<DetectedPlane> detectedPlanes)
    {
        var floorPlane = CalculateFloorPlane(detectedPlanes);
        var floorHeight = CalculateFloorHeight(floorPlane);

        Vector3 currentCorner, nextCorner;
        if (cornerObjectInstance == null)
        {
            Debug.Log("Creating Corner GameObject in: " + gameObject.name);
            cornerObjectInstance = Instantiate(cornerObject, GetNextCorner(), Quaternion.identity, transform);
            /*
            nextCorner = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            nextCorner.GetComponent<Renderer>().material = cornerMaterial;
            nextCorner.transform.localScale += new Vector3(cornerIndictaorScale, cornerIndictaorScale, cornerIndictaorScale);3
            */
        }
        /* The corner object's position is on the users height on top of the path corner
            The 1st case: A long path. Arrow is on next position and points towards the one after
            The 2nd case: Only one corner left before destination. Arrow points above destination.
            The 3rd case: Destination in front. The arrow points below, towards the destination. 
         */
        if (_navMeshAgent.path.corners.Length > 2)
        {
            // Arrow points towards next corner
            currentCorner = new Vector3(_navMeshAgent.path.corners[1].x, _navMeshAgent.transform.position.y, _navMeshAgent.path.corners[1].z);
            nextCorner = new Vector3(_navMeshAgent.path.corners[2].x, _navMeshAgent.transform.position.y, _navMeshAgent.path.corners[2].z);
        }
        else if (_navMeshAgent.path.corners.Length == 2)
        {
            // Arrow points above destination
            currentCorner = new Vector3(_navMeshAgent.path.corners[1].x, _navMeshAgent.transform.position.y, _navMeshAgent.path.corners[1].z);
            nextCorner = new Vector3(_destination.position.x, _navMeshAgent.transform.position.y, _destination.position.z);
        }
        else
        {
            // Arrow points to destination, which is below
            currentCorner = new Vector3(_destination.position.x, _navMeshAgent.transform.position.y, _destination.position.z);
            nextCorner = _destination.position;
        }
        cornerObjectInstance.transform.position = Vector3.Lerp(cornerObjectInstance.transform.position, currentCorner, Time.smoothDeltaTime * rotationSpeed);
        cornerObjectInstance.transform.LookAt(nextCorner);
        cornerObjectInstance.transform.Rotate(new Vector3(0, 1, 0), -90);
    }

    private float CalculateFloorHeight(DetectedPlane floorPlane)
    {
        // Calculate the distance to the floor to display navigation elements
        float floorHeight;
        if (floorPlane == null)
        {
            floorHeight = -1f; //TODO: Change behaviour from debugging state to dynamic
        }
        else
        {
            floorHeight = floorPlane.CenterPose.position.y;
        }

        return floorHeight;
    }

    private DetectedPlane CalculateFloorPlane(List<DetectedPlane> detectedPlanes)
    {
        // Initializing as null because error handling includes this case.
        // Needs to be optimized in the future
        DetectedPlane floorPlane = null;
        for (int i = 0; i < detectedPlanes.Count; i++)
        {
            if (detectedPlanes[i].PlaneType == DetectedPlaneType.HorizontalUpwardFacing)
            {
                // In case a smaller area, like a table, has been detected in the view,
                // the bigger plane is usually the floor area
                if (GetPlaneArea(detectedPlanes[i]) >= GetPlaneArea(floorPlane))
                {
                    floorPlane = detectedPlanes[i];
                }
            }
        }
        return floorPlane;
    }

    public bool ChangeDestination(string plateNumber)
    {
        var targetPlate = GameObject.Find(plateNumber);
        if (targetPlate == null)
        {
            Debug.Log("Augmented Image with the name: " + plateNumber + " could not be found.");
            return false;
        }
        else 
        {
            SetDestination(targetPlate.transform);
            return true;
        }

    }

    private void SetDestination(Transform targetVector)
    {
        if (targetVector != null)
        {
            _destination = targetVector;
            _navMeshAgent.SetDestination(_destination.position);
        }

    }

    private Vector3[] GetPath()
    {
        return _navMeshAgent.path.corners;
    }

    private Vector3 GetNextCorner()
    {
        if (_navMeshAgent.path.corners.Length > 1)
        {
            return _navMeshAgent.path.corners[1];
        }
        else return _destination.position;
    }


    /* Returns the area of a flat, upward facing, DetectedPlane. 
     * 
     * Source: https://answers.unity.com/questions/684909/how-to-calculate-the-surface-area-of-a-irregular-p.html
     */
    private float GetPlaneArea(DetectedPlane groundPlane)
    {

        if (groundPlane == null || groundPlane.PlaneType != DetectedPlaneType.HorizontalUpwardFacing)
        {
            return 0;
        }

        List<Vector3> list = new List<Vector3>();
        groundPlane.GetBoundaryPolygon(list);

        float area = 0;
        int i = 0;
        for (; i < list.Count; i++)
        {
            if (i != list.Count - 1)
            {
                float mulA = list[i].x * list[i + 1].z;
                float mulB = list[i + 1].x * list[i].z;
                area = area + (mulA - mulB);
            }
            else
            {
                float mulA = list[i].x * list[0].z;
                float mulB = list[0].x * list[i].z;
                area = area + (mulA - mulB);
            }
        }
        area *= 0.5f;
        return Mathf.Abs(area);
    }

    public void BakeMesh()
    {
        surface.BuildNavMesh();
        SetDestination(_destination);
    }

    // Old function to draw the nav mesh path with just a line. Might be useful later
    public void DrawPathLine(float floorHeight)
    {
        var line = this.GetComponent<LineRenderer>();
        if (line == null)
        {
            line = this.gameObject.AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Sprites/Default")) { color = Color.red };
            line.startWidth = 0.3f;
            line.endWidth = 0.2f;
            line.startColor = new Color(1, 0, 0, 0.5f);
            line.endColor = new Color(1, 0, 0, 0.5f);
        }
        line.SetPosition(0, new Vector3(_navMeshAgent.nextPosition.x, floorHeight, _navMeshAgent.nextPosition.z));
        line.SetPosition(1, GetNextCorner());
    }
}
