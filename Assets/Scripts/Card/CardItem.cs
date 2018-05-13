using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class CardItem : MonoBehaviour {
    public CardControl control {get;private set;}
    public Card cardData { get; private set; }

    private Text costText;
    private Text nameText;
    private Image iconImage;
    private Text iconName;
    private Image selectImage;

    private int lastSiblingIndex;

    public void SetData(CardControl control){
        this.control = control;
        cardData = control.cardData;

        ChangePosEnum();
        addEvent();
    }

    public void ChangePosEnum(){
        SetCost();
        SetName();
        SetIcon();
    }

    public void SetCost(){
        costText.transform.parent.gameObject.SetActive(control.IsShowContent());
        if(control.IsShowContent()){
            costText.text = cardData.Value.ToString();
        }
    }

    public void SetName(){
        nameText.transform.parent.gameObject.SetActive(control.IsShowContent());
        if(control.IsShowContent()){
            nameText.text = cardData.Name;
        }
    }

    public void SetIcon(){
        iconImage.transform.parent.gameObject.SetActive(control.IsShowContent());
        if(control.IsShowContent()){
            string name = string.Empty;
            if(cardData.IsT<Coins>()){
                name = cardData.GetData<Coins>().Name;
            }else{
                name = cardData.GetData<Animals>().Name;
            }
            iconName.text = name;
            // string path =Path.Combine(Application.dataPath,string.Format("Sprites/Card/{0}.png",iconName));
            // Texture2D tex = ResourceManager.Instance.LoadAs<Texture2D>(path);
            // iconImage.sprite = Sprite.Create(tex,new Rect(0,0,tex.width,tex.height),new Vector2(0.5f,0.5f));
        }
    }

    void Awake(){
        costText = gameObject.Find<Text>("Cost/Text");
        nameText = gameObject.Find<Text>("Name/Text");
        iconImage = gameObject.Find<Image>("Icon/Image");
        iconName = gameObject.Find<Text>("Icon/Text");
        selectImage = gameObject.Find<Image>("Icon/Select");
    }

    void addEvent(){
        // if(control.IsSelfCard()){
            UIEventListener.Get(gameObject).onClick = onClickCard;
            UIEventListener.Get(gameObject).onPress = onPressCard;
            // UIEventListener.Get(gameObject).onEnter = onEnter;
            // UIEventListener.Get(gameObject).onExit = onExit;
        // }
    }

    void OnDisable(){
        UIEventListener.Get(gameObject).onClick = null;
        UIEventListener.Get(gameObject).onPress = null;
    }
    
    private void onPressCard(GameObject go,bool isPress){
        selectImage.gameObject.SetActive(isPress);
        if(control.IsSelfCard()){
            if(isPress){
                lastSiblingIndex = transform.GetSiblingIndex();
                transform.SetAsLastSibling();
                transform.localScale = new Vector3(1.5f,1.5f,1.5f);
            }else{
                transform.SetSiblingIndex(lastSiblingIndex);
                transform.localScale = new Vector3(1f,1f,1f);
            }
        }
    }

    private void onClickCard(GameObject go){
        if(control.LastPosEnum == RoomCardPosEnum.PublicDesk){
            // control.Flip();
            SignalManager.Instance.Create<RoomControl.FlipCardSignal>().Dispatch(control);
        }
    }

    private void onEnter(GameObject go){
        onPressCard(go,true);
    }

    private void onExit(GameObject go){
        onPressCard(go,false);
    }
}