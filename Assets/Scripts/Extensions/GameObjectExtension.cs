using UnityEngine;
using System.Collections;

public static class GameObjectExtension {
    public static T Find<T>(this GameObject go,string path) where T : Component{
        return go.transform.Find(path).GetComponent<T>();
    }

    public static GameObject FindChild(this GameObject go,string path){
        return go.transform.Find(path).gameObject;
    }

    public static void SetParentWithZero(this Transform child,Transform parent,bool worldStays = true){
        child.SetParent(parent,worldStays);
        child.localPosition = new Vector3(0,0,0);
        child.localScale = new Vector3(1,1,1);
        child.localRotation = Quaternion.identity;
    }
}