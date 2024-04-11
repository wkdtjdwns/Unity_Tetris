using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    // �ٸ� Ŭ���������� ���� ������ �� �ֵ��� ��
    public static SoundManager instance;

    [SerializeField]
    private AudioClip[] bgm_clips;
    [SerializeField]
    private AudioClip[] audio_clips;

    public float bgm_volume;
    public float sfx_volume;

    public AudioSource bgm_player;
    public AudioSource sfx_player;

    private void Awake()
    {
        instance = this;

        bgm_volume = 0.5f;
        sfx_volume = 0.5f;

        bgm_player = GameObject.Find("Bgm Player").gameObject.GetComponent<AudioSource>();
        sfx_player = GameObject.Find("Sfx Player").gameObject.GetComponent<AudioSource>();

        /*PlayBgm("start");*/
    }

    private void Update()
    {
        // �������� ������ ������ �� �ְ���
        ChangeBgmSound();
        ChangeSfxSound();
    }

    public void PlaySound(string type)
    {
        int index = 0;

        // Ÿ�Կ� ���� �ٸ� ���带 �÷�����
        switch (type)
        {
            case "button": index = 0; break;;
            case "change key": index = 1; break;
            case "line clear": index = 2; break;
            case "put block": index = 3; break;
        }

        sfx_player.clip = audio_clips[index];

        // 1������ �ƴ� �������� ���尡 ���ĵ� ��� ������
        sfx_player.PlayOneShot(sfx_player.clip);
    }

    public void PlayBgm()
    {
        bgm_player.clip = bgm_clips[0];

        // �������� ���尡 ��ġ�� �������� ������ ���常 ������
        bgm_player.Play();
    }

    // Option���� ������ ���� ���� �����Ŵ
    private void ChangeBgmSound() { bgm_player.volume = bgm_volume; }

    private void ChangeSfxSound() { sfx_player.volume = sfx_volume; }
}
