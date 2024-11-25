using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class PassthroughManager : MonoBehaviour
{
    private static PassthroughManager instance;
    [SerializeField] private OVRPassthroughLayer passthroughLayer;

    public static PassthroughManager Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // passthroughLayer 초기화 확인
        if (passthroughLayer == null)
        {
            passthroughLayer = GetComponent<OVRPassthroughLayer>();
        }
        }
    }


}