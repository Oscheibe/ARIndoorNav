using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationController : MonoBehaviour
{
    [SerializeField]
    public Transform _destination;

    public NavMeshAgent _navMeshAgent;
    public GameObject cornerObject;
    public float cornerIndictaorScale = 1f;
    public float rotationSpeed = 3f;

    private GameObject cornerObjectInstance = null;

    void Awake()
    {
        //_navMeshAgent = this.GetComponent<NavMeshAgent>();
        if (_navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent component is not atached to " + gameObject.name);
        }
        else
        {
            SetDestination();
        }

        
    }

    private void Update()
    {

    }


    public void DrawPathLine(float floorHeight)
    {   
        var line = this.GetComponent<LineRenderer>();
        if( line == null )
        {
            line = this.gameObject.AddComponent<LineRenderer>();
            line.material = new Material( Shader.Find( "Sprites/Default" ) ) { color = Color.red };
            line.startWidth = 0.3f;
            line.endWidth = 0.2f;
            line.startColor =  new Color(1,0,0, 0.5f);
            line.endColor = new Color(1,0,0, 0.5f);
        }
        
        line.SetPosition(0, new Vector3(_navMeshAgent.nextPosition.x, floorHeight, _navMeshAgent.nextPosition.z));
        line.SetPosition(1, GetNextCorner());

        //Debug.DrawLine(_navMeshAgent.nextPosition, GetNextCorner(), Color.red);
    }

    /* The NavMesh path is made out of a list of corner positions.
     * The 1st corner is the agent itself, the 2nd is the next corner.
     * This 2nd corner is used to show display the way towards the destination.
     * 
     */
    public void DrawNextPathCorner(float floorHeight)
    {
        Vector3 currentCorner, nextCorner;
        if(cornerObjectInstance == null)
        {
            Debug.Log("Creating Corner GameObject in: "+ gameObject.name);
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
            currentCorner = new Vector3(_navMeshAgent.path.corners[1].x, _navMeshAgent.transform.position.y, _navMeshAgent.path.corners[1].z);
            nextCorner = new Vector3(_navMeshAgent.path.corners[2].x, _navMeshAgent.transform.position.y, _navMeshAgent.path.corners[2].z);
        }
        else if (_navMeshAgent.path.corners.Length == 2){
            currentCorner = new Vector3(_navMeshAgent.path.corners[1].x, _navMeshAgent.transform.position.y, _navMeshAgent.path.corners[1].z);
            nextCorner = new Vector3(_destination.position.x, _navMeshAgent.transform.position.y, _destination.position.z);
        } 
        else
        {
            currentCorner = new Vector3(_destination.position.x, _navMeshAgent.transform.position.y, _destination.position.z);
            nextCorner = _destination.position;
        }


        cornerObjectInstance.transform.position =  Vector3.Lerp(cornerObjectInstance.transform.position, currentCorner, Time.smoothDeltaTime * rotationSpeed);
        cornerObjectInstance.transform.LookAt(nextCorner);
        cornerObjectInstance.transform.Rotate(new Vector3(0,1,0), -90);
    }

    private void SetDestination()
    {
        if (_destination != null)
        {
            Vector3 targetVector = _destination.transform.position;
            _navMeshAgent.SetDestination(targetVector);
        }

    }

    public Vector3[] GetPath()
    {
        return _navMeshAgent.path.corners;
    }

    public Vector3 GetNextCorner()
    {
        if (_navMeshAgent.path.corners.Length > 1)
        {
            return _navMeshAgent.path.corners[1];
        }
        else return _destination.position;
    }


}
