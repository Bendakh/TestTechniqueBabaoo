using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

using SceneTransitionSystem;
using TeasingGame;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TeasingGameScene SceneForButtonHome;
    public TeasingGameScene SceneForButtonGame;

    public static UIManager _instance;

    [SerializeField]
    TextMeshProUGUI timerText;

    [SerializeField]
    GameObject endGamePanel;

    [SerializeField]
    TextMeshProUGUI endGameText;

    [SerializeField]
    GameObject saveNewBestScorePanel;

    [SerializeField]
    TMP_InputField nameInputField;

    [SerializeField]
    TextMeshProUGUI newScoreText;

    [SerializeField]
    Button startGameButton;

    private void Awake()
    {
        if (_instance == null)
        {
            //DontDestroyOnLoad(gameObject);
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {       
        timerText.text = ConvertSecondsToTimeFormat(GameManager._instance.TimerCountdown);
    }

    public void DisplayEndGamePanel(bool isVictorious)
    {
        endGamePanel.SetActive(true);
        if(isVictorious)
            endGameText.text = "Congratulations! Puzzle completed in " + ((int)(GameManager._instance.Timer - GameManager._instance.TimerCountdown)).ToString() + " seconds!";
        if (!isVictorious)
            endGameText.text = "Too bad! You lost :(";
    }

    public void DisplaySaveScorePanel()
    {
        saveNewBestScorePanel.SetActive(true);
    }

    public void SaveScoreButton()
    {
        if(nameInputField.text.Trim() != "")
        {
            PlayerPrefs.SetInt(GameManager.BEST_SCORE, GameManager._instance.BestScoreInSeconds);
            PlayerPrefs.SetString(GameManager.BEST_SCORE_PLAYER, nameInputField.text.Trim());
            saveNewBestScorePanel.SetActive(false);
        }
        else
        {
            newScoreText.text = "Please write your name";
        }
    }

    public void ReplayButton()
    {       
        STSSceneManager.LoadScene(SceneForButtonGame.ToString());
        GameManager._instance.ConfigGame();
        saveNewBestScorePanel.SetActive(false);
        endGamePanel.SetActive(false);
    }

    public void HomeButton()
    {
        STSSceneManager.LoadScene(SceneForButtonHome.ToString());
    }

    public void StartGameButton()
    {
        GameManager._instance.LaunchTimer();
        startGameButton.gameObject.SetActive(false);
    }

    public void DisplayStartGameButton()
    {
        startGameButton.gameObject.SetActive(true);
    }

    private string ConvertSecondsToTimeFormat(float seconds)
    {
        return TimeSpan.FromSeconds(seconds).Minutes.ToString() + ":" + TimeSpan.FromSeconds(seconds).Seconds.ToString();
    }
}
