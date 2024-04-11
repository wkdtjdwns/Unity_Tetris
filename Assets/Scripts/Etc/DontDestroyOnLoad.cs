using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    private void Awake()
    {
        // 파괴하지 않을 오브젝트 생성
        GameManagerDontDestroy();
        SoundManagerDontDestroy();
    }

    // GameManager 오브젝트 파괴 X
    private void GameManagerDontDestroy()
    {
        // 게임 오브젝트들 중에서 GameManager 태그를 지닌 오브젝트를 찾음
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameManager");

        // 만약 오브젝트의 중복이 없다면
        if (objs.Length == 1) { DontDestroyOnLoad(objs[0]); } // 해당 오브젝트를 파괴하지 않게 함

        // 만약 있다면 1개만 빼고 다른 오브젝트들은 파괴함
        else
        {
            for (int index = 1; index <= objs.Length; index++) { Destroy(objs[index]); }
        }
    }

    // SoundManager 오브젝트 파괴 X
    private void SoundManagerDontDestroy()
    {
        // 게임 오브젝트들 중에서 SoundManager 태그를 지닌 오브젝트를 찾음
        GameObject[] objs = GameObject.FindGameObjectsWithTag("SoundManager");

        // 만약 오브젝트의 중복이 없다면
        if (objs.Length == 1) { DontDestroyOnLoad(objs[0]); } // 해당 오브젝트를 파괴하지 않게 함

        // 만약 있다면 1개만 빼고 다른 오브젝트들은 파괴함
        else
        {
            for (int index = 1; index <= objs.Length; index++) { Destroy(objs[index]); }
        }
    }
}
