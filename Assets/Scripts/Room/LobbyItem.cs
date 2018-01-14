using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LobbyItem : MonoBehaviour {
    public class LobbyJoinSignal : Signal {}
    public class LobbyCreateSignal : Signal {}

    public Transform roomList { get; private set; }
    public Transform player { get; private set; }

    private Button btnJoin;
    private Button btnCreate;

    void Awake(){
        roomList = transform.Find("RoomList");
        player = transform.Find("Player");

        btnJoin = gameObject.Find<Button>("Operate/BtnJoin/Button");
        btnCreate = gameObject.Find<Button>("Operate/BtnCreate/Button");
    }

    void OnEnable(){
        btnJoin.onClick.AddListener(onJoin);
        btnCreate.onClick.AddListener(onCreate);
    }

    void OnDisable(){
        btnJoin.onClick.RemoveAllListeners();
        btnCreate.onClick.RemoveAllListeners();
    }

    private void onJoin(){
        SignalManager.Instance.Create<LobbyJoinSignal>().Dispatch();
    }

    private void onCreate(){
        SignalManager.Instance.Create<LobbyCreateSignal>().Dispatch();
    }
}