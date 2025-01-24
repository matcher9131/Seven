namespace Seven.Core.Engines.UCT
{
    public class State
    {
        // 出すカードの番号（パスの場合は-1）
        public int NextCard { get; set; }
        // この選択をした回数
        public ulong Count { get; set; }
        // この選択をしたことによる報酬
        public double Award { get; set; }
    }
}
