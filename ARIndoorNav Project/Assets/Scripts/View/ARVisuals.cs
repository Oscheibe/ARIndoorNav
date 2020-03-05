using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ARVisuals : MonoBehaviour
{
    public NavigationPresenter _NavigationPresenter;
    public SystemStatePresenter _SystemStatePresenter;

    public GameObject _2DArrow;
    public Camera _ARCamera;
    public LineRenderer _Line;

    void Update()
    {
        _Line.material.SetTextureOffset("_MainTex", Vector2.left * Time.time);
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
     * Gets called each update by NavigationPresenter
     */
    public void SendNavigationPath(Vector3[] path)
    {
        DrawPath(path);
        if (path.Length == 1)
            Indicate2dDirection(path[0]);
        else
            Indicate2dDirection(path[1]);
    }

    /**
     * Clears current ARDisplay GameObjects
     * Will be overritten by the next SendNavigationPath() call
     */
    public void StopARDisplay()
    {
        _Line.positionCount = 0;
        _2DArrow.SetActive(false);
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

    private void Indicate2dDirection(Vector3 nextCorner)
    {
        // Source: https://answers.unity.com/questions/1037969/arrows-pointing-to-offscreen-enemy.html
        var screenPos = _ARCamera.WorldToViewportPoint(nextCorner);

        if (screenPos.x >= 0 && screenPos.x <= 1 && screenPos.y >= 0 && screenPos.y <= 1 && screenPos.z >= 0)
        {
            _2DArrow.SetActive(false);
            return;
        }

        _2DArrow.SetActive(true);

        var onScreenPos = new Vector2(screenPos.x - 0.5f, screenPos.y - 0.5f) * 2;
        var max = Mathf.Max(Mathf.Abs(onScreenPos.x), Mathf.Abs(onScreenPos.y));
        onScreenPos = (onScreenPos / (max * 2)) + new Vector2(0.5f, 0.5f);

        float x = 0;
        float y = 0;

        /**
         * This part of the code is to counteract a bug found within the app
         * The bug was that after a 90° turn away from the next corner of the path (nextCorner) 
         * the x and y axis would flip their respective values. (For example, if the corner is on the left side
         * the 2D arrow would jump from the left side of the screen to the right)
         * The y axis is still not fixed.
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

        var arrowPos = new Vector3(x, y, 0);
        _2DArrow.transform.position = arrowPos;
    }
}
