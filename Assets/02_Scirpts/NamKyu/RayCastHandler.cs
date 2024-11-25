using UnityEngine;

public class RaycastHandler : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public float rayLength = 5f;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        lineRenderer.SetPosition(0, ray.origin);

        if (Physics.Raycast(ray, out hit, rayLength))
        {
            lineRenderer.SetPosition(1, hit.point);
            // 히트된 오브젝트와의 상호작용을 여기서 처리
        }
        else
        {
            lineRenderer.SetPosition(1, ray.origin + ray.direction * rayLength);
        }
    }
}
