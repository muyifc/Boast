using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SignalManager : Singleton<SignalManager> {
    public Dictionary<string,ISignal> signals = new Dictionary<string, ISignal>();

    public T Create<T>() where T : class,ISignal,new() {
        string key = typeof(T).FullName;
        if(!signals.ContainsKey(key)){
            signals.Add(key,new T());
        }
        return signals[key] as T;
    }
}