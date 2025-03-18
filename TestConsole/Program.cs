(int first, int second) getTopTwo(int pattern)
{
    int first = -1, second = -1;
    for (int bit = 0; bit < 6; ++bit)
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
int[] vertexes = Enumerable.Range(1, 63).Where(x =>
{
    (int first, int second) = getTopTwo(x);
    return second == -1 || first - second > 1;
}).ToArray();

// 2つのパターンの優先度に順位をつけるための辺集合（uが優先度高）
List<(int u, int v)> edges = [];
for (int i = 0; i < vertexes.Length; ++i)
{
    for (int j = i + 1; j < vertexes.Length; ++j)
    {
        int x = vertexes[i], y = vertexes[j];
        // 7に近い2枚が同じ and 全体が包含関係にあるパターン
        (int x1, int x2) = getTopTwo(x);
        (int y1, int y2) = getTopTwo(y);
        if (x1 == y1 && x2 == y2 && (x & y) == x)  // x < yに留意
        {
            edges.Add((y, x));
        }
        // スライドすれば一致するパターン
        for (int bit = 0; bit < 6; ++bit)
        {
            if (y == (x << bit))
            {
                edges.Add((x, y));
            }
        }
    }
}


