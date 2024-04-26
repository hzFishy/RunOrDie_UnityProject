using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using NaughtyAttributes;
using RotaryHeart.Lib.SerializableDictionary;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class LevelGenerationManager : MonoBehaviour
{
    #region Variables

    #region Core
    [Space(20)]
    [HorizontalLine(color: EColor.White)]
    [HideIf("_generate")]
    [InfoBox("Level Generation is disabled !", EInfoBoxType.Warning)]

    [BoxGroup("Core")] [SerializeField] private bool _showCoreVariables;
    [ShowIf("_showCoreVariables")] [BoxGroup("Core")] [SerializeField] private bool _showRealPoints;
    [ShowIf(EConditionOperator.And,"_showRealPoints", "_showCoreVariables")] [BoxGroup("Core")] [SerializeField] private List<Point> _points = new();
    [ShowIf("_showCoreVariables")] [BoxGroup("Core")] [SerializeField] private bool _generate = true;

    [ShowIf("_showCoreVariables")] [BoxGroup("Core")] [SerializeField] private float _requiredPlayerDistanceToGenerate_Floor = 40;
    [ShowIf("_showCoreVariables")] [BoxGroup("Core")] [SerializeField] private float _generationFloorCooldown = 2;
    [ShowIf("_showCoreVariables")] [BoxGroup("Core")] [SerializeField] private float _requiredPlayerDistanceToGenerate_Point = 50;
    [ShowIf("_showCoreVariables")] [BoxGroup("Core")] [SerializeField] private float _generationPointCooldown = 1.2f;
    [ShowIf("_showCoreVariables")] [BoxGroup("Core")] [SerializeField] private float _requiredPlayerDistanceToGenerate_Element = 50;


    [ShowIf("_showCoreVariables")] [BoxGroup("Core")] [SerializeField] private GameObject _player;

    [System.Serializable] private struct Point
    {
        public Point(Vector3 position, ElementsType elemType)
        {
            this._position = position;
            this._elemType = elemType;
        }

        [SerializeField] public Vector3 _position;
        [SerializeField] public ElementsType _elemType;
    }
    #endregion

    #region Global Start

    [Space(10)]
    [HorizontalLine(color: EColor.Red)]
    [BoxGroup("Global")] [SerializeField] private bool _showGlobalVariables;
    [ShowIf("_showGlobalVariables")] [BoxGroup("Global")] [SerializeField] private CSVReader _csvReader;
    [ShowIf("_showGlobalVariables")] [BoxGroup("Global")] [SerializeField] private Vector3 _startPoint;
    [ShowIf("_showGlobalVariables")] [BoxGroup("Global")] [SerializeField] private Vector3 _floorStartPoint;
    [ShowIf("_showGlobalVariables")] [BoxGroup("Global")] [SerializeField] private GameObject _floorPrefab;
    [ShowIf("_showGlobalVariables")] [BoxGroup("Global")] [SerializeField] private GameObject _wallFrontPrefab;
    [ShowIf("_showGlobalVariables")] [BoxGroup("Global")] [SerializeField] private float _minBeforeFirstElement;
    [ShowIf("_showGlobalVariables")] [BoxGroup("Global")] [SerializeField] private float _maxBeforeFirstElement;
    [ShowIf("_showGlobalVariables")] [BoxGroup("Global")] [SerializeField] [Range(0, 50)] private float _minRange;
    [ShowIf("_showGlobalVariables")] [BoxGroup("Global")] [SerializeField] [Range(0, 50)] private float _maxRange;
    [ShowIf("_showGlobalVariables")] [BoxGroup("Global")] [SerializeField] [Range(0, 50)] private float _minRangeGoDown;
    [ShowIf("_showGlobalVariables")] [BoxGroup("Global")] [SerializeField] [Range(0, 50)] private float _maxRangeGoDown;

    #endregion

    #region Background
    [Space(20)]
    [HorizontalLine(color: EColor.Orange)]
#pragma warning disable 0414 // this bool is only used for the ShowIf attribute, not used in code
    [BoxGroup("Background")] [SerializeField] private bool _showBackgroundVariables = true;
    [BoxGroup("Background")] [ShowIf("_showBackgroundVariables")] [SerializeField] private bool _showBackgroundDebugVariables = false;
#pragma warning restore 0414
    [BoxGroup("Background")] [ShowIf("_showBackgroundVariables")] [SerializeField] private BackgroundSceneSizes _bigBackgroundScenes;
    [BoxGroup("Background")] [ShowIf("_showBackgroundVariables")] [SerializeField] private List<SceneName> _bigBackgroundScenesFlat;
    [SerializeField] [HideInInspector] private List<string> _bigBackgroundScenesNames = new();
    [System.Serializable] private class BackgroundSceneSizes : SerializableDictionaryBase<SceneName, Bounds> { }
    [System.Serializable] private struct SceneName
    {
        public SceneName(string name)
        {
            this.name = name;
        }
        [Scene] public string name;
    }

#if UNITY_EDITOR
    private string BuildPath(string sceneName)
    {
        return "Assets/_hzFishy/LevelGeneration/Background/Scenes/"+sceneName+".unity";
    }

    [Button("Background Gather Scenes Bounds")]
    private void GatherBackgroundScenesBounds()
    {
        List<SceneName> sceneNameKeys = new();
        List<Bounds> boundsValues = new();

        foreach (var item in _bigBackgroundScenes)
        {
            SceneName key = item.Key;
            sceneNameKeys.Add(key);
            string path = BuildPath(key.name);
            EditorSceneManager.OpenScene(path, OpenSceneMode.Additive); // load scene
            Scene loadedScene = EditorSceneManager.GetSceneByPath(path); // get scene
            boundsValues.Add(loadedScene.GetRootGameObjects()[0].GetComponent<SceneBoundsUtility>().GetBounds()); // get bounds
            EditorSceneManager.CloseScene(loadedScene,true); // close
        }

        int index = 0;
        foreach (SceneName item in sceneNameKeys)
        {
            _bigBackgroundScenes[item] = boundsValues[index];
            index += 1;

        }

        _bigBackgroundScenesNames = _bigBackgroundScenes.Keys.ToList().ConvertAll(data => data.name);

        sceneNameKeys.Clear();
        boundsValues.Clear();
    }
#endif

    #endregion

    #region Debug Runtime
#if UNITY_EDITOR
    [Space(10)]
    [HorizontalLine(color: EColor.Green)]
#pragma warning disable 0414 // this bool is only used for the ShowIf attribute, not used in code
    [BoxGroup("Debug runtime")] [SerializeField] private bool _showDebugVariables = false; 
#pragma warning restore 0414
    [BoxGroup("Debug runtime")] [ShowIf("_showDebugVariables")] [SerializeField] private bool _debug = false;
    [BoxGroup("Debug runtime")] [ShowIf("_showDebugVariables")] [SerializeField] [Range(0.0f, 1.0f)] private float _sphereSize = 0.7f;
    [BoxGroup("Debug runtime")] [ShowIf("_showDebugVariables")] [SerializeField] private GUIStyle _distanceGUIStyle;
    [BoxGroup("Debug runtime")] [ShowIf("_showDebugVariables")] [SerializeField] [Tag] private string _debugTag;

    private bool _tagedVisible = true;
    private int _debugindex_AddPoint_fct = 0; // used in AddPoint
#endif
    #endregion

    #region Elements Data
    [Space(20)]
    [HorizontalLine(color: EColor.Blue)] [BoxGroup("Elements Data")] [SerializeField] private Elements _elements;
    [System.Serializable] private class Elements : SerializableDictionaryBase<ElementsType, Element> { }
    [System.Serializable] private struct Element
    {
        [Header("Global")]
        public GameObject Prefab;
        [HorizontalLine(color: EColor.White)]

        [Header("Size")]
        public Vector3 size;
        //[SerializeField] public Vector3 center;
        [HorizontalLine(color: EColor.Blue)]

        [Header("Requirements")]
        public bool useGlobalDistance;
        [SerializeField] public ElementSpecificFloat specificDistanceBefore;
        [SerializeField] [AllowNesting] [HideIf("useGlobalDistance")] public float specificDistanceAfter;
        [AllowNesting] [ShowIf("useGlobalDistance")] public bool overrideBaseDistanceBetweenPoints;
        [AllowNesting] [ShowIf(EConditionOperator.And, "useGlobalDistance", "overrideBaseDistanceBetweenPoints")] public float newOverrideDistance;
        [AllowNesting] [ShowIf("TempShow")] public float minDistanceBefore;
        [AllowNesting] [ShowIf("TempShow")] public float minDistanceAfter;
        bool TempShow() { return useGlobalDistance && !overrideBaseDistanceBetweenPoints; }
        [HorizontalLine(color: EColor.Red)]

        [Header("NoFloor")]
        public bool noFloorUnder;
        [AllowNesting] [ShowIf("noFloorUnder")] public float startX;
        [AllowNesting] [ShowIf("noFloorUnder")] public float endX;

        [Header("Vertical")]
        public bool isVerticalElement;
        [AllowNesting] [ShowIf("isVerticalElement")] public VerticalState verticalType;
        [AllowNesting] [ShowIf("isVerticalElement")] public bool useSpecificHeight;

        [SerializeField] public ElementSpecificFloat floatingSpecificAddedHeight;
        [AllowNesting] [Label("Floating Global Added Height (.FloatingHeight)")] [ShowIf("isVerticalElement")] public float floatingGlobalAddedHeight;
        [AllowNesting] [Label("Persistent Added Height (.ElementPersistentHeight)")] [ShowIf("isVerticalElement")] public float persistentAddedHeight;
        [AllowNesting] [Label("Persistent Element spot position (.ElementPersistentHeight)")] [ShowIf("isVerticalElement")] public Vector3 persistentElementTopSpotLocalPosition;
        [AllowNesting] [Label("Persistent Element X offset (.ElementPersistentHeight)")] [ShowIf("isVerticalElement")] public float persistentElementXoffset;

        //[HorizontalLine(color: EColor.Green)]
    }
    [System.Serializable] public class ElementSpecificFloat : SerializableDictionaryBase<ElementsType, float> { }

    #endregion

    #region Runtime Global
    /// <summary> the extra distance the previous placed element wants after him </summary>
    private float _afterPreviousElementPointDistanceToAdd = 0;
    private Vector3 _currentFloorPoint;
    private Vector3 _currentElementPoint; //last generated point for the elements
    private Vector3 _lastElementPlaced = Vector3.zero;
    /// <summary> the ElementsType of the previous element point </summary>
    private ElementsType _previousPointElementType = ElementsType.None;
    private List<FloorZone> _noFloorZones = new();
    private bool _noFloorStarted = false;
    private struct FloorZone
    {
        public FloorZone(float startPositionX, float endPositionX)
        {
            this.startPositionX = startPositionX;
            this.endPositionX = endPositionX;
        }

        public float startPositionX;
        public float endPositionX;
    }


    [System.Serializable]
    private struct PersistentHeightElement
    {
        public PersistentHeightElement(Vector3 elementPosition, Vector3 topSpotLocalPosition, float spotPositionXoffset)
        {
            this.elementPosition = elementPosition;
            this.topSpotLocalPosition = topSpotLocalPosition;
            this.spotPositionXoffset = spotPositionXoffset;
        }

        public Vector3 elementPosition;
        public Vector3 topSpotLocalPosition;
        public float spotPositionXoffset;
    }
    private struct PersistentGoDown
    {
        public PersistentGoDown(Vector3 positionToGoDown, float distance)
        {
            this.positionToGoDown = positionToGoDown;
            this.distance = distance;
        }

        public Vector3 positionToGoDown;
        public float distance;
    }
    private enum VerticalState //state of the generation
    {
        None,
        FloatingHeight,
        ElementPersistentHeight,
        PersistentGoDown
    }
    private VerticalState _verticalState = VerticalState.None;
    private List<float> _persistentHeightToAddNextFloor_ForPoints = new();
    private List<PersistentHeightElement> _persistentHeightElementsData = new();
    private List<PersistentGoDown> _persistentGoDownData = new();
    private float _heightBeforeFloating;
    private bool _changedHeightPreviousPoint = false;
    #endregion

    #region Runtime Background
#if UNITY_EDITOR
    [ShowIf("_showBackgroundVariables")] [BoxGroup("Background")] [SerializeField] private bool _canDrawBigBackground = true;
#endif
    [ShowIf("_showBackgroundVariables")][BoxGroup("Background")][SerializeField] private List<LoadedSceneData> _loadedScenesData = new();
    [ShowIf("_showBackgroundVariables")][BoxGroup("Background")][SerializeField] private float _distanceBetweenBigBackgroundFlat = 50;
    private enum SceneType
    {
        Walls,
        Flat
    }

    [Serializable] private struct LoadedSceneData
    {
        public SceneType SceneType;
        public string Name;
        public Bounds Bounds;
        public int RealIndex;
        public float FlatPositionX;

        public LoadedSceneData(SceneType sceneType, string name, int realIndex, Bounds? bounds = null, float flatPosition = 0)
        {
            SceneType = sceneType;
            Name = name;
            Bounds = bounds.GetValueOrDefault(new ());
            RealIndex = realIndex;
            FlatPositionX = flatPosition;
        }
    }

    private bool _isAsyncLoadingBigBackground = false;
    private bool _isAsyncUnloadingBigBackground = false;
    private delegate IEnumerator LoadAsyncEndedDelegate(int sceneIndexToUnloadAfterwards);

    private BigBackgroundFlatData? _lastPlacedBigBackgroundFlatData;
    private bool _isAsyncLoadingBigBackgroundFlatDataSet = true;
    private bool _isAsyncUnloadingBigBackgroundFlat = false;

    [Serializable] private struct BigBackgroundFlatData
    {
        public BigBackgroundFlatData(Vector3 position,bool isAbove)
        {
            this.position = position;
            this.isAbove = isAbove;
        }

        public Vector3 position;
        public bool isAbove;
    }

    #endregion

    #region Debug Draw
#if UNITY_EDITOR
    private bool _canDraw = false;

    [System.Serializable] private struct DebugPoints
    {
        public DebugPoints(Vector3 position, GameObject gameobject, ElementsType elemType)
        {
            this._position = position;
            this._gameObject = gameobject;
            this._elemType = elemType;
        }
        [SerializeField] public Vector3 _position;
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private ElementsType _elemType;
    }
    [BoxGroup("Debug runtime")] [ShowIf("_showDebugVariables")] [SerializeField] List<DebugPoints> _debugPoints;
    [System.Serializable]
    private struct OtherDebugPoints
    {
        public OtherDebugPoints(Vector3 position, string name)
        {
            this.position = position;
            this.name = name;
        }
        [SerializeField] public Vector3 position;
        [SerializeField] public string name;
    }
    [BoxGroup("Debug runtime")] [ShowIf("_showDebugVariables")] [SerializeField] List<OtherDebugPoints> _otherDebugPoints;
#endif
    #endregion

    #endregion

    public PlayerController PlayerController;

    private void OnEnable()
    {
        PlayerController.PlayerStartPlaying += StartAllGenerations;
        PlayerController.PlayerStopPlaying += StopAllGenerations;
    }
    private void OnDisable()
    {
        PlayerController.PlayerStartPlaying -= StartAllGenerations;
        PlayerController.PlayerStopPlaying -= StopAllGenerations;
    }


    #region Global Generation
    private void StartAllGenerations()
    {
        StartElementAndFloorGeneration();
        if (!PlayerController.BlockGeneration)
        {
            StartBackgroundGeneration();
        }
    }
    public void StopAllGenerations()
    {
        StopElementAndFloorGeneration();
        if (!PlayerController.BlockGeneration)
        {
            StopBackgroundGeneration();
        }
    }
    #endregion


    #region Background Generation

#if UNITY_EDITOR
    #region DebugLoadedScenes
    /*
    void Update()
    {
        UpdateDebugScenes();
    }
    */
    [ShowIf("_showBackgroundVariables")] [BoxGroup("Background")] [SerializeField] List<string> DebugLoadedScene = new();
    [Button("UpdateDebugScenes")]
    private void UpdateDebugScenes()
    {
        DebugLoadedScene.Clear();
        for (int i = 0; i < SceneManager.loadedSceneCount; i++)
        {
            DebugLoadedScene.Add(i + " / " + SceneManager.GetSceneAt(i).name);
        }
    }
    #endregion
#endif

    #region Start/Stop
    [Button("Start Background Generation")]
    private void StartBackgroundGeneration()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.EnterPlaymode();
            return;
        }
