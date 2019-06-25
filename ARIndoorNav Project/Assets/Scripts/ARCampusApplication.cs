using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class that gives access to the instance of the application
// this allows every other class to reach each other
public class ARCampusElement : MonoBehaviour
{
    public ARCampusApplication app
    {
        get
        {
            return GameObject.FindObjectOfType<ARCampusApplication>();
        }
    }
}


public class ARCampusApplication : MonoBehaviour
{
    public ARCampusModel model;
    public ARCampusPresenter presenter;
    public ARCampusView view;
    

    // All initiate things here
    void Start()
    {

    }
}
