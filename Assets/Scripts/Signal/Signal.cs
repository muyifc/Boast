using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Signal : BaseSignal {
    public event Action Listener = delegate {};
    public event Action OnceListener = delegate {};

    public override List<Type> GetTypes(){
        return new List<Type>();
    }

    public void Dispatch(){
        Listener();
        OnceListener();
        OnceListener = delegate {};
        base.Dispatch(null);
    }

    public void AddListener(Action callback){
        Listener += callback;
    }

    public void AddOnceListener(Action callback){
        OnceListener += callback;
    }

    public void RemoveListener(Action callback){
        Listener -= callback;
    }
}

public class Signal<T> : BaseSignal {
    public event Action<T> Listener = delegate {};
    public event Action<T> OnceListener = delegate {};

    public override List<Type> GetTypes(){
        List<Type> retv = new List<Type>();
        retv.Add(typeof(T));
        return retv;
    }

    public void Dispatch(T arg){
        Listener(arg);
        OnceListener(arg);
        OnceListener = delegate {};
        object[] outv = {arg};
        base.Dispatch(outv);
    }

    public void AddListener(Action<T> callback){
        Listener += callback;
    }

    public void AddOnceListener(Action<T> callback){
        OnceListener += callback;
    }

    public void RemoveListener(Action<T> callback){
        Listener -= callback;
    }
}

public class Signal<T,U> : BaseSignal {
    public event Action<T,U> Listener = delegate {};
    public event Action<T,U> OnceListener = delegate {};

    public override List<Type> GetTypes(){
        List<Type> retv = new List<Type>();
        retv.Add(typeof(T));
        retv.Add(typeof(U));
        return retv;
    }

    public void Dispatch(T arg1,U arg2){
        Listener(arg1,arg2);
        OnceListener(arg1,arg2);
        OnceListener = delegate {};
        object[] outv = {arg1,arg2};
        base.Dispatch(outv);
    }

    public void AddListener(Action<T,U> callback){
        Listener += callback;
    }

    public void AddOnceListener(Action<T,U> callback){
        OnceListener += callback;
    }

    public void RemoveListener(Action<T,U> callback){
        Listener -= callback;
    }
}

public class Signal<T,U,V> : BaseSignal {
    public event Action<T,U,V> Listener = delegate {};
    public event Action<T,U,V> OnceListener = delegate {};

    public override List<Type> GetTypes(){
        List<Type> retv = new List<Type>();
        retv.Add(typeof(T));
        retv.Add(typeof(U));
        retv.Add(typeof(V));
        return retv;
    }

    public void Dispatch(T arg1,U arg2,V arg3){
        Listener(arg1,arg2,arg3);
        OnceListener(arg1,arg2,arg3);
        OnceListener = delegate {};
        object[] outv = {arg1,arg2,arg3};
        base.Dispatch(outv);
    }

    public void AddListener(Action<T,U,V> callback){
        Listener += callback;
    }

    public void AddOnceListener(Action<T,U,V> callback){
        OnceListener += callback;
    }

    public void RemoveListener(Action<T,U,V> callback){
        Listener -= callback;
    }
}

public class Signal<T,U,V,W> : BaseSignal {
    public event Action<T,U,V,W> Listener = delegate {};
    public event Action<T,U,V,W> OnceListener = delegate {};

    public override List<Type> GetTypes(){
        List<Type> retv = new List<Type>();
        retv.Add(typeof(T));
        retv.Add(typeof(U));
        retv.Add(typeof(V));
        retv.Add(typeof(W));
        return retv;
    }

    public void Dispatch(T arg1,U arg2,V arg3,W arg4){
        Listener(arg1,arg2,arg3,arg4);
        OnceListener(arg1,arg2,arg3,arg4);
        OnceListener = delegate {};
        object[] outv = {arg1,arg2,arg3,arg4};
        base.Dispatch(outv);
    }

    public void AddListener(Action<T,U,V,W> callback){
        Listener += callback;
    }

    public void AddOnceListener(Action<T,U,V,W> callback){
        OnceListener += callback;
    }

    public void RemoveListener(Action<T,U,V,W> callback){
        Listener -= callback;
    }
}