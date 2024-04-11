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

    // Hierarchy 창에 오브젝트가 없어도 실행되고 실행시 딱 한번만 실행되는 속성
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void FirstLoad() { if (SceneManager.GetActiveScene().name.CompareTo("StartScene") != 0) { SceneManager.LoadScene("StartScene"); } }
    // 게임을 시작하면 어떤 씬을 보고 있어도 StartScene 씬에서 시작하게 함

    private void Awake()
    {
        // 키코드 값들 초기화
        left_key = KeyCode.LeftArrow;
        right_key = KeyCode.RightArrow;
        down_key = KeyCode.DownArrow;
        hard_drop_key = KeyCode.Space;
        left_rotate_key = KeyCode.Z;
        right_rotate_key = KeyCode.UpArrow;

        // BGM 실행
        SoundManager.instance.PlayBgm();
    }

    // Scene 매니저의 sceneLoaded에 체인을 검
    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }

    // 체인을 걸어서 이 함수는 매 Scene마다 호출됨
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindObjects();

        // Option 클래스에 있는 GameManager에 관련한 변수들의 값을 지정해줌 (Null 값을 인스턴스 중이라고 뜨길래 넣음)
        option_logic.game_manager_obj = this.gameObject;
        option_logic.game_manager = this;

        // TetrisScene으로 이동할 때에만 실행
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
        // 부모 오브젝트들 초기화
        canvas = GameObject.Find("Canvas");

        // canvas의 자식인 오브젝트들 찾기
        change_key_group = canvas.transform.Find("Ghange Key Group").gameObject;
        option = canvas.transform.Find("Option").gameObject;

        // Option 스크립트 초기화
        option_logic = option.GetComponent<Option>();
    }

    private void Update()
    {
        change_key_group.SetActive(is_changing);

        if (is_changing) { StartCoroutine(WaitForPress());}
    }

    private IEnumerator WaitForPress()
    {
        // 아무런 키라도 누를 때까지 무한 기다림
        while (!Input.anyKeyDown) { yield return null; }

        // foreach ~~~ : KeyCode 타입인 key_code 변수에 유니티에 존재하는 모든 KeyCode의 값을 대입하면서 반복
        // GetValues(typeof(KeyCode) -> 타입이 KeyCode인 값들을 받아옴
        foreach (KeyCode key_code in System.Enum.GetValues(typeof(KeyCode)))
        {
            // 만약 내가 누른 키가 key_code 변수와 같으면 (KeyCode 타입에 존재한다면)
            if (Input.GetKeyDown(key_code))
            {
                // ESC 키를 누르면 키를 적용시키지 않고 나감
                if (key_code == KeyCode.Escape) { break; }

                // 타입 (왼쪽 이동, 왼쪽 회전, 오른쪽 이동, 오른쪽 회전, 아래쪽 이동, 하드드랍)에 따라서
                switch (type)
                {
                    // 내가 누른 키를 해당 타입의 키에 적용함
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

                // 적용했으면 초기화 오브젝트와 변수들을 시켜줌
                type = null;
                is_changing = false;
                change_key_group.SetActive(false);

                // 사운드가 여러번 출력되는 것을 방지함
                if (!is_changing_sound) { SoundManager.instance.PlaySound("change key"); is_changing_sound = true; }
            }
        }
    }

    // 점수 및 클리어 라인을 증가시켜주는 메소드
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
