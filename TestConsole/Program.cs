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

// 対象となるパターン（パスおよび7に近い2枚が階段ではないパターン）の頂点集合
int[] vertexes = [-1, .. Enumerable.Range(1, VERTEX_MAX - 1).Where(x =>
{
    (int first, int second) = getTopTwo(x);
    return second == -1 || first - second > 1;
})];

// 2つのパターンの優先度に順位をつけるための辺集合（uが優先度高）
HashSet<(int u, int v)> edges = [];
// vertexes[0]はパスなのでスキップ
for (int i = 1; i < vertexes.Length; ++i)
{
    for (int j = i + 1; j < vertexes.Length; ++j)
    {
        int x = vertexes[i], y = vertexes[j];
        (int x1, int x2) = getTopTwo(x);
        (int y1, int y2) = getTopTwo(y);
        // 以下x < yに留意
        // 7に近い2枚が同じで、 全体が包含関係にあるパターン（枚数が多いほうが優先度高）
        // 例：010100と010110
        if (x1 == y1 && x2 == y2 && (x & y) == x)
        {
            edges.Add((y, x));
        }
        // 7に最も近いカードのみが異なり、以降が全く同じパターン（7から遠いほうが優先度高）
        // 例：100101と010101
        else if ((x ^ 1 << x1) == (y ^ 1 << y1))
        {
            edges.Add((x, y));
        }
        else
        {
            for (int bit = 1; bit < BIT_WIDTH; ++bit)
            {
                // スライドすれば一致するパターン（7から遠いほうが優先度高）
                // 例：100100と010010
                if (y == (x << bit))
                {
                    edges.Add((x, y));
                }
                // スライドして一致かつ右端から連続してカードを持っているパターン（枚数が多いほうが優先度高）
                // 例：000101と010111
                if (y == ((x << bit) | ((1 << bit) - 1)))
                {
                    edges.Add((y, x));
                }
            }
        }
    }
}

Dictionary<int, List<int>> graph = vertexes.ToDictionary(x => x, _ => new List<int>());
foreach ((int from, int to) in edges)
{
    graph[from].Add(to);
}

Console.WriteLine("{");
foreach ((int v, List<int> e) in graph)
{
    Console.WriteLine($"    {{ {v}, [{string.Join(", ", e)}] }},");
}
Console.WriteLine("}");
