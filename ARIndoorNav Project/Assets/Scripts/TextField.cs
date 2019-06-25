using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextField : MonoBehaviour, IPointerClickHandler

{
    [SerializeField] TextFieldController textFieldController;
	[SerializeField] Animator animator;
	[SerializeField] int thisIndex;
    [SerializeField] GameObject thisGameObject;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        animator.SetBool("selected", true);
        Debug.Log("I was clicked! It was me: " + this.name);
    }

}
