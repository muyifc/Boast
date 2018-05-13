using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {
    void Awake() {
        GameManager.Instance.Init();
    }

    void Start() {
        GameManager.Instance.Start();
    }

    void Update() {
        GameManager.Instance.Update();
    }
}