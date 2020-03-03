using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NavigationState
{
    NavigationInProgress = 0, 
    NavigationPaused = 1,
    ReachedDestination = 2, // Stop navigation
    EncounteredObstacle = 3, // Stop navigation
}