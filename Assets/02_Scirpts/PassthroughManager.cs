using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PassThroughManager : MonoBehaviour
{
    private ARCameraBackground arCameraBackground;
    public Material customMaterial; // 커스텀 AR Material

    private void Start()
    {
        arCameraBackground = GetComponent<ARCameraBackground>();

        if (arCameraBackground != null && customMaterial != null)
        {
            // AR Camera Background에 커스텀 머티리얼 적용
            arCameraBackground.customMaterial = customMaterial;
            Debug.Log("AR Camera Background에 커스텀 머티리얼이 적용되었습니다.");
        }
        else
        {
            Debug.LogError("ARCameraBackground 또는 커스텀 Material이 없습니다!");
        }
    }

    public void AdjustTransparencyAndBrightness(float alpha, float brightness)
    {
        if (customMaterial != null)
        {
            // 알파 값 (투명도)
            customMaterial.SetFloat("_Alpha", alpha);

            // 밝기 값 조정
            customMaterial.SetFloat("_Brightness", brightness);

            Debug.Log($"AR 배경 투명도: {alpha}, 밝기: {brightness}로 조정 완료");
        }
        else
        {
            Debug.LogError("커스텀 Material이 설정되지 않았습니다!");
        }
    }
}
