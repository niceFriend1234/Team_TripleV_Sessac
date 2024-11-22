using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{
    // 씬 전환 메서드
    public void ChangeToMainScene()
    {
        // Main Scene으로 이동
        SceneManager.LoadScene("Main"); // "Main"은 전환하려는 씬 이름
    }
}
