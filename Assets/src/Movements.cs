using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Security.Cryptography;
using Unity.Netcode;
using UnityEngine;

public class Movements : NetworkBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float jumpAmount = 10;
    private bool movementsEnabled = true;
    private bool isGrounded = true;
    private Vector3 mousePosition;
    private float angleInRadians;
  
    private void Update()
    {
        if (!IsOwner) {
            return;
        }

        if (movementsEnabled == true) {
            this.angleInRadians = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                transform.position += new Vector3(Mathf.Cos(this.angleInRadians), 0f, -Mathf.Sin(this.angleInRadians)) * speed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                transform.position -= new Vector3(Mathf.Cos(this.angleInRadians), 0f, -Mathf.Sin(this.angleInRadians)) * speed * Time.deltaTime;

            }

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                transform.position += new Vector3(Mathf.Sin(this.angleInRadians), 0f, Mathf.Cos(this.angleInRadians)) * speed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                transform.position -= new Vector3(Mathf.Sin(this.angleInRadians), 0f, Mathf.Cos(this.angleInRadians)) * speed * Time.deltaTime;
            }


            if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true)
            {
                GetComponent<Rigidbody>().AddForce(0, jumpAmount, 0, ForceMode.Impulse);

            }

            if (Input.GetKeyDown(KeyCode.H)) {
                GetComponent<HelpPanelControl>().ToggleHelpPanel();
            }

            if (Input.GetKeyDown(KeyCode.V)) {
                GetComponent<ToggleCameraClass>().ToggleCamera();
            }

            GetComponent<ToggleCameraClass>().RotateCamera((Input.mousePosition.y - this.mousePosition.y)/2);
            transform.Rotate(new Vector3(0f, Input.mousePosition.x - this.mousePosition.x, 0f));
            this.mousePosition = Input.mousePosition;
        }
    }

    public void EnableMovements() {
        movementsEnabled = true;
    }

    public void DisableMovements() {
        movementsEnabled = false;
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
