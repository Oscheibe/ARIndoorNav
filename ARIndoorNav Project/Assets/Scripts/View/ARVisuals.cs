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
    void SendNavigationInformation(NavigationInformation navigationInformation);

    /**
     * Gets called by NavigationPresenter each update
     * Function to update the state of each AR_Visuals technique with the necessary information 
     */
    //void SendNavigationInformation(NavigationInformation navigationInformation);

    /**
     * Clears current ARDisplay GameObjects
     * Will be overritten by the next SendNavigationPath() call
     */
    void ClearARDisplay();


}
