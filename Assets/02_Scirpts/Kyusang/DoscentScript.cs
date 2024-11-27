using UnityEngine;

public class DoscentScript : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    private void LateUpdate()
    {
        if (!audioSource.enabled)
        {
            audioSource.enabled = true;
        }
        // Get the camera's position
        Vector3 targetPosition = Camera.main.transform.position;

        // Lock the Y-axis by setting the object's Y position to its current Y position
        targetPosition.y = this.transform.position.y;

        // Look at the camera position on the Y-axis only
        this.transform.LookAt(targetPosition);
    }
}
