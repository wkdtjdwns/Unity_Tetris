using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
    I,
    O,
    T,
    J,
    L,
    S,
    Z,
}

// 구조체 변수를 직렬화 해줌 -> Inspector 창에서 확인할 수 있게 해줌
[System.Serializable]
public struct TetrominoData
{
    public Tetromino tetromino;
    public Tile tile;

    // int형을 취급하는 2차원 벡터형 배열
    public Vector2Int[] cells { get; private set; }

    // int형을 취급하는 2차원 벡터형 이차원 배열 ( [,] -> 이차원 배열 )
    public Vector2Int[,] wall_kicks { get; private set; }

    // cells 값 초기화
    public void Initialize()
    {
        // public static class Data 사용
        this.cells = Data.Cells[this.tetromino];

        // 각 블록의 Tetromino에 따른 WallKick 테이블이 할당됨
        this.wall_kicks = Data.WallKicks[this.tetromino];
    }
}