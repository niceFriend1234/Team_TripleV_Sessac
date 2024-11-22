using UnityEngine;

public class followObject : MonoBehaviour
{
    public GameObject objectToFollow;
    void Start()
    {
        objectToFollow = GameObject.Find("Camera Offset");
    }

    void Update()
    {
        transform.position = objectToFollow.transform.position;
    }
}
