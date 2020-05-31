using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ARVisuals_ArrowLine : MonoBehaviour, IARVisuals
{
    public NavigationPresenter _NavigationPresenter;
    public AR_Screen_Elements _AR_Screen_Elements;

    public LineRenderer _Line;

    /**
     * Doesn't Work. Might get it to work later. This is a reminder
     */
    Gradient gradient;
    GradientColorKey[] colorKey;
    GradientAlphaKey[] alphaKey;

    void Start()
    {

    }

    void Update()
    {
        _Line.material.SetTextureOffset("_MainTex", Vector2.left * Time.time);

    }


    /**
     * Gets called each update by NavigationPresenter
     */
    public void SendNavigationInformation(NavigationInformation navigationInformation)
    {
        // Only start calculations when the original path is finished
        if (navigationInformation.HasPath() == false)
            return;
        
        Vector3[] path = navigationInformation.GetPath();
        DrawPath(path);
        _AR_Screen_Elements.IndicateDirection(navigationInformation);
    }


    /**
     * Colors the navigation line red if the tracking accuracy reached 0%
     */
    public void IndicateTrackingAccuracy(int percentage)
    {
        //TODO
    }

    /**
     * Doesn't work
     */
    private void SetLineGradient()
    {

        gradient = new Gradient();

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        colorKey = new GradientColorKey[2];
        colorKey[0].color = Color.red;
        colorKey[0].time = 0.0f;
        colorKey[1].color = Color.blue;
        colorKey[1].time = 1.0f;
        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 0.0f;
        alphaKey[1].time = 1.0f;

        gradient.SetKeys(colorKey, alphaKey);
        _Line.colorGradient = gradient;
    }

    /**
    * Method to adjust the user rotation manually
    */
    public void RotateRight()
    {
        _NavigationPresenter.RotatePathToRight();
    }

    /**
     * Method to adjust the user rotation manually
     */
    public void RotateLeft()
    {
        _NavigationPresenter.RotatePathToLeft();
    }




    /**
     * Clears current ARDisplay GameObjects
     * Will be overritten by the next SendNavigationPath() call
     */
    public void ClearARDisplay()
    {
        _Line.positionCount = 0;
        _AR_Screen_Elements.HideDirectionIndicator();
    }

    private void DrawPath(Vector3[] path)
    {
        if (path.Length < 2) return;
        _Line.positionCount = path.Length;
        for (int i = 0; i < path.Length; i++)
        {
            var x = path[i].x;
            var y = path[i].y + 0.01f; // Line is invisible if parallel to ground
            var z = path[i].z;
            _Line.SetPosition(i, new Vector3(x, y, z));
        }


        //_Line.sortingLayerName = "Foreground";
    }

}
