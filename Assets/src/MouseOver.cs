using System.Collections.Generic;
using UnityEngine;

public class MouseOver : MonoBehaviour
{
    private Dictionary<Material, Color> startColor = new Dictionary<Material, Color>();
    private Material[] materials;
    private bool isMouseOver = false;
    public Transform handTransform; // Transform-ul pentru atașarea obiectului

    void Start() {
        materials = transform.GetComponent<Renderer>().materials;
    }

    void Update()
    {
        // Verificăm dacă mouse-ul este peste obiect și tasta E este apăsată
        if (isMouseOver && Input.GetKeyDown(KeyCode.E))
        {
            PickupObject();
        }
    }

    void OnMouseEnter()
    {
        HighlightObject(true);
    }

    void OnMouseExit()
    {
        HighlightObject(false);
    }

    void HighlightObject(bool highlight)
    {
        if (highlight)
        {
            foreach(var material in materials) {
                Color color = material.color;
                startColor[material] = color;
                material.color = Color.yellow;
            }
        }
        else
        {
            foreach(var material in materials) {
                material.color = startColor[material];
            }
        }
    }

    void PickupObject()
    {
        transform.SetParent(handTransform);
        transform.localPosition = Vector3.zero;
        // Opțional: Dezactivează fizica obiectului
    }
}