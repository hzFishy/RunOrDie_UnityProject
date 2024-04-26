using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoEnd : MonoBehaviour
{
    public GameObject UITutoEnd;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Instantiate(UITutoEnd);
            Time.timeScale = 0;
        }
    }
}
