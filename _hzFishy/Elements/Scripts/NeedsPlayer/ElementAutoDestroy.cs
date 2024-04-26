using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementAutoDestroy : MonoBehaviour
{
    [SerializeField] [Range(0, 20)] private float _timeBeforeDestroy = 5;
    [SerializeField] private GameObject _gameObjectToDestroy;
    //[SerializeField] private PlayerStats _playerStats;
    //[SerializeField] private bool _addScore = true;

    private void Awake()
    {
        //_playerStats = FindAnyObjectByType<PlayerStats>();
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            StopCoroutine(CoroutineSelfDestroy());
            StartCoroutine(CoroutineSelfDestroy());
            /*if (_addScore)
            {
                _playerStats.ScoreValue += Random.Range(10, 20);
            }*/
        }
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            StopCoroutine(CoroutineSelfDestroy());
        }
    }
    private IEnumerator CoroutineSelfDestroy()
    {
        yield return new WaitForSeconds(_timeBeforeDestroy);
        PlayerController playerC = FindFirstObjectByType<PlayerController>();
        if (playerC.isMovingForward && _gameObjectToDestroy != null && !playerC.InTutoLevel)
        {
            Destroy(_gameObjectToDestroy);
        }
    }
}
