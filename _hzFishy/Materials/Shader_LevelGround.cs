using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shader_LevelGround : MonoBehaviour
{
    private Material _material;
    private GameObject _player;
    private PlayerController _playerC;

    private Vector2 _playerPosition;
    private float _GradientTime;
    private float _playerGroundDistance;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerC = _player.GetComponent<PlayerController>();
        _material = GetComponent<MeshRenderer>().sharedMaterial;
    }

    void Update()
    {
        _playerPosition = new Vector2(_player.transform.position.x - 0.5f, _player.transform.position.z - 0.5f);
        _material.SetVector("_Position", _playerPosition);

        _GradientTime = Mathf.PingPong(Time.time * 0.5f, 1);
        _material.SetFloat("_GradientTime", _GradientTime);

        //_playerGroundDistance = _playerC.groundDistance;
        //_material.SetFloat("_DistanceGround", _playerGroundDistance);

        //_material.SetFloat("_MoveSpeed", _playerC.isMovingForward || _playerC.isInMenu ? _playerC.forwardSpeed : 0);
    }
}
