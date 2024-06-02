using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class AnimationState : NetworkBehaviour
{

    [SerializeField]
    private NetworkAnimator animator;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) {
            return;
        }
        
        if (animator == null)
        {
            throw new Exception(transform.name + " must have `NetworkAnimator` component!");
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        if (isMoving) {
            animator.enabled = true;
            animator.SetTrigger("isMoving");
        } else {
            animator.ResetTrigger("isMoving");
            animator.enabled = false;
        }
    }

    public void SetArmsUpAnimationState() {
        if (!IsOwner)
        {
            return;
        }
        animator.enabled = true;
        animator.ResetTrigger("isArmsDown");
        animator.SetTrigger("isArmsUp");
    }

    public void ResetArmsUpAnimationState() {
        if (!IsOwner)
        {
            return;
        }
        animator.enabled = true;
        animator.ResetTrigger("isArmsUp");
        animator.SetTrigger("isArmsDown");
    }
}