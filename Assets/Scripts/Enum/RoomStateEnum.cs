public enum RoomStateEnum {
    Ready,  // 准备阶段
    Start,  // 开始游戏
    Send,   // 发牌
    Playing,    // 按回合顺序依次出牌
    Check,  // 结算
}