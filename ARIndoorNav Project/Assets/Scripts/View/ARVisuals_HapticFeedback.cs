using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARVisuals_HapticFeedback : MonoBehaviour, IARVisuals
{
    public NavigationPresenter _NavigationPresenter;


    public void ClearARDisplay()
    {
        throw new System.NotImplementedException();
    }

    public void SendNavigationInformation(NavigationInformation navigationInformation)
    {
        Vector2 screen = navigationInformation.Vector3ToScreenPos();
        var tmpx = screen.x;
        var tmpy = screen.y;

        float middleX = HelperFunctions.BetweenZeroAndOne(tmpx, Screen.width);
        float middleY = HelperFunctions.BetweenZeroAndOne(tmpy, Screen.height);

        Debug.Log("x:" + middleX + " y:" + middleY);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
