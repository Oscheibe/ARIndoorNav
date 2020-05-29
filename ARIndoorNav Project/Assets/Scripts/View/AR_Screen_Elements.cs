using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AR_Screen_Elements : MonoBehaviour
{

    public GameObject _2DArrow;
    public Canvas _Canvas;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HideDirectionIndicator()
    {
        _2DArrow.SetActive(false);
    }

    private void ShowDirectionIndicator()
    {
        _2DArrow.SetActive(true);
    }

    public void IndicateDirection(NavigationInformation navigationInformation)
    {
        var arrowPos = navigationInformation.Vector3ToScreenPos(); //Vector3ToScreenPos(navigationInformation.GetNextCorner(), navigationInformation.GetARCamera());
        // Hide the arrow if the user is looking in the right direction 
        if(navigationInformation.IsVector3InView()) HideDirectionIndicator();
        else ShowDirectionIndicator();

        _2DArrow.transform.position = new Vector3(arrowPos.x, arrowPos.y, 0); ;
    }

    private Vector2 Vector3ToScreenPos(Vector3 nextCorner, Camera ARCamera)
    {
        Vector2 arrowPos;

        // Source: https://answers.unity.com/questions/1037969/arrows-pointing-to-offscreen-enemy.html
        var screenPos = ARCamera.WorldToViewportPoint(nextCorner);
        float x = 0;
        float y = 0;

        if (screenPos.x >= 0 && screenPos.x <= 1 && screenPos.y >= 0 && screenPos.y <= 1 && screenPos.z >= 0)
        {
            var screenPointUnscaled = ARCamera.WorldToScreenPoint(nextCorner);
            x = screenPointUnscaled.x / _Canvas.scaleFactor;
            y = screenPointUnscaled.y / _Canvas.scaleFactor;

            HideDirectionIndicator();
            return new Vector2(x, y);
        }

        ShowDirectionIndicator();

        var onScreenPos = new Vector2(screenPos.x - 0.5f, screenPos.y - 0.5f) * 2;
        var max = Mathf.Max(Mathf.Abs(onScreenPos.x), Mathf.Abs(onScreenPos.y));
        onScreenPos = (onScreenPos / (max * 2)) + new Vector2(0.5f, 0.5f);



        /**
         * This part of the code is to counteract a bug found within the app
         * The bug was that after a 90° turn away from the next corner of the path (nextCorner) 
         * the x and y axis would flip their respective values. (For example, if the corner is on the left side
         * the 2D arrow would jump from the left side of the screen to the right)
         * The y axis (may) still not be fixed. 
         */
        // Looking away from corner

        if (screenPos.z < Camera.main.nearClipPlane)
        {
            // Right side
            if (screenPos.x >= 0)
                x = 0;
            // Left side
            else
                x = Screen.width;
            y = onScreenPos.y * Screen.height;
        }
        // Looking towards corner
        else
        {
            x = onScreenPos.x * Screen.width;
            y = onScreenPos.y * Screen.height;
        }

        arrowPos = new Vector3(x, y, 0);


        return arrowPos;
    }
}
