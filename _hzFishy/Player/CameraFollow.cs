using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _speed = 10;
    [SerializeField] private Vector3 _offset;

    private void FixedUpdate()
    {
        Vector3 goalPosition = _target.position + _offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, goalPosition, _speed * Time.deltaTime);
        transform.position = smoothedPosition;

        transform.LookAt(_target);
    }

}
