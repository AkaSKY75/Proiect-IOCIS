using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationState : MonoBehaviour
{

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            // Do something with the animator
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (animator != null)
        {
            bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
            if (isMoving) {
                animator.enabled = true;
                animator.SetTrigger("isMoving");
            } else {
                animator.ResetTrigger("isMoving");
                animator.enabled = false;
            }
        }
    }
}