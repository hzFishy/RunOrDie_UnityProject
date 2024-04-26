using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
public enum CameraSwitcherCameras
{
    Main,
    Zipline,
    Ladder,
    Vault,
    Slide,
    PushingDown,
    WallRun
    //when adding update for loop for set
}

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private List<CameraSwitcherCameras> _cameraTypes = new List<CameraSwitcherCameras>(5);
    [SerializeField] private List<CinemachineVirtualCamera> _virtualCameras = new List<CinemachineVirtualCamera>(5);
    [SerializeField] private CinemachineVirtualCamera _virtualCameraMain;

    [SerializeField] private Dictionary<CameraSwitcherCameras, CinemachineVirtualCamera> _dictCamCam = new Dictionary<CameraSwitcherCameras, CinemachineVirtualCamera>();

    private void Start()
    {

        for (int i = 0; i < 6; i++)
        {
            //Debug.Log(_cameraTypes[i] +" "+ _virtualCameras[i].name);
            _dictCamCam.Add(_cameraTypes[i], _virtualCameras[i]);
        }
    }

    public void SwitchCamera(CameraSwitcherCameras goalType = CameraSwitcherCameras.Main)
    {
        foreach (CinemachineVirtualCamera vCamera in _virtualCameras)
        {
            vCamera.enabled = (vCamera == (_dictCamCam[goalType]));
        }
    }


    public void UpdateAimYOffset(float value = 0.5f)
    {
        _virtualCameraMain.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = new Vector3(0,value,0);
    }

    public void UpdateBodyYOffset(float value = 2)
    {
        _virtualCameraMain.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(-2.5f, value, 0);
    }
}
