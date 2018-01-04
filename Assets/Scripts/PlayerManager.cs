using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : Singleton<PlayerManager> {
    private int uuid = 0;

    /// 获得用户标识ID
    public int UUID {
        get{
            return uuid++;
        }
    }

    public PlayerControl PlayerSelf {get;set;}

    private List<PlayerControl> players;

    public void Init(){
        players = new List<PlayerControl>();
    }

    public void Clear(){
        players.Clear();
        players = null;
    }

    /// 参加一个用户
    public PlayerControl CreatePlayer(){
        PlayerControl player = new PlayerControl();
        PlayerData data = new PlayerData();
        data.UUID = UUID;
        data.Name = string.Format("Player_{0}",data.UUID);
        player.SetData(data);
        return player;
    }
}