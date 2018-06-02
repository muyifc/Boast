using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventListenerData {
    public object data;

    public EventListenerData(){
        data = null;
    }

    public EventListenerData(object data_){
        data = data_;
    }
}

public class EventListener {
    private Dictionary<string,System.Action<EventListenerData>> dict;

    public EventListener(){
        dict = new Dictionary<string,System.Action<EventListenerData>>();
    }

    public void clear(){
        dict.Clear();
        dict = null;
    }

    public void addListener(string sign,System.Action<EventListenerData> action){
        if(!dict.ContainsKey(sign)){
            dict.Add(sign,action);
        }else{
            dict[sign] += action;
        }
    }

    public void removeListener(string sign,System.Action<EventListenerData> action){
        if(dict.ContainsKey(sign)){
            dict[sign] -= action;
            if(dict[sign].GetInvocationList().Length == 0){
                dict.Remove(sign);
            }
        }
    }

    public void dispatch(string sign,EventListenerData args){
        if(dict.ContainsKey(sign)){
            dict[sign](args);
        }
    }
}