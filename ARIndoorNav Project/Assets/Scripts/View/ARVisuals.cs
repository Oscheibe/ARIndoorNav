using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARVisuals : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //TODO
    public void SendNaviagtionPath(Vector3[] path)
    {
        Debug.Log("Path:");
        foreach (var corner in path)
        {
            Debug.Log(corner);
        }
    }
}
