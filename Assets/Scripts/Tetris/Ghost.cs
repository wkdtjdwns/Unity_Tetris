using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board board;
    public Piece tracking_piece;

    public Tilemap tilemap { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.cells = new Vector3Int[4];
    }

    // Piece 클래스의 Update() 함수가 실행된 뒤에 실행하기 위함
    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tile_position = this.cells[i] + this.position;
            this.tilemap.SetTile(tile_position, null);
        }
    }

    private void Copy()
    {
        // 해당 클래스의 모든 cells 값에게 적용함
        for (int i = 0; i < this.cells.Length; i++)
        {
            // 해당 클래스의 cells 값을 따라다닐 블록의 cells 값으로 바꿈
            this.cells[i] = this.tracking_piece.cells[i];
        }
    }

    private void Drop()
    {
        Vector3Int position = this.tracking_piece.position;

        // 현재 블록의 y좌표
        int current = position.y;

        // 가장 아래에 있는 타일의 y좌표
        int bottom = -this.board.board_size.y / 2 - 1;

        // 현재 위치의 블록을 제거함
        this.board.Clear(this.tracking_piece);

        // 아래로 이동할 수 있는 위치로 찾아가기 (이동하기)
        // 현재 블록의 위치에서 가장 아래에 있는 타일 좌표까지 반복
        for (int row = current; row >= bottom; row--)
        {
            position.y = row;

            // 블록이 이동 가능한 위치인지 확인하고 가능하면 위치를 갱신하고 계속해서 아래로 이동함
            if (this.board.IsValidPosition(this.tracking_piece, position)) { this.position = position; }

            // 이동이 불가능하면 반복문을 종료함
            else { break; }
        }

        // 이동 가능한 위치에 블록을 새롭게 설정함
        this.board.Set(this.tracking_piece);
    }

    private void Set()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tile_position = this.cells[i] + this.position;
            this.tilemap.SetTile(tile_position, this.tile);
        }
    }
}