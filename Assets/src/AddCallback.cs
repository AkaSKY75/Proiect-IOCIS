using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public class IAddCallback : MonoBehaviour
{
    public virtual FinishCallback AddCallback(object[] args) {
        if (args.Length == 0) {
            throw new Exception ("Invalid number of arguments!");
        }
        foreach (var arg in args) {
            if ((arg as GameObject) == null) {
                throw new Exception ("`AddCallback` is expected to be called using arguments of Type=`GameObject`");
            }
        }
        return delegate{};
    }
}

public class AddCallback : MonoBehaviour
{
    [SerializeField]
    private List<MonoBehaviour> scripts;
    [SerializeField]
    private List<GameObject> gameObjects;

    public FinishCallback ReturnCallback() {
        FinishCallback delegates = null;
        if (gameObjects == null) {
            gameObjects = new List<GameObject>();
        }
        foreach (var script in scripts) {
            if (script.enabled == true) {
                Type derivedClass = script.GetType();
                MethodInfo method = derivedClass.GetMethod("AddCallback");
                if (method == null) {
                    throw new Exception(derivedClass.Name + " doesn't implement `AddCallback method!`");
                }
                object[] args = new object[] {gameObjects.ToArray()};
                FinishCallback result = method.Invoke(script, args) as FinishCallback;
                if(result == null) {
                    throw new Exception("Method invocation returned null!");
                }
                delegates += result;
            }
        }
        if (delegates == null) {
            throw new Exception("Callback list of delegates returned null!");
        }
        return delegates;
    }
}
