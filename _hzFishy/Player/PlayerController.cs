using Cinemachine;
using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum Actions : byte
{
    /// <summary> No action available </summary>
    None,
    /// <summary> player can jump, perform a "canJump" check before </summary>
    SimpleJump,
    /// <summary> player can jump, bypass ground check </summary>
    AirJump,
    /// <summary> slide on ground, half height </summary>
    Slide,
    /// <summary> slide on box </summary>
    Vault,
    /// <summary> climb the ladder/rope </summary>
    Ladder,
    /// <summary> attach to zipline </summary>
    Zipline,
    RopeSwing,
    Panel,
    WallRun
}

public class PlayerController : MonoBehaviour, IPlayerInteractions
{
    #region Inspector variables

    //public bool Debug_disableFixed = false;

    #region Speeds
    [Header("Speeds")]
    [Tooltip("Forward speed")][SerializeField] private float _forwardSpeed = 20f;
    [Tooltip("Down speed")][SerializeField] private float _downSpeed = 15f;
    #endregion

    #region SpeedIncreasing
    [Header("Speed Increasing")]
    [Tooltip("MAX Forward speed")] [SerializeField] private float _maxForwardSpeed = 30f;
    [Tooltip("Interval")] [SerializeField] private float _addInterval = 5f;
    #endregion

    #region Jump
    [Header("Jump")]
    [Tooltip("The height of the jump")][SerializeField] private float _jumpHeight = 2.5f;
    [Tooltip("The max distance authorized when performing ground check")][SerializeField] private float _groundDistanceTolerance = 0.2f;
    [Tooltip("the origin point of the check (on the lower body)")] [SerializeField] private Transform _groundChecker;
    [Tooltip("The floor")][SerializeField] private LayerMask _groundLayerMask;
    [Tooltip("prefab UI pushing down")] [SerializeField] private GameObject _pushingDownPrefab;
    [SerializeField] private GameObject _cancelJumpRollPrefab;
    #endregion

    #region Other
    [Header("Other")]
    [Tooltip("Hit material")][SerializeField] private Material _hitMaterial;
    [SerializeField] private TrailRenderer _trailr;
    [SerializeField] private SkinnedMeshRenderer _meshRendererMainSurface;

    public MusicPlayer musicPlayer;

    #endregion

    #region Elements

    #region Global

    public GameObject YBot;

    #endregion

    #region Elements : Ladder
    [Header("Ladder")]
    [Tooltip("Ladder Climbing speed")] [SerializeField] private float _ladderClimbingSpeed = 2.5f;
    private float _initLadderClimbingSpeed;
    [Tooltip("where the raycast when climbing starts gameobject")] [SerializeField] private Transform _ladderClimbingRaycastPoint;
    [Tooltip("the raycast length")] [SerializeField] private float _LadderRaycastLength = 1f;
    [Tooltip("Ladder")] [SerializeField] private LayerMask _ladderLayerMask;
    #endregion
    
    #region Elements : Zipline
    [Header("Zipline")]
    [Tooltip("Zipline Move speed")] [SerializeField] private float _ziplineMoveSpeed = 2.5f;
    [Tooltip("where the raycast when ziplining starts gameobject")] [SerializeField] private Transform _ziplineRaycastPoint;
    [Tooltip("the raycast length")] [SerializeField] private float _ziplineRaycastLength = 1f;
    [Tooltip("Zipline")] [SerializeField] private LayerMask _ziplineLayerMask;
    #endregion

    #region Elements : RopeSwing
    [Header("RopeSwing")]
    [Tooltip("the force to add when player press space on rope")] [SerializeField] private float _ropeSwingPushForce = 100;
    #endregion

    #region Elements : Panel
    [Header("Panel")]
    [SerializeField]  private float _actionPanelSpeed = 10f;
    #endregion

    #region Elements : WallRun
    [Header("WallRun")]
    [Tooltip("WallRun Move speed")][SerializeField] private float _wallRunMoveSpeed = 5f;
    [Tooltip("where the raycast when WallRun starts gameobject")][SerializeField] private Transform _wallRunRaycastPoint;
    [Tooltip("the raycast length")][SerializeField] private float _wallRunRaycastLength = 1f;
    [Tooltip("WallRun")][SerializeField] private LayerMask _wallRunLayerMask;
    #endregion

    #endregion

    #region UI
    [Header("UI")]
    [SerializeField] private GameObject _prefabUIPause;
    [SerializeField] private GameObject _tpDeadPrefab;
    #endregion

    #region Camera
    [Header("Camera")]
    public CinemachineVirtualCamera MainCamera;
    private CinemachineBasicMultiChannelPerlin _noiseMainCamera;
    public CinemachineVirtualCamera SlideCamera;
    private CinemachineBasicMultiChannelPerlin _noiseSlidingCamera;
    public NoisesProfile NoisesprofileBind;
    [System.Serializable] public class NoisesProfile : SerializableDictionaryBase<CameraShakes, NoiseSettings> { }

    #endregion

    #endregion


    #region Variables 

    #region Events
    public static event Action PlayerStartPlaying;
    public static event Action PlayerStopPlaying;
    #endregion

    #region Components
    public Rigidbody _rigidbody { get; private set; }

    private Animator _animator;
    private CapsuleCollider _capsuleCollider;
    private CameraSwitcher _cameraSwitcher;
    #endregion

    #region Global & utility stuff
    ///<summary> heights used for calcultions </summary>
    private float _playerHeight; 
    /// <summary> is player on the ground </summary>
    private bool _isGrounded = true;
    [HideInInspector] public float groundDistance;
    /// <summary> do we move the player forward </summary>S
    private bool _moveForward = true;
    /// <summary> player inputs (input system) </summary>
    private PlayerInputs playerInputActions;
    /// <summary> initial down speed </summary>
    private float _initdownSpeed;
    private float _initforwardSpeed;
    /// <summary> prevent reset of downspeed to early in jump  </summary>
    private bool _isOverridingDownSpeed = false;

