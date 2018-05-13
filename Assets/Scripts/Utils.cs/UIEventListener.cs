using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UIEventListener : UnityEngine.EventSystems.EventTrigger {
    public delegate void VoidDelegate(GameObject obj);
    public delegate void VoidDelegateWithBool(GameObject obj,bool isPress);
    public VoidDelegate onClick;
    public VoidDelegateWithBool onPress;
    public VoidDelegate onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelected;

    public static UIEventListener Get(GameObject obj){
        UIEventListener listener = obj.GetComponent<UIEventListener>();
        if(listener == null){
            listener = obj.AddComponent<UIEventListener>();
        }
        return listener;
    }

    public override void OnPointerClick(PointerEventData eventData){
        if(onClick != null) onClick(gameObject);
    }
    public override void OnPointerDown (PointerEventData eventData){
        if(onDown != null) onDown(gameObject);
        if(onPress != null) onPress(gameObject,true);
    }
    public override void OnPointerUp (PointerEventData eventData){
        if(onUp != null) onUp(gameObject);
        if(onPress != null) onPress(gameObject,false);
    }
    public override void OnPointerEnter (PointerEventData eventData){
        if(onEnter != null) onEnter(gameObject);
    }
    public override void OnPointerExit (PointerEventData eventData){
        if(onExit != null) onExit(gameObject);
    }
    public override void OnSelect (BaseEventData eventData){
        if(onSelect != null) onSelect(gameObject);
    }
    public override void OnUpdateSelected (BaseEventData eventData){
        if(onUpdateSelected != null) onUpdateSelected(gameObject);
    }
}