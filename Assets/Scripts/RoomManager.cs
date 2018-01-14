using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomManager : Singleton<RoomManager> {
    // 房间最大人数
    public const int RoomMaxPlayer = 4;

    private List<RoomControl> rooms;
    private LobbyItem lobby;
    private RoomControl curRoom;

    private int roomUId = 0;
    private int RoomUId { 
        get {
            return roomUId++;
        }
    }

    public void Init(){
        rooms = new List<RoomControl>();

        SignalManager.Instance.Create<LobbyItem.LobbyJoinSignal>().AddListener(onJoinRoom);
        SignalManager.Instance.Create<LobbyItem.LobbyCreateSignal>().AddListener(onCreateRoom);
    }

    public void Clear(){
        rooms.Clear();
        rooms = null;
    }

    /// 进入大厅
    public void EnterLobby(){
        lobby = ResourceManager.Instance.Load<LobbyItem>("Prefabs/Lobby.prefab");
        lobby.transform.SetParent(SceneManager.Instance.UICanvas.transform,false);
    
        updateAllRooms();
    }

    public void EnterRoom(){
    }

    public RoomControl CreateRoom(){
        RoomControl control = new RoomControl();
        control.Init();
        control.SetRoomID(RoomUId);
        rooms.Add(control);
        updateAllRooms();
        return control;
    }

    public bool ClearRoom(RoomControl control){
        int index = rooms.IndexOf(control);
        if(index != -1){
            control.Clear();
            rooms.RemoveAt(index);
            updateAllRooms();
            return true;
        }
        return false;
    }

    public RoomControl GetRoom(int roomId){
        for(int i = 0;i < rooms.Count;++i){
            if(rooms[i].RoomId == roomId){
                return rooms[i];
            }
        }
        return null;
    }

    /// 添加玩家
    public void AddPlayer(RoomControl control,PlayerControl player){
        control.AddPlayer(player);
    }

    /// 移除玩家
    public void RemovePlayer(RoomControl control,PlayerControl player){
        control.RemovePlayer(player);
    }

    /// 因外力而关闭房间，掉线等
    public void CloseRoom(RoomControl control){
        ClearRoom(control);
    }

    private void updateAllRooms(){
        for(int i = 0;i < rooms.Count;++i){
            LobbyRoomItem item = ResourceManager.Instance.Load<LobbyRoomItem>("Prefabs/LobbyRoomItem.prefab");
            item.transform.SetParent(lobby.roomList,false);
            item.SetRoomId(rooms[i].RoomId);
        }
    }

    /// 快速加入房间
    private void onJoinRoom(){
        if(rooms.Count > 0){
            RoomControl rc = rooms[Random.Range(0,rooms.Count)];
            rc.AddPlayer(PlayerManager.Instance.PlayerSelf);
        }
    }

    /// 创建房间
    private void onCreateRoom(){
        RoomControl room = CreateRoom();
        room.AddPlayer(PlayerManager.Instance.PlayerSelf);
        GameManager.Instance.ChangeState(GameStateEnum.Room);

        addRobot();
    }

    private void addRobot(){
        RoomControl rc = GetRoom(PlayerManager.Instance.PlayerSelf.roomControl.RoomId);
        // test 添加机器人测试
        for(int i = 0;i < 3;++i){
            PlayerControl pc = PlayerManager.Instance.CreatePlayer();
            pc.Init();
            pc.SetRoom(rc);
            RoomManager.Instance.AddPlayer(rc,pc);
            pc.Ready();
        }
    }
}