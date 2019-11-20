using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonListControl : MonoBehaviour
{
    public GameObject buttonTemplate;

    private List<int> intList = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i <= 20; i++)
        {
            GenButtons("Button #"+i);
        }
    }

    private void GenButtons(string buttonName)
    {
        GameObject button = Instantiate(buttonTemplate) as GameObject;
        button.SetActive(true);
        button.GetComponent<ButtonListButton>().SetText(buttonName);
        button.transform.SetParent(buttonTemplate.transform.parent, false); // set false so that button doesn't position themselves in worldspace. Makes it more dynamic

    }
}
