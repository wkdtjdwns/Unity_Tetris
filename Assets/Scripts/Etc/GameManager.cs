using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public KeyCode left_key;
    public KeyCode right_key;
    public KeyCode down_key;
    public KeyCode hard_drop_key;
    public KeyCode left_rotate_key;
    public KeyCode right_rotate_key;

    private GameObject canvas;
    private GameObject change_key_group;

    private int score;
    private int line;

    public string type;

    public bool is_changing;
    public bool is_over;
    public bool is_changing_sound;

    private GameObject option;
    public Option option_logic;

    // Hierarchy â�� ������Ʈ�� ��� ����ǰ� ����� �� �ѹ��� ����Ǵ� �Ӽ�
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void FirstLoad() { if (SceneManager.GetActiveScene().name.CompareTo("StartScene") != 0) { SceneManager.LoadScene("StartScene"); } }
    // ������ �����ϸ� � ���� ���� �־ StartScene ������ �����ϰ� ��

    private void Awake()
    {
        // Ű�ڵ� ���� �ʱ�ȭ
        left_key = KeyCode.LeftArrow;
        right_key = KeyCode.RightArrow;
        down_key = KeyCode.DownArrow;
        hard_drop_key = KeyCode.Space;
        left_rotate_key = KeyCode.Z;
        right_rotate_key = KeyCode.UpArrow;

        // BGM ����
        SoundManager.instance.PlayBgm();
    }

    // Scene �Ŵ����� sceneLoaded�� ü���� ��
    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }

    // ü���� �ɾ �� �Լ��� �� Scene���� ȣ���
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindObjects();

        // Option Ŭ������ �ִ� GameManager�� ������ �������� ���� �������� (Null ���� �ν��Ͻ� ���̶�� �߱淡 ����)
        option_logic.game_manager_obj = this.gameObject;
        option_logic.game_manager = this;

        // TetrisScene���� �̵��� ������ ����
        if (scene.name == "TetrisScene" || scene.name == "StartScene")
        {
            ButtonManager button_manager = GameObject.Find("ButtonManager").GetComponent<ButtonManager>();
            button_manager.game_manager = this;

            option_logic.score_text.text = "SCORE : 0";
            option_logic.line_text.text = "Line : 0";
        }
    }

    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    private void FindObjects()
    {
        // �θ� ������Ʈ�� �ʱ�ȭ
        canvas = GameObject.Find("Canvas");

        // canvas�� �ڽ��� ������Ʈ�� ã��
        change_key_group = canvas.transform.Find("Ghange Key Group").gameObject;
        option = canvas.transform.Find("Option").gameObject;

        // Option ��ũ��Ʈ �ʱ�ȭ
        option_logic = option.GetComponent<Option>();
    }

    private void Update()
    {
        change_key_group.SetActive(is_changing);

        if (is_changing) { StartCoroutine(WaitForPress());}
    }

    private IEnumerator WaitForPress()
    {
        // �ƹ��� Ű�� ���� ������ ���� ��ٸ�
        while (!Input.anyKeyDown) { yield return null; }

        // foreach ~~~ : KeyCode Ÿ���� key_code ������ ����Ƽ�� �����ϴ� ��� KeyCode�� ���� �����ϸ鼭 �ݺ�
        // GetValues(typeof(KeyCode) -> Ÿ���� KeyCode�� ������ �޾ƿ�
        foreach (KeyCode key_code in System.Enum.GetValues(typeof(KeyCode)))
        {
            // ���� ���� ���� Ű�� key_code ������ ������ (KeyCode Ÿ�Կ� �����Ѵٸ�)
            if (Input.GetKeyDown(key_code))
            {
                // ESC Ű�� ������ Ű�� �����Ű�� �ʰ� ����
                if (key_code == KeyCode.Escape) { break; }

                // Ÿ�� (���� �̵�, ���� ȸ��, ������ �̵�, ������ ȸ��, �Ʒ��� �̵�, �ϵ���)�� ����
                switch (type)
                {
                    // ���� ���� Ű�� �ش� Ÿ���� Ű�� ������
                    case "left":
                        left_key = key_code; break;

                    case "left_rotate":
                        left_rotate_key = key_code; break;

                    case "right":
                        right_key = key_code; break;

                    case "right_rotate":
                        right_rotate_key = key_code; break;

                    case "down":
                        down_key = key_code; break;

                    case "hard_drop":
                        hard_drop_key = key_code; break;
                }

                // ���������� �ʱ�ȭ ������Ʈ�� �������� ������
                type = null;
                is_changing = false;
                change_key_group.SetActive(false);

                // ���尡 ������ ��µǴ� ���� ������
                if (!is_changing_sound) { SoundManager.instance.PlaySound("change key"); is_changing_sound = true; }
            }
        }
    }

    // ���� �� Ŭ���� ������ ���������ִ� �޼ҵ�
    public void GetScore()
    {
        score += 1000;
        option_logic.score_text.text = string.Format("SCORE : {0}", score);

        line++;
        option_logic.line_text.text = string.Format("LINE : {0}", line);
    }

    public void GameOver()
    {
        print("Game Over");
    }
}
