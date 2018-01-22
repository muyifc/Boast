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
    private Dictionary<int,int> playerUId2Index;

    public void Init(){
        players = new List<PlayerControl>();
        playerUId2Index = new Dictionary<int,int>();
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
        player.Init();
        player.SetData(data);
        players.Add(player);
        playerUId2Index.Add(data.UUID,players.Count-1);
        return player;
    }

    /// 获得用户对象
    public PlayerControl GetPlayer(int uid){
        if(playerUId2Index.ContainsKey(uid)){
            return players[playerUId2Index[uid]];
        }
        return null;
    }
}