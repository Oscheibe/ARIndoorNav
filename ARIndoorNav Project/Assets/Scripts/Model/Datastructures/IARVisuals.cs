using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IARVisuals
{
    
    /**
     * Gets called each update by NavigationPresenter
     * Starts displaying AR content
     */
    public void SendNavigationPath(Vector3[] path);

    /**
     * Clears current ARDisplay GameObjects
     * Will be overritten by the next SendNavigationPath() call
     */
    public void ClearARDisplay();


}
