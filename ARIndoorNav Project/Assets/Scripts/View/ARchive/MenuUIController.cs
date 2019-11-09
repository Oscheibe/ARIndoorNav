using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIController : MonoBehaviour
{
    public SceneController sceneController;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdateSelectedRoom(string plateNumber)
    {
        sceneController.UpdateDestination(plateNumber);
    }
}
