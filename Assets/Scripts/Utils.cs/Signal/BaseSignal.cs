using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BaseSignal : ISignal {
    public event Action<ISignal,object[]> BaseListener = delegate {};
    public event Action<ISignal,object[]> BaseOnceListener = delegate {};

    public void Dispatch(object[] args){
        BaseListener(this,args);
        BaseOnceListener(this,args);
        BaseOnceListener = delegate {};
    }

    public virtual List<Type> GetTypes() { return new List<Type>();}

    public void AddListener(Action<ISignal,object[]> callback){
        foreach(Delegate del in BaseListener.GetInvocationList()){
            Action<ISignal,object[]> action = (Action<ISignal,object[]>)del;
            if(callback.Equals(action)){
                return;
            }
        }
        BaseListener += callback;
    }

    public void AddOnceListener(Action<ISignal,object[]> callback){
        foreach(Delegate del in BaseOnceListener.GetInvocationList()){
            Action<ISignal,object[]> action = (Action<ISignal,object[]>)del;
            if(callback.Equals(action)){
                return;
            }
        }
        BaseOnceListener += callback;
    }

    public void RemoveListener(Action<ISignal,object[]> callback){
        BaseListener -= callback;
    }
}