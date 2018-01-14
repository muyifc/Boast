using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SendCardControl {
    private RoomControl roomControl;
    private List<Card> cards;

    /// 每种动物个4张
    private const int MaxAnimalPer = 4;
    
    public void Init(RoomControl control){
        roomControl = control;
        cards = new List<Card>();

        initCards();
    }

    private void initCards(){
        for(int i = 0;i < System.Enum.GetValues(typeof(AnimalEnum)).Length;++i){
            for(int j = 0;j < RoomManager.RoomMaxPlayer;++j){
            }
        }
    }
}