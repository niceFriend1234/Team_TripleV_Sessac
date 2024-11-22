using UnityEngine;
public class SeasonChange : MonoBehaviour
{
    [SerializeField] private SeasonsSystemURP.seasons season;
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Coll");
        Debug.Log($"Colliding with object tagged: {other.gameObject.tag}");
        if (other.gameObject.CompareTag("Doscent"))
        {
            Debug.Log("Dosc");
            SeasonsSystemURP.onSeasonChange?.Invoke(this.season);
            Destroy(this.gameObject);
        }
    }
}
