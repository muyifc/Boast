using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl {
    public PlayerData playerData;
    public RoomControl roomControl;
    public bool isReadyPlay {get;private set;}
    
    private RoomPlayerItem playerItem;
    private Transform roomSeat;
    private List<CardControl> bidCards;

    public void Init(){
        SignalManager.Instance.Create<RoomControl.NotifyBidSignal>().AddListener(onSaleForBid);
    }

    public void Clear(){
        if(playerItem != null){
            GameObject.Destroy(playerItem.gameObject);
            playerItem = null;
        }
        SignalManager.Instance.Create<RoomControl.NotifyBidSignal>().RemoveListener(onSaleForBid);
    }

    public void SetData(PlayerData data){
        playerData = data;
    }

    public void SetRoom(RoomControl control){
        roomControl = control;
    }

    public void SetRoomState(RoomStateEnum stateEnum){
        GetPlayerItem().SetRoomState(stateEnum);
    }

    public void LeaveRoom(RoomControl control){
        if(playerItem != null){
            GameObject.Destroy(playerItem);
            playerItem = null;
        }
    }

    /// 手牌位置自适应
    public void SortHandCards(){
        // Transform hand = GetCardHand();
        // float space = 40;
        // float sx = 0 - (hand.childCount - 1) * 0.5f * space;
        // for(int i = 0;i < hand.childCount;++i){
        //     hand.GetChild(i).localPosition = new Vector3(sx + i*space,0,0);
        // }
    }

    public void SetSeat(Transform parent){
        roomSeat = parent;
        GetPlayerItem().transform.SetParent(GetHead(),false);
    }

    public Transform GetHead(){
        return roomSeat != null ? roomSeat.Find("Player") : null;
    }

    public Transform GetCardHand(){
        return roomSeat != null ? roomSeat.Find("CardHand") : null;
    }

    public Transform GetCardLibrary(){
        return roomSeat != null ? roomSeat.Find("CardLibrary") : null;
    }

    /// 获取当前拥有的动物卡
    public List<int> GetAnimalCards(){
        List<int> allcards = roomControl.sendCardControl.GetBindCards(playerData.UUID);
        List<int> ret = new List<int>();
        for(int i = 0;i < allcards.Count;++i){
            if(roomControl.sendCardControl.GetCardControl(allcards[i]).cardData.CardTypeEnum == CardTypeEnum.Animal){
                ret.Add(allcards[i]);
            }
        }
        return ret;
    }

    private RoomPlayerItem GetPlayerItem(){
        if(playerItem == null){
            playerItem = ResourceManager.Instance.Load<RoomPlayerItem>("Prefabs/RoomPlayerItem.prefab");
            playerItem.SetData(playerData);
            playerItem.OnReady = onReady;
        }
        return playerItem;
    }

    private void onReady(bool isReady){
        isReadyPlay = isReady;
        roomControl.CheckState();
    }

    /// test
    public void Ready(){
        onReady(true);
    }

    /// 同意购买
    public void PayFlipCard(){
        if(bidCards != null){
            for(int i = 0;i < bidCards.Count;++i){
                roomControl.sendCardControl.UnBindCard(playerData.UUID,bidCards[i].cardData.UUID);
                roomControl.sendCardControl.BindCard(roomControl.curRoundPlayer.playerData.UUID,bidCards[i].cardData.UUID);
            }
            roomControl.sendCardControl.BindCard(playerData.UUID,roomControl.curFlipCard.cardData.UUID);
        }
        bidCards = null;
    }

    private void onSaleForBid(){
        if(this == roomControl.curRoundPlayer) return;
        GetPlayerItem().ShowBidProcess(true);
        CoroutineManager.Instance.StartCoroutineWait(Random.value+0.5f,onCompleteBid);
    }

    /// 完成思考，开始出价
    private void onCompleteBid(){
        int value = roomControl.CurBidPrice;
        // TODO 电脑策略，暂时使用3种
        List<int> cards = roomControl.sendCardControl.GetBindCards(playerData.UUID);
        bidCards = new List<CardControl>();
        int curCardPrice = 0;
        for(int i = 0;i < cards.Count;++i){
            CardControl card = roomControl.sendCardControl.GetCardControl(cards[i]);
            Coins data = card.cardData.GetData<Coins>();
            if(data != null){
                    // TODO 最优解不同面值组合刚好达到目标价格
                    // 临时简单叠加，够了就不继续了
                bidCards.Add(card);
                curCardPrice += data.Value;
                if(curCardPrice > value){
                    break;
                }
            }
        }
        if(curCardPrice > value){
            SignalManager.Instance.Create<RoomControl.CompleteBidSignal>().Dispatch(this,curCardPrice);
            GetPlayerItem().ShowBidPrice(curCardPrice);
        }
    }
}