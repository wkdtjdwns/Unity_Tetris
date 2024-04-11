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

// ����ü ������ ����ȭ ���� -> Inspector â���� Ȯ���� �� �ְ� ����
[System.Serializable]
public struct TetrominoData
{
    public Tetromino tetromino;
    public Tile tile;

    // int���� ����ϴ� 2���� ������ �迭
    public Vector2Int[] cells { get; private set; }

    // int���� ����ϴ� 2���� ������ ������ �迭 ( [,] -> ������ �迭 )
    public Vector2Int[,] wall_kicks { get; private set; }

    // cells �� �ʱ�ȭ
    public void Initialize()
    {
        // public static class Data ���
        this.cells = Data.Cells[this.tetromino];

        // �� ����� Tetromino�� ���� WallKick ���̺��� �Ҵ��
        this.wall_kicks = Data.WallKicks[this.tetromino];
    }
}