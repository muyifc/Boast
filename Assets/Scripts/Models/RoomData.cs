using UnityEngine;
using System.Collections;

public class RoomData {
    public int RoomId;
    // 选择牌后随机决策时间
    public float ChooseCD = 1;
    // 翻到发钱牌的卡牌ID
    public int SendCoinCardId = 6;
    // 翻到驴需要给玩家发对应的金币卡
    public int[] DonkeySendCards = new int[]{3,4,5,6};
    // 翻出驴牌的次数
    public int FlipDonkeyCount = 0;
    // 当前出价
    public int CurBidPrice;
    // 等待下次竞价间隔 s
    public float WaitNextBidCD = 3;
}