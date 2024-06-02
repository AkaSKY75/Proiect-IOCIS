using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;

public class WebCamera : NetworkBehaviour
{
    const int cameraWidth = 640;
    const int cameraHeight = 480;
    private RawImage rawImage;
    private Thread imageProcThread;
    private bool shouldRunImageProc;
    private bool frameWasChanged;
    private int detectedGesture;
    private byte[] pixels;
    private Transform playerTransform;
    public enum DetectedGestureEnum : int {
        UNKNOWN_ACTION = -1,
        ARMS_UP
    };

    public void OnStartWebCamera(Transform rawImage) {
        if(!IsOwner) {
            return;
        }
        if (rawImage == null) {
            throw new Exception("rawImage parameter cannot be null for `OnStartWebCamera` function!");
        }
        this.rawImage = rawImage.GetComponent<RawImage>();
        DLLWrappers.OpenCV.GetInstance().LoadLibraries();
        shouldRunImageProc = true;
        frameWasChanged = false;
        detectedGesture = (int)DetectedGestureEnum.UNKNOWN_ACTION;
        pixels = new byte[cameraWidth * cameraHeight * 3];
        imageProcThread = new Thread(new ThreadStart(delegate{
            // blocking thread, until `shouldRunImageProc` is set to `false`
            unsafe {
                fixed(bool* shouldRunImageProcPtr = &shouldRunImageProc, frameWasChangedPtr = &frameWasChanged) {
                    fixed(byte* pixelsPtr = &pixels[0]) {
                        fixed(int* detectedGesturePtr = &detectedGesture) {
                            var ret = DLLWrappers.OpenCV.GetInstance().DetectGesture((IntPtr)shouldRunImageProcPtr, (IntPtr)frameWasChangedPtr,
                                (IntPtr)pixelsPtr, (IntPtr)detectedGesturePtr, cameraWidth, cameraHeight);
                            if (ret != 0) {
                                throw new Exception("Error occured during image processing: " + ret);
                            }
                        }
                    }
                }
            }
        }));
        imageProcThread.Start(); // frameWasChanged = true;
        this.rawImage.texture = new Texture2D(cameraWidth, cameraHeight, TextureFormat.RGB24, false);
        
        unsafe {
            fixed(bool* frameWasChangedPtr = &frameWasChanged) {
                while(*frameWasChangedPtr == false) {
                    continue;
                }
                ((Texture2D)this.rawImage.texture).LoadRawTextureData(this.pixels); // RGB
                ((Texture2D)this.rawImage.texture).Apply();
                *frameWasChangedPtr = false;
            }
        }
    }

    void Update()
    {
        if(!IsOwner) {
            return;
        }
        unsafe {
            fixed(bool* shouldRunImageProcPtr = &shouldRunImageProc) {
                fixed(bool* frameWasChangedPtr = &frameWasChanged) {
                    fixed(int* detectedGesturePtr = &detectedGesture) {
                        if (*frameWasChangedPtr == true) {
                            if (*detectedGesturePtr == (int)DetectedGestureEnum.ARMS_UP) {
                                *shouldRunImageProcPtr = false;
                                transform.GetComponent<HelpPanelControl>().DisableWebCameraPanel(rawImage.transform.parent.gameObject);
                                transform.GetComponent<AnimationState>().ResetArmsUpAnimationState();
                                transform.GetComponent<OnCameraRetarget>().OnStartMoveCamera();
                                enabled = false;
                            }
                            ((Texture2D)rawImage.texture).LoadRawTextureData(this.pixels); // RGB
                            ((Texture2D)rawImage.texture).Apply();
                            *frameWasChangedPtr = false;
                        }
                    }
                }
            }
        }
    }

    public override void OnDestroy() {
        if(!IsOwner) {
            return;
        }
        unsafe {
            fixed(bool* shouldRunImageProcPtr = &shouldRunImageProc) {
                *shouldRunImageProcPtr = false;
            }
        }
        while (imageProcThread.ThreadState == ThreadState.Running) {
            continue;
        }
        DLLWrappers.OpenCV.GetInstance().FreeLibraries();
        base.OnDestroy();
    }
}
