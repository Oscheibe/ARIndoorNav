using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARVisuals_WIM : MonoBehaviour, IARVisuals
{
    public NavigationPresenter _NavigationPresenter;
    public Camera _WIMCamera;
    public Camera _ARCamera;
    public GameObject _WIMPlane;
    public GameObject _WIMImage;
    public GameObject _CameraPos;
    public GameObject _CornerMarker;
    
    public float rotSpeed = 15;

    public void ClearARDisplay()
    {
        _WIMCamera.enabled = false;
        _WIMPlane.SetActive(false);
        _WIMImage.SetActive(false);
        _CornerMarker.SetActive(false);
    }

    public void SendNavigationInformation(NavigationInformation navigationInformation)
    {
        // Only start calculations when the original path is finished
        if (navigationInformation.HasPath() == false)
            return;
        _WIMCamera.enabled = true;
        _WIMPlane.SetActive(true);
        _WIMImage.SetActive(true);
        _CornerMarker.SetActive(true);
        
        _CornerMarker.transform.position = navigationInformation.GetNextCorner() + new Vector3(0,3,0); // make the corner marker 3 units higher
        RotatePlaneTo(_CameraPos.transform.position);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //_WIMCamera.transform.rotation = _ARCamera.transform.rotation;
        RotatePlaneTo(_CameraPos.transform.position);
    }

    private void RotatePlaneTo(Vector3 lookAtDir)
    {
        //var playerRot = Quaternion.LookRotation(lookAtDir);
        //_WIMPlane.transform.rotation = Quaternion.Slerp(_WIMPlane.transform.rotation, playerRot, rotSpeed * Time.deltaTime);
        _WIMPlane.transform.LookAt(lookAtDir);
    }
}
