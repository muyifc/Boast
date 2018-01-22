using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardItem : MonoBehaviour {
    public Card cardData { get; private set; }

    private Text costText;
    private Text nameText;
    private Image iconImage;

    void Awake(){
        costText = gameObject.Find<Text>("Cost/Text");
        nameText = gameObject.Find<Text>("Name/Text");
        iconImage = gameObject.Find<Image>("Icon/Image");
    }

    public void SetData(Card data){
        cardData = data;
        SetCost(cardData.Value);
        SetName(cardData.Name);
    }

    public void SetCost(int value){
        costText.text = value.ToString();
    }

    public void SetName(string name){
        nameText.text = name;
    }

    public void SetIcon(string path){
        Texture2D tex = ResourceManager.Instance.LoadAs<Texture2D>(path);
        iconImage.sprite = Sprite.Create(tex,new Rect(0,0,tex.width,tex.height),new Vector2(0.5f,0.5f));
    }
}