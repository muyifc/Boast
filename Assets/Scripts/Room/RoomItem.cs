using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour {
    public System.Action OnClose;

    private Transform topSeat;
    private Transform bottomSeat;
    private Transform leftSeat;
    private Transform rightSeat;
    private Transform[] seats;
    private Transform desk;
    private Transform cardEndRound;
    private Transform willDestroy;
    private Text roomInfo;
    private Button btnClose;
    private int roomId;

    /// 座位对象
    public Transform GetSeat(RoomSeatEnum seatEnum){
        return seats[(int)seatEnum];
    }

    /// 当前玩家的顺序座位
    public Transform[] GetSeats(){
        return seats;
    }

    /// 牌桌
    public Transform GetDesk(){
        return desk;
    }

    public Transform GetCardEndRound(){
        return cardEndRound;
    }

    public Transform GetWillDestroy(){
        return willDestroy;
    }

    public void SetRoomInfo(int roomId){
        this.roomId = roomId;
        roomInfo.text = string.Format("Room {0}",roomId);
    }

    void Awake(){
        topSeat = gameObject.Find<Transform>("Seats/TopSeat");
        bottomSeat = gameObject.Find<Transform>("Seats/BottomSeat");
        leftSeat = gameObject.Find<Transform>("Seats/LeftSeat");
        rightSeat = gameObject.Find<Transform>("Seats/RightSeat");

        seats = new Transform[RoomManager.RoomMaxPlayer];
        seats[(int)RoomSeatEnum.Bottom] = bottomSeat;
        seats[(int)RoomSeatEnum.Right] = rightSeat;
        seats[(int)RoomSeatEnum.Top] = topSeat;
        seats[(int)RoomSeatEnum.Left] = leftSeat;

        roomInfo = gameObject.Find<Text>("Info/Text");
        btnClose = gameObject.Find<Button>("BtnClose/Button");

        desk = gameObject.Find<Transform>("Desk");
        cardEndRound = gameObject.Find<Transform>("EndRoundArea");
        willDestroy = gameObject.Find<Transform>("WillDestroyArea");
    }

    void OnEnable(){
        btnClose.onClick.AddListener(onClose);
    }

    void OnDisable(){
        btnClose.onClick.RemoveAllListeners();
    }

    private void onClose(){
        if(OnClose != null){
            OnClose();
        }
    }
}