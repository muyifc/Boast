using UnityEngine;
using System.Collections;

public class SceneManager : Singleton<SceneManager> {
    public Canvas UICanvas { get; private set; }
    public LobbyItem Lobby {get;private set;}

    public void Init(){
        UICanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    /// 创建游戏大厅
    public void CreateLobby(){
        if(Lobby != null) return;
        Lobby = ResourceManager.Instance.Load<LobbyItem>("Prefabs/Lobby.prefab");
        Lobby.transform.SetParent(UICanvas.transform,false);
    }
}