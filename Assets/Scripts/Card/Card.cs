using UnityEngine;
using System.Collections;

public class Card {
    public string Name {get; private set;}
    public int Value {get; private set;}

    public Card(string name,int value){
        Name = name;
        Value = value;
    }
}