using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DataManager : Singleton<DataManager> {
    private Dictionary<string,Dictionary<int,Data>> cached = new Dictionary<string,Dictionary<int,Data>>();

    private string CSVDir {
        get {
            return Path.Combine(Application.dataPath,"Settings");
        }
    }

    public List<T> GetAll<T>() where T : Data,new() {
        loadData<T>();
        string fileName = typeof(T).FullName;
        List<T> list = new List<T>();
        if(cached.ContainsKey(fileName)){
            int id = 1;
            while(true){
                T data = Get<T>(id);
                if(data != null){
                    list.Add(data);
                    id++;
                }else{
                    break;
                }
            }
        }
        return list;
    }
    
    public T Get<T>(int id) where T : Data,new() {
        loadData<T>();
        string fileName = typeof(T).FullName;
        if(cached.ContainsKey(fileName)){
            if(cached[fileName].ContainsKey(id)){
                return cached[fileName][id] as T;
            }
        }
        return null;
    }

    /// 加载csv文件数据
    private void loadData<T>() where T : Data,new(){
        string fileName = typeof(T).FullName;
        if(cached.ContainsKey(fileName)) return;
        string[] lines = File.ReadAllLines(Path.Combine(CSVDir,fileName.Replace("Data","")+".csv"));
        if(lines.Length > 2){
            Dictionary<int,Data> list = new Dictionary<int,Data>();
            for(int i = 2;i < lines.Length;++i){
                T data = new T();
                data.Parse(lines[i]);
                if(list.ContainsKey(data.ID())){
                    Debug.LogErrorFormat("{0} 检测是否已存在相同数据：{1}",fileName,lines[i]);
                }
                list.Add(data.ID(),data);
            }
            cached.Add(fileName,list);
        }else{
            Debug.LogWarning("该表格无数据："+fileName);
        }
    }
}