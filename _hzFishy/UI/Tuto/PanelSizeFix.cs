using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelSizeFix : MonoBehaviour
{
    public RectTransform layoutRoot;
    // Start is called before the first frame update
    void Start()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);
    }
}
