using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Runtime.InteropServices;
using System;
using UnityEditor.SearchService;
using System.Data;
using UnityEngine.Diagnostics;
using System.Text;
namespace DLLWrappers {

public class Pair<T, U> {
    public Pair() {
    }

    public Pair(T first, U second) {
        this.First = first;
        this.Second = second;
    }

    public T First { get; set; }
    public U Second { get; set; }
};

public class DLLWrapper : MonoScript
{
    protected IntPtr hModule;
    protected string libPath;
    protected Dictionary<string, Pair<Delegate, Type>> delegates;
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetModuleHandle(string dllToLoad);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr LoadLibrary(string dllToLoad);
    /*[DllImport("Kernel32.dll", EntryPoint = "LoadLibraryW", CallingConvention = CallingConvention.Winapi)]
    public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPWStr)] string lpFileName);*/

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr LoadLibraryEx(string dllToLoad, IntPtr hFile, LoadLibraryFlags flags);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int AddDllDirectory(string NewDirectory);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool FreeLibrary(IntPtr hModule);
    public virtual void FreeLibraries()
    {
        FreeLibrary(hModule);
    }

        [System.Flags]
    public enum LoadLibraryFlags : uint
    {
        DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
        LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
        LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
        LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,
        LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
        LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008,
        LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR = 0x00000100,
        LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800,
        LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000
    }
    public static Delegate LoadFunction(IntPtr hModule, Type T, string functionName)
    {
        try {
            if (hModule == IntPtr.Zero) {
                throw new Exception ("hModule is nullptr!");
            }
            var functionAddress = GetProcAddress(hModule, functionName);
            if (functionAddress == IntPtr.Zero) {
                throw new Exception ("functionAddress is nullptr!");
            }
            return Marshal.GetDelegateForFunctionPointer(functionAddress, T);
        } catch (Exception e) {
            Debug.LogWarning("Exception: " + e + "(Error " + Marshal.GetLastWin32Error() + ": " + new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()).Message + ")");
            // Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.FatalError);
            Application.Quit(Marshal.GetLastWin32Error());
        }
        return new Action(delegate {});
    }
}

public class OpenCV : DLLWrapper {
    private delegate int DetectGestureD(IntPtr shouldRunImageProcPtr, IntPtr frameWasChangedPtr, IntPtr errorPath, IntPtr modelsPath, IntPtr pixels);
    const string detectGestureFN = "DetectGesture";
    private readonly string opencvWrapperLibPath = Application.dataPath + "\\ThirdParty\\opencv";
    private readonly string openvinoWrapperLibPath = Application.dataPath + "\\ThirdParty\\openvino";
    private readonly string modelsPath = Application.dataPath + "/ThirdParty/openvino/models";
    private readonly string errorLogPath = Application.dataPath;
    private static OpenCV singleton;
    public static OpenCV GetInstance()
    {
        if (singleton == null)
        {
                singleton = new OpenCV();
        }
        return singleton;
    }
    public void LoadLibraries() {
        libPath = opencvWrapperLibPath + "opencv_wrapper.dll";
        
        var res = AddDllDirectory(openvinoWrapperLibPath);
        if (res == 0) {
            throw new Exception("Couldn't add DLL directory openvino! Error is: " + Marshal.GetLastWin32Error());
        }
        delegates = new Dictionary<string, Pair<Delegate, Type>> {
            {detectGestureFN, new Pair<Delegate, Type>(null, typeof(DetectGestureD))}
        };
        LoadLibraryFlags flags = LoadLibraryFlags.LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR | LoadLibraryFlags.LOAD_LIBRARY_SEARCH_DEFAULT_DIRS;
        hModule = LoadLibraryEx(libPath, IntPtr.Zero, flags);
        foreach(var loadFunction in delegates) { 
            loadFunction.Value.First = LoadFunction(hModule, loadFunction.Value.Second, loadFunction.Key);
        }
    }

    public int DetectGesture(IntPtr shouldRunImageProcPtr, IntPtr frameWasChangedPtr, IntPtr pixels) {
       if (hModule == IntPtr.Zero) {
            LoadLibraries();
            return -2;
       }
       
       int ret = -1;
       char[] errorLogPathCharArray = errorLogPath.ToCharArray();
       char[] modelsPathCharArray = modelsPath.ToCharArray();
       unsafe {
            fixed (byte* errorLogPathPtr = &Encoding.GetEncoding("UTF-8").GetBytes(errorLogPathCharArray)[0], modelsPathPtr = &Encoding.GetEncoding("UTF-8").GetBytes(modelsPathCharArray)[0]) {
                ret = ((DetectGestureD)delegates[detectGestureFN].First)(shouldRunImageProcPtr, frameWasChangedPtr, (IntPtr)errorLogPathPtr, (IntPtr)modelsPathPtr, pixels);
            }
       }
       return ret;
    }
}

}  // namespace DLLWrappers
