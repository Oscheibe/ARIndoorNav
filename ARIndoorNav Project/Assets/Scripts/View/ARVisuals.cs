using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ARVisuals : MonoBehaviour
{
    public GameObject _ARDotTemplate;
    public LineRenderer _Line;

    private GameObject arDotGameObject;

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
    public void SendNavigationPath(Vector3[] path)
    {
        ChangeARDotPos(path[path.Length -1]);
        DrawPath(path);
        
    }

    private void GenARDot()
    {
        arDotGameObject = Instantiate(_ARDotTemplate) as GameObject;
        arDotGameObject.SetActive(false);
        //button.GetComponent<RoomButton>().InitializeButton(room, "Distance TBD");
        //button.name = room.Name;
        //button.transform.SetParent(_buttonTemplate.transform.parent, false); // set false so that button doesn't position themselves in worldspace. Makes it more dynamic
        //buttonGameobjects.Add(button);   
    }

    private void ChangeARDotPos(Vector3 position)
    {
        arDotGameObject.SetActive(true);
        arDotGameObject.transform.position = position;
    }

    private void DrawPath(Vector3[] path)
    {
        if(path.Length < 2 ) return;
        _Line.positionCount = path.Length;
        for(int i = 0; i < path.Length; i++)
        {
            _Line.SetPosition(i, path[i]);
        }

        //_Line.sortingLayerName = "Foreground";
    }
}
