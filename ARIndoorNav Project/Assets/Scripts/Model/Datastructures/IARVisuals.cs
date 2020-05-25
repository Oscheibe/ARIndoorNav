using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * An interface for AR navigation visuals. 
 * The information required for the navigation is: the path, the user's position
 */
public interface IARVisuals
{
    
    /**
     * Gets called each update by NavigationPresenter
     * Starts displaying AR content
     */
    void SendNavigationPath(Vector3[] path);

    /**
     * Clears current ARDisplay GameObjects
     * Will be overritten by the next SendNavigationPath() call
     */
    void ClearARDisplay();


}
