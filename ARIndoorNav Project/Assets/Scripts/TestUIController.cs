using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestUIController : MonoBehaviour
{
    public GameObject textField;

    private List<GameObject> textFieldList = new List<GameObject>();
    public int textFieldSize = 10;
    private RectTransform containerRectTrans;

    // Start is called before the first frame update
    void Start()
    {
        containerRectTrans = GetComponent<RectTransform>();
        FillContainer();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FillContainer()
    {
        // Adding first GameObject because the Y position is relative to the start and not the last added TextField
        float textFieldHeight = 129;//textField.GetComponent<Collider>().bounds.size.y;
        Vector3 position;
        float lastY = this.transform.position.y + (containerRectTrans.sizeDelta.y) / 2;
        Debug.Log("textFieldHeight: " + textFieldHeight);

        for (int i = 0; i < textFieldSize; i++) // Loop has to start at 1 because another TextField was added before
        {
            //lastY = textFieldList[textFieldList.Count - 1].transform.position.y;
            position = new Vector3(this.transform.position.x, lastY - textFieldHeight, this.transform.position.z);

            //textField.GetComponent("TextFieldManager").changeText("Test");

            var newTextField = Instantiate(textField, position, Quaternion.identity, transform);
            SetTextFieldText(newTextField, "PlateText", i.ToString());
            SetTextFieldText(newTextField, "NameText", "ne");
            textFieldList.Add(newTextField);
            lastY -= textFieldHeight;
            containerRectTrans.sizeDelta = new Vector2(containerRectTrans.sizeDelta.x, containerRectTrans.sizeDelta.y + textFieldHeight);
        }
        containerRectTrans.position = new Vector3(containerRectTrans.position.x, containerRectTrans.position.y + lastY, containerRectTrans.position.z);
    }

    private void SetTextFieldText(GameObject textField, string tag, string text)
    {
        foreach (Transform child in textField.transform)
        {
            if (child.tag == tag)
                child.gameObject.GetComponentInChildren<TMP_Text>().text = text;
        }
    }
}
