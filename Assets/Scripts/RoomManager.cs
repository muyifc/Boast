using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomManager : Singleton<RoomManager> {
    // 房间最大人数
    public const int RoomMaxPlayer = 4;

    private List<RoomControl> rooms;

    public void Init(){
        rooms = new List<RoomControl>();
    }

    public void Clear(){
        rooms.Clear();
        rooms = null;
    }

    public RoomControl CreateRoom(){
        RoomControl control = new RoomControl();
        control.Init();
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

    /// 添加玩家
    public void AddPlayer(RoomControl control,RoomPlayer player){
        control.AddPlayer(player);
    }

    /// 移除玩家
    public void RemovePlayer(RoomControl control,RoomPlayer player){
        control.RemovePlayer(player);
    }

    /// 因外力而关闭房间，掉线等
    public void CloseRoom(RoomControl control){
        ClearRoom(control);
    }
}