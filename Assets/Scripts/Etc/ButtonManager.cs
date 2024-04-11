using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public GameManager game_manager;

    private GameObject game_over_manager_obj;
    private GameOverManager game_over_manager;

    private void Awake()
    {
        game_over_manager_obj = GameObject.Find("GameOverManager").gameObject;
        game_over_manager = game_over_manager_obj.GetComponent<GameOverManager>();
    }

    // GameManager 스크립트에 있는 WaitForPress() 메소드를 활성화 시키기 위함
    public void ChangeKeyGroupOn(string type)
    {
        SoundManager.instance.PlaySound("button");

        game_manager.is_changing_sound = false;

        game_manager.type = type;
        game_manager.is_changing = true;
    }

    public void GameStart() { SceneManager.LoadScene("LoadingScene"); }

    public void GameExit()
    {
        // 유니티에서만 나가지는 코드
        UnityEditor.EditorApplication.isPlaying = false;

        // 유니티가 아닌 모든 곳에서 나가지는 코드
        Application.Quit();
    }

    public void Retry()
    {
        game_manager.is_over = false;
        game_over_manager.game_over_obj.SetActive(false);

        Time.timeScale = 1;

        SceneManager.LoadScene("LoadingScene");
    }
}