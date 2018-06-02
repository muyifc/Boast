using UnityEngine;
using System.Collections;
using System;

public class CardControl {
    public Card cardData {get;private set;}
    public RoomCardPosEnum LastPosEnum { get; private set; }
    
    // 是否翻到正面
    private bool isFlipFront;

    private CardItem cardItem;
    private Action<int> onMoveCompleted;
    private Coroutine moveCoroutine;
    private Transform cardMoveToParent;
    private Transform cardParent;
    // 当前卡牌是否选中
    private bool isSelect;

    public void Init(Card card){
        cardData = card;
        LastPosEnum = RoomCardPosEnum.None;
    }

    public CardItem GetItem(){
        if(cardItem == null){
            cardItem = ResourceManager.Instance.Load<CardItem>("Prefabs/CardItem.prefab");
            cardItem.SetData(this);
        }
        return cardItem;
    }

    /// 翻卡牌
    public bool Flip(){
        isFlipFront = !isFlipFront;
        GetItem().ChangePosEnum();
        return isFlipFront;
    }

    /// 选择卡牌 0 默认翻转当前状态 1 强制选中 2 强制取消选择
    public void Select(int forceSelect = 0){
        switch(forceSelect){
            case 0:
            isSelect = !isSelect;
            break;
            case 1:
            isSelect = true;
            break;
            case 2:
            isSelect = false;
            break;
        }
        GetItem().Select(isSelect);
    }

    /// 是否本人卡牌
    public bool IsSelfCard(){
        if(cardData == null) return false;
        return PlayerManager.Instance.PlayerSelf.roomControl.sendCardControl.GetCardPlayer(cardData.UUID) == PlayerManager.Instance.PlayerSelf;
    }

    /// 是否显示卡面
    public bool IsShowContent(){
        return IsSelfCard() || 
        (PlayerManager.Instance.PlayerSelf.roomControl.sendCardControl.GetCardControl(cardData.UUID).LastPosEnum == RoomCardPosEnum.PublicDesk && isFlipFront);
    }

    /// from,to = position
    public void MoveCard(Vector3 from,Vector3 to,RoomCardPosEnum toPosEnum,System.Action<int> onMoveCompleted){
        if(moveCoroutine != null) CoroutineManager.Instance.StopCoroutine(moveCoroutine);
        GetItem().transform.position = from;
        this.onMoveCompleted = onMoveCompleted;
        moveCoroutine = CoroutineManager.Instance.StartCoroutine(move(to));
        LastPosEnum = toPosEnum;
        cardItem.ChangePosEnum();
        setScale();
    }

    /// 不同位置的卡牌大小不同
    private void setScale(){
        switch(LastPosEnum){
            case RoomCardPosEnum.PublicDesk:
                GetItem().transform.localScale = new Vector3(0.3f,0.3f,0.3f);
                break;
            default:
                GetItem().transform.localScale = Vector3.one;
                break;
        }
    }

    private IEnumerator move(Vector3 to,float duration = 0.3f){
        float time = 0;
        Transform item = GetItem().transform;
        yield return null;
        while(true){
            float step = Mathf.Clamp01(time / duration);
            time += Time.deltaTime;
            Vector2 stepTo = Vector2.Lerp(item.position,to,step); 
            item.position = new Vector3(stepTo.x,stepTo.y,to.z);
            if(step >= 1) break;
            yield return null;
        }
        if(onMoveCompleted != null){
            onMoveCompleted(cardData.UUID);
            onMoveCompleted = null;
        }
    }
}