#endif
        _loadedScenesData.Clear();
        _isAsyncLoadingBigBackground = false;
        _isAsyncUnloadingBigBackground = false;
        _isAsyncLoadingBigBackgroundFlatDataSet = true;

        StartCoroutine(HandleBackgroundGeneration());
    }
    [Button("Stop Background Generation")]
    private void StopBackgroundGeneration()
    {
        StopCoroutine(HandleBackgroundGeneration());
    }
    #endregion

    #region Get

    private List<LoadedSceneData> GetLoadedScenesDataWithFilter(SceneType sceneType)
    {
        return _loadedScenesData.Where(sceneData => sceneData.SceneType == sceneType).ToList();
    }

    #endregion

    #region StateCheckers
    private bool CanGenerateBigBackground()
    {
        if (GetLoadedScenesDataWithFilter(SceneType.Walls).Count < 3)
        {
            return true;
        }
        else if (_isAsyncLoadingBigBackground || GetLoadedScenesDataWithFilter(SceneType.Walls).Count > 5)
        {
            return false;
        }
        return _player.transform.position.x > _loadedScenesData[1].Bounds.center.x;
    }
    private bool[] CanGenerateBigBackgroundFlat()
    {
        // [0] Can generate ?
        // [1] Unload Behind ?
        bool[] returnValues = new bool[2];

        if (!_lastPlacedBigBackgroundFlatData.HasValue && _isAsyncLoadingBigBackgroundFlatDataSet)
        {
            //Debug.Log("none");
            returnValues[0] = true;
        }
        else if (!_isAsyncLoadingBigBackgroundFlatDataSet)
        {
            //Debug.Log("not set");
        }
        else if (GetLoadedScenesDataWithFilter(SceneType.Flat).Count < 4)
        {
            returnValues[0] = true;
        }
        else
        {
            //returnValues[0] = _player.transform.position.x > (_lastPlacedBigBackgroundFlatData.Value.position.x + _distanceBetweenBigBackgroundFlat);
            returnValues[0] = _player.transform.position.x > (GetLoadedScenesDataWithFilter(SceneType.Flat)[0].FlatPositionX + _distanceBetweenBigBackgroundFlat);
            if (GetLoadedScenesDataWithFilter(SceneType.Flat).Count > 3)
            {
                returnValues[1] = true;
            }
        }
            return returnValues;
    }
    
    private bool AsyncTasksRunning()
    {
        return _isAsyncLoadingBigBackground || _isAsyncUnloadingBigBackground || !_isAsyncLoadingBigBackgroundFlatDataSet;
    }
    private bool NoAsyncTasksRunning()
    {
        return !(_isAsyncLoadingBigBackground && _isAsyncUnloadingBigBackground && !_isAsyncLoadingBigBackgroundFlatDataSet && _isAsyncUnloadingBigBackgroundFlat);
    }
    #endregion

    private IEnumerator HandleBackgroundGeneration()
    {
#if UNITY_EDITOR
        _canDraw = true;
#endif
        while (true && !PlayerController.BlockGeneration)
        {
            bool[] canGenerateBigBackgroundFlat = CanGenerateBigBackgroundFlat();
            if (CanGenerateBigBackground())
            {
                SpawnBigBackground();
            }
            else if (canGenerateBigBackgroundFlat[0])
            {
                SpawnBigBackgroundFlat(canGenerateBigBackgroundFlat[1]);
            }

            yield return AsyncTasksRunning() ? new WaitUntil(() => NoAsyncTasksRunning()) :  new WaitForSeconds(2);
        }
    }

    #region Spawn
    private void SpawnBigBackground()
    {
        string choosenScene = _bigBackgroundScenesNames[Random.Range(0, _bigBackgroundScenesNames.Count)];
        if (GetLoadedScenesDataWithFilter(SceneType.Walls).Count > 2)
        {
            StartCoroutine(LoadBigBackgroundWallsScene(choosenScene, new LoadAsyncEndedDelegate(UnloadBigBackgroundWallsScene), 0));
        }
        else
        {
            StartCoroutine(LoadBigBackgroundWallsScene(choosenScene));
        }
    }
    private void SpawnBigBackgroundFlat(bool unload)
    {
        string choosenScene = _bigBackgroundScenesFlat[Random.Range(0, _bigBackgroundScenesFlat.Count)].name;
        if (!unload)
        {
            StartCoroutine(LoadBigBackgroundFlatScene(choosenScene));
        }
        else
        {
            //Debug.Log(" === Requesting unloading BG Flat");
            StartCoroutine(LoadBigBackgroundFlatScene(choosenScene, new LoadAsyncEndedDelegate(UnloadBigBackgroundFlatScene), 0));
        }
    }
    #endregion

    #region Main Load/Unload Async Scene
    private IEnumerator MainLoadAsyncScene(string choosenScene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(choosenScene, LoadSceneMode.Additive);

        _isAsyncLoadingBigBackground = true;
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        _isAsyncLoadingBigBackground = false;
    }
    private IEnumerator MainUnloadAsyncScene(int realSceneIndex)
    {
        Scene scene = SceneManager.GetSceneAt(realSceneIndex);
        Debug.Log(" === Unloading: " + scene.name +" realIndex: " + realSceneIndex);
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);

        _isAsyncUnloadingBigBackground = true;
        while (!asyncUnload.isDone)
        {
            yield return null;
        }
        _isAsyncUnloadingBigBackground = false;
        Debug.Log(" === Finished unloading");
    }
    #endregion

    #region Flat
    private IEnumerator LoadBigBackgroundFlatScene(string choosenScene, LoadAsyncEndedDelegate sceneLoadCallback = null, int sceneLoadCallbackValue = 0)
    {
        _isAsyncLoadingBigBackgroundFlatDataSet = false;
        if (sceneLoadCallback != null)
        {
            _isAsyncUnloadingBigBackgroundFlat = true;
        }
        yield return StartCoroutine(MainLoadAsyncScene(choosenScene));

        List<LoadedSceneData> data = GetLoadedScenesDataWithFilter(SceneType.Flat);
        int realSceneIndex = _loadedScenesData[^1].RealIndex + 1;


        _loadedScenesData.Add(new(SceneType.Flat, choosenScene, realSceneIndex, null));
        int indexAdded = _loadedScenesData.Count-1;


        SceneName sceneName = new(choosenScene);

        Scene scene = SceneManager.GetSceneAt(realSceneIndex);
        GameObject[] sceneGameObjects = scene.GetRootGameObjects();

        Vector3 newPosition = new(
            Random.Range(_player.transform.position.x + 50, _player.transform.position.x + 150),
            Random.value > 0.5f ? Random.Range(_currentElementPoint.y - 5, _currentElementPoint.y - 20) : Random.Range(_currentElementPoint.y + 10, _currentElementPoint.y + 25),
            Random.value > 0.5f ? Random.Range(5,20) : Random.Range(-5,-20)
            );

        sceneGameObjects[0].transform.position = newPosition;
        
        Vector3 rot = sceneGameObjects[0].transform.localRotation.eulerAngles;
        rot = new Vector3(rot.x, Random.Range(0,360), rot.z);
        sceneGameObjects[0].transform.localRotation = Quaternion.Euler(rot);


        LoadedSceneData tempData = _loadedScenesData[indexAdded];
        tempData.FlatPositionX = newPosition.x;
        _loadedScenesData[indexAdded] = tempData;
        _lastPlacedBigBackgroundFlatData = new BigBackgroundFlatData(newPosition, newPosition.y > 0);
        _isAsyncLoadingBigBackgroundFlatDataSet = true;
        
        /*
        if (_lastPlacedBigBackgroundFlatData.HasValue)
        {
            Debug.Log(_lastPlacedBigBackgroundFlatData.Value.position);
        }
        */

        if (sceneLoadCallback != null)
        {
            StartCoroutine(sceneLoadCallback(sceneLoadCallbackValue));
        }
    }
    private IEnumerator UnloadBigBackgroundFlatScene(int sceneIndex)
    {
        int realSceneIndex = GetLoadedScenesDataWithFilter(SceneType.Flat)[sceneIndex].RealIndex;
        //Debug.Log(" === For unloading, found realindex:" + realSceneIndex);

        yield return StartCoroutine(MainUnloadAsyncScene(realSceneIndex));

        RemoveBigBackgroundData(realSceneIndex);
        //Debug.Log(" === Removed index");
        _isAsyncUnloadingBigBackgroundFlat = false;
    }
    #endregion

    #region Walls
    private IEnumerator LoadBigBackgroundWallsScene(string choosenScene, LoadAsyncEndedDelegate sceneLoadCallback = null, int sceneLoadCallbackValue = 0)
    {

        yield return StartCoroutine(MainLoadAsyncScene(choosenScene));


        int loadedScenesDataIndex = 0;
        int realSceneIndex = 1;
        List<LoadedSceneData> data = GetLoadedScenesDataWithFilter(SceneType.Walls);

        if (data.Count < 1)
        {
            _loadedScenesData.Add(new (SceneType.Walls, choosenScene, realSceneIndex, null));
        }
        else
        {
            realSceneIndex = _loadedScenesData[^1].RealIndex + 1;
            _loadedScenesData.Add(new (SceneType.Walls, choosenScene, realSceneIndex, null));
            loadedScenesDataIndex = _loadedScenesData.Count - 1;
        }

        SceneName sceneName = new (choosenScene);

        Scene scene = SceneManager.GetSceneAt(realSceneIndex);
        GameObject[] sceneGameObjects = scene.GetRootGameObjects();
        
        Bounds bounds = _bigBackgroundScenes[sceneName];
        Vector3 offset = Vector3.zero;

        data = GetLoadedScenesDataWithFilter(SceneType.Walls);

        if (data.Count < 2)
        {
            offset.x = _player.transform.position.x;
        }
        else
        {
            offset.x = data[^2].Bounds.center.x + data[^2].Bounds.extents.x;
        }
        offset.y = _player.transform.position.y;


        sceneGameObjects[0].transform.position = bounds.center = offset;

        if (Random.value > 0.5f)
        {
            Vector3 rot = sceneGameObjects[0].transform.rotation.eulerAngles;
            rot = new Vector3(rot.x, rot.y + 180, rot.z);
            sceneGameObjects[0].transform.rotation = Quaternion.Euler(rot);
        }
        
        LoadedSceneData tempSaveLoadedSceneData = _loadedScenesData[loadedScenesDataIndex];
        tempSaveLoadedSceneData.Bounds = bounds;
        _loadedScenesData[loadedScenesDataIndex] = tempSaveLoadedSceneData;

        if (sceneLoadCallback != null)
        {
            StartCoroutine(sceneLoadCallback(sceneLoadCallbackValue));
        }
    }
    private IEnumerator UnloadBigBackgroundWallsScene(int sceneIndex)
    {
        int realSceneIndex = GetLoadedScenesDataWithFilter(SceneType.Walls)[sceneIndex].RealIndex;

        yield return StartCoroutine(MainUnloadAsyncScene(realSceneIndex));

        RemoveBigBackgroundData(realSceneIndex);
    }
    #endregion

    private void RemoveBigBackgroundData(int realSceneIndex)
    {
        //Debug.Log(" === removing index at "+ _loadedScenesData.FindIndex((SceneData => SceneData.RealIndex == realSceneIndex)));
        _loadedScenesData.RemoveAt(_loadedScenesData.FindIndex((SceneData => SceneData.RealIndex == realSceneIndex)));

        int index = 0;
        for (int i = 0; i < _loadedScenesData.Count; i++)
        {
            if (_loadedScenesData[i].RealIndex > realSceneIndex)
            {
                LoadedSceneData LoadedSceneData = _loadedScenesData[i];
                LoadedSceneData.RealIndex -= 1;
                _loadedScenesData[i] = LoadedSceneData;
            }

            index += 1;
        }
    }

    #endregion


    #region Elements & Floor Generation
    /// <summary> Add a element point that will be retreive later on to generate it </summary>
    private void AddPoint(Vector3 position, ElementsType elemType, bool debugOnly = false)
    {
        //Debug.Log("point " + _debugindex_AddPoint_fct + " " + elemType);
        if (!debugOnly)
        {
            _previousPointElementType = elemType;
            _points.Add(new Point(position, elemType));
        }

#if UNITY_EDITOR
        GameObject newgameobject = new("point " + _debugindex_AddPoint_fct);
        newgameobject.transform.SetParent(transform, false);
        newgameobject.transform.position = position;
        newgameobject.tag = _debugTag;
        _debugPoints.Add(new DebugPoints(position, newgameobject, elemType));

        _debugindex_AddPoint_fct += 1;
#endif
    }
    


    #region Start/Stop
    [Button("Start Element And Floor Generation")]
    private void StartElementAndFloorGeneration()
    {
        if (_generate)
        {
            StopElementAndFloorGeneration();
        }
        ClearValues();
#if UNITY_EDITOR
        _canDraw = true;
#endif
        _generate = true;

        _currentElementPoint = _startPoint;
        AddPoint(_startPoint, ElementsType.None, true);

        _currentElementPoint.x += Random.Range(_minBeforeFirstElement, _maxBeforeFirstElement);
        AddPoint(_currentElementPoint, GetNextElementDependingOnPreviousOne(ElementsType.None));

        StartCoroutine(HandleElementGeneration());
        StartCoroutine(HandleFloorGeneration());
    }
    
    [Button("Stop Element And Floor Generation")]
    private void StopElementAndFloorGeneration()
    {
        StopCoroutine(HandleElementGeneration());
        StopCoroutine(HandleFloorGeneration());
        _generate = false;
    }

    #endregion

    #region Handle Generations
    private bool IsPlayerCloseEnough_Point()
    {
        return Vector3.Distance(_player.transform.position, _currentElementPoint) < _requiredPlayerDistanceToGenerate_Point;
    }
    private bool IsPlayerCloseEnough_Element()
    {
        return _lastElementPlaced.x - _player.transform.position.x < _requiredPlayerDistanceToGenerate_Element;
    }

    private IEnumerator HandleElementGeneration()
    {
        while (_generate)
        {
            if (IsPlayerCloseEnough_Point())
            {
                GatherNextElementPoint();
            }
            if (IsPlayerCloseEnough_Element())
            {
                PlaceElements(1);
            }
            yield return new WaitForSeconds(_generationPointCooldown);
        }
    }
    private bool IsPlayerCloseEnough_Floor()
    {
        return Vector3.Distance(_player.transform.position, _currentFloorPoint) < _requiredPlayerDistanceToGenerate_Floor;
    }
    private IEnumerator HandleFloorGeneration()
    {
        _currentFloorPoint = _floorStartPoint;
        while (_generate)
        {
            if (IsPlayerCloseEnough_Floor())
            {
                GatherNextFloorPointAndPlaceFloor(1);
            }
            yield return new WaitForSeconds(_generationFloorCooldown);
        }
    }
    #endregion

    #region Gather & Place & Spawn
    private ElementsType GetNextElementDependingOnPreviousOne(ElementsType lastSpawnedElement)
    {
        return _csvReader.GetRandomElement(lastSpawnedElement);
    }
    private void GatherNextElementPoint()
    {
        //bool debugtempheight = false;
        void Gonext()
        {
            bool isGoingDownPoint = false; // are we going to go down for this point ?

            ElementsType newElemType;
            newElemType = GetNextElementDependingOnPreviousOne(_previousPointElementType);

            if (_verticalState == VerticalState.PersistentGoDown)
            {
                while (_elements[newElemType].isVerticalElement || _elements[newElemType].noFloorUnder) // to avoid changing height in a row we do this
                {
                    newElemType = GetNextElementDependingOnPreviousOne(_previousPointElementType);
                }
            }
            Element elemData = _elements[newElemType];

            
            switch (_verticalState)
            {
                #region None
                case VerticalState.None:
                    _changedHeightPreviousPoint = false;
                    break;
                #endregion
                #region FloatingHeight
                case VerticalState.FloatingHeight:
                    if (elemData.isVerticalElement)
                    {
                        switch (elemData.verticalType)
                        {
                            case VerticalState.FloatingHeight:
                                break; // because this one is also vertical we dont reset anything
                            default:
                                // reset height
                                ResetHeightAfterFloating();
                                break;
                        }
                    }
                    else
                    {
                        ResetHeightAfterFloating();
                    }
                    _changedHeightPreviousPoint = true;
                    break;
                #endregion
                #region ElementPersistentHeight
                case VerticalState.ElementPersistentHeight:
                    _currentElementPoint.y += _persistentHeightToAddNextFloor_ForPoints[0];
                    _persistentHeightToAddNextFloor_ForPoints.RemoveAt(0);
                    //debugtempheight = true;
                    _verticalState = VerticalState.None;
                    _changedHeightPreviousPoint = true;
                    break;
                #endregion
                case VerticalState.PersistentGoDown:
                    _changedHeightPreviousPoint = true;
                    _currentElementPoint.x += Random.Range(_minRangeGoDown, _maxRangeGoDown) + elemData.size.x;
                    isGoingDownPoint = true;
                    _verticalState = VerticalState.None;
                    break;
                default:
                    break;
            }
            
            if (Random.Range(0, 10) > 6 && _verticalState == VerticalState.None && !_changedHeightPreviousPoint)
            {
                float addedX = Random.Range(_minRangeGoDown, _maxRangeGoDown);

                if (
                    _noFloorZones.Count == 0
                    ||
                    (
                        (_currentElementPoint.x + addedX) < _noFloorZones[0].startPositionX
                        ||
                        (_currentElementPoint.x + addedX) > _noFloorZones[0].endPositionX + 10
                    )
                   )
                {
                    isGoingDownPoint = true;

                    _verticalState = VerticalState.PersistentGoDown;
                    float downDistance = Random.Range(3, 7);
                    //AddOtherPoint(_currentElementPoint, "GO DOWN special 1");

                    _currentElementPoint.x += addedX; // special distance

                    _persistentGoDownData.Add(new PersistentGoDown(_currentElementPoint, downDistance));
                    //AddOtherPoint(_currentElementPoint, "GO DOWN special 2");
                    _currentElementPoint.y -= downDistance;
                    //AddOtherPoint(_currentElementPoint, "GO DOWN special 3");
                    newElemType = ElementsType.None;
                }
            }
            

            if (!isGoingDownPoint)
            {
                if (elemData.useGlobalDistance)
                {
                    if (elemData.overrideBaseDistanceBetweenPoints)
                    {
                        _currentElementPoint.x += elemData.newOverrideDistance;
                    }
                    else
                    {
                        _currentElementPoint.x +=
                            Random.Range(_minRange, _maxRange) +
                            elemData.size.x +
                            elemData.minDistanceBefore +
                            _afterPreviousElementPointDistanceToAdd;

                        _afterPreviousElementPointDistanceToAdd = elemData.minDistanceAfter;
                    }
                }
                else
                {
                    _currentElementPoint.x +=
                        elemData.size.x +
                        elemData.specificDistanceBefore[
                            (elemData.specificDistanceBefore.ContainsKey(_previousPointElementType)) ? _previousPointElementType : ElementsType.None
                        ] +
                        _afterPreviousElementPointDistanceToAdd;

                    _afterPreviousElementPointDistanceToAdd = elemData.specificDistanceAfter;
                }

                //if (debugtempheight) { /*AddOtherPoint(_currentElementPoint, "RESULT PERSISTENT"); */}

                if (elemData.isVerticalElement)
                {
                    switch (elemData.verticalType)
                    {
                        #region FloatingHeight
                        case VerticalState.FloatingHeight:
                            if (_verticalState == VerticalState.None)
                            {
                                _heightBeforeFloating = _currentElementPoint.y;
                            }
                            if (elemData.useSpecificHeight)
                            {
                                if (elemData.floatingSpecificAddedHeight.ContainsKey(_previousPointElementType))
                                {
                                    _currentElementPoint.y += elemData.floatingSpecificAddedHeight[_previousPointElementType];
                                }
                                else
                                {
                                    _currentElementPoint.y += elemData.floatingSpecificAddedHeight[ElementsType.None];
                                }
                            }
                            else
                            {
                                _currentElementPoint.y += elemData.floatingGlobalAddedHeight;
                            }
                            break;
                        #endregion
                        #region ElementPersistentHeight
                        case VerticalState.ElementPersistentHeight:
                            _persistentHeightToAddNextFloor_ForPoints.Add(elemData.persistentAddedHeight);
                            _persistentHeightElementsData.Add(new PersistentHeightElement(
                                _currentElementPoint, elemData.persistentElementTopSpotLocalPosition,
                                elemData.persistentElementXoffset));
                            //AddOtherPoint(_currentElementPoint, "INIT PERSISTENT");
                            break;
                        #endregion
                        default:
                            break;
                    }
                    _verticalState = elemData.verticalType;
                }

            }

            if (elemData.noFloorUnder && !isGoingDownPoint && newElemType != ElementsType.None)
            {
#if UNITY_EDITOR
                AddOtherPoint(new Vector3(_currentElementPoint.x + elemData.startX, _currentElementPoint.y, _currentElementPoint.z), "start");
                AddOtherPoint(new Vector3(_currentElementPoint.x + elemData.endX, _currentElementPoint.y, _currentElementPoint.z), "end");
#endif
                _noFloorZones.Add(new FloorZone(
                    _currentElementPoint.x + elemData.startX, // "+" because "startX" is already negative
                    _currentElementPoint.x + elemData.endX));
            }

            AddPoint(_currentElementPoint, newElemType); //_previousPointElementType updated here
        }

        Gonext();
    }

    private void ResetHeightAfterFloating()
    {
        _currentElementPoint.y = _heightBeforeFloating;
        _heightBeforeFloating = 0;
        _verticalState = VerticalState.None;
    }

    [Button("PlaceElements")]
    private void PlaceElements(int amount = 1)
    {
        foreach (Point point in _points.GetRange(0, amount))
        {
            _lastElementPlaced = SpawnNextElement(point._position, point._elemType);
        }
        _points.RemoveRange(0, amount);
    }
    private Vector3 SpawnNextElement(Vector3 position, ElementsType elemType)
    {
        GameObject placedElement = new();
        switch (elemType)
        {
            case ElementsType.None:
                return Vector3.zero;
            default:
                placedElement = Instantiate(_elements[elemType].Prefab, position, Quaternion.identity);
#if UNITY_EDITOR
                placedElement.tag = _debugTag;
#endif
                return placedElement.transform.position;
        }
    }

    
    private void GatherNextFloorPointAndPlaceFloor(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (_noFloorZones.Count > 0)
            {
                 
                //  if futur floor is in no floor zone
                if (_currentFloorPoint.x + 10 + 5 > _noFloorZones[0].startPositionX && !_noFloorStarted)
                {
                    GameObject newFloorGameObjectNoFloorBefore = Instantiate(
                        _floorPrefab,
                        new Vector3((_currentFloorPoint.x + 5 + _noFloorZones[0].startPositionX) / 2, _currentFloorPoint.y, _currentFloorPoint.z),
                        Quaternion.identity);

                    newFloorGameObjectNoFloorBefore.transform.localScale = new Vector3(
                        (_noFloorZones[0].startPositionX - (_currentFloorPoint.x+5)) /2,
                        newFloorGameObjectNoFloorBefore.transform.localScale.y,
                        newFloorGameObjectNoFloorBefore.transform.localScale.z);
                    newFloorGameObjectNoFloorBefore.transform.GetChild(0).localScale = new Vector3(0.2f, 1, 1);

                    _currentFloorPoint = newFloorGameObjectNoFloorBefore.transform.position;
                    _noFloorStarted = true;
#if UNITY_EDITOR
                    newFloorGameObjectNoFloorBefore.tag = _debugTag;
#endif
                    continue;
                }
                else if (_currentFloorPoint.x + 10 + 5 > _noFloorZones[0].endPositionX)
                {
                    _currentFloorPoint = new Vector3(_noFloorZones[0].endPositionX+5, _currentFloorPoint.y, _currentFloorPoint.z);

                    GameObject newFloorGameObjectNoFloorAfter = Instantiate(_floorPrefab, _currentFloorPoint, Quaternion.identity);
#if UNITY_EDITOR
                    newFloorGameObjectNoFloorAfter.tag = _debugTag;
#endif
                    _noFloorZones.RemoveAt(0);
                    _noFloorStarted = false;
                    continue;
                }
                else if (_noFloorStarted)
                {
                    _currentFloorPoint.x += 10;
                    continue;
                }
                
            }

            _currentFloorPoint.x += 10;
            if (/*_currentFloorPoint.y != _currentElementPoint.y*/ _persistentHeightElementsData.Count > 0 || _persistentGoDownData.Count > 0)
            {
                if (/* _currentElementPoint.y > _currentFloorPoint.y &&*/ _persistentHeightElementsData.Count > 0) // maybe replace '_currentElementPoint.y' if elements gen to far away
                {
                    PersistentHeightElement currentPersistentHeightElement = _persistentHeightElementsData[0];
                    Vector3 currentPersistentHeightElementPosition = currentPersistentHeightElement.elementPosition;
                    Vector3 currentPersistentHeightElementTopSpotLocalPosition = currentPersistentHeightElement.topSpotLocalPosition;

                    if (_currentFloorPoint.x < (currentPersistentHeightElementPosition.x + currentPersistentHeightElementTopSpotLocalPosition.x)
                        && _currentFloorPoint.x + 10 > (currentPersistentHeightElementPosition.x + currentPersistentHeightElementTopSpotLocalPosition.x))
                    {
                        GameObject newFloorGameObjectPersistentHeight = Instantiate(_floorPrefab, _currentFloorPoint, Quaternion.identity); // on same ground 
#if UNITY_EDITOR
                        newFloorGameObjectPersistentHeight.tag = _debugTag;
#endif
                        if (newFloorGameObjectPersistentHeight.transform.position.x + 5 < (currentPersistentHeightElementPosition.x + currentPersistentHeightElementTopSpotLocalPosition.x))
                        {
                            newFloorGameObjectPersistentHeight = Instantiate(_floorPrefab, new Vector3(newFloorGameObjectPersistentHeight.transform.position.x + 5 + 5, _currentFloorPoint.y, _currentFloorPoint.z), Quaternion.identity); // on same ground 
#if UNITY_EDITOR
                            newFloorGameObjectPersistentHeight.tag = _debugTag;
#endif
                        }
                        PlaceWall(
                            new Vector3(
                                currentPersistentHeightElementPosition.x + currentPersistentHeightElementTopSpotLocalPosition.x + currentPersistentHeightElement.spotPositionXoffset,
                                currentPersistentHeightElementPosition.y,
                                currentPersistentHeightElementPosition.z),
                            currentPersistentHeightElementPosition + currentPersistentHeightElementTopSpotLocalPosition + new Vector3(currentPersistentHeightElement.spotPositionXoffset, 0,0));

                        _currentFloorPoint.x = currentPersistentHeightElementPosition.x + currentPersistentHeightElementTopSpotLocalPosition.x + 5;
                        _currentFloorPoint.y = currentPersistentHeightElementPosition.y + currentPersistentHeightElementTopSpotLocalPosition.y;
                        newFloorGameObjectPersistentHeight = Instantiate(_floorPrefab, _currentFloorPoint, Quaternion.identity); // on floor, snapped to height and x
#if UNITY_EDITOR
                        newFloorGameObjectPersistentHeight.tag = _debugTag;
#endif

                        _persistentHeightElementsData.RemoveAt(0);
                        continue;
                    }
                }
                /*else */if (/*_currentElementPoint.y < _currentFloorPoint.y &&*/ _persistentGoDownData.Count > 0)
                {
                    PersistentGoDown currentGoDownData = _persistentGoDownData[0];
                    if (_currentFloorPoint.x < currentGoDownData.positionToGoDown.x && (_currentFloorPoint.x + 10) > currentGoDownData.positionToGoDown.x) // close enough to go down
                    {
                        GameObject newFloorGameObjectGoDown = Instantiate(_floorPrefab, _currentFloorPoint, Quaternion.identity); // on same ground , can hover floor
#if UNITY_EDITOR
                        newFloorGameObjectGoDown.tag = _debugTag;
#endif

                        _currentFloorPoint.y -= currentGoDownData.distance;

                        PlaceWall(
                            new Vector3(_currentFloorPoint.x + 10 - 5, _currentFloorPoint.y, _currentFloorPoint.z),
                            new Vector3(_currentFloorPoint.x + 10 - 5, _currentFloorPoint.y + currentGoDownData.distance, _currentFloorPoint.z),
                            true);

                        _persistentGoDownData.RemoveAt(0);
                        continue;
                    }
                }
            }
            
            

            GameObject newFloorGameObject = Instantiate(_floorPrefab, _currentFloorPoint, Quaternion.identity);
#if UNITY_EDITOR
            newFloorGameObject.tag = _debugTag;
#endif
            //Debug.Log("normal spawn: " + _currentFloorPoint);
        }
    }

    private void PlaceWall(Vector3 bottomposition, Vector3 topposition, bool reverse = false)
    {
        //Debug.Log(bottomposition + "    " + topposition);
        GameObject newWallGameObject = Instantiate(_wallFrontPrefab, bottomposition, Quaternion.identity);
#if UNITY_EDITOR
        newWallGameObject.tag = _debugTag;
#endif
        WallFront wallFront = newWallGameObject.GetComponent<WallFront>();
        wallFront.wallRef.transform.localScale = new Vector3(wallFront.wallRef.transform.localScale.x,topposition.y-bottomposition.y, wallFront.wallRef.transform.localScale.z);

        foreach (var directionGameObject in wallFront.directions)
        {
            // place on top
            directionGameObject.transform.localPosition = new Vector3(directionGameObject.transform.localPosition.x, wallFront.wallRef.transform.localPosition.y + wallFront.wallRef.transform.localScale.y + 2, directionGameObject.transform.localPosition.z);
            if (!reverse)
            {
                Vector3 rot = directionGameObject.transform.localRotation.eulerAngles;
                rot = new Vector3(rot.x, rot.y, rot.z + 180);
                directionGameObject.transform.localRotation = Quaternion.Euler(rot);
            }
        }

        if (reverse)
        {
            Vector3 rot = newWallGameObject.transform.rotation.eulerAngles;
            rot = new Vector3(rot.x, rot.y + 180, rot.z);
            newWallGameObject.transform.rotation = Quaternion.Euler(rot);
        }
        
        //temp fix bug
        //wallFront.wall.transform.GetChild(0).localPosition = new Vector3(0,0.5f,0);
    }

    #endregion

    #endregion


