using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        /*for( var i = 0 ; i < devices.Length ; i++) {
            Debug.Log(devices[i].name);
        }*/
        WebCamTexture webcam = new WebCamTexture(devices[0].name);
        transform.GetComponent<RawImage>().material.mainTexture = webcam;
        webcam.Play();
    }
}
