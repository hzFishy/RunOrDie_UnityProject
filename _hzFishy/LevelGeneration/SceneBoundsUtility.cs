using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class SceneBoundsUtility : MonoBehaviour
{
    public Bounds worldBounds;
#if UNITY_EDITOR
    [SerializeField] private bool _canDraw = false;
#endif


    [Button("GetBounds")]
    public Bounds GetBounds()
    {
        Renderer[] renderers = FindObjectsOfType<Renderer>();

        worldBounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; ++i)
        {
            worldBounds.Encapsulate(renderers[i].bounds);
        }
        return worldBounds;
    }
 
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_canDraw)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.1f);
            Gizmos.DrawCube(worldBounds.center, worldBounds.size);

            Gizmos.color = new Color(0f, 1f, 0f, 1f);
            Gizmos.DrawWireCube(worldBounds.center, worldBounds.size);
        }
    }
#endif
}
