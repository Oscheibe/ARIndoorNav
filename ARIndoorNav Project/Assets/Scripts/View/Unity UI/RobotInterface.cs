using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotInterface : MonoBehaviour
{
    public float speed = 5;
    public float rotSpeed = 15;
    public float waitingTime = 5;

    bool moving = false;

    Animator animator;

    Vector3 moveDir = Vector3.zero;
    Vector3 targetPosition;
    Quaternion playerRot;



    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //ClickToSetTarget();
        if (moving) Move();
    }



    private void ClickToSetTarget()
    {
        if (Input.GetMouseButton(0))
        {
            SetTargetPosition();
        }
    }

    private void SetTargetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000))
        {
            targetPosition = hit.point;
            //this.transform.LookAt(targetPosition);
            var lookAtTarget = new Vector3(targetPosition.x - transform.position.x, transform.position.y, targetPosition.z - transform.position.z);
            playerRot = Quaternion.LookRotation(lookAtTarget);
            moving = true;
        }
    }

    private void Move()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, playerRot, rotSpeed * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) <= 0.2)
        {
            moving = false;
            TriggerIdleAnimation();
        } 
        else
        {
            TriggerWalkingAnimation();
        }

    }

    public void TriggerIdleAnimation()
    {
        animator.SetInteger("condition", 0);
    }

    public void TriggerWalkingAnimation()
    {
        animator.SetInteger("condition", 1);
    }
    public void TriggerJumpingAnimation()
    {
        animator.SetInteger("condition", 2);
    }

}
