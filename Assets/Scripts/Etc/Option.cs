using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Option : MonoBehaviour
{
    [SerializeField]
    private GameObject option;

    public Text left_key_text;
    public Text left_rotate_key_text;
    public Text right_key_text;
    public Text right_rotate_key_text;
    public Text down_key_text;
    public Text hard_drop_key_text;

    public Text score_text;
    public Text line_text;

    [SerializeField]
    private Slider bgm_slider;
    [SerializeField]
    private Slider sfx_slider;

    public bool is_option;

    public GameObject game_manager_obj;
    public GameManager game_manager;

    private void Start()
    {
        bgm_slider.value = SoundManager.instance.bgm_volume;
        sfx_slider.value = SoundManager.instance.sfx_volume;

        is_option = false;

        // 각 슬라이더의 값이 바뀌었을 때 실행할 메소드 설정
        bgm_slider.onValueChanged.AddListener(ChangeBgmSound);
        sfx_slider.onValueChanged.AddListener(ChangeSfxSound);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { OptionOnOff(); }

        ChangeKeyText();
    }

    private void ChangeKeyText()
    {
        left_key_text.text = game_manager.left_key.ToString();
        left_rotate_key_text.text = game_manager.left_rotate_key.ToString();
        right_key_text.text = game_manager.right_key.ToString();
        right_rotate_key_text.text = game_manager.right_rotate_key.ToString();
        down_key_text.text = game_manager.down_key.ToString();
        hard_drop_key_text.text = game_manager.hard_drop_key.ToString();
    }

    // 옵션창을 껐다 켰다함
    public void OptionOnOff()
    {
        if (game_manager.is_changing) { game_manager.is_changing = false; return; }
        if (game_manager.is_over) { return; }

        SoundManager.instance.PlaySound("button");

        is_option = !is_option;

        option.SetActive(is_option);

        Time.timeScale = is_option ? 0 : 1;
    }

    // bgm, sfx의 사운드 볼륨을 조절함
    private void ChangeBgmSound(float value) { SoundManager.instance.bgm_volume = value; }

    private void ChangeSfxSound(float value) { SoundManager.instance.sfx_volume = value; }
}