using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoHandleCheckpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collider.transform.parent.gameObject.GetComponent<PlayerController>().CurrentCheckpoint = gameObject.transform.position;
        }
    }
}
