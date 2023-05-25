using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    public Slider brightnessSlider;

    private void Start()
    {
        // �����̴� ���� ����� ������ ��� ���� �Լ��� ȣ���մϴ�.
        brightnessSlider.onValueChanged.AddListener(AdjustBrightness);
    }

    private void AdjustBrightness(float brightnessValue)
    {
        // ��� ���� ����Ͽ� ����ȭ���� ��⸦ �����ϴ� ������ �����մϴ�.
        // ���÷δ� �������� ��� ��ü�� ��⸦ �����ϴ� ���� �����մϴ�.
        float brightness = brightnessValue * 2f; // �����̴� ���� 0~2 ������ ��ȯ
        RenderSettings.ambientLight = new Color(brightness, brightness, brightness);
    }
}
