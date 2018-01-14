using System;
using System.Collections.Generic;

public class Data {
    public virtual void Parse(string data){}

    public virtual int ID() {
        return 0;
    }

    public int GetIntsValue(int[] indexs,int[] values,int find){
        int index = Array.IndexOf(indexs,find);
        if(index != -1){
            return values[index];
        }
        return 0;
    }

    public float GetFloatsValue(int[] indexs,float[] values,int find){
        int index = Array.IndexOf(indexs,find);
        if(index != -1){
            return values[index];
        }
        return 0f;
    }

    protected int intValue(string value){
        if(string.IsNullOrEmpty(value)){
            return 0;
        }
        return int.Parse(value);
    }

    protected float floatValue(string value){
        if(string.IsNullOrEmpty(value)){
            return 0f;
        }
        return float.Parse(value);
    }

    protected bool boolValue(string value){
        if(string.IsNullOrEmpty(value)){
            return false;
        }
        return bool.Parse(value);
    }

    protected string stringValue(string value){
        if(string.IsNullOrEmpty(value)){
            return "";
        }
        return value.Trim('"').Replace("\"\"","\"");
    }

    protected int[] intsValue(string value){
        if(!string.IsNullOrEmpty(value)){
            string[] split = value.Split(',');
            int[] ret = new int[split.Length];
            for(int i = 0;i < split.Length;++i){
                ret[i] = intValue(split[i]);
            }
            return ret;
        }
        return new int[]{};
    }

    protected float[] floatsValue(string value){
        if(!string.IsNullOrEmpty(value)){
            string[] split = value.Split(',');
            float[] ret = new float[split.Length];
            for(int i = 0;i < split.Length;++i){
                ret[i] = floatValue(split[i]);
            }
            return ret;
        }
        return new float[]{};
    }

    protected bool[] boolsValue(string value){
        if(!string.IsNullOrEmpty(value)){
            string[] split = value.Split(',');
            bool[] ret = new bool[split.Length];
            for(int i = 0;i < split.Length;++i){
                ret[i] = boolValue(split[i]);
            }
            return ret;
        }
        return new bool[]{};
    }
}