using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoHandler : MonoBehaviour
{
    [SerializeField] private GameObject _prefabUIElement;
    private GameObject _currentUIElement;
    [HideInInspector] public PlayerController _playerC;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (_playerC == null)
            {
                _playerC = collider.transform.parent.gameObject.GetComponent<PlayerController>();
            }
            StartHandle();
        }
    }

    private void StartHandle()
    {
        _currentUIElement = Instantiate(_prefabUIElement);
        _playerC.InTutoMode = true;
        _playerC.CurrentTutoHandler = gameObject;
        _playerC.CurrentUITutoHandler = _currentUIElement;
        StartCoroutine(PlayerController.ChangeTimeScale(0,0.5f));
    }
}