    //private bool _trailChanged = false;
    //private Color _trailDefaultColor;
    private enum SetPositionUnderTypes
    {
        ignore,
        use,
        custom
    }
    private float _timePassed = 0f;

    private bool _pushingDownCamera = false;

    private ElementStaticCamera _currentElementStaticCamera;

    private bool _inMenu = true;

    public float forwardSpeed { get { return _forwardSpeed; } }
    public bool isMovingForward { get { return _moveForward; } }
    public bool isInMenu { get { return _inMenu; } }

    [HideInInspector] public bool InPause = false;
    [HideInInspector] public GameObject CurrentUIPause;

    [HideInInspector] public bool InTutoMode = false;
    [HideInInspector] public bool InTutoLevel = false;
    [HideInInspector] public Vector3 CurrentCheckpoint;
    [HideInInspector] public GameObject CurrentTutoHandler;
    [HideInInspector] public GameObject CurrentUITutoHandler;

    [HideInInspector] public ElementsType KilledByElemType;

    #endregion

    #region Actions

    #region Global
    /// <summary> the current element in which the player is in </summary>
    private ElementsType _currentElementType = ElementsType.None;
    /// <summary> the current authorized action (performed with Space) </summary>
    [Header("Debug")] [SerializeField] private Actions _currentAction = Actions.None;
    /// <summary> the TMP ref  </summary>
    private Elements_SpeedText _currentElementSpeedText;
    #endregion

    #region Jump
    private bool _rollingAfterJump = false;
    #endregion

    #region Slide
    /// <summary> is player sliding </summary>
    private bool _isSliding = false;
    #endregion

    #region Vault
    /// <summary> is player vaulting </summary>
    private bool _isVaulting = false;
    #endregion

    #region Ladder climbing
    /// <summary> is player climbing ladder </summary>
    private bool _isLadderClimbing = false;
    /// <summary> "once" logic </summary>
    private bool _onceCoroutinePostClimbingLadder = false;
    private bool _onceCoroutineClimbingLadderEnded = false;
    #endregion

    #region Zipline
    /// <summary> the current reference of the zipline </summary>
    private Element_Zipline_Custom _currentZiplineCustomScript;
    /// <summary> is the player on the zipline ? </summary>
    private bool _isZipline = false;
    private bool _ziplineOverrideStart = false;
    /// <summary> "once" logic </summary>
    private bool _onceCoroutinePostZipline = false;
    private float _InitZiplineMoveSpeed;
    private bool _inZiplineArea = false;
    private bool _ziplineAttached = false;
    private GameObject _ziplineGameobject;
    private GameObject _currentZiplineHandle;
    #endregion

    #region Rope swing
    /// <summary> the current reference of the zipline </summary>
    private Element_RopeSwing_Custom _currentRopeSwingCustomScript;
    /// <summary> is the player on the zipline ? </summary>
    private bool _isRopeSwing = false;
    private HingeJoint _addedHingeJoint;
    private bool _canRopeSwing = true;
    #endregion

    #region Panel
    private bool _actionPanelIsLeft;
    #endregion

    #region WallRun

    private bool _isWallRunning = false;
    private bool _wallRunOverrideStart;
    private float _wallRunInitSpeed;
    private GameObject _currentWallRunSpeedText;
    private bool _isWallRunLeft;
    private bool _isWallRunAttached = false;
    #endregion

    #endregion

    #region Camera

    public enum CameraShakes
    {
        Wobble,
        Shake
    }

    #endregion

    #endregion


    public bool BlockGeneration = false;

    private void Awake()
    {
        playerInputActions = new PlayerInputs();
        _rigidbody = GetComponent<Rigidbody>(); // get the player rigidboy
        _initdownSpeed = _downSpeed; // initialize down speed
        _animator = GetComponentInChildren<Animator>(); // get animator of Adam
        _capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        _cameraSwitcher = GetComponent<CameraSwitcher>();

        //_trailDefaultColor = _trailr.startColor;
        _playerHeight = Mathf.Ceil(_ziplineRaycastPoint.position.y - _groundChecker.position.y);
          
        _moveForward = _inMenu;
        _initLadderClimbingSpeed = _ladderClimbingSpeed;
        _initforwardSpeed = _forwardSpeed;
        _InitZiplineMoveSpeed = _ziplineMoveSpeed;
        _wallRunInitSpeed = _wallRunMoveSpeed;

        playerInputActions.Player.Debug_RestartScene.performed += Debug_RestartScene;
        playerInputActions.Player.Debug_RestartScene.Enable();
        playerInputActions.Player.Debug_ToggleBackgroundGeneration.performed += Debug_ToggleBackgroundGeneration;
        playerInputActions.Player.Debug_ToggleBackgroundGeneration.Enable();

        _noiseMainCamera = MainCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _noiseSlidingCamera = SlideCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        UpdateCameraMainAmplitude(2);
    }
    private void OnDisable()
    {
        playerInputActions.Player.SpaceAction_Tap.Disable(); // Disable "space tap"
        playerInputActions.Player.SpaceAction_DoubleTap.Disable();
        playerInputActions.Player.SpaceAction_Hold.Disable();
        playerInputActions.Player.Debug_RestartScene.Disable();
        playerInputActions.Player.Debug_ToggleBackgroundGeneration.Disable();
        playerInputActions.Player.Extra_Pause.Disable();
    }


