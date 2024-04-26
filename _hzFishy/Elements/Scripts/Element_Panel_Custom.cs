using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;


public class Element_Panel_Custom : MonoBehaviour
{
    [SerializeField] private Elements _elements;

    [System.Serializable] private class Elements : SerializableDictionaryBase<int, pair> { }

    [System.Serializable]
    private struct pair
    {
        public ElementChild left;
        public ElementChild right;
    }

    [SerializeField] private Material _safeMaterial;

    private int _panelIndex = 0;
    private ElementChild _chosenBlock;
    private ElementChild _chosenSafe;
    private int _randomSafeChoice;

    //private bool _generate = true;

    private void Start()
    {
        GetNext();
    }

    public void GetNext()
    {
        _randomSafeChoice = Random.Range(1, 3);
        if (_randomSafeChoice == 1)
        {
            _chosenSafe = _elements[_panelIndex].left;
            _chosenBlock = _elements[_panelIndex].right;
        }
        else
        {
            _chosenSafe = _elements[_panelIndex].right;
            _chosenBlock = _elements[_panelIndex].left;
        }

        _chosenSafe.blockPlayer = false;
        _chosenSafe.noCollision = true;

        _chosenSafe.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = _safeMaterial;

        _panelIndex += 1;
    }
}

