using UnityEngine;
using System.Collections;

public class LobbyItem : MonoBehaviour {
    public Transform roomList { get; private set; }
    public Transform player { get; private set; }

    void Awake(){
        roomList = transform.Find("RoomList");
        player = transform.Find("Player");
    }
}