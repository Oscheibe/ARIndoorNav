using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerDetectionUI : MonoBehaviour
{
    public SystemStatePresenter _SystemStatePresenter;

    public GameObject _loadingAnimationGO;
    public float _loadingTime = 1.0f;

    public void ShowLoadingAnimation()
    {
        _loadingAnimationGO.SetActive(true);
        Invoke("HideLoadingAnimation", _loadingTime);
    }

    private void HideLoadingAnimation()
    {
        _loadingAnimationGO.SetActive(false);
    }

    /**
     * Called by a button
     * Method to start the marker detection process
     */
    public void StartMarkerDetection()
    {
        _SystemStatePresenter.ConfirmMarkerTracking();
    }


}
