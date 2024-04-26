using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using static UnityEngine.EventSystems.StandaloneInputModule;
using RotaryHeart.Lib.SerializableDictionary;

public class MenuPause : MonoBehaviour
{
    [HideInInspector] public PlayerDeathTips PlayerDeathTipsData;
    public TextMeshProUGUI PlayerDeathTipsText;
    public GameObject PlayerDeathTipsPanel;
    public GameObject ResumeButton;
    public GameObject HighScoreGO;
    public TextMeshProUGUI HighScoreText;
    private PlayerController _playerC;

    private void Awake()
    {
        Cursor.visible = false;
        PlayerDeathTipsData = GetComponent<PlayerDeathTips>();
        _playerC = FindFirstObjectByType<PlayerController>();
        if (!_playerC.isMovingForward)
        {
            ResumeButton.SetActive(false);
            GetComponent<NavigationSelectables>().RemoveSelectable(0);
            HighScoreGO.SetActive(false);

            PlayerDeathTipsText.text = PlayerDeathTipsData.DeathTips[_playerC.KilledByElemType];
        }
        else
        {
            PlayerDeathTipsPanel.SetActive(false);

            PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();
            if (playerStats.NewHighScoreChecker())
            {
                HighScoreGO.SetActive(true);
                HighScoreText.text = "! NEW best distance ! \n" + Mathf.Floor(playerStats.ScoreValue).ToString()+"m";
            }
            else
            {
                HighScoreGO.SetActive(false);
            }
        }
    }

    public void Resume()
    {
        PlayerController player = (PlayerController)FindFirstObjectByType(typeof(PlayerController));
        player.musicPlayer.SetVolume(0.3f);
        //StartCoroutine(player.musicPlayer.FadeVolume(MusicPlayer.FadeVolumeGoal.Normal, 1));
        Destroy(player.CurrentUIPause);
        Time.timeScale = 1;
        player.InPause = false;
    }
    public void Menu()
    {
        CrossSceneDataHolder.InstantStart = false;
        SceneManager.LoadScene("Game");
        Time.timeScale = 1;
    }

    public void Restart()
    {
        Debug.Log("Set CrossSceneDataHolder.InstantStart : true");
        CrossSceneDataHolder.InstantStart = true;
        CrossSceneDataHolder.StartInTuto = _playerC.InTutoLevel;
        Debug.Log("Set CrossSceneDataHolder.StartInTuto :" + CrossSceneDataHolder.StartInTuto);
        SceneManager.LoadScene("Game");
        Time.timeScale = 1;
    }
}
