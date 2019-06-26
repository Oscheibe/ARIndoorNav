using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TestUIController : MonoBehaviour
{
    public GameObject textField;

    private List<GameObject> textFieldList = new List<GameObject>();
    public int textFieldSize = 10;
    private RectTransform containerRectTrans;
    private List<TextField> roomPlateList;

    // Start is called before the first frame update
    void Start()
    {
        containerRectTrans = GetComponent<RectTransform>();
        GetNamePlateInfo(out roomPlateList);
        FillContainer();
    }

    private void GetNamePlateInfo(out List<TextField> roomPlateList)
    {
        roomPlateList = new List<TextField>();
        
        roomPlateList.Add(new TextField("3.215", "Dr. Dr. Oliver Scheibert"));
        roomPlateList.Add(new TextField("3.216", "Dr. Dr. Oliver Scheibert"));
        roomPlateList.Add(new TextField("3.217 ", "Dr. Dr. Oliver Scheibert"));
        roomPlateList.Add(new TextField("3.218", "Dr. Dr. Oliver Scheibert"));
        roomPlateList.Add(new TextField("3.219", "Dr. Benjamin Meyer, Florian Herborn"));
        roomPlateList.Add(new TextField("3.220", "Dr. Dr. Oliver Scheibert"));
        roomPlateList.Add(new TextField("3.221", "Dr. Dr. Oliver Scheibert"));
        roomPlateList.Add(new TextField("3.222", "Dr. Dr. Oliver Scheibert"));
        roomPlateList.Add(new TextField("3.222a", "Dr. Dr. Oliver Scheibert"));

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
        
        float lastY = (containerRectTrans.sizeDelta.y) + transform.position.y; //+ (textFieldHeight/2); // this.transform.position.y + 

        foreach (var roomPlate in roomPlateList)
        {
            //position = new Vector3(this.transform.position.x, lastY - textFieldHeight, this.transform.position.z);
            position = new Vector3(this.transform.position.x, lastY - textFieldHeight, this.transform.position.z);
            lastY -= textFieldHeight;
            var newTextField = Instantiate(textField, position, Quaternion.identity, transform);
            newTextField.name = roomPlate.roomText;
            roomPlate.SetPlateFieldText(newTextField);
            roomPlate.SetNameFieldText(newTextField);
            newTextField.SetActive(true);
            textFieldList.Add(newTextField);
            //containerRectTrans.sizeDelta = new Vector2(containerRectTrans.sizeDelta.x, containerRectTrans.sizeDelta.y + textFieldHeight);
        }

        for (int i = 0; i < textFieldSize; i++) // Loop has to start at 1 because another TextField was added before
        {

        }
        //containerRectTrans.position = new Vector3(containerRectTrans.position.x, containerRectTrans.position.y + lastY, containerRectTrans.position.z);
    }



    private class TextField
    {
        public string roomText;
        public string nameText;

        public TextField(string plateText, string nameText)
        {
            this.roomText = plateText;
            this.nameText = nameText;
        }

        public void SetPlateFieldText(GameObject textField)
        {
            foreach (Transform child in textField.transform)
            {
                if (child.tag == "PlateText")
                    child.gameObject.GetComponentInChildren<TMP_Text>().text = roomText;
            }
        }

        public void SetNameFieldText(GameObject textField)
        {
            foreach (Transform child in textField.transform)
            {
                if (child.tag == "NameText")
                    child.gameObject.GetComponentInChildren<TMP_Text>().text = nameText;
            }
        }


    }
}
