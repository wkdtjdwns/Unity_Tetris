using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece active_piece { get; private set; }
    public TetrominoData[] tetrominoes;
    public Vector3Int spawn_position;
    public Vector2Int board_size = new Vector2Int(10, 20);

    [SerializeField]
    private Image next_block_img;
    [SerializeField]
    private Sprite[] next_block_sprites;

    private bool is_start;

    [SerializeField]
    private int cur_block_index;
    [SerializeField]
    private int next_block_index;

    private GameObject game_manager_obj;
    private GameManager game_manager;

    private GameObject game_over_manager_obj;
    private GameOverManager game_over_manager;

    // RectInt -> 사각형의 크기 (정수형)
    public RectInt Bounds
    {
        get
        {
            /* -this.board_size.x : 왼쪽
             * -this.board_size.y : 아래쪽
             * / 2 : 좌표를 이동시킬 때 board_size의 중심을 기준으로 이동시키기 위함임 */
            Vector2Int position = new Vector2Int(-this.board_size.x / 2, -this.board_size.y / 2);
            return new RectInt(position, this.board_size);
        }
    }

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.active_piece = GetComponentInChildren<Piece>();

        game_manager_obj = GameObject.Find("GameManager").gameObject;
        game_manager = game_manager_obj.GetComponent<GameManager>();

        game_over_manager_obj = GameObject.Find("GameOverManager").gameObject;
        game_over_manager = game_over_manager_obj.GetComponent<GameOverManager>();

        // 셀들의 값을 초기화 시킴
        for (int i = 0; i < this.tetrominoes.Length; i++) { this.tetrominoes[i].Initialize(); }
    }

    private void Start()
    {
        // 셀들의 값을 초기화 시킨 후에 블록 (조각)을 소환함
        int index = SetIndex();

        SpawnPiece(index);
    }

    // 블록을 소환하고 블록의 위치를 설정해줌
    public void SpawnPiece(int index)
    {
        // 다음에 생성할 블록의 인덱스에 따라서
        // 다음에 생성할 블록을 알려주는 이미지를 바꿈
        next_block_img.sprite = next_block_sprites[next_block_index];

        TetrominoData data = this.tetrominoes[cur_block_index];
        this.active_piece.Initialize(this, this.spawn_position, data);

        // 블록을 소환할 때 이미 해당 위치에 블록이 없다면 (블록이 최상단까지 쌓여있지 않다면) 블록을 생성함
        if (IsValidPosition(this.active_piece, this.spawn_position)) { Set(this.active_piece); }

        // 블록이 있다면 (블록이 최상단까지 쌓여있다면)
        else { GameOver(); } // 게임 오버
    }

    // 어떤 블록을 소환할지 결정하는 메소드
    public int SetIndex()
    {
        if (is_start) // 블록 생성이 처음일 경우에만
        {
            is_start = false;

            // 현재 생성할 블록의 인덱스를 정하고
            cur_block_index = Random.Range(0, this.tetrominoes.Length);

            // 다음에 생성할 블록의 인덱스도 정함
            next_block_index = Random.Range(0, this.tetrominoes.Length);

            return cur_block_index;
        }

        else // 나머지 경우에는
        {
            // 현재 생성할 블록의 인덱스는 저번에 정했던 다음 생성 인덱스로 바꾸고
            cur_block_index = next_block_index;

            // 다음에 생성할 블록 인덱스는 다시 정함
            next_block_index = Random.Range(0, this.tetrominoes.Length);
        }

        return next_block_index;
    }

    private void GameOver()
    {
        // 게임에 있는 모든 타일을 제거함
        this.tilemap.ClearAllTiles();

        // 게임 오버 화면을 띄움
        game_manager.is_over = true;

        Time.timeScale = 0;

        game_over_manager.game_over_obj.SetActive(true);
    }

    // 블록의 위치를 설정함
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tile_position = piece.cells[i] + piece.position;

            // Tilemap.SetTile(Vector3Int position, TileBase tile)
            // - 특정 좌표에 타일을 설정함
            this.tilemap.SetTile(tile_position, piece.data.tile);
        }
    }

    // 블록을 없앰 (이동하기 전에 실행함)
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tile_position = piece.cells[i] + piece.position;

            // 해당 위치에 있는 블록의 값을 null 값으로 설정함
            this.tilemap.SetTile(tile_position, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = this.Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tile_position = piece.cells[i] + position;

            // RectInt.Contains(Vector2Int position)
            // - 주어진 위치가 현재 RectInt의 범위 안에 속하는지의 여부를 판단함

            // tile_position이 bounds의 범위에 속해있지 않다면 false 값을 리턴함
            if (!bounds.Contains((Vector2Int)tile_position)) { return false; }

            // tile_position에 움직일 수 있는 타일이 있는지 (해당 위치에 다른 타일이 없는지) 확인하고
            // 없으면 (해당 위치에 다른 타일이 있으면) false 값을 리턴함
            if (this.tilemap.HasTile(tile_position)) { return false; }
        }

        return true;
    }

    // 1줄을 다 채우면 라인을 지우는 메소드
    public void ClearLines()
    {
        RectInt bounds = this.Bounds;

        // row 값을 가장 아래에 있는 타일로 설정하고
        int row = bounds.yMin;

        // row의 값이 최상단 타일의 y좌표가 될 때까지
        while (row < bounds.yMax)
        {
            // 해당 줄이 꽉 차있으면 LineClear() 함수를 실행하고
            if (IsLineFull(row)) { LineClear(row); }

            // 아니면 row 값을 증가시킴
            else { row++; }
        }
    }

    // 1줄이 다 찼는지 확인하는 메소드
    private bool IsLineFull(int row)
    {
        RectInt bounds = this.Bounds;

        // col 값을 가장 왼쪽에서 가장 오른쪽까지 증가시켜가며 반복하고
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            // col 값과 받아온 row 값을 바탕으로 줄들을 확인 함
            Vector3Int position = new Vector3Int(col, row, 0);

            // 해당 줄에 타일이 없으면 false 반환
            if (!this.tilemap.HasTile(position)) { return false; }
        }

        return true;
    }

    // 실질적으로 라인을 지우는 메소드
    private void LineClear(int row)
    {
        RectInt bounds = this.Bounds;

        // 가장 왼쪽부터 가장 오른쪽까지
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            // 가로/세로의 위치를 받아서 위치를 결정한 뒤
            Vector3Int position = new Vector3Int(col, row, 0);

            // 해당 위치에 있는 타일을 없앰
            this.tilemap.SetTile(position, null);
        }

        // row의 값이 최상단의 y좌표보다 크거나 같을 때까지 반복
        while (row < bounds.yMax)
        {
            // 가장 왼쪽부터 가장 오른쪽까지

            // 한마디로 모든 타일을 말함
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                // 위에서 클리어한 줄의 바로 위에 있는 좌표들의 위치 값을 받고
                Vector3Int position = new Vector3Int(col, row + 1, 0);

                // 해당 위치에 있는 모든 타일들의 위치를 받아서 저장하고
                TileBase above = this.tilemap.GetTile(position);

                // 그 타일들을 당길 위치를 설정 (row + 1에서 그냥 row로 바뀜 | 한마디로 바로 아래임)
                position = new Vector3Int(col, row, 0);

                // 윗 줄에 있던 타일들(above)을 1칸 아래 (position)으로 당겨줌
                this.tilemap.SetTile(position, above);
            }

            // 다음 행으로 이동해서 반복함
            row++;
        }

        SoundManager.instance.PlaySound("line clear");
        game_manager.GetScore();
    }
}