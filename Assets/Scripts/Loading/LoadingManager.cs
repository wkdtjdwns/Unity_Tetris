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

        // � �ؽ�Ʈ�� ������ �� ��
        int ran_index = Random.Range(0, loading_strs.Length);
        loading_str = loading_strs[ran_index];

        UpdateLoadingText();
    }

    private void Update()
    {
        // �����̴��� �� ���� ���� ����
        if (is_full) { SceneManager.LoadScene("TetrisScene"); }
    }

    private void UpdateLoadingText()
    {
        // i�� ���� 3���� ũ�ų� ������ 1��, �ƴϸ� i + 1 ����
        i = (i > 3) ? 0: (i + 1);

        // i�� ���� ���� "."�� ������ �ٸ��� ��
        switch (i)
        {
            case 0: loading_text.text = loading_str; break;
            case 1: loading_text.text = loading_str + "."; break;
            case 2: loading_text.text = loading_str + ".."; break;
            case 3: loading_text.text = loading_str + "..."; break;
        }

        // �ε��� ������ ������ 0.25�ʸ��� ���� �ڵ带 �ݺ���
        if (!is_full) { Invoke("UpdateLoadingText", 0.35f); }
    }
}
