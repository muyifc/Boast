using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomControl {
    public int RoomId {get;private set;}

    private List<PlayerControl> roomPlayers;

    private RoomItem roomItem;
    
    public void Init(){
        roomPlayers = new List<PlayerControl>(RoomManager.RoomMaxPlayer);
        roomItem = ResourceManager.Instance.Load<RoomItem>("Prefabs/RoomItem");
    }

    public void Clear(){
        roomPlayers.Clear();
        roomPlayers = null;
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
            updateSeat();
        }
    }

    // 移除玩家
    public void RemovePlayer(PlayerControl player){
        int index = roomPlayers.IndexOf(player);
        if(index != -1){
            player.Clear();
            roomPlayers.RemoveAt(index);
        }
    }

    /// 更新玩家座次
    private void updateSeat(){
        
    }
}