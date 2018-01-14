using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomControl {
    public int RoomId {get;private set;}

    private List<PlayerControl> roomPlayers;

    private RoomItem roomItem;

    private RoomStateEnum stateEnum;
    
    public void Init(){
        roomPlayers = new List<PlayerControl>(RoomManager.RoomMaxPlayer);
        stateEnum = RoomStateEnum.Ready;
    }

    public void Clear(){
        roomPlayers.Clear();
        roomPlayers = null;
        clearRoomInstance();
    }

    public void SetRoomID(int roomId){
        RoomId = roomId;
    }

    /// 添加玩家
    public void AddPlayer(PlayerControl player){
        if(roomPlayers.Capacity == roomPlayers.Count){
            Debug.LogWarning("Room is Full");
            return;
        }
        if(!roomPlayers.Contains(player)){
            roomPlayers.Add(player);
            player.SetRoom(this);
            if(player == PlayerManager.Instance.PlayerSelf){
                createRoomInstance();
                roomItem.SetRoomInfo(RoomId);
            }
            updateSeat();
        }
    }

    // 移除玩家
    public void RemovePlayer(PlayerControl player){
        int index = roomPlayers.IndexOf(player);
        if(index != -1){
            player.Clear();
            roomPlayers.RemoveAt(index);
            if(player == PlayerManager.Instance.PlayerSelf){
                clearRoomInstance();
            }
        }
        if(roomPlayers.Count == 0){
            RoomManager.Instance.ClearRoom(this);
        }
    }

    /// 检测所有玩家是否准备好，进入下一状态
    public void CheckState(){
        switch(stateEnum){
            case RoomStateEnum.Ready:
                if(roomPlayers.Count == RoomManager.RoomMaxPlayer){
                    bool isReady = true;
                    for(int i = 0;i < roomPlayers.Count;++i){
                        if(roomPlayers[i].isReadyPlay == false){
                            isReady = false;
                            break;
                        }
                    }
                    if(isReady){
                        changeState(RoomStateEnum.Start);
                    }
                }
                break;
        }
    }

    private void changeState(RoomStateEnum stateEnum){
        Debug.Log("Change State:"+stateEnum.ToString());
        this.stateEnum = stateEnum;
        switch(stateEnum){
            case RoomStateEnum.Ready:
            break;
            case RoomStateEnum.Start:
            CoroutineManager.Instance.StartCoroutine(onWaitStart());
            break;
            case RoomStateEnum.Send:
            break;
        }
    }

    private void createRoomInstance(){
        roomItem = ResourceManager.Instance.Load<RoomItem>("Prefabs/RoomItem.prefab");
        roomItem.transform.SetParent(SceneManager.Instance.UICanvas.transform,false);
        roomItem.OnClose = onClose;
    }

    private void clearRoomInstance(){
        roomItem.OnClose = null;
        GameObject.Destroy(roomItem.gameObject);
        roomItem = null;
    }

    /// 更新玩家座次
    private void updateSeat(){
        Transform[] seats = roomItem.GetSeats();
        int index = roomPlayers.IndexOf(PlayerManager.Instance.PlayerSelf);
        if(index != -1){
            for(int i = 0;i < roomPlayers.Count;++i){
                roomPlayers[(i+index) % roomPlayers.Count].SetSeat(seats[i]);
            }
        }
    }

    private IEnumerator onWaitStart(){
        yield return new WaitForSeconds(1.0f);
        changeState(RoomStateEnum.Send);
    }

    private void onClose(){
        RemovePlayer(PlayerManager.Instance.PlayerSelf);
    }
}