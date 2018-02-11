using UnityEngine;
using System.Collections;
using System;

public class CardControl {
    public Card cardData {get;private set;}
    public RoomCardPosEnum LastPosEnum { get; private set; }
    
    private CardItem cardItem;
    private Action<int> onMoveCompleted;
    private Coroutine moveCoroutine;
    private Transform cardMoveToParent;
    private Transform cardParent;

    public void Init(Card card){
        cardData = card;
        LastPosEnum = RoomCardPosEnum.None;
    }

    public Transform GetItem(){
        if(cardItem == null){
            cardItem = ResourceManager.Instance.Load<CardItem>("Prefabs/CardItem.prefab");
            cardItem.SetData(cardData);
        }
        return cardItem.transform;
    }

    /// from,to = position
    public void MoveCard(Vector3 from,Vector3 to,RoomCardPosEnum toPosEnum,System.Action<int> onMoveCompleted){
        if(moveCoroutine != null) CoroutineManager.Instance.StopCoroutine(moveCoroutine);
        GetItem().position = from;
        this.onMoveCompleted = onMoveCompleted;
        moveCoroutine = CoroutineManager.Instance.StartCoroutine(move(to));
        LastPosEnum = toPosEnum;
        setScale();
    }

    /// 不同位置的卡牌大小不同
    private void setScale(){
        switch(LastPosEnum){
            case RoomCardPosEnum.PublicDesk:
                GetItem().localScale = new Vector3(0.3f,0.3f,0.3f);
                break;
            default:
                GetItem().localScale = Vector3.one;
                break;
        }
    }

    private IEnumerator move(Vector3 to,float duration = 0.3f){
        float time = 0;
        Transform item = GetItem();
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