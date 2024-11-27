using UnityEngine;
using System.Collections;

public class TrailManager : MonoBehaviour
{
    [SerializeField] private GameObject trail1;
    [SerializeField] private GameObject trail2;
    [SerializeField] private GameObject trail3;
    [SerializeField] private GameObject trail4;
    [SerializeField] private GameObject trail5;
    [SerializeField] private float moveSpeed = 10f;

    private Vector3[] originalPositions;
    private GameObject[] trails;

    void Start()
    {
        originalPositions = new Vector3[5]
        {
            trail1.transform.position,
            trail2.transform.position,
            trail3.transform.position,
            trail4.transform.position,
            trail5.transform.position
        };

        trails = new GameObject[5] { trail1, trail2, trail3, trail4, trail5 };

        for (int i = 0; i < trails.Length; i++)
        {
            StartCoroutine(MoveTrail(trails[i], originalPositions[i]));
        }
    }

    IEnumerator MoveTrail(GameObject trail, Vector3 originalPosition)
    {
        while (true)
        {
            Vector3 targetPosition = new Vector3(originalPosition.x, originalPosition.y, 0);
            while (Vector3.Distance(trail.transform.position, targetPosition) > 0.01f)
            {
                trail.transform.position = Vector3.MoveTowards(trail.transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            Destroy(trail);

            trail = Instantiate(trail, originalPosition, Quaternion.identity);

            yield return new WaitForSeconds(0.1f);
        }
    }
}