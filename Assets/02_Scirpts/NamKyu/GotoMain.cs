using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoMain : MonoBehaviour
{
    public void ChangeToMainScene()
    {
        SceneManager.LoadScene("Main");
    }
}
