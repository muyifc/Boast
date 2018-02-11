using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoomPlayerItem : MonoBehaviour {
    public System.Action<bool> OnReady;

    private Text nameText;
    private Button btnReady;
    private PlayerData playerData;
    private bool isReady;

    public void SetData(PlayerData data){
        playerData = data;
        nameText.text = playerData.Name;
        btnReady.interactable = playerData == PlayerManager.Instance.PlayerSelf.playerData;
    }

    /// 当前房间状态
    public void SetRoomState(RoomStateEnum stateEnum){
        btnReady.gameObject.SetActive(stateEnum == RoomStateEnum.Ready);
    }

    void Awake(){
        nameText = gameObject.Find<Text>("Name/Text");
        btnReady = gameObject.Find<Button>("BtnReady/Button");
    }

    void OnEnable(){
        btnReady.onClick.AddListener(onReady);
    }

    void OnDisable(){
        btnReady.onClick.RemoveAllListeners();
    }

    private void onReady(){
        isReady = !isReady;
        OnReady(isReady);
        btnReady.image.color = isReady ? Color.green : Color.white;
    }
}