using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    private void Awake()
    {
        // �ı����� ���� ������Ʈ ����
        GameManagerDontDestroy();
        SoundManagerDontDestroy();
    }

    // GameManager ������Ʈ �ı� X
    private void GameManagerDontDestroy()
    {
        // ���� ������Ʈ�� �߿��� GameManager �±׸� ���� ������Ʈ�� ã��
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameManager");

        // ���� ������Ʈ�� �ߺ��� ���ٸ�
        if (objs.Length == 1) { DontDestroyOnLoad(objs[0]); } // �ش� ������Ʈ�� �ı����� �ʰ� ��

        // ���� �ִٸ� 1���� ���� �ٸ� ������Ʈ���� �ı���
        else
        {
            for (int index = 1; index <= objs.Length; index++) { Destroy(objs[index]); }
        }
    }

    // SoundManager ������Ʈ �ı� X
    private void SoundManagerDontDestroy()
    {
        // ���� ������Ʈ�� �߿��� SoundManager �±׸� ���� ������Ʈ�� ã��
        GameObject[] objs = GameObject.FindGameObjectsWithTag("SoundManager");

        // ���� ������Ʈ�� �ߺ��� ���ٸ�
        if (objs.Length == 1) { DontDestroyOnLoad(objs[0]); } // �ش� ������Ʈ�� �ı����� �ʰ� ��

        // ���� �ִٸ� 1���� ���� �ٸ� ������Ʈ���� �ı���
        else
        {
            for (int index = 1; index <= objs.Length; index++) { Destroy(objs[index]); }
        }
    }
}
