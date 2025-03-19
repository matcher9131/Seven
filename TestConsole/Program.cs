const int BIT_WIDTH = 6;
const int VERTEX_MAX = 1 << BIT_WIDTH;

(int first, int second) getTopTwo(int pattern)
{
    int first = -1, second = -1;
    for (int bit = 0; bit < BIT_WIDTH; ++bit)
    {
        if ((pattern & (1 << bit)) > 0)
        {
            second = first;
            first = bit;
        }
    }
    return (first, second);
}

// 対象となるパターン（カードあり and 7に近い2枚が階段ではない）の頂点集合
int[] vertexes = Enumerable.Range(1, VERTEX_MAX - 1).Where(x =>
{
    (int first, int second) = getTopTwo(x);
    return second == -1 || first - second > 1;
}).ToArray();

// 2つのパターンの優先度に順位をつけるための辺集合（uが優先度高）
// 後の操作で計算量を減らすためにu,vは実際の頂点番号ではなくvertexes内のインデックスにする
HashSet<(int u, int v)> edges = [];
for (int i = 0; i < vertexes.Length; ++i)
{
    for (int j = i + 1; j < vertexes.Length; ++j)
    {
        int x = vertexes[i], y = vertexes[j];
        (int x1, int x2) = getTopTwo(x);
        (int y1, int y2) = getTopTwo(y);
        // 7に近い2枚が同じで、 全体が包含関係にあるパターン（枚数が多いほうが優先度高）
        if (x1 == y1 && x2 == y2 && (x & y) == x)  // x < yに留意
        {
            edges.Add((j, i));
        }
        // 7に最も近いカードのみが異なり、以降が全く同じパターン（7から遠いほうが優先度高）
        else if ((x ^ 1 << x1) == (y ^ 1 << y1))
        {
            edges.Add((i, j));
        }
        // スライドすれば一致するパターン（7から遠いほうが優先度高）
        else
        {
            for (int bit = 1; bit < BIT_WIDTH; ++bit)
            {
                if (y == (x << bit))
                {
                    edges.Add((i, j));
                }
            }
        }
    }
}

List<int>[] graph = Enumerable.Range(0, vertexes.Length).Select(_ => new List<int>()).ToArray();
foreach ((int i, int j) in edges)
{
    graph[i].Add(j);
}

// トポロジカルソートの数え上げ
long[] dp = new long[1L << vertexes.Length];
dp[0] = 1;
for (long bit = 0; bit < dp.LongLength; bit++)
{
    for (int i = 0; i < vertexes.Length; ++i)
    {
        if ((bit & 1L << i) > 0) continue;
        // より右に来るべき頂点が集合内にあるならスキップ
        if (graph[i].Any(j => (bit & 1 << j) > 0)) continue;
        checked
        {
            dp[bit | 1L << i] += dp[bit];
        }
    }
}

Console.WriteLine(dp[(1L << vertexes.Length) - 1L]);