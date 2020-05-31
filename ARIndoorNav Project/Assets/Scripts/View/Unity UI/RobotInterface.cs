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
    bool looking = false;

    Animator animator;

    Vector3 moveDir = Vector3.zero;
    Vector3 targetPosition;
    Quaternion playerRot;

    bool isJumping = false;



    // Awake to counter a bug where the animator would not get loaded on the first Update after activation 
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //ClickToSetTarget();
        //if (moving) Move();
        if (looking) Looking();
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

    private void Looking()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, playerRot, rotSpeed * Time.deltaTime);
    }

    public void LookAtPos(Vector3 pos)
    {
        playerRot = Quaternion.LookRotation(pos);
    }

    public void TriggerIdleAnimation()
    {
        isJumping = false;
        looking = false;
        animator.SetInteger("condition", 0);
    }

    public void TriggerWalkingAnimation()
    {
        isJumping = false;
        looking = false;
        animator.SetInteger("condition", 1);
    }

    public void TriggerJumpingAnimation()
    {
        // The avatar should look at the user when jumping 
        looking = true;
        
        // Skip this function if the avatar is already jumping 
        if (isJumping)
        {
            return;
        }
        isJumping = true;
        animator.SetInteger("condition", 2);
    }

}
