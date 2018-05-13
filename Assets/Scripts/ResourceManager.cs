using UnityEngine;
using System.Collections;
using System.IO;
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
        path = getFullPath(path);
#if UNITY_EDITOR
        path = getRelatePath(path);
        Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
        if(asset == null){
            Debug.LogWarning("该路径下无资源："+path);
        }
        return asset;
#endif  
    }

    private string getFullPath(string path){
        return Path.Combine(Application.dataPath,path);
    }

    private string getRelatePath(string path){
        return path.Substring(path.IndexOf("Assets"));
    }
}