#if UNITY_EDITOR
    #region Gizmo & Handles
    private void OnDrawGizmos()
    {
        if (_debug && _canDraw)
        {
            int index = 0;
            foreach (DebugPoints debugPoint in _debugPoints)
            {
                DrawSphere(debugPoint._position, Color.yellow);
                if (index < (_debugPoints.Count-1))
                {
                    Handles.Label((_debugPoints[index + 1]._position + _debugPoints[index]._position)/2 + new Vector3(0, 0.5f, 0), Mathf.Ceil(_debugPoints[index + 1]._position.x - _debugPoints[index]._position.x).ToString(), _distanceGUIStyle);
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(_debugPoints[index]._position, _debugPoints[index + 1]._position);
                }
                index += 1;
            }
            foreach (var point in _otherDebugPoints)
            {
                DrawSphere(point.position,Color.red);
                Handles.Label(point.position, point.name);
            }
            
            if (_canDrawBigBackground)
            {
                index = 0;
                foreach (var bound in GetLoadedScenesDataWithFilter(SceneType.Walls).Select(SceneData => SceneData.Bounds))
                {
                    switch (index)
                    {
                        case 0:
                            Gizmos.color = Color.red;
                            break;
                        case 1:
                            Gizmos.color = Color.yellow;
                            break;
                        case 2:
                            Gizmos.color = Color.green;
                            break;
                        default:
                            break;
                    }
                    Gizmos.DrawCube(bound.center, bound.size);
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireCube(bound.center, bound.size);

                    index += 1;
                }
            }
        }
    }
    public void DrawSphere(Vector3 position, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(position, _sphereSize);
    }
    #endregion
