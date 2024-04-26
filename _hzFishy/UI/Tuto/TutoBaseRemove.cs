using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoBaseRemove : MonoBehaviour
{
    private Animator _animator;
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    public IEnumerator StartDestroy()
    {
        yield return new WaitForSeconds(2.0f);
        _animator.SetBool("Remove", true);
    }

    void RemoveSelf()
    {
        Destroy(gameObject.transform.parent.transform.parent.gameObject);
    }
}
