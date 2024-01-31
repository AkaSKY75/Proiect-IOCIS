using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
   
    public Transform handTransform; 
    public float interactionDistance = 2f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance))
            {
                if (hit.collider.CompareTag("Interactable")) 
                {
                    PickupObject(hit.collider.gameObject);
                }
            }
        }
    }

    void PickupObject(GameObject pickObject)
    {
        pickObject.transform.SetParent(handTransform); 
        pickObject.transform.localPosition = Vector3.zero; 
        
    }
}
