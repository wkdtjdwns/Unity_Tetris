using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotation_index { get; private set; }

    private Vector2Int move_translation;

    public float step_delay = 1f;
    public float lock_delay = 0.5f;

    private float step_time;
    private float lock_time;

    private GameObject game_manager_obj;
    private GameManager game_manager;

    private void Awake()
    {
        game_manager_obj = GameObject.Find("GameManager").gameObject;
        game_manager = game_manager_obj.GetComponent<GameManager>();
    }

    // 블록의 위치와 정보를 초기화 시킴
    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        this.rotation_index = 0;

        // Time.time -> 게임이 시작하고 난 이후 경과한 시간(초)
        this.step_time = Time.time + this.step_delay;
        this.lock_time = 0f;

        if (this.cells == null) { this.cells = new Vector3Int[data.cells.Length]; }

        for (int i = 0; i < data.cells.Length; i++) { this.cells[i] = (Vector3Int)data.cells[i]; }
    }

    private void Update()
    {
        // 옵션창이 띄워져 있으면 실행하지 않음
        if (game_manager.option_logic.is_option) { return; }

        // 이동하기 전에 자신을 없앤 뒤에
        this.board.Clear(this);

        this.lock_time += Time.deltaTime;

        // 블록을 좌/우로 회전시킴
        if (Input.GetKeyDown(game_manager.left_rotate_key)) { Rotate(-1); }
        else if (Input.GetKeyDown(game_manager.right_rotate_key)) { Rotate(1); }

        // 블록을 좌/우로 움직임
        if (Input.GetKeyDown(game_manager.left_key)){ this.move_translation = Vector2Int.left; Move(); }
        else if (Input.GetKeyDown(game_manager.right_key)) { this.move_translation = Vector2Int.right; Move(); }

        // 블록을 아래로 움직임
        if (Input.GetKeyDown(game_manager.down_key)) { this.move_translation = Vector2Int.down; Move(); }

        // 블록을 맨 아래로 움직임 (Update 함수에서 실행했으니 쭉 내려가게 됨)
        if (Input.GetKeyDown(game_manager.hard_drop_key)) { HardDrop(); }

        if (Time.time >= this.step_time) { Step(); }

        // 이동하고 나서 자신의 위치를 설정해줌
        this.board.Set(this);
    }

    // 일정 시간마다 블록이 아래로 내려오도록 함
    private void Step()
    {
        this.step_time = Time.time + this.step_delay;

        this.move_translation = Vector2Int.down;
        Move();

        // 아래로 내려오고 일정 시간 동안 가만히 있으면
        // 해당 블록을 고정시키고 다음 블록을 소환함
        if (this.lock_time >= this.lock_delay) { Lock(); }
    }

    private void HardDrop()
    {
        // 게임 오버 되었을 땐 실행 X
        if (game_manager.is_over) { return; }

        // 더 내려가지 못할 때까지 아래로 내려가게 함
        this.move_translation = Vector2Int.down;
        while (Move()) { continue; }

        // 해당 블록을 움직이지 못하게 함
        Lock();
    }

    // 해당 블록의 위치를 저장 (움직이지 못하게 함)하고 새로운 블록을 소환함
    private void Lock()
    {
        // 게임 오버 되었을 땐 실행 X
        if (!game_manager.is_over) { SoundManager.instance.PlaySound("put block"); }

        this.board.Set(this);
        this.board.ClearLines();
        int index = this.board.SetIndex();
        this.board.SpawnPiece(index);
    }

    // 블록을 움직이는 메소드
    private bool Move()
    {
        // 게임 오버 되었을 땐 실행 X
        if (game_manager.is_over) { return false; }

        Vector3Int new_position = this.position;

        // 다음 위치에 매개변수로 받아온 값을 더해줌
        new_position.x += move_translation.x;
        new_position.y += move_translation.y;

        // 다음 위치로 이동할 수 있는지 확인하고
        bool valid = this.board.IsValidPosition(this, new_position);

        // 이동할 수 있으면 이동하고
        if (valid)
        {
            this.position = new_position;

            // 이동할 때마다 lock_time이 초기화되어서
            // 움직이고 있으면 계속 움직일 수 있게 함
            this.lock_time = 0f;
        }

        // valid 값을 리턴함
        return valid;
    }

    // 블록을 회전시키는 메소드
    private void Rotate(int direction)
    {
        // rotation_index를 정하기 전에 값을 저장해 놓음
        int original_rotation = this.rotation_index;

        // rotation_index = {0, 1, 2, 3}
        this.rotation_index = Wrap(this.rotation_index + direction, 0, 4);

        ApplyRotationMatrix(direction);

        if (!TestWallKicks(this.rotation_index, direction))
        {
            this.rotation_index = original_rotation;
            ApplyRotationMatrix(-direction);
        }
    }

    // 블록을 실질적으로 회전시키는 메소드
    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            // 오프셋 값 때문에 int 값이 아닌 float 값이 필요함
            // -> Vector3Int가 아닌 Vector3 사용
            Vector3 cell = this.cells[i];

            int x, y;

            // 테트로미노의 종류에 따라 다른 회전 로직을 적용함
            switch (this.data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    /* I와 O 테트로미노는 중심을 기준으로 회전하며, 각 셀의 좌표를 조정해야함

                     * I는 세로로 길쭉한 형태이며 중심을 기준으로 화전하려면
                     * 셀의 좌표를 조정해야 원하는 모양을 유지할 수 있음
                     * 그래서 cell.x, cell.y -= 0.5f를 통해서 중심을 기준으로 좌표를 조정한 뒤에
                     * CeilToInt를 사용해 소수점 이하를 올림 처리함

                     * O는 정사각형 모양이며 중심을 기준으로 회전해도 형태가 변하지 않음
                     * 그래서 따로 좌표를 지정하지 않고 올림 처리함 */

                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                default:
                    /* 다른 테트로미노들은 회전 시에도 모양이 그대로 유지되기 때문에 따로 좌표를 조정할 필요가 없음
                     * 그래서 기본적으로 RoundToInt를 사용해 반올림 처리함 */

                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            this.cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotation_index, int rotation_direction)
    {
        int wall_kick_index = GetWallKickIndex(rotation_index, rotation_direction);

        // Data.WallKicks.GetLength(1) : WallKick을 할 때의 좌표들
        for (int i = 0; i < this.data.wall_kicks.GetLength(1); i++)
        {
            // 가져온 좌표들로 움직이게 하기위한 변수
            Vector2Int translation = this.data.wall_kicks[wall_kick_index, i];

            // 가져온 좌표들로 움직이려고 시도하고
            // 가능하면 움직이고 true 반환
            this.move_translation = translation;
            if (Move()) { return true; }
        }

        // 불가능하면 false 반환
        return false;
    }

    // 특정 회전 인덱스와 회전 방향에 따라서 계산된 wall_kick_index를 반환함
    private int GetWallKickIndex(int rotation_index, int rotation_direction)
    {
        int wall_kick_index = rotation_index * 2;

        // 회전 방향이 반시계 방향인 경우에 wall_kick_index를 조정함
        if (rotation_direction < 0) { wall_kick_index--; }

        // Data.WallKicks.GetLength(0) : WallKick을 할 수있는 모든 경우의 수
        return Wrap(wall_kick_index, 0, this.data.wall_kicks.GetLength(0));
    }

    // 인덱스 값을 순환시키기 위한 메소드 (주어진 범위 내에서 래핑 (감싸기)함)
    private int Wrap(int input, int min, int max)
    {
        /* input = 4
         * min = 0
         * max = 4
         * -> 0 + (4 - 0) % 4 -> 4 % 4 = "0"

         * input = -1
         * min = 0
         * max = 4
         * -> 4 - (0 - (-1)) % 4 -> (4 - 1) % 4 -> 3 % 4 = "3" */

        if (input < min) { return max - (min - input) % (max - min); }
        else { return min + (input - min) % (max - min); }
    }
}
