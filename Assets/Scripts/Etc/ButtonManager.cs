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

    // GameManager ��ũ��Ʈ�� �ִ� WaitForPress() �޼ҵ带 Ȱ��ȭ ��Ű�� ����
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
        // ����Ƽ������ �������� �ڵ�
        UnityEditor.EditorApplication.isPlaying = false;

        // ����Ƽ�� �ƴ� ��� ������ �������� �ڵ�
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