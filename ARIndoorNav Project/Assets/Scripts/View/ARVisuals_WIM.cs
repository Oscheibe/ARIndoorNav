using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARVisuals_WIM : MonoBehaviour, IARVisuals
{
    public NavigationPresenter _NavigationPresenter;
    public Camera _WIMCamera;
    public Camera _ARCamera;

    public void ClearARDisplay()
    {
        _WIMCamera.enabled = false;
    }

    public void SendNavigationInformation(NavigationInformation navigationInformation)
    {
        // Only start calculations when the original path is finished
        if (navigationInformation.HasPath() == false)
            return;
        _WIMCamera.enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _WIMCamera.transform.rotation = _ARCamera.transform.rotation;
    }
}
