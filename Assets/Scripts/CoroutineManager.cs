using UnityEngine;
using System.Collections;

public class CoroutineManager : Singleton<CoroutineManager> {
    public class InnerCoroutine : MonoBehaviour {}

    private InnerCoroutine innerCoroutine;

    public CoroutineManager(){
        innerCoroutine = new GameObject("_Coroutine").AddComponent<InnerCoroutine>();
    }

    public Coroutine StartCoroutine(IEnumerator enumerator){
        return innerCoroutine.StartCoroutine(enumerator);
    }

    public void StartCoroutineWithCallback(IEnumerator enumerator,System.Action<object> callback){
        StartCoroutine(StartCoroutineInner(enumerator,callback));
    }

    public void StopCoroutine(Coroutine c){
        innerCoroutine.StopCoroutine(c);
    }

    public void StopAllCoroutine(){
        innerCoroutine.StopAllCoroutines();
    }

    private IEnumerator StartCoroutineInner(IEnumerator enumerator,System.Action<object> callback){
        yield return StartCoroutine(enumerator);
        callback(enumerator.Current);
    }
}