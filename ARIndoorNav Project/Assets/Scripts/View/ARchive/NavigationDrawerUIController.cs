using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationDrawerUIController : MonoBehaviour
{
    public MenuUIController menuUIController;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTextFieldClick(string plateNumber)
    {
        menuUIController.UpdateSelectedRoom(plateNumber);
    }

}
