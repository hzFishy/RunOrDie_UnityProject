using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using NaughtyAttributes;
using RotaryHeart.Lib.SerializableDictionary;

public class CSVReader : MonoBehaviour
{
    #region Bindings
    [Serializable] public class Bindings : SerializableDictionaryBase<string, ElementsType> { }
    [BoxGroup("Bindings")]
    [Space(10)]
    [HorizontalLine(color: EColor.Red)]
    [SerializeField] private Bindings _bindings;


    [Button("Setup Bindings - Prefill")]
    private void BuildBindings_Prefill()
    {
        _bindings.Clear();
        foreach (ElementsType elemType in Enum.GetValues(typeof(ElementsType)))
        {
            //_bindings.Add(elemType);
            _bindings.Add(elemType.ToString(), elemType);
        }
    }
    #endregion

    
    [Serializable] public class CsvData : SerializableDictionaryBase<ElementsType, CsvDataColumn> { }
    [Serializable] public class CsvDataColumn { public CsvDataColumnLine values; }
    [Serializable] public class CsvDataColumnLine : SerializableDictionaryBase<ElementsType, float> { }
    [Space(10)]
    [HorizontalLine(color: EColor.Blue)] [BoxGroup("Last Saved Read results")] [ReadOnly] [SerializeField] private CsvData _csvData;

    [Serializable] public class ElementChances : SerializableDictionaryBase<ElementsType, ElementChancesValues> { }
    [Serializable] public class ElementChancesValues { public List<ElementsType> elements; }
    [SerializeField] [ReadOnly] private ElementChances _elementChances;


    #region CSV Read

    [BoxGroup("Last Saved Read results")] [ReadOnly] [SerializeField] private List<string> _foundTopHeaders;
    [BoxGroup("Last Saved Read results")] [ReadOnly] [SerializeField] private List<string> _foundSideHeaders;
    [BoxGroup("Last Saved Read results")] [ReadOnly] [SerializeField] private List<List<float>> _foundvalues = new List<List<float>>();

    [Button("CSV Reader - Clear Only")]
    private void Clear()
    {
        _foundTopHeaders.Clear();
        _foundSideHeaders.Clear();
        _foundvalues.Clear();
    }
    [Button("CSV Reader - Clear & Read")]
    private void ReadCSV()
    {
        if (_bindings.Count < 1)
        {
            Debug.LogError("Bindings should be build before");
            return;
        }
        Clear();
        string path = "Assets/_hzFishy/LevelGeneration/OBG MATH - Export CSV.csv";
        StreamReader strReader = new StreamReader(path);

        bool endOfFile = false;
        int i = 0;

        List<string> rawDataLines = new List<string>();
        int lineIndex = 0;
        while (!endOfFile)
        {
            string line = strReader.ReadLine();
            if (line == null)
            {
                endOfFile = true;
                break;
            }

            rawDataLines.Add(line);
            List<string> lineValues = line.Split(",").ToList();
            List<string> lineValuesClean = new List<string>();
            foreach (var item in lineValues)
            {
                lineValuesClean.Add(item.Replace("%", ""));
            }
            if (lineIndex > 0)
            {
                _foundSideHeaders.Add(lineValues[0]);

                List<float> values = lineValuesClean.GetRange(1, lineValuesClean.Count -1).ConvertAll(float.Parse);

                _foundvalues.Add(values);


            }
            lineIndex += 1;

            i += 1;
        }
        strReader.Close();
        _foundTopHeaders.Clear();
        _foundTopHeaders = rawDataLines[0].Split(",").ToList();
        _foundTopHeaders.RemoveAt(0);
    }
    #endregion


    #region Data Csv Build & methods

    [Button("CSV Data - Clear & Build CsvData")]
    private void BuildCsvData()
    {       
        _csvData.Clear();

        int indexTopHeader = 0;
        foreach (string topHeader in _foundTopHeaders)
        {
            CsvDataColumn chances = new CsvDataColumn();
            chances.values = new CsvDataColumnLine();
            int indexSideHeader = 0;
            foreach (string elem in _foundSideHeaders)
            {
                chances.values.Add(_bindings[elem], _foundvalues[indexSideHeader][indexTopHeader]);
                indexSideHeader += 1;
            }
            _csvData.Add(_bindings[topHeader], chances);
            indexTopHeader += 1;
        }

        

    }

    [Button("CSV Data - Clear & Build ElementChances")]
    void BuildElementChances()
    {
        _elementChances.Clear();
        foreach (ElementsType elem in _csvData.Keys)
        {
            ElementChancesValues _createdElements = new ElementChancesValues();
            _createdElements.elements = new List<ElementsType>();
            foreach (var item in _csvData[elem].values)
            {
                for (int i = 0; i < item.Value; i++)
                {
                    _createdElements.elements.Add(item.Key);
                }
            }
            _elementChances.Add(elem, _createdElements);
        }
    }
    public float GetData(ElementsType first, ElementsType second)
    {
        if (_csvData.ContainsKey(first))
        {
            if (_csvData[first].values.ContainsKey(second))
            {
                return _csvData[first].values[second];
            }
        }
        return -1;
    }
    public ElementsType GetRandomElement(ElementsType elemT)
    {
        List<ElementsType> elements = _elementChances[elemT].elements;
        var random = new System.Random();
        int rIndex = random.Next(elements.Count);
        return elements[rIndex];
    }

    #endregion


    #region Testing

    [Space(10)]
    [HorizontalLine(color: EColor.Yellow)]
    [BoxGroup("Testing")] [SerializeField] private ElementsType _GetDataBefore;
    [BoxGroup("Testing")] [SerializeField] private ElementsType _GetDataWanted;

    [Button("Testing - GetData")]
    private void TestGetData()
    {
        Debug.Log(GetData(_GetDataBefore, _GetDataWanted));
    }

    [BoxGroup("Testing")] [SerializeField] private ElementsType _GetRandomElementElement;

    [Button("Testing - GetRandomElement")]
    private void TestGetRandomElement()
    {
        Debug.Log(GetRandomElement(_GetRandomElementElement));
    }
    #endregion

}