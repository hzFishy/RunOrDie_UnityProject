using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheckAttach : MonoBehaviour
{
    [SerializeField] private CapsuleCollider _capsuleCollider;
    [SerializeField] private GameObject _YBot;

    private void LateUpdate()
    {
        if (_capsuleCollider != null)
        {
            transform.position = _YBot.transform.position + _capsuleCollider.center - new Vector3(0,_capsuleCollider.height/2,0);
        }
    }
}
