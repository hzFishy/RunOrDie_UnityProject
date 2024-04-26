using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element_DownSlide : MonoBehaviour
{
    [SerializeField] private float _minRangeZScale;
    [SerializeField] private float _maxRangeZScale;
    [SerializeField] private GameObject[] _directions;
    void Start()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, Random.Range(_minRangeZScale, _maxRangeZScale));

        foreach (var direction in _directions)
        {
            direction.transform.position = new Vector3(direction.transform.position.x-(direction.transform.localScale.z/ 2) - 2, direction.transform.position.y, direction.transform.position.z);
        }
    }
}