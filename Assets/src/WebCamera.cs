using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;

public class WebCamera : MonoBehaviour
{
    const int cameraWidth = 640;
    const int cameraHeight = 480;
    private WebCamTexture webcam;
    private RawImage rawImage;
    private Thread imageProcThread;
    private bool shouldRunImageProc;
    private bool frameWasChanged;
    private byte[] pixels;
    // Start is called before the first frame update
    void Start()
    {
        DLLWrappers.OpenCV.GetInstance().LoadLibraries();
        /*WebCamDevice[] devices = WebCamTexture.devices;
        for( var i = 0 ; i < devices.Length ; i++) {
            Debug.Log(devices[i].name);
        }
        webcam = new WebCamTexture(devices[0].name, cameraWidth, cameraHeight);*/
        shouldRunImageProc = true;
        frameWasChanged = false;
        pixels = new byte[cameraWidth * cameraHeight * 3];
        imageProcThread = new Thread(new ThreadStart(delegate{
            unsafe {
                fixed(bool* shouldRunImageProcPtr = &shouldRunImageProc, frameWasChangedPtr = &frameWasChanged) {
                    fixed(byte* pixelsPtr = &pixels[0]) {
                        var ret = DLLWrappers.OpenCV.GetInstance().DetectGesture((IntPtr)shouldRunImageProcPtr, (IntPtr)frameWasChangedPtr, (IntPtr)pixelsPtr);
                        if (ret != 0) {
                            throw new Exception("Error occured during image processing: " + ret);
                        }
                    }
                }
            }
            /*while(shouldRunImageProc) {
                Debug.LogWarning("Thread running...");
                Thread.Sleep(5000);
            }*/
        }));
        /* imageProcThread.Start(); */ frameWasChanged = true;
        rawImage = transform.GetComponent<RawImage>();
        rawImage.texture = new Texture2D(cameraWidth, cameraHeight, TextureFormat.RGB24, false);
        
        unsafe {
            fixed(bool* frameWasChangedPtr = &frameWasChanged) {
                while(*frameWasChangedPtr == false) {
                    continue;
                }
                ((Texture2D)rawImage.texture).LoadRawTextureData(this.pixels); // RGB
                ((Texture2D)rawImage.texture).Apply();
                *frameWasChangedPtr = false;
            }
        }
        
        // rawImage.material.mainTexture = webcam;
        // webcam.Play();
    }

    // Update is called once per frame
    void Update()
    {
        // Texture2D texture = new Texture2D(cameraWidth, cameraHeight);
        // texture.SetPixels(webcam.GetPixels());
        // Debug.LogWarning("webcam.requestedHeight = " + webcam.requestedHeight + "; webcam.requestedWidth = " + webcam.requestedWidth);
        // Debug.LogWarning("webcam.GetPixels().Length = " + webcam.GetPixels().Length);
        // var texture = (Texture2D)rawImage.material.mainTexture;
        /*Texture mainTexture = rawImage.material.mainTexture;
        Debug.LogWarning("webcam.height = " + webcam.height + "webcam.width = " + webcam.width);
        Texture2D texture2D = new Texture2D(mainTexture.width, mainTexture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = new RenderTexture(mainTexture.width, mainTexture.height, 32);
        Graphics.Blit(mainTexture, renderTexture);
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();
        RenderTexture.active = currentRT;*/
        
        //var pixels = texture.EncodeToPNG();
        
        /*IntPtr unmanagedPointer = Marshal.AllocHGlobal(pixels.Length);
        Marshal.Copy(pixels, 0, unmanagedPointer, pixels.Length);
        // Call unmanaged code
        var ret = DLLWrappers.OpenCV.GetInstance().SaveImage(cameraHeight, cameraWidth, unmanagedPointer);
        Debug.LogWarning("ret = " + ret);
        Marshal.FreeHGlobal(unmanagedPointer);*/

        /*var pixels = Texture2D.redTexture.EncodeToPNG();
        unsafe {
            fixed(byte* p = pixels) {
                IntPtr intPtr = (IntPtr)p;
                // var ret = DLLWrappers.OpenCV.GetInstance().SaveImage(pixels.Length, intPtr);
                var ret = DLLWrappers.OpenCV.GetInstance().DetectGesture(intPtr, pixels.Length);
                if (ret != IntPtr.Zero) {
                    var texture2D = new Texture2D(cameraWidth, cameraHeight, TextureFormat.RGB24, false);
                    texture2D.LoadRawTextureData(ret, cameraWidth * cameraHeight * 3); // RGB
                    texture2D.Apply();
                    // rawImage.material.mainTexture = texture2D;
                    rawImage.texture = texture2D;
                }
            }
        }*/
        unsafe {
            fixed(bool* frameWasChangedPtr = &frameWasChanged) {
                if (*frameWasChangedPtr == true) {
                    ((Texture2D)rawImage.texture).LoadRawTextureData(this.pixels); // RGB
                    ((Texture2D)rawImage.texture).Apply();
                    *frameWasChangedPtr = false;
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        // shouldRunImageProc = false;
        unsafe {
            fixed(bool* shouldRunImageProcPtr = &shouldRunImageProc) {
                *shouldRunImageProcPtr = false;
            }
        }
        while (imageProcThread.ThreadState == ThreadState.Running) {
            continue;
        }
        DLLWrappers.OpenCV.GetInstance().FreeLibraries();
    }
}
