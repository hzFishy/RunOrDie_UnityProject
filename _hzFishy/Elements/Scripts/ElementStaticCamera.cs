using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ElementStaticCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _vCamera;
    
    public void UpdateCameraStatus(bool activate)
    {
        if (_vCamera)
        {
            _vCamera.LookAt = GameObject.FindGameObjectWithTag("Player").transform;
            _vCamera.Priority = activate ? 15 : 9;
        }
        else
        {
            Debug.LogError("Camera null");
        }
    }
}
