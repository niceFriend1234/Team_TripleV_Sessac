using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadSceneAsync("LayerDesign", LoadSceneMode.Additive);
    }

}
