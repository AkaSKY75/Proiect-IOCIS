using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOver : MonoBehaviour
{
    // Start is called before the first frame update
    private Dictionary<GameObject, Color> startcolor = new Dictionary<GameObject, Color>();
    public List<GameObject> bodyParts;
    void OnMouseEnter()
    {
        foreach(var bodyPart in bodyParts) {
            Color color = bodyPart.GetComponent<Renderer>().material.color;
            startcolor[bodyPart] = color;
            bodyPart.GetComponent<Renderer>().material.color = Color.yellow;
        }
    }
    void OnMouseExit()
    {
        foreach(var item in startcolor) {
            item.Key.GetComponent<Renderer>().material.color = item.Value;
        }
    }
}
