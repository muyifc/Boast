using UnityEngine;
using System.Collections;

public class PlayerControl {
    private PlayerData playerData;
    private PlayerItem playerItem;
    private RoomControl roomControl;

    public void Init(){
        playerItem = ResourceManager.Instance.Load<PlayerItem>("Prefabs/PlayerItem");
    }

    public void Clear(){
        GameObject.Destroy(playerItem.gameObject);
        playerItem = null;
    }

    public void SetData(PlayerData data){
        playerData = data;
    }

    public void SetRoom(RoomControl control){
        roomControl = control;
    }
}