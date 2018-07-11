using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomControl {
    // 翻牌
    public class FlipCardSignal : Signal<CardControl> {}
    // 翻牌通知开始出价
    public class NotifyBidSignal : Signal {}
    // 出价结束取消竞价
    public class CancelBidSignal : Signal {}
    // 完成出价
    public class CompleteBidSignal : Signal<PlayerControl,int> {}
    // 选择自己的卡牌
    public class SelectSelfCardSignal : Signal<CardControl> {}

    public int RoomId {get;private set;}

    public List<PlayerControl> roomPlayers;

    public SendCardControl sendCardControl {get;private set;}

    private RoomItem roomItem;
    private RoomStateEnum stateEnum;
    private LobbyRoomItem lobbyItem;

    // 当前回合玩家
    public PlayerControl curRoundPlayer;
    // 当前回合的翻牌
    public CardControl curFlipCard;
    private RoundStateEnum roundStateEnum;

    private float chooseTime;
    public SaleCardEnum curSaleEnum;
    public PlayerControl behindBusinessPlayer;
    // 等待下一个出价者
    private bool isWaitNextBid;
    private float startWaitNextBidTime;
    public PlayerControl CurBidPlayer;

    private RoomData mRoomData;
    public RoomData RoomData {
        get {
            return mRoomData;
        }
    }

    public void Init(){
        roomPlayers = new List<PlayerControl>(RoomManager.RoomMaxPlayer);
        stateEnum = RoomStateEnum.Ready;
        sendCardControl = new SendCardControl();
        sendCardControl.Init(this);

        mRoomData = new RoomData();

        SignalManager.Instance.Create<FlipCardSignal>().AddListener(onFlipCard);
        SignalManager.Instance.Create<CompleteBidSignal>().AddListener(onCompleteBid);
        SignalManager.Instance.Create<SelectSelfCardSignal>().AddListener(onSelectSelfCard);
    }

    public void Clear(){
        roomPlayers.Clear();
        roomPlayers = null;
        clearLobbyItem();
        clearRoomInstance();
        sendCardControl.Clear();
        sendCardControl = null;

        SignalManager.Instance.Create<FlipCardSignal>().RemoveListener(onFlipCard);
        SignalManager.Instance.Create<CompleteBidSignal>().RemoveListener(onCompleteBid);
        SignalManager.Instance.Create<SelectSelfCardSignal>().RemoveListener(onSelectSelfCard);
    }

    public void SetRoomID(int roomId){
        RoomId = roomId;
    }

    public Transform GetRoomDesk(){
        return GetRoomItem().GetDesk();
    }

    public void Update(){
        if(isWaitNextBid){
            startWaitNextBidTime += Time.deltaTime;
            if(startWaitNextBidTime >= RoomData.WaitNextBidCD){
                isWaitNextBid = false;
                startWaitNextBidTime = 0;
                chooseBidToPay();
            }
        }
    }

    /// 添加玩家
    public void AddPlayer(PlayerControl player){
        if(roomPlayers.Capacity == roomPlayers.Count){
            Debug.LogWarning("Room is Full");
            return;
        }
        if(!roomPlayers.Contains(player)){
            roomPlayers.Add(player);
            player.SetRoom(this);
            if(player == PlayerManager.Instance.PlayerSelf){
                GetRoomItem().SetRoomInfo(RoomId);
            }
            updateSeat();
        }
        GetLobbyItem().SetRoomPlayer(roomPlayers.Count);
    }

    // 移除玩家
    public void RemovePlayer(PlayerControl player){
        int index = roomPlayers.IndexOf(player);
        if(index != -1){
            player.Clear();
            roomPlayers.RemoveAt(index);
            if(player == PlayerManager.Instance.PlayerSelf){
                clearRoomInstance();
            }
        }
        GetLobbyItem().SetRoomPlayer(roomPlayers.Count);
        
        bool isAllRobot = true;
        for(int i = 0;i < roomPlayers.Count;++i){
            if(roomPlayers[i].playerData.isRobot == false){
                isAllRobot = false;
                break;
            }
        }
        if(roomPlayers.Count == 0 || isAllRobot){
            RoomManager.Instance.ClearRoom(this);
        }
    }

    /// 根据位置获取所在父级
    public Transform GetTransByPos(RoomCardPosEnum posEnum,Card cardData,int playerUId = -1){
        Transform target = null;
        switch(posEnum){
            case RoomCardPosEnum.PublicDesk:
                target = GetRoomDesk();
            break;
            case RoomCardPosEnum.PlayerHand:
                PlayerControl pc = PlayerManager.Instance.GetPlayer(playerUId);
                if(pc != null){
                    if(cardData.IsT<Animals>()){
                        target = pc.GetCardAnimal();
                    }else{
                        target = pc.GetCardHand();
                    }
                }
                break;
            case RoomCardPosEnum.PlayerLibrary:
                pc = PlayerManager.Instance.GetPlayer(playerUId);
                if(pc != null){
                    target = pc.GetCardLibrary();
                }
                break;
            case RoomCardPosEnum.TmpRound:
                break;
            case RoomCardPosEnum.EndRound:
                target = roomItem.GetCardEndRound();
                break;
            case RoomCardPosEnum.WillDestroy:
                target = roomItem.GetWillDestroy();
                break;
        }
        return target;
    }

    /// 舞台中卡牌的对应坐标
    public Vector3 GetCardPos(RoomCardPosEnum posEnum,Card cardData,int playerUId = -1){
        Transform target = GetTransByPos(posEnum,cardData,playerUId);
        if(target != null){
            CardLayoutData layoutData = target.GetComponent<CardLayoutData>();
            if(layoutData != null){
                return layoutData.GetNextPos();
            }else{
                return target.position;
            }
        }
        return GetRoomItem().transform.position;
    }

    /// 检测所有玩家是否准备好，进入下一状态
    public void CheckState(){
        if(stateEnum == RoomStateEnum.Ready){
            if(roomPlayers.Count == RoomManager.RoomMaxPlayer){
                bool isReady = true;
                for(int i = 0;i < roomPlayers.Count;++i){
                    if(roomPlayers[i].isReadyPlay == false){
                        isReady = false;
                        break;
                    }
                }
                if(isReady){
                    changeState(RoomStateEnum.Start);
                }
            }
        }
    }

    /// 状态切换
    private void changeState(RoomStateEnum stateEnum){
        this.stateEnum = stateEnum;
        switch(stateEnum){
            case RoomStateEnum.Ready:
                break;
            case RoomStateEnum.Start:
                sendCardControl.InitDeskCard(()=>{
                    CoroutineManager.Instance.StartCoroutineWait(0.3f,()=>{
                        changeState(RoomStateEnum.Send);
                    });
                });
                break;
            case RoomStateEnum.Send:
                sendCardControl.StartGameSendCard();
                changeState(RoomStateEnum.Playing);
                break;
            case RoomStateEnum.Playing:
                if(curRoundPlayer == null){
                    curRoundPlayer = PlayerManager.Instance.PlayerSelf;
                }
                break;
        }
        for(int i = 0;i < roomPlayers.Count;++i){
            roomPlayers[i].SetRoomState(stateEnum);
        }
    }

    private RoomItem GetRoomItem(){
        if(roomItem == null){
            roomItem = ResourceManager.Instance.Load<RoomItem>("Prefabs/RoomItem.prefab");
            roomItem.transform.SetParent(SceneManager.Instance.UICanvas.transform,false);
            roomItem.OnClose = onClose;
        }
        return roomItem;
    }

    private void clearRoomInstance(){
        if(roomItem == null) return;
        roomItem.OnClose = null;
        GameObject.Destroy(roomItem.gameObject);
        roomItem = null;
    }

    private LobbyRoomItem GetLobbyItem(){
        if(lobbyItem == null){
            lobbyItem = ResourceManager.Instance.Load<LobbyRoomItem>("Prefabs/LobbyRoomItem.prefab");
            lobbyItem.transform.SetParent(SceneManager.Instance.Lobby.roomList,false);
            lobbyItem.SetRoomId(RoomId);
        }
        return lobbyItem;
    }

    private void clearLobbyItem(){
        if(lobbyItem != null){
            GameObject.Destroy(lobbyItem.gameObject);
            lobbyItem = null;
        }
    }

    /// 更新玩家座次
    private void updateSeat(){
        Transform[] seats = roomItem.GetSeats();
        int index = roomPlayers.IndexOf(PlayerManager.Instance.PlayerSelf);
        if(index != -1){
            for(int i = 0;i < roomPlayers.Count;++i){
                roomPlayers[(i+index) % roomPlayers.Count].SetSeat(seats[i]);
            }
        }
    }

    private void onClose(){
        RemovePlayer(PlayerManager.Instance.PlayerSelf);
    }

    private void onFlipCard(CardControl control){
        if(curFlipCard == null){
            curFlipCard = control;
            curFlipCard.Flip();
            roundStateEnum = RoundStateEnum.FlipAnimal;
            chooseTime = 0;

            // 如果是 驴，需要从银行各发[100]给玩家
            if(curFlipCard.cardData.GetData<Animals>().Id == RoomData.SendCoinCardId){
                sendCoinForDonkey();
                sendCardControl.MoveCardPosEnum(curFlipCard.cardData.UUID,RoomCardPosEnum.WillDestroy);
                curFlipCard = null;
                nextRound();
                return;
            }

            List<PlayerControl> hasSameAnimalPlayers = getSameAnimalPlayers();
            if(hasSameAnimalPlayers.Count == 0){
                curSaleEnum = SaleCardEnum.SaleAnimal;
            }else{
                // TODO 需要做成玩家选择使用那种方式拍卖
                curSaleEnum = Random.value < 0.5f ? SaleCardEnum.SaleAnimal : SaleCardEnum.BehindBusiness;
            }
            CustomDebug.Log("准备拍卖模式："+curSaleEnum.ToString());
            if(curSaleEnum == SaleCardEnum.BehindBusiness){
                behindBusinessPlayer = hasSameAnimalPlayers[Random.Range(0,hasSameAnimalPlayers.Count)];
            }else{
                // 通知其他玩家出价
                RoomData.CurBidPrice = 0;
                SignalManager.Instance.Create<NotifyBidSignal>().Dispatch();
            }
        }else{
            Debug.LogWarning("本回合已翻牌，无法继续翻牌");
        }
    }

    /// 获取拥有与当前翻牌相同动物类型的其他玩家
    private List<PlayerControl> getSameAnimalPlayers(){
        List<PlayerControl> players = new List<PlayerControl>();
        for(int i = 0;i < roomPlayers.Count;++i){
            if(roomPlayers[i] == curRoundPlayer) continue;
            List<int> cards = roomPlayers[i].GetAnimalCards();
            for(int j = 0;j < cards.Count;++j){
                if(curFlipCard.cardData.GetData<Animals>().Id == sendCardControl.GetCardControl(cards[j]).cardData.GetData<Animals>().Id){
                    players.Add(roomPlayers[i]);
                }
            }
        }
        return players;
    }

    /// 翻到驴发钱
    private void sendCoinForDonkey(){
        int cardId = RoomData.DonkeySendCards[Mathf.Min(RoomData.FlipDonkeyCount++,RoomData.DonkeySendCards.Length-1)];
        Coins coin = DataManager.Instance.Get<Coins>(cardId);
        for(int i = 0;i < roomPlayers.Count;++i){
            Card card = sendCardControl.CreateCard(coin.Name,coin.Value,typeof(Coins),cardId,CardTypeEnum.Coin);
            sendCardControl.BindCard(roomPlayers[i].playerData.UUID,card.UUID);
        }
    }

    private void onCompleteBid(PlayerControl player,int price){
        CurBidPlayer = player;
        RoomData.CurBidPrice = price;
        isWaitNextBid = true;
        startWaitNextBidTime = 0;
    }

    /// 暂无人出价，选择最后一个出价者
    private void chooseBidToPay(){
        if(CurBidPlayer != null){
            CurBidPlayer.PayFlipCard();
        }
        CurBidPlayer = null;
        curFlipCard = null;
        roundComplete();
        nextRound();
    }

    /// 回合结束
    private void roundComplete(){
        SignalManager.Instance.Create<CancelBidSignal>().Dispatch();
    }

    /// 回合结束，下一回合
    private void nextRound(){
        int curIndex = -1;
        for(int i = 0;i < roomPlayers.Count;++i){
            if(curRoundPlayer == roomPlayers[i]){
                curIndex = i;
                break;
            }
        }
        curRoundPlayer = roomPlayers[(curIndex + 1) % roomPlayers.Count];
        roundStateEnum = RoundStateEnum.Ready;
    }

    /// 竞价阶段选择自己的金币牌出售
    private void onSelectSelfCard(CardControl card){
        if(curRoundPlayer == PlayerManager.Instance.PlayerSelf) return;
        if(sendCardControl.GetCardPlayer(card.cardData.UUID) != PlayerManager.Instance.PlayerSelf) return;
        card.Select();
    }
}