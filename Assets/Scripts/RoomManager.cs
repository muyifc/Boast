using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomManager : Singleton<RoomManager> {
    // 房间最大人数
    public const int RoomMaxPlayer = 4;

    private List<RoomControl> rooms;
    private LobbyItem lobby;
    private RoomControl curRoom;

    public void Init(){
        rooms = new List<RoomControl>();
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
        curRoom = CreateRoom();
    }

    public RoomControl CreateRoom(){
        RoomControl control = new RoomControl();
        control.Init();
        rooms.Add(control);
        return control;
    }

    public bool ClearRoom(RoomControl control){
        int index = rooms.IndexOf(control);
        if(index != -1){
            control.Clear();
            rooms.RemoveAt(index);
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
        for(int i = 0;i < 5;++i){
            LobbyRoomItem item = ResourceManager.Instance.Load<LobbyRoomItem>("Prefabs/LobbyRoomItem.prefab");
            item.transform.SetParent(lobby.roomList,false);
            RoomControl room = CreateRoom();
            room.SetRoomID(0);
            item.SetRoomId(0);
        }
    }
}