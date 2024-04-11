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

    // ����� ��ġ�� ������ �ʱ�ȭ ��Ŵ
    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        this.rotation_index = 0;

        // Time.time -> ������ �����ϰ� �� ���� ����� �ð�(��)
        this.step_time = Time.time + this.step_delay;
        this.lock_time = 0f;

        if (this.cells == null) { this.cells = new Vector3Int[data.cells.Length]; }

        for (int i = 0; i < data.cells.Length; i++) { this.cells[i] = (Vector3Int)data.cells[i]; }
    }

    private void Update()
    {
        // �ɼ�â�� ����� ������ �������� ����
        if (game_manager.option_logic.is_option) { return; }

        // �̵��ϱ� ���� �ڽ��� ���� �ڿ�
        this.board.Clear(this);

        this.lock_time += Time.deltaTime;

        // ����� ��/��� ȸ����Ŵ
        if (Input.GetKeyDown(game_manager.left_rotate_key)) { Rotate(-1); }
        else if (Input.GetKeyDown(game_manager.right_rotate_key)) { Rotate(1); }

        // ����� ��/��� ������
        if (Input.GetKeyDown(game_manager.left_key)){ this.move_translation = Vector2Int.left; Move(); }
        else if (Input.GetKeyDown(game_manager.right_key)) { this.move_translation = Vector2Int.right; Move(); }

        // ����� �Ʒ��� ������
        if (Input.GetKeyDown(game_manager.down_key)) { this.move_translation = Vector2Int.down; Move(); }

        // ����� �� �Ʒ��� ������ (Update �Լ����� ���������� �� �������� ��)
        if (Input.GetKeyDown(game_manager.hard_drop_key)) { HardDrop(); }

        if (Time.time >= this.step_time) { Step(); }

        // �̵��ϰ� ���� �ڽ��� ��ġ�� ��������
        this.board.Set(this);
    }

    // ���� �ð����� ����� �Ʒ��� ���������� ��
    private void Step()
    {
        this.step_time = Time.time + this.step_delay;

        this.move_translation = Vector2Int.down;
        Move();

        // �Ʒ��� �������� ���� �ð� ���� ������ ������
        // �ش� ����� ������Ű�� ���� ����� ��ȯ��
        if (this.lock_time >= this.lock_delay) { Lock(); }
    }

    private void HardDrop()
    {
        // ���� ���� �Ǿ��� �� ���� X
        if (game_manager.is_over) { return; }

        // �� �������� ���� ������ �Ʒ��� �������� ��
        this.move_translation = Vector2Int.down;
        while (Move()) { continue; }

        // �ش� ����� �������� ���ϰ� ��
        Lock();
    }

    // �ش� ����� ��ġ�� ���� (�������� ���ϰ� ��)�ϰ� ���ο� ����� ��ȯ��
    private void Lock()
    {
        // ���� ���� �Ǿ��� �� ���� X
        if (!game_manager.is_over) { SoundManager.instance.PlaySound("put block"); }

        this.board.Set(this);
        this.board.ClearLines();
        int index = this.board.SetIndex();
        this.board.SpawnPiece(index);
    }

    // ����� �����̴� �޼ҵ�
    private bool Move()
    {
        // ���� ���� �Ǿ��� �� ���� X
        if (game_manager.is_over) { return false; }

        Vector3Int new_position = this.position;

        // ���� ��ġ�� �Ű������� �޾ƿ� ���� ������
        new_position.x += move_translation.x;
        new_position.y += move_translation.y;

        // ���� ��ġ�� �̵��� �� �ִ��� Ȯ���ϰ�
        bool valid = this.board.IsValidPosition(this, new_position);

        // �̵��� �� ������ �̵��ϰ�
        if (valid)
        {
            this.position = new_position;

            // �̵��� ������ lock_time�� �ʱ�ȭ�Ǿ
            // �����̰� ������ ��� ������ �� �ְ� ��
            this.lock_time = 0f;
        }

        // valid ���� ������
        return valid;
    }

    // ����� ȸ����Ű�� �޼ҵ�
    private void Rotate(int direction)
    {
        // rotation_index�� ���ϱ� ���� ���� ������ ����
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

    // ����� ���������� ȸ����Ű�� �޼ҵ�
    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            // ������ �� ������ int ���� �ƴ� float ���� �ʿ���
            // -> Vector3Int�� �ƴ� Vector3 ���
            Vector3 cell = this.cells[i];

            int x, y;

            // ��Ʈ�ι̳��� ������ ���� �ٸ� ȸ�� ������ ������
            switch (this.data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    /* I�� O ��Ʈ�ι̳�� �߽��� �������� ȸ���ϸ�, �� ���� ��ǥ�� �����ؾ���

                     * I�� ���η� ������ �����̸� �߽��� �������� ȭ���Ϸ���
                     * ���� ��ǥ�� �����ؾ� ���ϴ� ����� ������ �� ����
                     * �׷��� cell.x, cell.y -= 0.5f�� ���ؼ� �߽��� �������� ��ǥ�� ������ �ڿ�
                     * CeilToInt�� ����� �Ҽ��� ���ϸ� �ø� ó����

                     * O�� ���簢�� ����̸� �߽��� �������� ȸ���ص� ���°� ������ ����
                     * �׷��� ���� ��ǥ�� �������� �ʰ� �ø� ó���� */

                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                default:
                    /* �ٸ� ��Ʈ�ι̳���� ȸ�� �ÿ��� ����� �״�� �����Ǳ� ������ ���� ��ǥ�� ������ �ʿ䰡 ����
                     * �׷��� �⺻������ RoundToInt�� ����� �ݿø� ó���� */

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

        // Data.WallKicks.GetLength(1) : WallKick�� �� ���� ��ǥ��
        for (int i = 0; i < this.data.wall_kicks.GetLength(1); i++)
        {
            // ������ ��ǥ��� �����̰� �ϱ����� ����
            Vector2Int translation = this.data.wall_kicks[wall_kick_index, i];

            // ������ ��ǥ��� �����̷��� �õ��ϰ�
            // �����ϸ� �����̰� true ��ȯ
            this.move_translation = translation;
            if (Move()) { return true; }
        }

        // �Ұ����ϸ� false ��ȯ
        return false;
    }

    // Ư�� ȸ�� �ε����� ȸ�� ���⿡ ���� ���� wall_kick_index�� ��ȯ��
    private int GetWallKickIndex(int rotation_index, int rotation_direction)
    {
        int wall_kick_index = rotation_index * 2;

        // ȸ�� ������ �ݽð� ������ ��쿡 wall_kick_index�� ������
        if (rotation_direction < 0) { wall_kick_index--; }

        // Data.WallKicks.GetLength(0) : WallKick�� �� ���ִ� ��� ����� ��
        return Wrap(wall_kick_index, 0, this.data.wall_kicks.GetLength(0));
    }

    // �ε��� ���� ��ȯ��Ű�� ���� �޼ҵ� (�־��� ���� ������ ���� (���α�)��)
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
