using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextField : MonoBehaviour, IPointerClickHandler

{
	[SerializeField] Animator animator;
	[SerializeField] int thisIndex;
    [SerializeField] GameObject sceneControllerObject;

    private SceneController sceneController;

    // Start is called before the first frame update
    void Start()
    {
        sceneController = sceneControllerObject.GetComponent<SceneController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        animator.SetBool("selected", true);
        //Debug.Log("I was clicked! It was me: " + this.name + " And I am here; " + this.transform.position);
        sceneController.UpdateDestination(this.name);
        //menuUIController.UpdateSelectedRoom(this.name);
    }

}
