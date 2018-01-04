using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager> {
    public void Init(){
        RoomManager.Instance.Init();
        PlayerManager.Instance.Init();
    }

    public void Start(){
        PlayerManager.Instance.PlayerSelf = PlayerManager.Instance.CreatePlayer();
    }
}