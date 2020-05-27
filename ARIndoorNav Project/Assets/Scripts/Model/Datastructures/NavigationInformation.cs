using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationInformation : MonoBehaviour
{
    // List of all path 3D coordinates in Unity worldspace
    private Vector3[] path;

    // Next Corner position
    private Vector3 nextCorner;

    // Position of corner after next corner 
    private Vector3 nextNextCorner;

    // Next turn instruction "right", or "left"
    private string turnInstruction;

    // Distance to next corner
    private float distanceToNextCorner;

    // When below this distance, the turn instruction is "go straight"
    private float maxDistanceToNextCorner = 20; // in meter

    // Distance to target
    private float totalDistance;

    // User position
    private Vector3 currentUserPos;

    // Current destination name
    private string destinationName;

    public void UpdateNavigationInformation(Vector3[] path, Vector3 currentUserPos)
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

        var angle = Vector3.Angle(currentUserPos, nextCorner);
    }

    private string GenerateTurnInstruction(Vector3[] path, Vector3 currentUserPos)
    {
        var forwardVector = currentUserPos;
        var upwardsVector = Vector3.up;
        var targetVector = nextNextCorner;

        if (distanceToNextCorner >= maxDistanceToNextCorner) // if the next corner is too far away, the user needs to go straight
        {
            return "straight";
        }

        // -1 = left, 1 = right, 0 = backwards/forwards 
        var directionResult = HelperFunctions.AngleDir(forwardVector, targetVector, upwardsVector);
        
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

    public void SetDestinationName(string newDestinationName)
    {
        destinationName = newDestinationName;
    }

}
