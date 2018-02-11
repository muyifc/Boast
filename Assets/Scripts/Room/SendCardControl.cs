using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SendCardControl {
    /// 初始卡牌设置
    public class InitCard {
        public int cardId;
        public List<int> cardUIds;
        public int initCountPer;
        public int maxCount;
    }

    private RoomControl roomControl;
    private Card[] allCards;
    private int[] animalCards;
    private int[] coinCards;
    private Dictionary<int,int> cardUUId2Index;
    private Dictionary<int,InitCard> startCards;
    private Dictionary<int,CardControl> cardUUId2Control;

    // 玩家绑定的牌
    private Dictionary<int,List<int>> playerBindCards;
    private Dictionary<int,int> cardUId2Player;

    /// 卡牌位置列表
    private Dictionary<RoomCardPosEnum,List<int>> cardPosList;

    // 当前绑定的移动卡牌
    private HashSet<int> bindMove;

    /// 每种动物各4张
    private const int MaxAnimalPer = 4;
    
    public void Init(RoomControl control){
        roomControl = control;
        cardUUId2Index = new Dictionary<int, int>();
        cardUUId2Control = new Dictionary<int,CardControl>();
        playerBindCards = new Dictionary<int, List<int>>();
        cardUId2Player = new Dictionary<int,int>();
        bindMove = new HashSet<int>();
        cardPosList = new Dictionary<RoomCardPosEnum, List<int>>();

        initCards();
    }

    public void Clear(){
        roomControl = null;
    }

    /// 获取卡牌绑定的卡牌控制器
    public CardControl GetCardControl(int cardUId){
        if(cardUUId2Control.ContainsKey(cardUId)){
            return cardUUId2Control[cardUId];
        }
        return null;
    }

    /// 当前卡牌绑定的玩家
    public PlayerControl GetCardPlayer(int cardUId){
        if(cardUId2Player.ContainsKey(cardUId)){
            return PlayerManager.Instance.GetPlayer(cardUId2Player[cardUId]);
        }
        return null;
    }

    /// 初始化牌桌的牌
    public void InitDeskCard(System.Action callback){
        CoroutineManager.Instance.StartCoroutineWithCallback(onInitDeskCard(),callback);
    }

    /// 开局发牌
    public void StartGameSendCard(){
        Dictionary<int,InitCard> startCards = getStartCards();
        foreach(InitCard ic in startCards.Values){
            for(int i = 0 ;i < roomControl.roomPlayers.Count;++i){
                for(int m = 0;m < ic.initCountPer;++m){
                    BindCard(roomControl.roomPlayers[i].playerData.UUID,ic.cardUIds[i * ic.initCountPer+m]);
                }
            }
        }
    }

    /// 绑定玩家与卡牌
    public void BindCard(int playerUId,int cardUId){
        if(!playerBindCards.ContainsKey(playerUId)){
            playerBindCards.Add(playerUId,new List<int>(allCards.Length));
        }
        playerBindCards[playerUId].Add(cardUId);
        cardUId2Player.Add(cardUId,playerUId);
        bindMove.Add(cardUId);
        moveCardPosEnum(cardUId,RoomCardPosEnum.PlayerHand);
    }

    /// 解绑玩家与卡牌
    public void UnBindCard(int playerUId,int cardUId){
        if(playerBindCards.ContainsKey(playerUId)){
            playerBindCards[playerUId].Remove(cardUId);
            cardUId2Player.Remove(cardUId);
        }
    }

    /// 开局生成本局用到的所有卡牌
    private void initCards(){
        List<Animals> animals = DataManager.Instance.GetAll<Animals>();
        List<Coins> coins = DataManager.Instance.GetAll<Coins>();
        int coinCount = 0;
        for(int i = 0;i < coins.Count;++i){
            coinCount += coins[i].MaxCount;
        }
        
        animalCards = new int[animals.Count*MaxAnimalPer];
        coinCards = new int[coinCount];
        allCards = new Card[animalCards.Length + coinCards.Length];

        int count = 0;
        for(int i = 0;i < MaxAnimalPer;++i){
            for(int j = 0;j < animals.Count;++j){
                Card card = createCard(animals[j].Name,animals[j].Value,typeof(Animals),animals[j].ID(),CardTypeEnum.Animal);
                allCards[count] = card;
                cardUUId2Index.Add(card.UUID,count);
                animalCards[count] = card.UUID;
                count++;
            }
        }

        for(int j = 0;j < coins.Count;++j){
            for(int i = 0;i < coins[j].MaxCount;++i){
                Card card = createCard(coins[j].Name,coins[j].Value,typeof(Coins),coins[j].ID(),CardTypeEnum.Coin);
                allCards[count] = card;
                cardUUId2Index.Add(card.UUID,count);
                coinCards[count-animalCards.Length] = card.UUID;
                count++;
            }
        }
    }

    /// 创建卡牌
    private Card createCard(string name,int value,Type dataType,int id,CardTypeEnum typeEnum){
        Card card = new Card(name,value);
        card.SetDataFrom(dataType,id);
        card.SetCardType(typeEnum);

        CardControl cc = new CardControl();
        cc.Init(card);
        cardUUId2Control.Add(card.UUID,cc);
        return card;
    }

    /// 获取所以初始卡牌
    private Dictionary<int,InitCard> getStartCards(){
        if(startCards != null) return startCards;
        startCards = new Dictionary<int,InitCard>();
        for(int i = 0;i < coinCards.Length;++i){
            Coins coins = GetCard(coinCards[i]).GetData<Coins>();
            int id = coins.ID();
            if(coins.InitCountPer > 0){
                if(!startCards.ContainsKey(id)){
                    InitCard ic = new InitCard(){
                        cardId = id,
                        initCountPer = coins.InitCountPer,
                        maxCount = coins.MaxCount,
                        cardUIds = new List<int>(coins.MaxCount),
                    };
                    startCards.Add(coins.ID(),ic);
                }
                startCards[id].cardUIds.Add(coinCards[i]);
            }
        }
        return startCards;
    }

    private Card GetCard(int UUID){
        if(cardUUId2Index.ContainsKey(UUID)){
            return allCards[cardUUId2Index[UUID]];
        }
        return null;
    }

    /// 卡牌位置变更
    private void moveCardPosEnum(int cardUId,RoomCardPosEnum toPosEnum){
        CardControl cc = GetCardControl(cardUId);
        if(cc.LastPosEnum == toPosEnum) return;
        if(cc.LastPosEnum != RoomCardPosEnum.None){
            if(cardPosList.ContainsKey(cc.LastPosEnum)){
                int last = cardPosList[cc.LastPosEnum].IndexOf(cardUId);
                if(last != -1){
                    cardPosList[cc.LastPosEnum].RemoveAt(last);
                }
            }
        }
        if(toPosEnum != RoomCardPosEnum.None){
            if(!cardPosList.ContainsKey(toPosEnum)){
                cardPosList.Add(toPosEnum,new List<int>());
            }
            cardPosList[toPosEnum].Add(cardUId);
        }
        int playerUId = GetCardPlayer(cardUId) == null ? -1 : GetCardPlayer(cardUId).playerData.UUID;
        Transform cardItem = cc.GetItem();
        cardItem.SetParent(roomControl.GetTransByPos(toPosEnum,playerUId),true);
        cardItem.localScale = Vector3.one;
        cardItem.localRotation = Quaternion.identity;
        cc.MoveCard(roomControl.GetCardPos(cc.LastPosEnum,playerUId),roomControl.GetCardPos(toPosEnum,playerUId),toPosEnum,onMoveCompleted);
    }

    private IEnumerator onInitDeskCard(){
        int count = 0;
        while(true){
            moveCardPosEnum(animalCards[count],RoomCardPosEnum.PublicDesk);
            count++;
            if(count >= animalCards.Length) break;
            yield return null;
        }
    }

    /// 单张卡片移动完成
    private void onMoveCompleted(int cardUId){
        if(bindMove.Remove(cardUId)){
            GetCardPlayer(cardUId).SortHandCards();
        }
    }
}