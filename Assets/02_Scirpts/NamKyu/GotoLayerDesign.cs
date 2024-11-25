using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private OVRPassthroughLayer _passthroughLayer; // Passthrough Layer 참조

    public string vrSceneName = "LayerDesign"; // VR 씬 이름 설정

    // 싱글턴 참조
    public static SceneTransition Instance { get; private set; }

    private void Awake()
    {
        // 싱글턴 초기화
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 중복 싱글턴 방지
        }
    }

    public void GoToVRScene()
    {
        // 1. 패스스루 비활성화
        if (OVRManager.instance != null)
        {
            OVRManager.instance.isInsightPassthroughEnabled = false;
        }
        else
        {
            Debug.LogWarning("OVRManager instance is not found.");
        }

        // 2. 씬 전환
        SceneManager.LoadScene(vrSceneName);

        // 3. 씬 로드 후 VR 초기화 작업
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
        OVRCameraRig cameraRig = GetComponentInChildren<OVRCameraRig>(); // 자식 객체에서 OVRCameraRig 검색
        if (cameraRig != null)
        {
            cameraRig.transform.position = new Vector3(0, 1.6f, 0); // VR 카메라 위치 초기화
            cameraRig.transform.rotation = Quaternion.identity;
        }
        else
        {
            Debug.LogWarning("OVRCameraRig is not found under OVRCameraRigInteraction.");
        }
    }
}
