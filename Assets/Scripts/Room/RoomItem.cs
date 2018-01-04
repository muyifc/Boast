using UnityEngine;
using System.Collections;

public class RoomItem : MonoBehaviour {
    private Transform topSeat;
    private Transform bottomSeat;
    private Transform leftSeat;
    private Transform rightSeat;

    private Transform[] seats;

    void Awake(){
        topSeat = gameObject.Find<Transform>("TopSeat");
        bottomSeat = gameObject.Find<Transform>("BottomSeat");
        leftSeat = gameObject.Find<Transform>("leftSeat");
        rightSeat = gameObject.Find<Transform>("RightSeat");
    
        seats[(int)RoomSeatEnum.Top] = topSeat;
        seats[(int)RoomSeatEnum.Bottom] = bottomSeat;
        seats[(int)RoomSeatEnum.Left] = leftSeat;
        seats[(int)RoomSeatEnum.Right] = rightSeat;
    }

    /// 座位对象
    public Transform GetSeat(RoomSeatEnum seatEnum){
        return seats[(int)seatEnum];
    }
}