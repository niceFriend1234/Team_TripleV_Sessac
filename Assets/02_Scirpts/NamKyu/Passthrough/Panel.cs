using System;
using System.Collections;
using Meta.XR.MRUtilityKit;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    [SerializeField] private Slider _passthroughBrightnessSlider;
    [SerializeField] private OVRPassthroughLayer _passthroughLayer;

    private void Awake()
    {
        // 패스스루 레이어가 비활성화된 경우 활성화
        if (_passthroughLayer != null && !_passthroughLayer.enabled)
        {
            _passthroughLayer.enabled = true;
        }

        // 기본 설정을 재적용 (필요에 따라 추가 설정 가능)
        _passthroughLayer.SetBrightnessContrastSaturation(1.0f); // 초기 밝기, 대비, 채도 설정
    }

    private void Start()
    {
        // 슬라이더의 값이 변경될 때, 패스스루 밝기를 업데이트
        _passthroughBrightnessSlider.onValueChanged.AddListener(
            (brightness) =>
            {
                _passthroughLayer.SetBrightnessContrastSaturation(brightness);
            }
        );
    }

    
}
