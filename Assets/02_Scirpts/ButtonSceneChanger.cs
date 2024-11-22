using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Management;
using System.Collections;
using UnityEngine.UI;

public class ButtonSceneChanger : MonoBehaviour
{
    public Slider loadingSlider; // UI Progress Bar (Optional)
    public ARSession arSession; // ARSession 참조 (AR 씬에서 할당 필요)

    public void ChangeScene(string vrSceneName)
    {
        StartCoroutine(PrepareAndLoadScene(vrSceneName));
    }

    private IEnumerator PrepareAndLoadScene(string vrSceneName)
    {
        // 1. AR 세션 종료 (MR에서 VR로 전환 전 처리)
        if (arSession != null)
        {
            Debug.Log("Stopping AR Session...");
            arSession.Reset(); // AR 세션 초기화
            arSession.enabled = false; // 비활성화
        }

        // 2. 메모리 정리
        CleanUpBeforeSceneChange();
        yield return null; // 한 프레임 대기

        // 3. 씬 비동기 로드 시작
        yield return StartCoroutine(LoadSceneAsync(vrSceneName));
    }

    private IEnumerator LoadSceneAsync(string vrSceneName)
    {
        // 씬 유효성 검사 (옵션)
        if (SceneUtility.GetBuildIndexByScenePath(vrSceneName) == -1)
        {
            Debug.LogError("Scene not found: " + vrSceneName);
            yield break;
        }

        // 비동기 씬 로드
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(vrSceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            // 로딩 진행도 계산
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");

            // Progress Bar 업데이트 (선택 사항)
            if (loadingSlider != null)
            {
                loadingSlider.value = progress;
            }

            // 로딩 완료되면 씬 활성화
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    private void CleanUpBeforeSceneChange()
    {
        // AR 세션 종료 및 메모리 정리
        Debug.Log("Cleaning up resources before scene change...");
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}