    void Update()
    {
        if (_inMenu || InPause)
        {
            return;
        }
        _isGrounded = Physics.CheckSphere(_groundChecker.position, _groundDistanceTolerance, _groundLayerMask, QueryTriggerInteraction.Ignore); // perform ground check
        RaycastHit hit;
        _animator.SetBool("Falling", _isWallRunning ? false : !(Physics.CheckSphere(_groundChecker.position, 0.3f, _groundLayerMask, QueryTriggerInteraction.Ignore)));
        Physics.Raycast(_groundChecker.position, -transform.up, out hit, 100, _groundLayerMask);
        groundDistance = hit.distance;
        _timePassed += Time.deltaTime;

        /*float[] spectrum = new float[256];
        musicPlayer.GetComponent<AudioSource>().GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        for (int i = 1; i < spectrum.Length - 1; i++)
        {
            if (spectrum[i] > 0.1)
            {
                Debug.Log(spectrum[i]);
            }
            //gameObject.GetComponent().threshold = highestBass;
        }*/
    }
    void FixedUpdate()
    {
        if (_inMenu || InPause)
        {
            return;
        }
        switch (_currentAction)
        {
            #region Actions.Ladder
            case Actions.Ladder:
                bool ladderActualState = ClimbLadder();
                if (ladderActualState != _isLadderClimbing)
                {
                    if (!ladderActualState)
                    {
                        if (!_onceCoroutinePostClimbingLadder)
                        {
                            StartCoroutine(CoroutinePostClimbingLadder());
                        }
                    }
                    UpdateLadderClimbingState(ladderActualState);
                }
                if (ladderActualState)
                {
                    _rigidbody.MovePosition(_rigidbody.position + new Vector3(0, 1, 0) * Time.fixedDeltaTime * _ladderClimbingSpeed);
                }
                break;
            #endregion
            #region Actions.Zipline
            case Actions.Zipline:
                if (_ziplineOverrideStart || !_ziplineAttached)
                {
                    break;
                }
                bool ziplineActualState = MoveZipline();
                if (ziplineActualState != _isZipline)
                {
                    if (!ziplineActualState)
                    {
                        if (!_onceCoroutinePostZipline)
                        {
                            StartCoroutine(CoroutinePostZipline());
                        }
                    }
                    else
                    {
                        StopCoroutine(CoroutinePostZipline());
                        _onceCoroutinePostZipline = false;
                    }
                    UpdateZiplineState(ziplineActualState);
                }
                if (ziplineActualState)
                {
                    _rigidbody.MovePosition(_rigidbody.position + new Vector3(1, 0, 0) * Time.fixedDeltaTime * _ziplineMoveSpeed);
                    if (_currentZiplineHandle != null)
                    {
                        _currentZiplineHandle.transform.position = new Vector3(YBot.transform.position.x+0.5f, _currentZiplineHandle.transform.position.y, _currentZiplineHandle.transform.position.z);
                    }
                }
                break;
            #endregion
            #region Actions.Panel
            case Actions.Panel:
                if (_actionPanelIsLeft ? (_rigidbody.transform.position.z < 1.5f) : (_rigidbody.transform.position.z > -1.5f))
                {
                    PanelMove(_actionPanelIsLeft);
                }
                break;
            #endregion
            #region WallRun
            case Actions.WallRun:
                if (_wallRunOverrideStart)
                {
                    break;
                }
                _rigidbody.velocity = Vector3.zero;

                bool onWall = MoveWallRunChecker();

                if (onWall)
                {
                    _rigidbody.MovePosition(_rigidbody.position + new Vector3(1, 0, 0) * Time.fixedDeltaTime * _wallRunMoveSpeed);
                }
                else
                {
                    if (_currentElementSpeedText != null)
                    {
                        UpdateWallRunState(false);
                    }
                }
                break;
            #endregion
            default:
                break;
        }

        if (_isVaulting || _isZipline || _isWallRunning)
        {
            _downSpeed = 0;
        }
        else if (!_isGrounded && (_rigidbody.velocity.y < 0 || !_isOverridingDownSpeed) && _currentElementType != ElementsType.AirBox && !_isLadderClimbing && !_isZipline && !_isRopeSwing && !_isWallRunning)
        {
            _downSpeed = _initdownSpeed;
        }
        else if (_isGrounded)
        {
            _downSpeed = 0;
        }
        if (_isGrounded && !_isOverridingDownSpeed)
        {
            _cameraSwitcher.UpdateBodyYOffset();
        }
        if (_moveForward && !_isLadderClimbing && !_isZipline && !_isRopeSwing && !_isWallRunning)
        {
            //Debug.Log("Moving forward");
            _rigidbody.MovePosition(_rigidbody.position + new Vector3(1, 0, 0) * Time.fixedDeltaTime * _forwardSpeed);
        }
        if (!_isGrounded && !_isLadderClimbing && !_isZipline && !_isRopeSwing && !_isWallRunning)
        {
            //Debug.Log("========== going down");
            _rigidbody.MovePosition(_rigidbody.position + new Vector3(0, -1, 0) * Time.fixedDeltaTime * _downSpeed);
        }
        if (_isGrounded && _pushingDownCamera)
        {
            _cameraSwitcher.SwitchCamera(CameraSwitcherCameras.Main);
            _pushingDownCamera = false;
        }
        if (!_isGrounded && _isSliding)
        {
            UpdateSlideState(false);
        }
    }


    #region Core
    private IEnumerator CoroutineCoreIncreaseSpeed()
    {
        if (_inMenu || InPause)
        {
            yield return null;
        }
        while (_forwardSpeed < _maxForwardSpeed)
        {
            _forwardSpeed += 0.1f;
            yield return new WaitForSeconds(_addInterval);
        }
    }

    #endregion


    #region Utility methods

    private void UpdateCameraMainAmplitude(float value)
    {
        _noiseMainCamera.m_AmplitudeGain = value;
    }
    private IEnumerator DelayedUpdateCameraMainAmplitude(float instantValue, float delayedValue, float delay)
    {
        _noiseMainCamera.m_AmplitudeGain = instantValue;
        yield return new WaitForSeconds(delay);
        _noiseMainCamera.m_AmplitudeGain = delayedValue;

    }

    private void UpdateCameraMainFrequency(float value = 0.5f)
    {
        _noiseMainCamera.m_FrequencyGain = value;
    }
    private IEnumerator DelayedUpdateCameraFrequency(float instantValue, float delayedValue, float delay)
    {
        _noiseMainCamera.m_FrequencyGain = instantValue;
        yield return new WaitForSeconds(delay);
        _noiseMainCamera.m_FrequencyGain = delayedValue;

    }

