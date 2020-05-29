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

    public void ClearARDisplay()
    {
        WordsPrefab1.SetActive(false);
        WordsPrefab2.SetActive(false);
    }

    public void SendNavigationInformation(NavigationInformation navigationInformation)
    {
        WordsPrefab1.SetActive(true);
        WordsPrefab2.SetActive(true);

        var nextCorner = navigationInformation.GetNextCorner();
        var nextNextCorner = navigationInformation.GetNextNextCorner();
        var currentUserPos = navigationInformation.GetCurrentUserPos();

        WordsPrefab1.GetComponent<RectTransform>().position = nextCorner;
        WordsPrefab1.GetComponent<RectTransform>().transform.LookAt(currentUserPos);

        WordsPrefab2.GetComponent<RectTransform>().position = nextCorner;
        WordsPrefab2.GetComponent<RectTransform>().transform.LookAt(nextNextCorner);

        _BendingWordsController.UpdateWords("Go", navigationInformation.GetTurnInstruction());
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
