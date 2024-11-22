using UnityEngine;

public class SphereInternalRendering : MonoBehaviour
{
    public Material sphereMaterial;  // 스피어의 재질

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // 플레이어가 스피어 내부로 들어가면
        {
            // 스피어 내부를 불투명하게 설정
            sphereMaterial.SetFloat("_Opacity", 1f);  // 내부를 완전 불투명하게 설정
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))  // 플레이어가 스피어 밖으로 나가면
        {
            // 스피어 외부를 반투명하게 설정
            sphereMaterial.SetFloat("_Opacity", 0.5f);  // 외부는 반투명
        }
    }
}
