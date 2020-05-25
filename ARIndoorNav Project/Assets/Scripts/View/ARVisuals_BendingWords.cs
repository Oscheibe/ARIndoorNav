using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Bending words is a display technique where the next action is described as 3D words floating in space
 * This next action could be: "Go left", "Go right", "Go straight"
 * The direction of the words in 3D space represent the action associated with them 
 */
public class ARVisuals_BendingWords : MonoBehaviour, IARVisuals
{
    public NavigationPresenter _NavigationPresenter;

    public void ClearARDisplay()
    {
        throw new System.NotImplementedException();
    }

    public void SendNavigationPath(Vector3[] path)
    {
        throw new System.NotImplementedException();
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