    private void UpdateCameraMainProfile(CameraShakes value)
    {
        _noiseMainCamera.m_NoiseProfile = NoisesprofileBind[value];
    }
    private IEnumerator DelayedUpdateCameraProfile(CameraShakes instantValue, CameraShakes delayedValue, float delay)
    {
        _noiseMainCamera.m_NoiseProfile = NoisesprofileBind[instantValue];
        yield return new WaitForSeconds(delay);
        _noiseMainCamera.m_NoiseProfile = NoisesprofileBind[delayedValue];

    }


    private void UpdateCameraSlideAmplitude(float value)
    {
        _noiseSlidingCamera.m_AmplitudeGain = value;
    }
    private IEnumerator DelayedUpdateCameraSlideAmplitude(float instantValue, float delayedValue, float delay)
    {
        _noiseSlidingCamera.m_AmplitudeGain = instantValue;
        yield return new WaitForSeconds(delay);
        _noiseSlidingCamera.m_AmplitudeGain = delayedValue;

    }

    private void UpdateCameraSlideFrequency(float value = 0.5f)
    {
        _noiseSlidingCamera.m_FrequencyGain = value;
    }
    private IEnumerator DelayedUpdateCameraSlideFrequency(float instantValue, float delayedValue, float delay)
    {
        _noiseSlidingCamera.m_FrequencyGain = instantValue;
        yield return new WaitForSeconds(delay);
        _noiseSlidingCamera.m_FrequencyGain = delayedValue;

    }

    private void UpdateCameraSlideProfile(CameraShakes value)
    {
        _noiseSlidingCamera.m_NoiseProfile = NoisesprofileBind[value];
    }
    private IEnumerator DelayedUpdateCameraSlideProfile(CameraShakes instantValue, CameraShakes delayedValue, float delay)
    {
        _noiseSlidingCamera.m_NoiseProfile = NoisesprofileBind[instantValue];
        yield return new WaitForSeconds(delay);
        _noiseSlidingCamera.m_NoiseProfile = NoisesprofileBind[delayedValue];

    }


    private void Debug_RestartScene(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("Game");
    }
    /// <summary> set player pos to under something</summary>
    private void SetPositionUnder(Transform objectTransform, SetPositionUnderTypes type = SetPositionUnderTypes.use, float customSubtractedHeight = 0)
    {
        float used = 0;
        switch (type)
        {
            case SetPositionUnderTypes.use:
                used = _playerHeight;
                break;
            case SetPositionUnderTypes.custom:
                used = customSubtractedHeight;
                break;
            default:
                break;
        }
        transform.position = new Vector3(objectTransform.position.x, (objectTransform.position.y - used), objectTransform.position.z);
    }
    public void UpdateMenuStatus(bool state)
    {
        _animator.SetBool("inMenu", state);
        _inMenu = state;
        if (!_inMenu)
        {
            if (PlayerStartPlaying != null)
            {
                PlayerStartPlaying();
            }
            musicPlayer.SetVolume(0.3f);
            //StartCoroutine(musicPlayer.FadeVolume(MusicPlayer.FadeVolumeGoal.Normal, 1));
            UpdateCameraMainAmplitude(0.5f);
            StartCoroutine(CoroutineCoreIncreaseSpeed());
            playerInputActions.Player.SpaceAction_Tap.performed += SpaceAction_Tap; // bind function to "Space tap "
            playerInputActions.Player.SpaceAction_Tap.Enable(); // Enable "space tap"
            playerInputActions.Player.SpaceAction_DoubleTap.performed += SpaceAction_DoubleTap;
            playerInputActions.Player.SpaceAction_DoubleTap.Enable();


            playerInputActions.Player.SpaceAction_Hold.performed += SpaceAction_HoldPerformed;
            playerInputActions.Player.SpaceAction_Hold.Enable();

            playerInputActions.Player.SpaceAction_Hold.canceled += SpaceAction_HoldCancel;
            playerInputActions.Player.SpaceAction_Hold.Enable();

            playerInputActions.Player.Extra_Pause.performed += Extra_Pause;
            playerInputActions.Player.Extra_Pause.Enable();
        }
    }
    
