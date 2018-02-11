using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoroutineManager : Singleton<CoroutineManager> {
    public class InnerCoroutine : MonoBehaviour {}

    private InnerCoroutine innerCoroutine;
    private Dictionary<float,WaitForSeconds> waits;

    public CoroutineManager(){
        innerCoroutine = new GameObject("_Coroutine").AddComponent<InnerCoroutine>();
        waits = new Dictionary<float, WaitForSeconds>();
    }

    public Coroutine StartCoroutine(IEnumerator enumerator){
        return innerCoroutine.StartCoroutine(enumerator);
    }

    public void StartCoroutineWithCallback(IEnumerator enumerator,System.Action<object> callback){
        StartCoroutine(startCoroutineInner(enumerator,callback));
    }

    public void StartCoroutineWithCallback(IEnumerator enumerator,System.Action callback){
        StartCoroutine(startCoroutineInner(enumerator,callback));
    }

    /// 延迟执行
    public void StartCoroutineWait(float wait,System.Action callback){
        StartCoroutine(waitCoroutine(wait,callback));
    }

    public void StopCoroutine(Coroutine c){
        innerCoroutine.StopCoroutine(c);
    }

    public void StopAllCoroutine(){
        innerCoroutine.StopAllCoroutines();
    }

    private IEnumerator startCoroutineInner(IEnumerator enumerator,System.Action<object> callback){
        yield return StartCoroutine(enumerator);
        callback(enumerator.Current);
    }

    private IEnumerator startCoroutineInner(IEnumerator enumerator,System.Action callback){
        yield return StartCoroutine(enumerator);
        callback();
    }

    private IEnumerator waitCoroutine(float wait,System.Action callback){
        if(!waits.ContainsKey(wait)){
            waits.Add(wait,new WaitForSeconds(wait));
        }
        yield return waits[wait];
        callback();
    }
}