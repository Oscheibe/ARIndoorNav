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
    public BendingWordsController _BendingWordsController;
    public GameObject WordsPrefab1;
    public GameObject WordsPrefab2;

    public float wordsDistance = 2;

    public void ClearARDisplay()
    {
        WordsPrefab1.SetActive(false);
        WordsPrefab2.SetActive(false);
    }

    public void SendNavigationInformation(NavigationInformation navigationInformation)
    {
        // Only start calculations when the original path is finished
        if (navigationInformation.HasPath() == false)
            return;

        WordsPrefab1.SetActive(true);
        WordsPrefab2.SetActive(true);

        var nextCorner = navigationInformation.GetNextCorner();
        var nextNextCorner = navigationInformation.GetNextNextCorner();
        var currentUserPos = navigationInformation.GetCurrentUserPos();

        PlaceWordsPrefab1(currentUserPos, nextCorner);

        if (navigationInformation.GetTurnInstruction().Equals("straight"))
        {

            PlaceWordsPrefab2(currentUserPos, nextCorner, wordsDistance * 4);
        }
        else
            PlaceWordsPrefab2(nextCorner, nextNextCorner, wordsDistance);

        _BendingWordsController.UpdateWords("Go", navigationInformation.GetTurnInstruction());
    }

    /**
     * Calculates the position of the "go" instruction
     */
    private void PlaceWordsPrefab1(Vector3 currentUserPos, Vector3 nextCorner)
    {
        // Calculating the relative angle between the two vectors
        Quaternion targetAngle = Quaternion.LookRotation(nextCorner - currentUserPos);
        // Make the vector longer, depending on how far away the new point has to be
        var unitVectorForward = targetAngle * Vector3.forward * wordsDistance;
        var resultVector = currentUserPos + unitVectorForward;

        WordsPrefab1.GetComponent<RectTransform>().position = resultVector;
        WordsPrefab1.GetComponent<RectTransform>().transform.LookAt(currentUserPos);
    }

    /**
     * Calculates the position of the turn instruction
     * If the user has to go "straight", the instruction position will not rely on the next and nextNextCorner
     */
    private void PlaceWordsPrefab2(Vector3 nextCorner, Vector3 nextNextCorner, float secondWordDistance)
    {
        // Calculating the relative angle between the two vectors
        Quaternion targetAngle = Quaternion.LookRotation(nextNextCorner - nextCorner);
        // Make the vector longer, depending on how far away the new point has to be
        var unitVectorForward = targetAngle * Vector3.forward * secondWordDistance;
        var resultVector = nextCorner + unitVectorForward;
        WordsPrefab2.GetComponent<RectTransform>().position = resultVector;
        WordsPrefab2.GetComponent<RectTransform>().transform.LookAt(nextNextCorner);
    }

}
