using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element_block_scaleAnimation : MonoBehaviour
{
    private float _targetScaleX;
    public float timeToLerp = 1.5f;
    void Start()
    {
        _targetScaleX = transform.localScale.x;
        StartCoroutine(LerpFunction(_targetScaleX, timeToLerp));
    }
    IEnumerator LerpFunction(float endValue, float duration)
    {
        float time = 0;
        float startValue = 0.1f;
        while (time < duration)
        {
            transform.localScale = new Vector3(Mathf.Lerp(startValue, endValue, time / duration), transform.localScale.y, transform.localScale.z);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = new Vector3(endValue, transform.localScale.y, transform.localScale.z);
    }
}
