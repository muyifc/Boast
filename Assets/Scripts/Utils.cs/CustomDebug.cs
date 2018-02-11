using UnityEngine;
using System.Collections;
using System.Text;

public class CustomDebug {
    static StringBuilder sb = new StringBuilder();
    
    public static void Log(params object[] args){
        Debug.Log(GetString(args));
    }

    public static void LogWarning(params object[] args){
        Debug.LogWarning(GetString(args));
    }

    public static void LogError(params object[] args){
        Debug.LogError(GetString(args));
    }

    private static string GetString(params object[] args){
        sb.Remove(0,sb.Length);
        for(int i = 0;i < args.Length;++i){
            if(i != 0) sb.Append(';');
            sb.Append(args[i] == null ? "Null" : args[i].ToString());
        }
        return sb.ToString();
    }
}