    private IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }

    private void TutoHandlerCheck()
    {
        if (InTutoMode)
        {
            if (CurrentTutoHandler != null)
            {
                StartCoroutine(CurrentUITutoHandler.GetComponentInChildren<TutoBaseRemove>().StartDestroy());
            }
            InTutoMode = false;
            StartCoroutine(ChangeTimeScale(1, 1.5f));
        }
    }
    public static IEnumerator ChangeTimeScale(float target, float duration = 0.5f)
    {
        float time = 0;
        float current = Time.timeScale;
        while (time < duration)
        {
            Time.timeScale = Mathf.Lerp(current, target, Mathf.SmoothStep(0.00f, 1.00f,time / duration));
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = target;
    }

    #endregion


    #region Actions

    #region Input
    /// <summary> Binded function to "Space tap" </summary>
    private void SpaceAction_Tap(InputAction.CallbackContext context)
    {
        if (InPause) { return; }
        TutoHandlerCheck();
        //Debug.Log("SpaceAction_Tap");
        switch (_currentAction)
        {
            #region None
            case Actions.None:
                if (_isOverridingDownSpeed)
                {
                    PushDown();
                }
                else if (_rollingAfterJump)
                {
                    Instantiate(_cancelJumpRollPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    _animator.SetBool("CancelJumpRoll", true);
                    _rollingAfterJump = false;
                    _animator.SetBool("CancelJumpRoll", false);
                }
                else if (!_isSliding)
                {
                    Jump(_jumpHeight);
                }
                break;
            #endregion
            #region SimpleJump
            case Actions.SimpleJump:
                Jump(_jumpHeight);
                break;
            #endregion
            #region AirJump
            case Actions.AirJump:
                Jump(_jumpHeight*1.5f, false,true);
                break;
            #endregion
            #region Vault
            case Actions.Vault:
                if (!_isVaulting)
                {
                    UpdateVaultState(true);
                    _rigidbody.AddForce(Vector3.up * Mathf.Sqrt(0.5f * -2f * Physics.gravity.y), ForceMode.VelocityChange);
                }
                break;
            #endregion
            #region Ladder
            case Actions.Ladder:
                _ladderClimbingSpeed += 0.75f;
                _animator.speed = _ladderClimbingSpeed/5;
                UpdateElementSpeedText(_ladderClimbingSpeed);
                break;
            #endregion
            #region Zipline
            case Actions.Zipline:
                if (_ziplineAttached)
                {
                    _ziplineMoveSpeed += 1f;
                    UpdateElementSpeedText(_ziplineMoveSpeed);
                    _animator.SetFloat("Zipline Speed", _ziplineMoveSpeed);
                }
                else if (_inZiplineArea && _isGrounded)
                {
                    ZiplineInit();
                }
                else
                {
                    PushDown();
                }

                break;
            #endregion
            #region RopeSwing
            case Actions.RopeSwing:
                if (_currentRopeSwingCustomScript.rigibody.velocity.x > -0.1)
                {
                    RopeSwing();
                }
                break;
            #endregion
            #region Panel
            case Actions.Panel:
                _actionPanelIsLeft = !_actionPanelIsLeft;
                PanelMove(_actionPanelIsLeft);
                break;
            #endregion
            #region WallRun
            case Actions.WallRun:
                if (!_isWallRunning)
                {
                    InitWallRun(_currentWallRunSpeedText);
                    UpdateWallRunState(true);
                    break;
                }
                _wallRunMoveSpeed += 1f;
                UpdateElementSpeedText(_wallRunMoveSpeed);
                break;
            #endregion
            default:
                break;
        }
        Debug.Log("Requested" + _currentAction);
    }
    private void SpaceAction_DoubleTap(InputAction.CallbackContext context)
    {
        if (InPause) { return; }
        switch (_currentAction)
        {
            case Actions.RopeSwing:
                DetachPlayerRopeSwing();
                UpdateRopeSwingState(false);
                break;
            default:
                break;
        }
    }
    private void SpaceAction_HoldPerformed(InputAction.CallbackContext context)
    {
        if (InPause) { return; }
        //Debug.Log("SpaceAction_HoldPerformed");
        switch (_currentAction)
        {
            #region Actions.Slide
            case Actions.Slide or Actions.None:
                if (_isGrounded)
                {
                    UpdateSlideState(true);
                }
                break;
            #endregion
            default:
                break;
        }
        TutoHandlerCheck();
    }
    private void SpaceAction_HoldCancel(InputAction.CallbackContext context)
    {
        if (InPause) { return; }
        //Debug.Log("SpaceAction_HoldCancel");
        switch (_currentAction)
        {
            #region Actions.Slide
            case Actions.Slide or Actions.None:
                if (_isSliding)
                {
                    UpdateSlideState(false);
                }
                break;
            #endregion
            default:
                break;
        }
    }
    
    private void Extra_Pause (InputAction.CallbackContext context)
    {
        Debug.Log("Requested Pause");
        ToggleOptionMenu();
    }

    private void Debug_ToggleBackgroundGeneration(InputAction.CallbackContext context)
    {
        BlockGeneration = !BlockGeneration;
    }

    private void ToggleOptionMenu()
    {
        if (_inMenu)
        {
            Debug.Log("Canceled Pause");
            return;
        }
        if (InPause)
        {
            if (isMovingForward)
            {
                //StartCoroutine(musicPlayer.FadeVolume(MusicPlayer.FadeVolumeGoal.Normal, 1));
                Destroy(CurrentUIPause);
                Time.timeScale = 1;
                musicPlayer.SetVolume(0.3f);
                InPause = false;
            }
        }
        else
        {
            musicPlayer.SetVolume(0.1f);
            //StartCoroutine(musicPlayer.FadeVolume(MusicPlayer.FadeVolumeGoal.Low, 1));
            CurrentUIPause = Instantiate(_prefabUIPause, Vector3.zero, Quaternion.identity);
            Time.timeScale = 0;
            InPause = true;
        }
    }

    #endregion

    #region IPlayerInteractions methods
    public void StopPlayer(ElementsType elementType, GameObject gameobject) // IPlayerInteractions
    {
        if (InTutoLevel && CurrentCheckpoint != null)
        {
            Instantiate(_tpDeadPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            transform.position = CurrentCheckpoint + new Vector3(0,2,0);
            return;
        }
        if (PlayerStopPlaying != null)
        {
            PlayerStopPlaying();
        }
        Debug.Log("Player hit !");
        _animator.enabled = false;
        _moveForward = false; //Stop Moving/Translating
        _rigidbody.velocity = Vector3.zero; // Stop any velocity
        _rigidbody.angularVelocity = Vector3.zero; //Stop rotating
        _meshRendererMainSurface.material = _hitMaterial; // change material
        _rigidbody.useGravity = false;
        if (_isSliding)
        {
            UpdateSlideState(false);
        }
        KilledByElemType = elementType;
        ToggleOptionMenu();
    }

    public void UnlockAction(ElementsType elementType, GameObject gameobject) //IPlayerInteractions
    {
        _currentElementType = elementType;
        switch (elementType)
        {
            #region FloorBox
            case ElementsType.FloorBox:
                _currentAction = Actions.SimpleJump;
                break;
            #endregion
            #region AirBox
            case ElementsType.AirBox:
                _currentAction = Actions.AirJump;
                break;
            #endregion
            #region DownSlideBox
            case ElementsType.DownSlideBox:
                _currentAction = Actions.Slide;
                break;
            #endregion
            #region UpSlideBox
            case ElementsType.UpSlideBox:
                _currentAction = Actions.Vault;
                //_currentElementStaticCamera = gameobject.transform.parent.gameObject.GetComponentInChildren<ElementStaticCamera>();
                //_currentElementStaticCamera.UpdateCameraStatus(true);
                //_cameraSwitcher.SwitchCamera(CameraSwitcherCameras.Vault); placer la camera vue du dessus
                break;
            #endregion
            #region Ladder
            case ElementsType.Ladder:
                _currentAction = Actions.Ladder;
                SetElementSpeedText(gameobject);
                _cameraSwitcher.SwitchCamera(CameraSwitcherCameras.Ladder);
                UpdateElementSpeedText(_ladderClimbingSpeed);
                break;
            #endregion
            #region Zipline
            case ElementsType.Zipline:
                _currentAction = Actions.Zipline;
                _inZiplineArea = true;
                _ziplineGameobject = gameobject;
                break;
            #endregion
            #region RopeSwing
            case ElementsType.RopeSwing:
                _currentAction = Actions.RopeSwing;
                _currentRopeSwingCustomScript = gameobject.transform.parent.gameObject.GetComponent<Element_RopeSwing_Custom>();
                AttachPlayerRopeSwing();
                UpdateRopeSwingState(true);
                break;
            #endregion
            #region Panel
            case ElementsType.Panel:
                _currentAction = Actions.Panel;
                _actionPanelIsLeft = UnityEngine.Random.value > 0.5f;
                transform.Translate(0,0,(_actionPanelIsLeft ? 1.5f : -1.5f));
                break;
            #endregion
            #region WallRUn
            case ElementsType.WallRun:
                if (_isWallRunning) { return; }
                _currentWallRunSpeedText = gameobject;
                _currentAction = Actions.WallRun;
                break;
            #endregion
            default:
                break;
        }
        Debug.Log("Unlocked action : "+ _currentAction +" by : " + elementType);
    }

    public void LockAction(ElementsType elementType, GameObject gameobject) //IPlayerInteractions
    {
        switch (_currentAction)
        {
            case Actions.Vault:
                Debug.Log("didnt reset '_currentAction' because of 'Actions.Vault', it is handled in 'CoroutinePostVault'");
                break;
            case Actions.Ladder:
                Debug.Log("didnt reset '_currentAction' because of 'Actions.Ladder', it is handled in 'CoroutinePostClimbingLadder'");
                return; // we dont reset "_currentAction" because we handle it in "CoroutinePostClimbingLadder"
            case Actions.Zipline:
                _inZiplineArea = false;
                Debug.Log("didnt reset '_currentAction' because of 'Actions.Zipline', it is handled in 'CoroutinePostZipline'");
                return; // we dont reset "_currentAction" because we handle it in "CoroutinePostZipline"
            case Actions.RopeSwing:
                Debug.Log("didnt reset '_currentAction' because of 'Actions.RopeSwing', it is handled in 'DetachPlayerRopeSwing'");
                return; // we dont reset "_currentAction" because we handle it in "DetachPlayerRopeSwing"
            case Actions.Panel:
                transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                break;
            #region WallRun
            case Actions.WallRun:
                Debug.Log("didnt reset '_currentAction' because of 'Actions.WallRun', it is handled in 'notdefined'");
                return;
            #endregion
            default:
                break;
        }
        _currentAction = Actions.None;
        Debug.Log("currentAction set to None, left : " + elementType);
    }
    #endregion

    #region Speed Text
    private Elements_SpeedText SetElementSpeedText(GameObject gameobject)
    {
        _currentElementSpeedText = gameobject.GetComponentInParent<Elements_ConnectSpeedtext>()._speedText;
        _currentElementSpeedText.ChangeAndPlayAnimation(true);
        return _currentElementSpeedText;
    }
    private void UpdateElementSpeedText(float speed)
    {
        _currentElementSpeedText.UpdateText(speed);
    }
    private IEnumerator DisableElementSpeedText()
    {
        _currentElementSpeedText.ChangeAndPlayAnimation(false);
        yield return new WaitForSeconds(_currentElementSpeedText.Animation.clip.length);
        _currentElementSpeedText.gameObject.SetActive(false);
    }
    #endregion


    #region Jump
    private void PushDown()
    {
        _downSpeed = _initdownSpeed;
        Instantiate(_pushingDownPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        _cameraSwitcher.SwitchCamera(CameraSwitcherCameras.PushingDown);
        _pushingDownCamera = true;
        /*
        StartCoroutine(DelayedUpdateCameraMainAmplitude(2, 0.5f, 0.1f));
        StartCoroutine(DelayedUpdateCameraFrequency(4, 0.5f, 0.1f));
        StartCoroutine(DelayedUpdateCameraFrequency(4, 0.5f, 0.1f));
        StartCoroutine(DelayedUpdateCameraProfile(CameraShakes.Shake, CameraShakes.Wobble, 0.1f));
        */
    }

    private bool CanJump()
    {
        if (_isGrounded && !_isSliding)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void Jump(float height = 2f, bool checkbefore = true, bool isAir = false)
    {
        //SceneManager.GetSceneByName("Tuto").GetRootGameObjects()[0].SetActive(false);
        if (checkbefore)
        {
            if (!CanJump())
            {
                return;
            }
        }
        _animator.SetBool("Jump", true);
        _downSpeed = 0;
        _isOverridingDownSpeed = true;
        _rigidbody.AddForce(Vector3.up * Mathf.Sqrt(height * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        StartCoroutine(CoroutineResetOverrideDownSpeed(isAir ? 0.6f : 0.6f));

        _cameraSwitcher.UpdateBodyYOffset(3f);

        /*StartCoroutine(DelayedUpdateCameraMainAmplitude(2, 0.5f, 0.1f));
        StartCoroutine(DelayedUpdateCameraFrequency(4,0.5f, 0.1f));
        StartCoroutine(DelayedUpdateCameraFrequency(4, 0.5f, 0.1f));
        StartCoroutine(DelayedUpdateCameraProfile(CameraShakes.Shake,CameraShakes.Wobble, 0.1f));*/

        Debug.Log("Performed jump");
    }
    private IEnumerator CoroutineResetOverrideDownSpeed(float time = 0.5f)
    {
        //(10/_forwardSpeed)
        yield return new WaitForSeconds(time);
        _isOverridingDownSpeed = false;
        if (_currentElementType != ElementsType.AirBox)
        {
            _animator.SetBool("Jump", false);
            _downSpeed = _initdownSpeed;
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
            _animator.SetBool("Jump", false);
            _downSpeed = _initdownSpeed;
        }
    }
    
    public void UpdateRolling(bool newState)
    {
        _rollingAfterJump = newState;
    }

    #endregion

    #region Slide
    private void UpdateSlideState(bool state)
    {
        Debug.Log("Update slide state: " + state);
        if (state)
        {
            UpdateCameraSlideAmplitude(0.8f);
            UpdateCameraSlideFrequency(0.5f);
            UpdateCameraSlideProfile(CameraShakes.Shake);

            _cameraSwitcher.SwitchCamera(CameraSwitcherCameras.Slide);
        }
        else
        {
            UpdateCameraSlideAmplitude(0);
            UpdateCameraSlideFrequency(0);
            UpdateCameraSlideProfile(CameraShakes.Shake);
            _cameraSwitcher.SwitchCamera(CameraSwitcherCameras.Main);
        }
        Debug.Log("_isSliding: " + _isSliding);
        _isSliding = state;
        _animator.SetBool("Slide", state);
    }
    #endregion
    
    #region Vault
    private void UpdateVaultState(bool state)
    {
        if (state)
        {
            StartCoroutine(CoroutinePostVault());
            //_currentElementStaticCamera.UpdateCameraStatus(false);
            //_cameraSwitcher.SwitchCamera(CameraSwitcherCameras.Main);
        }
        _isVaulting = state;
        //_rigidbody.useGravity = !state;
        _animator.SetBool("Vault", state);
    }

    private IEnumerator CoroutinePostVault()
    {
        yield return new WaitForSeconds(1.5f);
        UpdateVaultState(false);
    }

    #endregion

    #region ladderClimbing
    private void UpdateLadderClimbingState(bool state)
    {
        if (state)
        {
            _rigidbody.useGravity = false;
            _isLadderClimbing = true;
            
        }
        else
        {
            if (!_onceCoroutineClimbingLadderEnded)
            {
                StartCoroutine(LerpPosition(transform.position + new Vector3(0, 2f, 0), 1.1f));
                _onceCoroutineClimbingLadderEnded = true;
            }
        }
        _animator.SetBool("LadderClimbing", state);
    }
    private bool ClimbLadder()
    {
        RaycastHit hit;
        if (Physics.Raycast(_ladderClimbingRaycastPoint.position, transform.TransformDirection(Vector3.right), out hit, _LadderRaycastLength, _ladderLayerMask))
        {
            Debug.DrawRay(_ladderClimbingRaycastPoint.position, transform.TransformDirection(Vector3.right) * hit.distance, Color.yellow, 10);
            return true;
        }
        else
        {
            Debug.DrawRay(_ladderClimbingRaycastPoint.position, transform.TransformDirection(Vector3.right) * _LadderRaycastLength, Color.blue, 10);
            return false;
        }
    }
    private IEnumerator CoroutinePostClimbingLadder()
    {
        _onceCoroutinePostClimbingLadder = true;
        yield return new WaitForSeconds(1.15f / 1.5f);
        _animator.speed = 1;
        //transform.Translate(new Vector3(0, 1, 0) * Time.deltaTime);
        _onceCoroutineClimbingLadderEnded = false;
        _isLadderClimbing = false;
        _rigidbody.useGravity = true;
        _ladderClimbingSpeed = _initLadderClimbingSpeed;
        _currentAction = Actions.None;
        _cameraSwitcher.SwitchCamera(CameraSwitcherCameras.Main);
        StartCoroutine(DisableElementSpeedText());
        _onceCoroutinePostClimbingLadder = false;
    }
    #endregion
    
    #region Zipline
    private void UpdateZiplineState(bool state)
    {
        _isZipline = state;
        _rigidbody.useGravity = !state;
        _animator.SetBool("Zipline", state);
    }
    private bool MoveZipline()
    {
        RaycastHit hit;
        if (Physics.Raycast(_ziplineRaycastPoint.position, transform.TransformDirection(Vector3.up), out hit, _ziplineRaycastLength, _ziplineLayerMask))
        {
            Debug.DrawRay(_ziplineRaycastPoint.position, transform.TransformDirection(Vector3.up) * hit.distance, Color.green, 10);
            return true;
        }
        else
        {
            Debug.DrawRay(_ziplineRaycastPoint.position, transform.TransformDirection(Vector3.up) * _ziplineRaycastLength, Color.red, 10);
            return false;
        }
    }
    
    private void ZiplineInit()
    {
        _ziplineAttached = true;
        _ziplineOverrideStart = true;
        SetElementSpeedText(_ziplineGameobject);
        _ziplineMoveSpeed = _InitZiplineMoveSpeed;
        UpdateElementSpeedText(_ziplineMoveSpeed);
        _animator.SetFloat("Zipline Speed", _ziplineMoveSpeed);
        _currentZiplineCustomScript = _ziplineGameobject.transform.parent.transform.parent.gameObject.GetComponent<Element_Zipline_Custom>();
        _currentZiplineCustomScript.Handle.SetActive(true);
        _currentZiplineHandle = _currentZiplineCustomScript.Handle;
        StartCoroutine(CoroutineZiplineAttach());
        UpdateZiplineState(true);
        _cameraSwitcher.SwitchCamera(CameraSwitcherCameras.Zipline);
    }
    
    private IEnumerator CoroutineZiplineAttach()
    {
        float time = 0;
        float duration = 0.978f;
        float nbLoops = 0f;
        Vector3 start = _rigidbody.position;
        Vector3 goal = new Vector3(_rigidbody.position.x, (_currentZiplineCustomScript.startPoint.position.y - 1.85f), _rigidbody.position.z);
        while (time < duration)
        {
            _rigidbody.position = Vector3.Lerp(start, goal, time / duration) + ((new Vector3(1, 0, 0) * Time.deltaTime * _ziplineMoveSpeed) * nbLoops);
            time += Time.deltaTime;
            nbLoops += 1;
            yield return null;
        }
        _ziplineOverrideStart = false;
    }

    private IEnumerator CoroutinePostZipline()
    {
        _onceCoroutinePostZipline = true;
        yield return new WaitForSeconds(0.05f);
        _currentZiplineCustomScript.Handle.SetActive(false);
        _currentAction = Actions.None;
        _cameraSwitcher.SwitchCamera(CameraSwitcherCameras.Main);
        StartCoroutine(DisableElementSpeedText());
        _onceCoroutinePostZipline = false;
        _ziplineAttached = false;
    }
    #endregion
    
    #region Rope swing
    private void UpdateRopeSwingState(bool state)
    {
        _isRopeSwing = state;
        _capsuleCollider.enabled = !state;
    }
    private void AttachPlayerRopeSwing()
    {
        SetPositionUnder(_currentRopeSwingCustomScript.endRope, SetPositionUnderTypes.custom, 1);
        transform.SetParent(_currentRopeSwingCustomScript.endRope, true);
        _addedHingeJoint = gameObject.AddComponent<HingeJoint>();
        _addedHingeJoint.connectedBody = _currentRopeSwingCustomScript.rigibody;
        _addedHingeJoint.axis = new Vector3(0, 0, 1);
        JointLimits limits = _addedHingeJoint.limits;
        limits.min = -80;
        limits.max = 80;
        _addedHingeJoint.useLimits = true;
    }
    private void RopeSwing()
    {
        if (!_canRopeSwing)
        {
            return;
        }
        _rigidbody.AddForce(transform.right * _ropeSwingPushForce * Time.deltaTime, ForceMode.VelocityChange);
        StartCoroutine(RopeSwingStartCooldown());
    }
    public IEnumerator RopeSwingStartCooldown(float duration = 3)
    {
        _canRopeSwing = false;

        yield return new WaitForSeconds(duration);

        _canRopeSwing = true;
    }
    private void DetachPlayerRopeSwing()
    {
        Destroy(_addedHingeJoint);
        transform.SetParent(null);
        _currentAction = Actions.None;
    }
    #endregion

    #region Panel

    private void PanelMove(bool left)
    {
        _rigidbody.MovePosition(_rigidbody.position + new Vector3(0, 0, (left ? 1 : -1 )) * _actionPanelSpeed * Time.deltaTime);
    }
    #endregion

    #region WallRun

    private void UpdateWallRunState(bool state)
    {
        Debug.Log("UpdateWallRunState");
        _isWallRunning = state;
        if (!state && _isWallRunAttached)
        {
            _rigidbody.useGravity = true;
            StartCoroutine(CoroutineWallRunDettach());
            StartCoroutine(DisableElementSpeedText());
            _currentAction = Actions.None;
        }
        else
        {
            _isWallRunLeft = !Physics.Raycast(_wallRunRaycastPoint.position, transform.TransformDirection(new Vector3(0, 0, -1)), out _, _wallRunRaycastLength, _wallRunLayerMask);
            _animator.SetBool("WallRunLeft", _isWallRunLeft);
            StartCoroutine(CoroutineWallRunAttach());
        }
        _cameraSwitcher.SwitchCamera(state ? CameraSwitcherCameras.WallRun : CameraSwitcherCameras.Main);
        _animator.SetBool("WallRun", state);
    }

    private void InitWallRun(GameObject gameobject)
    {
        _wallRunMoveSpeed = _wallRunInitSpeed;
        SetElementSpeedText(gameobject);
        UpdateElementSpeedText(_wallRunMoveSpeed);
    }

    private bool MoveWallRunChecker()
    {
        RaycastHit hit;
        if (Physics.Raycast(_wallRunRaycastPoint.position, transform.TransformDirection(new Vector3(0, 0, _isWallRunLeft ? 1 : -1)), out hit, _wallRunRaycastLength, _wallRunLayerMask))
        {
            Debug.DrawRay(_wallRunRaycastPoint.position, transform.TransformDirection(new Vector3(0, 0, _isWallRunLeft ? 1 : -1)) * hit.distance, Color.green, 10);
            return true;
        }
        else
        {
            Debug.DrawRay(_wallRunRaycastPoint.position, transform.TransformDirection(new Vector3(0, 0, _isWallRunLeft ? 1 : -1)) * _wallRunRaycastLength, Color.red, 10);
            return false;
        }
    }

    private IEnumerator CoroutineWallRunAttach()
    {
        _wallRunOverrideStart = true;
        float time = 0;
        float duration = 0.25f;
        RaycastHit hit;
        Physics.Raycast(_wallRunRaycastPoint.position, transform.TransformDirection(new Vector3(0, 0, _isWallRunLeft ? 1 : -1)), out hit, _wallRunRaycastLength, _wallRunLayerMask);

        Vector3 start = _rigidbody.position;
        Vector3 goal = new Vector3(_rigidbody.position.x, hit.point.y + 0.5f, hit.point.z + (_isWallRunLeft ? -0.5f : 0.5f));
        while (time < duration)
        {
            _rigidbody.position = Vector3.Lerp(start, goal, (time / duration));
            time += Time.deltaTime;
            yield return null;
        }
        _rigidbody.useGravity = false;
        _wallRunOverrideStart = false;
        _isWallRunAttached = true;

    }
    private IEnumerator CoroutineWallRunDettach()
    {
        float time = 0;
        float duration = 0.4f;
        _isWallRunAttached = false;
        Vector3 start = _rigidbody.position;
        Vector3 goal = new Vector3(_rigidbody.position.x, _rigidbody.position.y, 0);
        while (time < duration)
        {
            _rigidbody.position = Vector3.Lerp(start, goal, (time / duration));
            time += Time.deltaTime;
            yield return null;
        }
    }

    #endregion

    #endregion
}