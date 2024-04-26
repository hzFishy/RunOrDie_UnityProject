using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element_Zipline_Handle : MonoBehaviour
{
    private GameObject _player;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    void LateUpdate()
    {
        Debug.Log(_player.transform.position.x);
        transform.position = new Vector3(_player.transform.position.x, transform.position.y, transform.position.z);
    }
}
