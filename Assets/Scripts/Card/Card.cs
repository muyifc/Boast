using UnityEngine;
using System.Collections;
using System;

public class Card {
    private static int uid;
    private static int CardUID {
        get {
            return uid++;
        }
    }

    public int UUID {get;private set;}
    public string Name {get; private set;}
    public int Value {get; private set;}
    public CardTypeEnum CardTypeEnum {get;private set;}

    // 数据来源
    private Type dataType;
    private int dataId;

    public Card(string name,int value){
        UUID = CardUID;
        Name = name;
        Value = value;
    }

    public void SetCardType(CardTypeEnum typeEnum){
        CardTypeEnum = typeEnum;
    }

    /// 卡牌数据来源
    public void SetDataFrom(Type dataType,int id){
        this.dataType = dataType;
        this.dataId = id;
    }

    public bool IsT<T>(){
        return typeof(T).FullName.Equals(dataType.FullName);
    }

    public T GetData<T>() where T : Data,new(){
        if(!IsT<T>()) return null;
        return DataManager.Instance.Get<T>(dataId);
    }
}