using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoomPlayerItem : MonoBehaviour {
    public System.Action<bool> OnReady;

    private Text nameText;
    private Button btnReady;
    private GameObject bidPrice;
    private Text bidPriceText;
    private PlayerData playerData;
    private bool isReady;

    public void SetData(PlayerData data){
        playerData = data;
        nameText.text = playerData.Name;
        btnReady.interactable = playerData == PlayerManager.Instance.PlayerSelf.playerData;
    }

    /// 当前房间状态
    public void SetRoomState(RoomStateEnum stateEnum){
        btnReady.gameObject.SetActive(stateEnum == RoomStateEnum.Ready);
    }

    /// 显示思考出价中状态
    public void ShowBidProcess(bool isProcess){
        bidPrice.SetActive(isProcess);
        if(isProcess){
            bidPriceText.text = "...";
        }
    }

    /// price -1 未选择任何卡牌
    public void ShowBidPrice(int price = -1){
        bidPrice.SetActive(price != -1);
        if(price != -1){
            bidPriceText.text = string.Format("Bid:{0}",price);
        }
    }

    void Awake(){
        nameText = gameObject.Find<Text>("Name/Text");
        btnReady = gameObject.Find<Button>("BtnReady/Button");
        bidPrice = gameObject.FindChild("BidPrice");
        bidPriceText = gameObject.Find<Text>("BidPrice/Text");
    }

    void OnEnable(){
        btnReady.onClick.AddListener(onReady);
    }

    void OnDisable(){
        btnReady.onClick.RemoveAllListeners();
    }

    private void onReady(){
        isReady = !isReady;
        OnReady(isReady);
        btnReady.image.color = isReady ? Color.green : Color.white;
    }
}