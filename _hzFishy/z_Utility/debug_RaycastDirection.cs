using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class debug_RaycastDirection : MonoBehaviour
{
    public Vector3 Direction = Vector3.zero;
    public float Distance = 10;
    public float TimeLasting = 1;

    [ExecuteInEditMode]
    void Update()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Direction) * Distance, Color.green, TimeLasting);
    }
}
