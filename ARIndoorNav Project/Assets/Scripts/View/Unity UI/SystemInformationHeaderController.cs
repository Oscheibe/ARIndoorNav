using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SystemInformationHeaderController : MonoBehaviour
{
    public float systemInformationTime = 4.0f;
    public UIMenuSwitchingManager _UIMenuSwitchingManager;

    private TMP_Text systemInformationText;
    private bool isDisplaying = false;
    
    // Start is called before the first frame update
    void Start()
    {
        systemInformationText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplaySystemInformation(string text)
    {
        if(isDisplaying)
        {
            systemInformationText.text += "\n- " + text;
        }
        else
        {
            systemInformationText.text = text;
            StartCoroutine("DisplayCoroutine");
        }

    }

    private IEnumerable DisplayCoroutine()
    {
        _UIMenuSwitchingManager.OpenSystemInformationHeader();
        isDisplaying = true;
        yield return new WaitForSeconds(systemInformationTime);
        isDisplaying = false;
        _UIMenuSwitchingManager.CloseSystemInformationHeader();
    }
}
