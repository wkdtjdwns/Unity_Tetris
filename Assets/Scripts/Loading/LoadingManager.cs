using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    [SerializeField]
    private string[] loading_strs;
    [SerializeField]
    private Text loading_text;

    private string loading_str;

    private int i;

    public bool is_full;

    private void Awake()
    {
        is_full = false;

        // 어떤 텍스트를 적용할 지 고름
        int ran_index = Random.Range(0, loading_strs.Length);
        loading_str = loading_strs[ran_index];

        UpdateLoadingText();
    }

    private void Update()
    {
        // 슬라이더가 꽉 차면 게임 실행
        if (is_full) { SceneManager.LoadScene("TetrisScene"); }
    }

    private void UpdateLoadingText()
    {
        // i의 값이 3보다 크거나 같으면 1로, 아니면 i + 1 해줌
        i = (i > 3) ? 0: (i + 1);

        // i의 값에 따라서 "."의 개수를 다르게 함
        switch (i)
        {
            case 0: loading_text.text = loading_str; break;
            case 1: loading_text.text = loading_str + "."; break;
            case 2: loading_text.text = loading_str + ".."; break;
            case 3: loading_text.text = loading_str + "..."; break;
        }

        // 로딩이 끝나기 전까지 0.25초마다 위의 코드를 반복함
        if (!is_full) { Invoke("UpdateLoadingText", 0.35f); }
    }
}
