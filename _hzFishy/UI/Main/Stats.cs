using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    public void Fade(bool state)
    {
        _animator.SetBool("Fade", state);
    }

    public void FadeEnd()
    {
        _animator.SetBool("Fade", false);
    }
}