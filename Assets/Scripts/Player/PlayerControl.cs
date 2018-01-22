using UnityEngine;
using System.Collections;

public class PlayerControl {
    public PlayerData playerData;
    public RoomControl roomControl;
    public bool isReadyPlay {get;private set;}
    
    private RoomPlayerItem playerItem;

    public void Init(){
    }

    public void Clear(){
        if(playerItem != null){
            GameObject.Destroy(playerItem.gameObject);
            playerItem = null;
        }
    }

    public void SetData(PlayerData data){
        playerData = data;
    }

    public void SetRoom(RoomControl control){
        roomControl = control;
    }

    public void LeaveRoom(RoomControl control){
        if(playerItem != null){
            GameObject.Destroy(playerItem);
            playerItem = null;
        }
    }

    public void SetSeat(Transform parent){
        if(playerItem == null){
            playerItem = ResourceManager.Instance.Load<RoomPlayerItem>("Prefabs/RoomPlayerItem.prefab");
        }
        playerItem.transform.SetParent(parent,false);
        playerItem.SetData(playerData);
        playerItem.OnReady = onReady;
    }

    public Transform GetHead(){
        return playerItem.transform;
    }

    public Transform GetCardHand(){
        return playerItem.transform.parent != null ? playerItem.transform.Find("CardHand") : null;
    }

    public Transform GetCardLibrary(){
        return playerItem.transform.parent != null ? playerItem.transform.Find("CardLibrary") : null;
    }

    private void onReady(bool isReady){
        isReadyPlay = isReady;
        roomControl.CheckState();
    }

    /// test
    public void Ready(){
        onReady(true);
    }
}