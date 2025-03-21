﻿#include <iostream>
#include <vector>
#include <utility>
#include <set>
#include <algorithm>
#include <climits>
#include <cassert>
#include <bitset>
using namespace std;
using ll = long long;

constexpr int BIT_WIDTH = 6;
constexpr int VERTEX_MAX = 1 << BIT_WIDTH;

pair<int, int> getTopTwo(int pattern) {
    int first = -1, second = -1;
    for (int bit = 0; bit < BIT_WIDTH; ++bit) {
        if ((pattern & (1 << bit)) > 0) {
            second = first;
            first = bit;
        }
    }
    return { first, second };
}

int main() {
    // 対象となるパターン（2枚以上のカードあり and 7に近い2枚が階段ではない）の頂点集合
    // ※計算量の都合で単体はあとから差し込む
    vector<int> vertexes;
    for (int i = 1; i < VERTEX_MAX; ++i) {
        auto [first, second] = getTopTwo(i);
        if (second >= 0 && first - second > 1) {
            vertexes.push_back(i);
        }
    }
    int numVertexes = vertexes.size();

    for (const int v : vertexes) {
        cout << bitset<6>(v) << endl;
    }

    // 2つのパターンの優先度に順位をつけるための辺集合（uが優先度高）
    // 後の操作で計算量を減らすためにu,vは実際の頂点番号ではなくvertexes内のインデックスにする
    set<pair<int, int>> edges;
    for (int i = 0; i < numVertexes; ++i) {
        for (int j = i + 1; j < numVertexes; ++j) {
            int x = vertexes[i], y = vertexes[j];
            auto [x1, x2] = getTopTwo(x);
            auto [y1, y2] = getTopTwo(y);
            // 以降x < yに留意
            if (x1 == y1 && x2 == y2 && (x & y) == x) {
                // 7に近い2枚が同じで、 全体が包含関係にあるパターン（枚数が多いほうが優先度高）
                // 例：010100と010110
                edges.emplace(j, i);
            } else if ((x ^ 1 << x1) == (y ^ 1 << y1)) {
                // 7に最も近いカードのみが異なり、以降が全く同じパターン（7から遠いほうが優先度高）
                // 例：100101と010101
                edges.emplace(i, j);
            } else {
                for (int bit = 1; bit < BIT_WIDTH; ++bit) {
                    // スライドすれば一致するパターン（7から遠いほうが優先度高）
                    // 例：100100と010010
                    if (y == (x << bit)) {
                        edges.emplace(i, j);
                    }
                    // スライドして一致かつ右端から連続してカードを持っているパターン（枚数が多いほうが優先度高）
                    // 例：000101と010111
                    if (y == ((x << bit) | ((1 << bit) - 1))) {
                        edges.emplace(j, i);
                    }
                }
            }
        }
    }

    vector<vector<int>> graph(numVertexes);
    for (const auto& [i, j] : edges) {
        graph[i].push_back(j);
    }

    // トポロジカルソートの数え上げ
    vector<ll> dp(1LL << numVertexes);
    dp[0] = 1;
    for (ll bit = 0; bit < (ll)dp.size(); ++bit) {
        assert(bit < dp.max_size());
        if (bit % 100000LL == 0) {
            cout << bit << endl;
        }

        for (int i = 0; i < numVertexes; ++i) {
            if ((bit & 1LL << i) > 0) continue;
            // より右に来るべき頂点が集合内にあるならスキップ
            if (any_of(
                graph[i].begin(), 
                graph[i].end(), 
                [&](int j) { return (bit & 1LL << j) > 0; }
            )) continue;

            ll newBit = bit | 1LL << i;
            assert(newBit < dp.max_size());
            // オーバーフローチェック
            if (dp[newBit] > LLONG_MAX - dp[bit]) {
                cout << "overflow" << endl;
                return 0;
            }
            dp[newBit] += dp[bit];
        }
    }
    
    cout << endl;
    cout << "Ans = " << dp[(1LL << numVertexes) - 1] << endl;

    return 0;
}

// 現状、単体を除いても34,538,776,845,694,347通り

// スライド＋最右以降が全てカードあり
// 001010 弱
// 010111 強

// これでも 24,990,225,757,718通り