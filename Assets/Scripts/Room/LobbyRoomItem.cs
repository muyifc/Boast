using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LobbyRoomItem : MonoBehaviour {
    private Text roomName;
    private Text curNum;
    private Button btnJoin;
    private int roomId;

    public void SetRoomId(int id){
        roomId = id;
    }

    void Awake(){
        roomName = gameObject.Find<Text>("Name/Text");
        curNum = gameObject.Find<Text>("Num/Text");
        btnJoin = gameObject.Find<Button>("BtnJoin/Button");

        btnJoin.onClick.AddListener(onJoin);
    }

    void OnDestroy(){
        btnJoin.onClick.RemoveListener(onJoin);
    }

    private void onJoin(){
        // TODO
        GameManager.Instance.ChangeState(GameStateEnum.Room);
        RoomManager.Instance.AddPlayer(RoomManager.Instance.GetRoom(roomId),PlayerManager.Instance.PlayerSelf);
    }
}