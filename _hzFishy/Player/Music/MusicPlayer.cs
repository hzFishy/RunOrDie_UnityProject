using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MusicPlayer : MonoBehaviour
{
    public List<AudioClip> MusicList;
    public List<string> MusicListNames;
    public AudioSource PlaySource;

    public GameObject CreditsPrefab;
    private GameObject _currentUI;
    private MusicCredits _currentUIMusicCredits;

    private PlayerInputs playerInputActions;

    private static bool _isMuted;

    public enum FadeVolumeGoal
    {
        Normal,
        Low
    }

    [Button("Clear list and get files")]
    private void GetFiles()
    {
        MusicList.Clear();
        MusicListNames.Clear();
        MusicList = (Resources.LoadAll("Musics", typeof(AudioClip))).ToList().ConvertAll(x => (AudioClip)x);
        foreach (AudioClip clip in MusicList)
        {
            MusicListNames.Add(clip.name);
        }
    }

    private void Start()
    {
        Debug.Log(" respawn :" + _isMuted);
        StartCoroutine(HandlePlay());
    }

    private IEnumerator HandlePlay()
    {
        _currentUI = Instantiate(CreditsPrefab);
        _currentUIMusicCredits = _currentUI.GetComponent<MusicCredits>();
        SetVolume(0.3f);
        //StartCoroutine(FadeVolume(FadeVolumeGoal.Normal));
        while (true)
        {
            yield return StartCoroutine(Play());
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator Play()
    {
        AudioClip selected = MusicList[Random.Range(0, MusicListNames.Count)];
        PlaySource.PlayOneShot(selected);
        _currentUIMusicCredits.Text.text = selected.name;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_currentUIMusicCredits.Text.gameObject.transform.parent.GetComponent<RectTransform>());
        if (_isMuted)
        {
            PlaySource.volume = 0;
        }
        yield return new WaitUntil(() => !PlaySource.isPlaying);
    }

    public IEnumerator FadeVolume(FadeVolumeGoal goal = FadeVolumeGoal.Normal, float duration = 1)
    {
        if (!_isMuted)
        {
            float start = PlaySource.volume;
            float goalValue = 1;
            switch (goal)
            {
                case FadeVolumeGoal.Normal:
                    goalValue = 0.3f;
                    break;
                case FadeVolumeGoal.Low:
                    goalValue = 0.1f;
                    break;
                default:
                    break;
            }
            float time = 0;
            while (time < duration)
            {
                PlaySource.volume = Mathf.Lerp(start, goalValue, time / duration);
                time += Time.unscaledDeltaTime;
                yield return null;
            }
            PlaySource.volume = goalValue;
        }
    }

    public void SetVolume(float v)
    {
        if (_isMuted)
        {
            return;
        }
        PlaySource.volume = v;
    }

    private void ToggleMuteSong(InputAction.CallbackContext context)
    {
        _isMuted = !_isMuted;
        Debug.Log(" new :" + _isMuted);
        PlaySource.volume = _isMuted ? 0 : 0.3f;
    }
    private void SwitchSong(InputAction.CallbackContext context)
    {
        PlaySource.Stop();
    }

    private void Awake()
    {
        playerInputActions = new PlayerInputs();
        playerInputActions.Player.Debug_ToggleSong.performed += ToggleMuteSong;
        playerInputActions.Player.Debug_ToggleSong.Enable();
        playerInputActions.Player.Debug_SwitchSong.performed += SwitchSong;
        playerInputActions.Player.Debug_SwitchSong.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Player.Debug_ToggleSong.Disable();
        playerInputActions.Player.Debug_SwitchSong.Disable();
    }
}
