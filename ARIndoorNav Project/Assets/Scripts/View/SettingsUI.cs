using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsUI : MonoBehaviour
{
    public SystemStatePresenter _SystemStatePresenter;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartMarkerDetection()
    {
        _SystemStatePresenter.RequestMarkerTracking();
    }
}
