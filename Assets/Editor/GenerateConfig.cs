using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public class GenerateConfig : EditorWindow {
    [MenuItem("CustomTool/GenerateConfig")]
    public static void Generate(){
        GetWindow<GenerateConfig>(false,"生成配置数据");
    }

    const string Prefs_Generate = "Editor.Generate";
    string backupFile = null;
    Configs configs = null;
    int leftWidth = 100;

    [System.Serializable]
    internal class Configs {
        public List<Config> ConfigInfos = null;
    }

    [System.Serializable]
    internal class Config {
        public string infolder;
        public string outfolder;
        public string fileName;
    }

    void OnEnable(){
        backupFile = Path.Combine(Application.dataPath,"Backup/Generate.json");
        if(File.Exists(backupFile)){
            configs = JsonUtility.FromJson<Configs>(File.ReadAllText(backupFile));
        }
        if(configs == null){
            configs = new Configs();
        }
        if(configs.ConfigInfos == null){
            configs.ConfigInfos = new List<Config>();
        }
    }

    void OnDisable(){
        // 备份配置数据
        string dir = Path.GetDirectoryName(backupFile);
        if(!Directory.Exists(dir)){
            Directory.CreateDirectory(dir);
        }
        string json = JsonUtility.ToJson(configs);
        File.WriteAllText(backupFile,json);
        AssetDatabase.Refresh();
    }

    void OnGUI(){
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("将指定文件夹中的预制对应生成路径配置文件");
        if(GUILayout.Button("新增")){
            configs.ConfigInfos.Add(new Config());
        }
        EditorGUILayout.EndHorizontal();
        
        if(GUILayout.Button("全部生成")){
            for(int i = 0;i < configs.ConfigInfos.Count;++i){
                generateSingle(configs.ConfigInfos[i]);
            }
            AssetDatabase.Refresh();
        }
        
        for(int i = 0;i < configs.ConfigInfos.Count;++i){
            EditorGUILayout.BeginVertical();
            createArea(configs.ConfigInfos[i]);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
    }

    // 创建一个配置区域块
    void createArea(Config config){
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("源文件夹",GUILayout.Width(leftWidth))){
            string temp = EditorUtility.OpenFolderPanel("源文件夹",string.IsNullOrEmpty(config.infolder) ? Application.dataPath : config.infolder,"");
            if(!string.IsNullOrEmpty(temp)){
                config.infolder = temp.Substring(temp.LastIndexOf("Assets"));
            }
        }
        EditorGUILayout.TextField(config.infolder);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("输出文件夹",GUILayout.Width(leftWidth))){
            string temp = EditorUtility.OpenFolderPanel("输出文件夹",string.IsNullOrEmpty(config.outfolder) ? Application.dataPath : config.outfolder,"");
            if(!string.IsNullOrEmpty(temp)){
                config.outfolder = temp.Substring(temp.LastIndexOf("Assets"));
            }
        }
        EditorGUILayout.TextField(config.outfolder);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("收集的文件后缀",GUILayout.Width(leftWidth));
        config.fileName = EditorGUILayout.TextField(config.fileName);
        if(string.IsNullOrEmpty(config.fileName)){
            config.fileName = Path.GetFileName(config.infolder)+".json";
        }
        if(string.IsNullOrEmpty(Path.GetExtension(config.fileName))){
            config.fileName += ".json";
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("生成")){
            generateSingle(config);
            AssetDatabase.Refresh();
        }
        if(GUILayout.Button("删除")){
            configs.ConfigInfos.Remove(config);
        }
        EditorGUILayout.EndHorizontal();
    }

    // 根据单条数据生成对应的配置文件
    private void generateSingle(Config config){
        if(!Directory.Exists(config.infolder) || !Directory.Exists(config.outfolder)){
            return;
        }
        string[] files = Directory.GetFiles(config.infolder,config.fileName,SearchOption.TopDirectoryOnly);
        for(int i = 0;i < files.Length;++i){
            // string[] fileds = Path.
            string[] lines = File.ReadAllLines(files[i]);
            if(lines.Length > 2){
                string txt = createCSFile(Path.GetFileNameWithoutExtension(files[i]),lines[0].Split(';'),lines[1].Split(';'));
                if(string.IsNullOrEmpty(txt)) continue;
                saveFile(txt,string.Format("{0}/{1}.cs",config.outfolder,Path.GetFileNameWithoutExtension(files[i])));
            }
        }
    }

   private string createCSFile(string fileName,string[] notes,string[] fields){
        if(notes.Length != fields.Length){
            EditorUtility.DisplayDialog("Warning!!!",string.Format("{0}表注释列数与标识列数不匹配",fileName),"确定");
            return "";
        }
        StringBuilder sb = new StringBuilder();
        sb.Append(head(fileName));
        sb.AppendFormat("    public override int ID() {{ return {0}; }}\n",parseField(fields[0])[0]);
        for(int i = 0;i < fields.Length;++i){
            sb.Append(field(notes[i],fields[i]));
            sb.Append("\n");
        }
        sb.Append(parse(fileName,fields));
        sb.Append(foot());
        return sb.ToString();
   }

   private void saveFile(string txt,string path){
        // save
        if(File.Exists(path)){
            File.Delete(path);
        }
        File.WriteAllText(path,txt);
    }

    private string head(string fileName){
        return string.Format("public class {0} : Data {{\n",fileName);
    }

    private string foot(){
        return "}";
    }

    private string field(string note,string field){
        string[] data = parseField(field);
        return string.Format("    // {0}\n",note) + string.Format("    public {0} {1};\n",data[1],data[0]);
    }

    private string[] parseField(string field){
        // Id(int)
        string[] data = new string[2];
        Regex r = new Regex(@"\((.*)\)");
        Match m = r.Match(field);
        if(m != null){
            data[0] = field.Replace(m.Result("$0"),""); // Id
            data[1] = m.Result("$1"); // int
        }
        return data;
    }

    private string parse(string fileName,string[] fields){
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("    public override void Parse(string data){{\n",fileName);
        sb.Append("        string[] split = data.Split(';');\n");
        for(int i = 0;i < fields.Length;++i){
            string[] data = parseField(fields[i]);
            string p = string.Format("split[{0}].ToString()",i);
            switch(data[1]){
                case "string":
                p = string.Format("{0} = stringValue({1})",data[0],p);
                break;
                case "int":
                p = string.Format("{0} = intValue({1})",data[0],p);
                break;
                case "float":
                p = string.Format("{0} = floatValue({1})",data[0],p);
                break;
                case "bool":
                p = string.Format("{0} = boolValue({1})",data[0],p);
                break;
                case "int[]":
                p = string.Format("{0} = intsValue(split[{1}])",data[0],i);
                break;                
                case "float[]":
                p = string.Format("{0} = floatsValue(split[{1}])",data[0],i);
                break;                
                case "bool[]":
                p = string.Format("{0} = boolsValue(split[{1}])",data[0],i);
                break;
            }
            sb.AppendFormat("        {0};\n",p);
        }
        sb.Append("    }\n");
        return sb.ToString();
    }
}