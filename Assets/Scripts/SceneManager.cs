using UnityEngine;
using System.Collections;

public class SceneManager : Singleton<SceneManager> {
    public Canvas UICanvas { get; private set; }

    public void Init(){
        UICanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }
}