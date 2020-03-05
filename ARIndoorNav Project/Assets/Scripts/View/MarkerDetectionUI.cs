using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerDetectionUI : MonoBehaviour
{
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


}
