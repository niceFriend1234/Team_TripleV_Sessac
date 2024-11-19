using UnityEngine;
public class SeasonChange : MonoBehaviour
{
    [SerializeField] private SeasonsSystemURP.seasons season;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Doscent"))
        {
            SeasonsSystemURP.onSeasonChange?.Invoke(this.season);
            Destroy(this.gameObject);
        }
    }
}
