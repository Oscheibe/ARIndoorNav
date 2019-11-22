using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UserMessageUI : MonoBehaviour
{
    public TMP_Text _TrackingStateText;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void SendUserMessage(string message)
    {
        _TrackingStateText.text = "Tracking state: " + message;
    }
}
