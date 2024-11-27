using UnityEngine;

public class SeasonChangeDemo : MonoBehaviour
{
    [SerializeField] private SeasonsSystemURP.seasons season;
    [SerializeField] private GameObject rock;
    [SerializeField] private GameObject tree;
    [SerializeField] private GameObject terrains;
    [SerializeField] private GameObject cloud;
    [SerializeField] private GameObject walls;
    [SerializeField] private GameObject arrow;

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Coll");
        Debug.Log($"Colliding with object tagged: {other.gameObject.tag}");
        if (other.gameObject.CompareTag("Doscent"))
        {
            Debug.Log("Dosc");
            SeasonsSystemURP.onSeasonChange?.Invoke(this.season);
            ActivateObjects();
            Destroy(this.gameObject);
        }
    }

    private void ActivateObjects()
    {
        if (rock != null)
        {
            rock.SetActive(true);
            Debug.Log("Rock activated.");
        }
        else
        {
            Debug.LogWarning("Rock object is not assigned.");
        }

        if (tree != null)
        {
            tree.SetActive(true);
            Debug.Log("Tree activated.");
        }
        else
        {
            Debug.LogWarning("Tree object is not assigned.");
        }

        if (terrains != null)
        {
            terrains.SetActive(true);
            Debug.Log("Terrains activated.");
        }
        else
        {
            Debug.LogWarning("Terrains object is not assigned.");
        }

        if (cloud != null)
        {
            terrains.SetActive(true);
            Debug.Log("Terrains activated.");
        }
        else
        {
            Debug.LogWarning("Terrains object is not assigned.");
        }

        if (walls != null)
        {
            walls.SetActive(false); // walls 오브젝트 비활성화
            Debug.Log("Walls deactivated.");
        }
        else
        {
            Debug.LogWarning("Walls object is not assigned.");
        }

        if (arrow != null)
        {
            arrow.SetActive(false);
            Debug.Log("arrow deactivated.");
        }
        else
        {
            Debug.LogWarning("Walls object is not assigned.");
        }
    }
}