using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movements : MonoBehaviour
{
    public float speed = 5f;
    public float jumpAmount = 10;
    private bool isGrounded = true;
    private Vector3 mousePosition;

    private float angleInRadians;

    private void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            angleInRadians = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
            Vector3 movement = new Vector3(Mathf.Cos(angleInRadians), 0f, Mathf.Sin(angleInRadians)) * speed * Time.deltaTime;

            transform.position += movement;
            transform.position += new Vector3(speed * Time.deltaTime + Convert.ToSingle(Math.Cos(Convert.ToDouble(transform.rotation.eulerAngles.y))), 0f, Convert.ToSingle(Math.Cos(Convert.ToDouble(transform.rotation.eulerAngles.y))));
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.position -= new Vector3(speed * Time.deltaTime + Convert.ToSingle(Math.Cos(Convert.ToDouble(transform.rotation.eulerAngles.y))), 0f, Convert.ToSingle(Math.Cos(Convert.ToDouble(transform.rotation.eulerAngles.y))));
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0f, 0f, speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            transform.position -= new Vector3(0f, 0f, speed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true)
        {
            GetComponent<Rigidbody>().AddForce(0, jumpAmount, 0, ForceMode.Impulse);
        }

        transform.Rotate(new Vector3(0f, Input.mousePosition.x - this.mousePosition.x, 0f));
        this.mousePosition = Input.mousePosition;

    }

    private void OnTriggerStay(Collider other)
    {
        isGrounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isGrounded = false;
    }
}
