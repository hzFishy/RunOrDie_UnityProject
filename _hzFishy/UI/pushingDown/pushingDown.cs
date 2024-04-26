using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pushingDown : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CoroutineDestroySelf());
    }

    IEnumerator CoroutineDestroySelf()
    {
        yield return new WaitForSeconds(1);
    }
}
