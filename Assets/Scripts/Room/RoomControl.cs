using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomControl {
    public int RoomId {get;private set;}

    public List<PlayerControl> roomPlayers;

    private RoomItem roomItem;

    private RoomStateEnum stateEnum;
    private LobbyRoomItem lobbyItem;

    private SendCardControl sendCardControl;

    public void Init(){
        roomPlayers = new List<PlayerControl>(RoomManager.RoomMaxPlayer);
        stateEnum = RoomStateEnum.Ready;
        sendCardControl = new SendCardControl();
        sendCardControl.Init(this);
    }

    public void Clear(){
        roomPlayers.Clear();
        roomPlayers = null;
        clearLobbyItem();
        clearRoomInstance();
        sendCardControl.Clear();
        sendCardControl = null;
    }

    public void SetRoomID(int roomId){
        RoomId = roomId;
        createLobbyItem();
    }

    public Transform GetRoomDesk(){
        return roomItem.GetDesk();
    }

    public void MoveCard(RoomCardPosEnum from,RoomCardPosEnum to,int uid){
        CardControl cc = sendCardControl.GetCardControl(uid);
        cc.MoveCard(GetCardPos(from),GetCardPos(to),to);
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
        lobbyItem.SetRoomPlayer(roomPlayers.Count);
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
        lobbyItem.SetRoomPlayer(roomPlayers.Count);
        
        bool isAllRobot = true;
        for(int i = 0;i < roomPlayers.Count;++i){
            if(roomPlayers[i].playerData.isRobot == false){
                isAllRobot = false;
                break;
            }
        }
        if(roomPlayers.Count == 0 || isAllRobot){
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
            case RoomStateEnum.Start:
            break;
        }
    }

    /// 状态切换
    private void changeState(RoomStateEnum stateEnum){
        this.stateEnum = stateEnum;
        switch(stateEnum){
            case RoomStateEnum.Ready:
            break;
            case RoomStateEnum.Start:
                sendCardControl.InitDeskCard(()=>{
                    changeState(RoomStateEnum.Send);
                });
            break;
            case RoomStateEnum.Send:
                sendCardControl.StartGameSendCard();
            break;
        }
    }

    private void createRoomInstance(){
        roomItem = ResourceManager.Instance.Load<RoomItem>("Prefabs/RoomItem.prefab");
        roomItem.transform.SetParent(SceneManager.Instance.UICanvas.transform,false);
        roomItem.OnClose = onClose;
    }

    private void clearRoomInstance(){
        if(roomItem == null) return;
        roomItem.OnClose = null;
        GameObject.Destroy(roomItem.gameObject);
        roomItem = null;
    }

    private void createLobbyItem(){
        if(lobbyItem == null){
            lobbyItem = ResourceManager.Instance.Load<LobbyRoomItem>("Prefabs/LobbyRoomItem.prefab");
            lobbyItem.transform.SetParent(SceneManager.Instance.Lobby.roomList,false);
            lobbyItem.SetRoomId(RoomId);
        }
    }

    private void clearLobbyItem(){
        if(lobbyItem != null){
            GameObject.Destroy(lobbyItem.gameObject);
            lobbyItem = null;
        }
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

    /// 舞台中卡牌的对应位置
    private Transform GetCardPos(RoomCardPosEnum posEnum,int playerUId = 0){
        Transform pos = null;
        switch(posEnum){
            case RoomCardPosEnum.PublicDesk:
                pos = GetRoomDesk();
            break;
            case RoomCardPosEnum.PlayerHand:
                PlayerControl pc = PlayerManager.Instance.GetPlayer(playerUId);
                if(pc != null){
                    pos = pc.GetCardHand();
                }
            break;
            case RoomCardPosEnum.PlayerLibrary:
                pc = PlayerManager.Instance.GetPlayer(playerUId);
                if(pc != null){
                    pos = pc.GetCardLibrary();
                }
            break;
            case RoomCardPosEnum.TmpRound:
            break;
            case RoomCardPosEnum.EndRound:
                pos = roomItem.GetCardEndRound();
            break;
            case RoomCardPosEnum.WillDestroy:
                pos = roomItem.GetWillDestroy();
            break;
        }
        return pos;
    }

    private void onClose(){
        RemovePlayer(PlayerManager.Instance.PlayerSelf);
    }
}