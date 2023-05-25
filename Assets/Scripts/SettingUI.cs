using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    public Slider brightnessSlider;

    private void Start()
    {
        // 슬라이더 값이 변경될 때마다 밝기 조절 함수를 호출합니다.
        brightnessSlider.onValueChanged.AddListener(AdjustBrightness);
    }

    private void AdjustBrightness(float brightnessValue)
    {
        // 밝기 값을 사용하여 게임화면의 밝기를 조절하는 로직을 구현합니다.
        // 예시로는 렌더링된 모든 객체의 밝기를 변경하는 것을 가정합니다.
        float brightness = brightnessValue * 2f; // 슬라이더 값을 0~2 범위로 변환
        RenderSettings.ambientLight = new Color(brightness, brightness, brightness);
    }
}
