using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.LoadSceneAsync("LayerDesign", LoadSceneMode.Additive);
    }

}
