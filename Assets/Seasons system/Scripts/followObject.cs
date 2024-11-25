using UnityEngine;

public class followObject : MonoBehaviour
{
    public GameObject objectToFollow;
    private void Awake()
    {
        objectToFollow = GameObject.Find("Camera Offset");
    }

    void Update()
    {
        transform.position = objectToFollow.transform.position;
    }
}
