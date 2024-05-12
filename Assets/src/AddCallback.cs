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
    public GameObject script;
    public List<GameObject> gameObjects = new List<GameObject>();

    public FinishCallback ReturnCallback() {
        enabled = false;
        Type derivedClass = script.GetType();
        MethodInfo method = derivedClass.GetMethod("AddCallback");
        if (method == null) {
            throw new Exception(derivedClass.Name + " doesn't implement `AddCallback method!`");
        }
        object obj = Activator.CreateInstance(derivedClass);
        object[] args = new object[] {gameObjects.ToArray()};
        FinishCallback result = method.Invoke(obj, args) as FinishCallback;
        if(result == null) {
            throw new Exception("Method invocation returned null!");
        }
        return method.Invoke(obj, args) as FinishCallback;
    }
}
