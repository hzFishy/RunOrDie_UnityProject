using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementChild : MonoBehaviour
{
    public bool blockPlayer = true;
    public bool noCollision = false;
    [SerializeField] private Element parentElement;

    [SerializeField] private List<GameObject> _directions;

    private void Awake()
    {
        if (blockPlayer)
        {
            return;
        }
        foreach (GameObject dir in _directions)
        {
            dir.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player") && !noCollision)
        {
            foreach (GameObject dir in _directions)
            {
                dir.SetActive(true);
            }
            parentElement.ParentOnEnter(collider, this);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player") && !noCollision)
        {
            foreach (GameObject dir in _directions)
            {
                dir.SetActive(true);
            }
            parentElement.ParentOnExit(collider, this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player") && !noCollision)
        {
            parentElement.ParentOnEnter(collision.collider, this);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player") && !noCollision)
        {
            parentElement.ParentOnExit(collision.collider, this);
        }
    }
}