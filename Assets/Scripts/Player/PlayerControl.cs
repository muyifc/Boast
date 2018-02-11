using UnityEngine;
using System.Collections;

public class PlayerControl {
    public PlayerData playerData;
    public RoomControl roomControl;
    public bool isReadyPlay {get;private set;}
    
    private RoomPlayerItem playerItem;
    private Transform roomSeat;

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

    public void SetRoomState(RoomStateEnum stateEnum){
        GetPlayerItem().SetRoomState(stateEnum);
    }

    public void LeaveRoom(RoomControl control){
        if(playerItem != null){
            GameObject.Destroy(playerItem);
            playerItem = null;
        }
    }

    /// 手牌位置自适应
    public void SortHandCards(){
        // Transform hand = GetCardHand();
        // float space = 40;
        // float sx = 0 - (hand.childCount - 1) * 0.5f * space;
        // for(int i = 0;i < hand.childCount;++i){
        //     hand.GetChild(i).localPosition = new Vector3(sx + i*space,0,0);
        // }
    }

    public void SetSeat(Transform parent){
        roomSeat = parent;
        GetPlayerItem().transform.SetParent(GetHead(),false);
    }

    public Transform GetHead(){
        return roomSeat != null ? roomSeat.Find("Player") : null;
    }

    public Transform GetCardHand(){
        return roomSeat != null ? roomSeat.Find("CardHand") : null;
    }

    public Transform GetCardLibrary(){
        return roomSeat != null ? roomSeat.Find("CardLibrary") : null;
    }

    private RoomPlayerItem GetPlayerItem(){
        if(playerItem == null){
            playerItem = ResourceManager.Instance.Load<RoomPlayerItem>("Prefabs/RoomPlayerItem.prefab");
            playerItem.SetData(playerData);
            playerItem.OnReady = onReady;
        }
        return playerItem;
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