using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{

    [SerializeField] private GameObject _player;

    // Update is called once per frame
    void Update()
    {
        if (_player != null)
        {
            transform.position = new Vector3(_player.transform.position.x, transform.position.y, transform.position.z);
        }
    }
}
