using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum SpeedTextMovement
{
    playerY,
    playerX
}

public class Elements_SpeedText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    private GameObject _player;
    [SerializeField] private SpeedTextMovement _movement = SpeedTextMovement.playerY;
    public float AddedOffset = 0;
    [SerializeField] private AnimationClip[] _clips;
    public Animation Animation;

    private GameObject _camera;

    private float _goalSpeed = 2;
    private float _currentSpeed = 0;
    private float _duration = 0.8f;

    private void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("Camera");
    }

    public void UpdateText(float content)
    {
        if (!_text.enabled)
        {
            _text.enabled = true;
            _player = GameObject.FindGameObjectWithTag("Player");
        }
        _duration += 05f;
        _goalSpeed = content;
        StartCoroutine(SmoothUpdateText());
    }

    private IEnumerator SmoothUpdateText()
    {
    

    float time = 0;
        while (_goalSpeed != _currentSpeed)
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, _goalSpeed, time / _duration);
            _text.text = _currentSpeed.ToString() .Length > 4 ? _currentSpeed.ToString()[..4] : _currentSpeed.ToString();
            time += Time.deltaTime;
            yield return null;
        }
    }


    private void Update()
    {
        if (_player != null){
            transform.position = new Vector3(
                                            (_movement == SpeedTextMovement.playerX ? _player.transform.position.x + AddedOffset : transform.position.x),
                                            (_movement == SpeedTextMovement.playerY ? _player.transform.position.y + AddedOffset : transform.position.y),
                                            transform.position.z
                                 );
        }
    }

    private void LateUpdate()
    {
        if (_camera != null && _text.enabled)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _camera.transform.rotation, 5f * Time.deltaTime);
        }
    }

    public void ChangeAndPlayAnimation(bool newState)
    {
        Animation.clip = _clips[newState ? 0 : 1];
        Animation.Play();
    }
}
