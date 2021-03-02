using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HomeMenuBestScoreScript : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI bestScoreText;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey(GameManager.BEST_SCORE))
        {
            bestScoreText.text = "Best score : " + PlayerPrefs.GetString(GameManager.BEST_SCORE_PLAYER) + " --- " + PlayerPrefs.GetInt(GameManager.BEST_SCORE);
        }
    }

    
}
