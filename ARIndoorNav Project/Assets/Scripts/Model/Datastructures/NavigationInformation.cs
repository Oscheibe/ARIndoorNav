using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationInformation
{
    // List of all path 3D coordinates in Unity worldspace
    private Vector3[] path;

    // Next Corner position
    private Vector3 nextCorner = Vector3.zero;

    // Position of corner after next corner 
    private Vector3 nextNextCorner;

    // Next turn instruction "right", or "left"
    private string turnInstruction;

    // Distance to next corner
    private float distanceToNextCorner;

    // When below this distance, the turn instruction is "go straight"
    private float maxDistanceToNextCorner = 15; // in meter

    // Distance to target
    private float totalDistance;

    // User position
    private Vector3 currentUserPos;

    // Current destination name
    private string destinationName;

    // Destination position with a y value of the _GroundFloor detected by Navigation.cs
    private Vector3 destinationPos;

    // A reference to the ARCamera, used to calculate screen positions 
    private Camera ARCamera;

    // Signals if the NavMesh has finished its calculations 
    private bool hasPath = false;

    public void UpdateNavigationInformation(Vector3[] path, Vector3 currentUserPos, Vector3 destinationPos, Camera ARCamera, bool hasPath)
    {
        this.path = path;

        /** Set nextCorner and nextNextCorner
         * Case: Three or more corners. Corner 0, Corner 1, Corner 2, ... Corner X
         * Corner 0 = User pos
         * Corner 1 = Position of next turn/corner
         * Corner 2 = Corner after next turn
         * => nextCorner = Corner 1 (Position of next turn/corner)
         * => nextNextCorner = Corner 2 (Corner after next turn)
         */
        if (path.Length >= 3)
        {
            nextCorner = path[1];
            nextNextCorner = path[2];
        }
        /**
         * Case: Only two corners. Corner 0, Corner 1
         * Corner 0 = User pos
         * Corner 1 = Target
         * => nextCorner = Corner 1 (Target)
         * => nextNextCorner = Corner 1 (Target)
         */
        else if (path.Length >= 2)
        {
            nextCorner = path[1];
            nextNextCorner = path[1];
        }
        // Default: Everything is user position!
        else
        {
            nextCorner = path[0];
            nextNextCorner = path[0];
        }


        turnInstruction = GenerateTurnInstruction(path, currentUserPos);

        distanceToNextCorner = Vector3.Distance(currentUserPos, nextCorner);

        totalDistance = GetPathDistance(path);

        this.currentUserPos = currentUserPos;

        this.destinationPos = destinationPos;

        var angle = Vector3.Angle(currentUserPos, nextCorner);

        this.ARCamera = ARCamera;

        this.hasPath = hasPath;
    }

    /**
     * Calculates the turn direction based on the user position, next corner, and nextNext corner
     * Returns "left", "right", or "straight" if the target is far away or the goal is the next corner
     */
    private string GenerateTurnInstruction(Vector3[] path, Vector3 currentUserPos)
    {
        // if the next corner is too far away OR the goal is the last corner, the user needs to go straight
        if (distanceToNextCorner >= maxDistanceToNextCorner || path.Length <= 2)
        {
            return "straight";
        }

        var upwardsVector = Vector3.up;

        Quaternion targetAngle = Quaternion.LookRotation(nextCorner - currentUserPos);
        var unitVectorForward = targetAngle * Vector3.forward;

        Quaternion targetAngle2 = Quaternion.LookRotation(nextNextCorner - nextCorner);
        var unitVectorForward2 = targetAngle2 * Vector3.forward;

        // -1 = left, 1 = right, 0 = backwards/forwards 
        var directionResult = HelperFunctions.AngleDir(unitVectorForward, unitVectorForward2, upwardsVector);

        if (directionResult == -1)
        {
            return "left";
        }
        else
            return "right"; // This ignores the forward/backward result by intention 
    }

    /**
     * Calculates the distance between all the points on a path
     */
    private float GetPathDistance(Vector3[] path)
    {
        float result = 0;
        // Iterates over each path corner expect the last and 
        // calculates the distance between each one
        for (int i = 0; i < path.Length - 1; i++)
        {
            result += Vector3.Distance(path[i], path[i + 1]);
        }

        return result;
    }

    public Vector3[] GetPath()
    {
        return path;
    }

    public Vector3 GetNextCorner()
    {
        return nextCorner;
    }

    public Vector3 GetNextNextCorner()
    {
        return nextNextCorner;
    }

    public string GetTurnInstruction()
    {
        return turnInstruction;
    }

    public float GetDistanceToNextCorner()
    {
        return distanceToNextCorner;
    }

    public float GetTotalDistance()
    {
        return totalDistance;
    }

    public Vector3 GetCurrentUserPos()
    {
        return currentUserPos;
    }

    public string GetDestinationName()
    {
        return destinationName;
    }

    public Vector3 GetDestinationPos()
    {
        return destinationPos;
    }

    public void SetDestinationName(string newDestinationName)
    {
        destinationName = newDestinationName;
    }

    public Camera GetARCamera()
    {
        return ARCamera;
    }

    public bool IsVector3InView(params Vector3[] vector3)
    {
        Vector3 nextCorner;
        if (vector3.Length >= 1)
        {
            nextCorner = vector3[0];
        }

        else nextCorner = this.nextCorner;
        var screenPos = ARCamera.WorldToViewportPoint(nextCorner);
        if (screenPos.x >= 0 && screenPos.x <= 1 && screenPos.y >= 0 && screenPos.y <= 1 && screenPos.z >= 0)
        {
            return true;
        }
        else return false;
    }

    public bool HasPath()
    {
        return hasPath;
    }

    /**
     * Calculates a screen position x,y (x = 0 to screen.width / y = 0 to screen.height) based on a 
     * given Vector3. If the Vector3 is out of screen, the position will be at the edge of the screen
     * 
     * I tried something fancy with params. Only the first param will get computed
     * the next corner of the path is the standard value if no param was provided
     */
    public Vector2 Vector3ToScreenPos(params Vector3[] vector3)
    {
        Vector2 arrowPos;
        Vector3 nextCorner;
        if (vector3.Length >= 1)
        {
            nextCorner = vector3[0];
        }
        else nextCorner = this.nextCorner;

        // Source: https://answers.unity.com/questions/1037969/arrows-pointing-to-offscreen-enemy.html
        var screenPos = ARCamera.WorldToViewportPoint(nextCorner);
        float x = 0;
        float y = 0;

        if (screenPos.x >= 0 && screenPos.x <= 1 && screenPos.y >= 0 && screenPos.y <= 1 && screenPos.z >= 0)
        {
            var screenPointUnscaled = ARCamera.WorldToScreenPoint(nextCorner);
            x = screenPointUnscaled.x; // / _Canvas.scaleFactor;
            y = screenPointUnscaled.y; // / _Canvas.scaleFactor;

            return new Vector2(x, y);
        }

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
