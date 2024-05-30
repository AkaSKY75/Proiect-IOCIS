using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VocalAssistant : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DLLWrappers.Speech.GetInstance().LoadLibraries();
       int ret = DLLWrappers.Speech.GetInstance().RunPythonScript();
        Debug.Log(ret +" da");
    }

    // Update is called once per frame
    void Update()
    {
        enabled = false;
    }
}
