using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element_WallRun_Custom : MonoBehaviour
{
    public GameObject wall;
    public GameObject dir;

    private void Awake()
    {
        bool right = Random.value > 0.5f;
        if (!right)
        {
            Quaternion newRotDir = Quaternion.Euler(0, -90, -90);
            dir.transform.rotation = newRotDir;
        }
        wall.transform.position += new Vector3(0,0, right ? -3 : 3);
        Quaternion newRot = Quaternion.Euler(right ? 45 : 135, 0, 0);
        wall.transform.rotation = newRot;
    }
}
