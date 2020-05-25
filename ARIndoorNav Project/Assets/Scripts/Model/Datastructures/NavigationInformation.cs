using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationInformation : MonoBehaviour
{
    // List of all path 3D coordinates in Unity worldspace
    private Vector3[] path;

    // Next Corner position
    private Vector3 nextCorner = new Vector3();

    // Next turn instruction "right", or "left"
    private string turnInstruction;

    // Distance to next corner
    private float distanceToNextCorner;

    // Distance to target
    private float totalDistance;

    // User position
    private Vector3 currentUserPos;

    // Current destination name
    private string destinationName;

    public void UpdateNavigationInformation(Vector3[] path, Vector3 currentUserPos)
    {
        //TODO 
        this.path = path;
        
        if(path.Length > 0)
            nextCorner = path[0];
        else   
            nextCorner = currentUserPos;
        
        turnInstruction = GenerateTurnInstruction(path, currentUserPos);

        distanceToNextCorner = Vector3.Distance(currentUserPos, nextCorner);

        totalDistance = GetPathDistance(path);

        this.currentUserPos = currentUserPos;
    }

    private string GenerateTurnInstruction(Vector3[] path, Vector3 currentUserPos)
    {
        //TODO
        return "TODO";
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
