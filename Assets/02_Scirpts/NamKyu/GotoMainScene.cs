using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;  // AR 관련 네임스페이스

public class GotoMainScene : MonoBehaviour
{
    [SerializeField] private OVRPassthroughLayer _passthroughLayer; // Passthrough Layer 참조
    [SerializeField] private GameObject ovrCameraRig; // 기존 OVRCameraRig (MR용)
    [SerializeField] private ARSession arSession; // ARSession 참조

    public string vrSceneName = "VRTest"; // VR 씬 이름 설정

    // 싱글턴 참조
    public static GotoMainScene Instance { get; private set; }

    // private void Awake()
    // {
    //     // 싱글턴 초기화
    //     if (Instance == null)
    //     {
    //         Instance = this;
    //         DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
    //     }
    //     else
    //     {
    //         Destroy(gameObject); // 중복 싱글턴 방지
    //     }
    // }

    public void GoToVRScene()
    {
        // 1. 패스스루 비활성화
        Debug.Log("OVRManager instance: " + OVRManager.instance);
        if (OVRManager.instance != null)
        {
            OVRManager.instance.isInsightPassthroughEnabled = false;
            Debug.Log("OVRManager instance found and passthrough disabled.");
        }
        else
        {
            Debug.LogWarning("OVRManager instance is not found.");
        }

        // 2. 기존의 OVRCameraRig 비활성화
        if (ovrCameraRig != null)
        {
            ovrCameraRig.SetActive(false); // MR 카메라 비활성화
            Debug.Log("OVRCameraRig found and deactivated.");
        }
        else
        {
            Debug.LogWarning("OVRCameraRig is not found.");
        }

        // 3. ARSession 비활성화
        if (arSession != null)
        {
            arSession.Reset(); // ARSession 리소스 초기화
            arSession.enabled = false;
            Debug.Log("ARSession found and reset.");
        }
        else
        {
            Debug.LogWarning("ARSession is not found.");
        }

        // 4. 씬 전환
        Debug.Log("Transitioning to VR scene...");
        SceneManager.LoadScene(vrSceneName);

        // 5. 씬 로드 후 VR 초기화 작업
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == vrSceneName)
        {
            InitializeVR();
        }

        // 이벤트 구독 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void InitializeVR()
    {
        var ovrCameraRig = FindObjectOfType<OVRCameraRig>();
        if (ovrCameraRig != null)
        {
            Destroy(ovrCameraRig.gameObject); // 중복된 OVRCameraRig 제거
            Debug.Log("Duplicate OVRCameraRig found and destroyed.");
        }
        else
        {
            Debug.Log("No duplicate OVRCameraRig found.");
        }

        Debug.Log("XR Origin is automatically activated when the VR scene is loaded.");
    }
}