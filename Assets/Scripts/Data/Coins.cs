public class Coins : Data {
    public override int ID() { return Id; }
    // ID
    public int Id;

    // 标识
    public string Sign;

    // 名称
    public string Name;

    // 面值
    public int Value;

    // 玩家开局拥有数
    public int InitCountPer;

    // 全局数量
    public int MaxCount;

    public override void Parse(string data){
        string[] split = data.Split(';');
        Id = intValue(split[0].ToString());
        Sign = stringValue(split[1].ToString());
        Name = stringValue(split[2].ToString());
        Value = intValue(split[3].ToString());
        InitCountPer = intValue(split[4].ToString());
        MaxCount = intValue(split[5].ToString());
    }
}