using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ARVisuals : MonoBehaviour
{
    public GameObject _ARDotTemplate;
    public LineRenderer _Line;

    private GameObject arDotGameeObject;

    // Start is called before the first frame update
    void Start()
    {
        GenARDot();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //TODO
    public void SendNaviagtionPath(Vector3[] path)
    {
        ChangeARDotPos(path[path.Length -1]);
        DrawPath(path);
        
    }

    private void GenARDot()
    {
        arDotGameeObject = Instantiate(_ARDotTemplate) as GameObject;
        arDotGameeObject.SetActive(false);
        //button.GetComponent<RoomButton>().InitializeButton(room, "Distance TBD");
        //button.name = room.Name;
        //button.transform.SetParent(_buttonTemplate.transform.parent, false); // set false so that button doesn't position themselves in worldspace. Makes it more dynamic
        //buttonGameobjects.Add(button);   
    }

    private void ChangeARDotPos(Vector3 position)
    {
        arDotGameeObject.SetActive(true);
        arDotGameeObject.transform.position = position;
    }

    private void DrawPath(Vector3[] path)
    {
        if(path.Length < 2 ) return;
        _Line.positionCount = path.Length;
        for(int i = 0; i < path.Length; i++)
        {
            _Line.SetPosition(i, path[i]);
        }
    }
}
