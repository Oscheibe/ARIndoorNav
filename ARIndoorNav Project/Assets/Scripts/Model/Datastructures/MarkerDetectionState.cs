using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MarkerDetectionState
{
    Idle = 0,
    DetectingMarker = 1,
    WaitingForService = 2,
}