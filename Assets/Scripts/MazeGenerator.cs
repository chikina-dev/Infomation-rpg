using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    public int width = 21;
    public int height = 21;
    public Vector2Int startPosition = Vector2Int.zero;

    public Tilemap tilemap;
    public Tile wallTile;

    void Start()
    {
        GenerateMaze();
    }

    void GenerateMaze()
    {
        width = Mathf.Max(5, width | 1);   // 奇数化
        height = Mathf.Max(5, height | 1); // 奇数化

        Tile[,] grid = new Tile[width, height];

        // 1. 全マスnullに初期化（何も配置しない）
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = null;
            }
        }

        System.Random rand = new System.Random();

        for (int x = 1; x < width; x += 2)
        {
            for (int y = 1; y < height; y += 2)
            {
                // 奇数マスに壁を立てる
                grid[x, y] = wallTile;

                List<Vector2Int> directions = new List<Vector2Int>
                {
                    Vector2Int.up,
                    Vector2Int.down,
                    Vector2Int.left,
                    Vector2Int.right
                };

                // 倒す方向をランダムに1つ選択
                while (directions.Count > 0)
                {
                    int idx = rand.Next(directions.Count);
                    Vector2Int dir = directions[idx];
                    directions.RemoveAt(idx);

                    int nx = x + dir.x;
                    int ny = y + dir.y;

                    // 範囲内かチェック
                    if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                    {
                        grid[nx, ny] = wallTile;
                        break; // 1回倒したら終了
                    }
                }
            }
        }


        // 4. Tilemapに描画（Y軸反転して右下原点に）
        tilemap.ClearAllTiles();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] != null)
                {
                    Vector3Int pos = new Vector3Int(x + startPosition.x, -y + startPosition.y, 0);
                    tilemap.SetTile(pos, grid[x, y]);
                }
            }
        }

        // 5. コライダー更新
        var collider = tilemap.GetComponent<TilemapCollider2D>();
        if (collider != null)
        {
            collider.enabled = false;
            collider.enabled = true;
        }
    }
}
