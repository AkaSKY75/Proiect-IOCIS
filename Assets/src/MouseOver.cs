using System.Collections.Generic;
using UnityEngine;

public class MouseOver : MonoBehaviour
{
    private Dictionary<GameObject, Color> startColors = new Dictionary<GameObject, Color>();
    public List<GameObject> bodyParts;
    private bool isMouseOver = false;
    public Transform handTransform; // Transform-ul pentru atașarea obiectului

    void Update()
    {
        // Verificăm dacă mouse-ul este peste obiect și tasta E este apăsată
        if (isMouseOver && Input.GetKeyDown(KeyCode.E))
        {
            foreach (var bodyPart in bodyParts)
            {
                PickupObject(bodyPart);
            }
        }
    }

    void OnMouseEnter()
    {
        isMouseOver = true;
        HighlightObjects(true);
    }

    void OnMouseExit()
    {
        isMouseOver = false;
        HighlightObjects(false);
    }

    void HighlightObjects(bool highlight)
    {
        foreach (var bodyPart in bodyParts)
        {
            var renderer = bodyPart.GetComponent<Renderer>();
            if (highlight)
            {
                if (renderer != null)
                {
                    Color color = renderer.material.color;
                    startColors[bodyPart] = color;
                    renderer.material.color = Color.yellow;
                }
            }
            else
            {
                if (startColors.TryGetValue(bodyPart, out Color originalColor) && renderer != null)
                {
                    renderer.material.color = originalColor;
                }
            }
        }
    }

    void PickupObject(GameObject pickObject)
    {
        pickObject.transform.SetParent(handTransform);
        pickObject.transform.localPosition = Vector3.zero;
        // Opțional: Dezactivează fizica obiectului
    }
}