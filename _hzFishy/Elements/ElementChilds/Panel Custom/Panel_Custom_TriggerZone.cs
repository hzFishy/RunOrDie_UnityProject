using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_Custom_TriggerZone : MonoBehaviour
{
    public Element_Panel_Custom PanelCustomScript;
    public LayerMask LayerMask;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PanelCustomScript.GetNext();
            Destroy(this);
        }
    }
}
