using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResetBestScoreScript : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI bestScoreText;

    public void ResetBestScoreButton()
    {
        PlayerPrefs.DeleteAll();
        bestScoreText.text = "";
    }
}
