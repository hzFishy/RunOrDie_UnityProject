using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    //private float _distanceTraveled = 0f;
    private Vector3 _startPosition;
    /*public float distanceTraveled { 
        get
        {
            return _distanceTraveled;
        }
        
    }*/
    //[SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _distanceTraveledText;
    [SerializeField] private TextMeshProUGUI _HighScoreText;
    [SerializeField] private PlayerController _playerC;

    public float ScoreValue;

    private void Start()
    {
        _startPosition = transform.position;
        PlayerController.PlayerStopPlaying += PlayerFinished;

        float tScore = GetHightScore();
        if (tScore > 0)
        {
            _HighScoreText.text = "Best distance \n" + tScore.ToString();
        }
        else
        {
           Destroy(_HighScoreText);
        }
    }

    private void OnDisable()
    {
        NewHighScoreChecker();
        PlayerController.PlayerStopPlaying -= PlayerFinished;
    }

    void Update()
    {
        //_scoreText.text = ScoreValue.ToString();
        ScoreValue = (transform.position.x - _startPosition.x) + (transform.position.y - _startPosition.y);
        _distanceTraveledText.text = Mathf.Floor(ScoreValue) + " m";
    }

    private void OnApplicationPause(bool pause) { NewHighScoreChecker(); }

    private void OnApplicationQuit() { NewHighScoreChecker(); }

    private void PlayerFinished() { NewHighScoreChecker(); }

    public bool NewHighScoreChecker()
    {
        if (!_playerC.InTutoLevel)
        {
            if (ScoreValue > GetHightScore())
            {
                SaveHightScore(ScoreValue);
                return true;
            }
            return false;
        }
        return false;
    }

    public void SaveHightScore(float Value)
    {
        _HighScoreText.text = Value.ToString();
        PlayerPrefs.SetFloat("HighScore", Mathf.Floor(ScoreValue));
    }

    public float GetHightScore()
    {
        return PlayerPrefs.HasKey("HighScore") ? PlayerPrefs.GetFloat("HighScore") : 0;
    }
}