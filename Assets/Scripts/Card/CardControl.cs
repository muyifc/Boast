using UnityEngine;
using System.Collections;

public class CardControl {
    public Card cardData {get;private set;}
    public RoomCardPosEnum LastPosEnum { get; private set; }
    
    private CardItem cardItem;
    private Coroutine moveCoroutine;

    public void Init(Card card){
        cardData = card;
    }

    public Transform GetItem(){
        if(cardItem == null){
            cardItem = ResourceManager.Instance.Load<CardItem>("Prefabs/CardItem.prefab");
            cardItem.SetData(cardData);
        }
        return cardItem.transform;
    }

    public void MoveCard(Transform from,Transform to,RoomCardPosEnum posEnum){
        GetItem().SetParent(to,false);
        if(moveCoroutine != null) CoroutineManager.Instance.StopCoroutine(moveCoroutine);
        if(to != null)
            moveCoroutine = CoroutineManager.Instance.StartCoroutine(move(to));
        LastPosEnum = posEnum;
    }

    private IEnumerator move(Transform to){
        float time = 0;
        float duration = 0.3f;
        Transform item = GetItem().transform;
        while(true){
            float step = Mathf.Clamp01(time / duration);
            Debug.Log(to);
            item.position = Vector3.Lerp(item.position,to.position,step);
            yield return null;
            time += Time.deltaTime;
            if(time >= duration) break;
        }
    }
}