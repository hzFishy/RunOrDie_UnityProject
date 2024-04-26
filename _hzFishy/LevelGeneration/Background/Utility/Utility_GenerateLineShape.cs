using UnityEngine;
using NaughtyAttributes;

using UnityEditor;
using System.Collections.Generic;
#if UNITY_EDITOR

public class Utility_GenerateLineShape : MonoBehaviour
{
    public GameObject Prefab;
    public Transform[] Parents;
    public float DistanceBetween;
    public Vector3 StartPosition;
    public Vector3 EndPosition;

    [Header("Gizmo")]
    public float radius = 0.5f;
    public bool GizmosDraw = false;

    private Vector3 currentPosition;

    [Button("Generate")]
    public void Gen()
    {
        currentPosition = StartPosition;
        foreach (var item in Parents)
        {
            currentPosition.z = StartPosition.z - DistanceBetween;
            for (int i = 0; i < (Vector3.Distance(StartPosition, EndPosition) / DistanceBetween) + 1; i++)
            {
                GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(Prefab);
                obj.transform.position = new Vector3(0, 0, currentPosition.z + DistanceBetween);
                obj.transform.SetParent(item,false);

                currentPosition.z -= DistanceBetween;
            }
        }
    }
    [Button("Remove Childs")]
    private void removeChilds()
    {
        foreach (var item in Parents)
        {
            List<GameObject> gos = new();
            foreach (Transform child in item)
            {
                gos.Add(child.gameObject);
            }
            foreach (var go in gos)
            {
                DestroyImmediate(go.gameObject);

            }
        }
    }

    private void OnDrawGizmos()
    {
        if (GizmosDraw)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(StartPosition, radius);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(EndPosition, radius);
        }
    }
}
#endif
