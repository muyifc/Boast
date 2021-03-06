public class Animals : Data {
    public override int ID() { return Id; }
    // ID
    public int Id;

    // 标识
    public string Sign;

    // 名称
    public string Name;

    // 面值
    public int Value;

    // 卡面名
    public string IconName;

    public override void Parse(string data){
        string[] split = data.Split(';');
        Id = intValue(split[0].ToString());
        Sign = stringValue(split[1].ToString());
        Name = stringValue(split[2].ToString());
        Value = intValue(split[3].ToString());
        IconName = stringValue(split[4].ToString());
    }
}