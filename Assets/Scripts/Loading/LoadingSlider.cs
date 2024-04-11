using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSlider : MonoBehaviour
{
    private Slider loading_slider;

    private GameObject loading_manager_obj;
    private LoadingManager loading_manager;

    private void Awake()
    {
        loading_slider = this.GetComponent<Slider>(); ;

        loading_slider.value = 0f;

        // �÷��̾� ü�� �����̴��� ���� �ǵ鿩�� ���� �ٲ� �� ���� ������
        loading_slider.interactable = false;

        loading_manager_obj = GameObject.Find("LoadingManager").gameObject;
        loading_manager = loading_manager_obj.GetComponent<LoadingManager>();
    }

    private void Update()
    {
        SliderValueUpdate();
    }

    private void SliderValueUpdate()
    {
        // ������ ���� �޾Ƽ� ��� �����̴� ���� ������Ŵ
        float ran_value = Random.Range(0f, 0.00075f);

        loading_slider.value += ran_value;

        if (loading_slider.value >= 1f) { loading_manager.is_full = true; }
    }
}
