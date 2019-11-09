using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrimDimmer : MonoBehaviour
{
    public GameObject scrim;

    private Vector3 startPosition;
    private Vector3 lastPosition;
    private float startAlpha;
    
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        startAlpha = scrim.GetComponent<Image>().color.a;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(transform.position != startPosition && lastPosition != transform.position)
        {
            DimScrim(startPosition.x - transform.position.x);
            lastPosition = transform.position;
        }
        
    }

    private void DimScrim(float difference)
    {
        var scrimColor = scrim.GetComponent<Image>().color;
        var newAlpha = startAlpha * (1- Mathf.Abs(difference)/930);
        
        scrim.GetComponent<Image>().color = new Color(scrimColor.r, scrimColor.g, scrimColor.b, newAlpha );
    }
}
