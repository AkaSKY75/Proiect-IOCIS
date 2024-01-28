using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;

public class ObjectCollision : MonoBehaviour
{
    private GameObject lastHighlightedObject;

    void Update()
    {
        HighlightObjectWithMouseOver();
    }

    void HighlightObjectWithMouseOver()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            // Highlight the object
            if (lastHighlightedObject != hitObject)
            {
                ResetLastHighlightedObject();
                HighlightObject(hitObject);
                lastHighlightedObject = hitObject;
            }
        }
        else
        {
            // No object under mouse, reset the last highlighted object
            ResetLastHighlightedObject();
        }
    }

    void HighlightObject(GameObject obj)
    {
        // Add your highlight effect here
        // For example, change the color
        var renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.yellow;
        }
    }

    void ResetLastHighlightedObject()
    {
        if (lastHighlightedObject != null)
        {
            // Reset the highlight effect
            // For example, revert the color change
            var renderer = lastHighlightedObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.white; // Assume white is the original color
            }
            lastHighlightedObject = null;
        }
    }
}