#endif

    #region Debug
#if UNITY_EDITOR
    [Button("Toggle Taged Visibility")]
    private void ToggleTageVisibility()
    {
        _tagedVisible = !_tagedVisible;
        if (_tagedVisible)
        {
            SceneVisibilityManager.instance.Show(GameObject.FindGameObjectsWithTag(_debugTag), true);
        }
        else
        {
            SceneVisibilityManager.instance.Hide(GameObject.FindGameObjectsWithTag(_debugTag), true);
        }

    }
    [Button("Destroy Taged")]
    private void DestroyTaged()
    {
        foreach (var item in GameObject.FindGameObjectsWithTag(_debugTag))
        {
            DestroyImmediate(item);
        }
    }
    private void AddOtherPoint(Vector3 position, string name = "unknown")
    {
        _otherDebugPoints.Add(new OtherDebugPoints(position, name));
    }
#endif
    [Button("Clear")]
    private void ClearValues()
    {
        _verticalState = VerticalState.None;
        _persistentHeightToAddNextFloor_ForPoints.Clear();
        _persistentHeightElementsData.Clear();
        _persistentGoDownData.Clear();
        _points.Clear();

#if UNITY_EDITOR
        _canDraw = true;
        _debugPoints.Clear();
        DestroyTaged();
        _otherDebugPoints.Clear();
        _debugindex_AddPoint_fct = 0;
#endif

    }

#endregion
}