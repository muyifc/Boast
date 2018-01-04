using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ResourceManager : Singleton<ResourceManager> {
    /// 加载预制资源
    public GameObject Load(string path){
#if UNITY_EDITOR
        return GameObject.Instantiate(getAsset(path)) as GameObject;
#endif
    }

    /// 加载预制资源组件
    public T Load<T>(string path) where T : Component {
        return Load(path).GetComponent<T>();
    }

    /// 加载原始资源
    public T LoadAs<T>(string path) where T : Object {
        return getAsset(path) as T;
    }

    /// 加载 Bundle
    private Object getAsset(string path){
#if UNITY_EDITOR
        Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
        return asset;
#endif  
    }
}