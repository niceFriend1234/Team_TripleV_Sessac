using UnityEngine;
using UnityEngine.UI;

public class BtnScript : MonoBehaviour
{
   [SerializeField] private Button springBtn;
   [SerializeField] private Button summerBtn;
   [SerializeField] private Button autumnBtn;
   [SerializeField] private Button winterBtn;
   private void OnEnable() {
    springBtn.onClick.AddListener(() => {
        SeasonsSystemURP.onSeasonChange?.Invoke(SeasonsSystemURP.seasons.spring);
    });
    summerBtn.onClick.AddListener(()=>{
        SeasonsSystemURP.onSeasonChange?.Invoke(SeasonsSystemURP.seasons.summer);
    });
    autumnBtn.onClick.AddListener(()=>{
        SeasonsSystemURP.onSeasonChange?.Invoke(SeasonsSystemURP.seasons.autumn);
    });
    winterBtn.onClick.AddListener(()=>{
        SeasonsSystemURP.onSeasonChange?.Invoke(SeasonsSystemURP.seasons.winter);
    });
   }
}
