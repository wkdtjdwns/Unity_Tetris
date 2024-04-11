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

    // Piece Ŭ������ Update() �Լ��� ����� �ڿ� �����ϱ� ����
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
        // �ش� Ŭ������ ��� cells ������ ������
        for (int i = 0; i < this.cells.Length; i++)
        {
            // �ش� Ŭ������ cells ���� ����ٴ� ����� cells ������ �ٲ�
            this.cells[i] = this.tracking_piece.cells[i];
        }
    }

    private void Drop()
    {
        Vector3Int position = this.tracking_piece.position;

        // ���� ����� y��ǥ
        int current = position.y;

        // ���� �Ʒ��� �ִ� Ÿ���� y��ǥ
        int bottom = -this.board.board_size.y / 2 - 1;

        // ���� ��ġ�� ����� ������
        this.board.Clear(this.tracking_piece);

        // �Ʒ��� �̵��� �� �ִ� ��ġ�� ã�ư��� (�̵��ϱ�)
        // ���� ����� ��ġ���� ���� �Ʒ��� �ִ� Ÿ�� ��ǥ���� �ݺ�
        for (int row = current; row >= bottom; row--)
        {
            position.y = row;

            // ����� �̵� ������ ��ġ���� Ȯ���ϰ� �����ϸ� ��ġ�� �����ϰ� ����ؼ� �Ʒ��� �̵���
            if (this.board.IsValidPosition(this.tracking_piece, position)) { this.position = position; }

            // �̵��� �Ұ����ϸ� �ݺ����� ������
            else { break; }
        }

        // �̵� ������ ��ġ�� ����� ���Ӱ� ������
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