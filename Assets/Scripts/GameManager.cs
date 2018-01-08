using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager> {
    public GameStateEnum stateEnum;

    public void Init(){
        SceneManager.Instance.Init();
        RoomManager.Instance.Init();
        PlayerManager.Instance.Init();
    }

    public void Start(){
        PlayerManager.Instance.PlayerSelf = PlayerManager.Instance.CreatePlayer();
    
        ChangeState(GameStateEnum.Lobby);
    }

    public void ChangeState(GameStateEnum stateEnum){
        this.stateEnum = stateEnum;
        switch(stateEnum){
            case GameStateEnum.Lobby:
                RoomManager.Instance.EnterLobby();
                break;
            case GameStateEnum.Room:
                RoomManager.Instance.EnterRoom();
                break;
        }
    }
}