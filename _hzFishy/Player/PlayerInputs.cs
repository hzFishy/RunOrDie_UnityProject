//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.3
//     from Assets/_hzFishy/Player/PlayerInputs.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInputs: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputs"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""64845593-c708-485f-b431-584752df9312"",
            ""actions"": [
                {
                    ""name"": ""SpaceAction_Tap"",
                    ""type"": ""Button"",
                    ""id"": ""3e2155d1-5b63-4aa0-a23c-e1e1e52488fd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SpaceAction_DoubleTap"",
                    ""type"": ""Button"",
                    ""id"": ""a4eecf59-f725-408a-80c8-08610761baf4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SpaceAction_Hold"",
                    ""type"": ""Button"",
                    ""id"": ""ec8f8a2c-a07a-4ed3-93a6-ed07ad3f0a82"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Debug_RestartScene"",
                    ""type"": ""Button"",
                    ""id"": ""8f9d11d3-aee0-4f03-8572-54fe80584be2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Extra_Pause"",
                    ""type"": ""Button"",
                    ""id"": ""28a691e7-f70e-45d5-9b27-b75e0001691b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Debug_ToggleSong"",
                    ""type"": ""Button"",
                    ""id"": ""caa3a185-ef9a-414a-89ea-57dd66323c1f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Debug_SwitchSong"",
                    ""type"": ""Button"",
                    ""id"": ""9d7db2b6-8d9f-4510-819d-fa1c08ef1b32"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Debug_ToggleBackgroundGeneration"",
                    ""type"": ""Button"",
                    ""id"": ""72595edd-20e5-4ec5-a5b4-1c87ef2d8365"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""dfa10294-8aec-4fe9-a978-f2d7a28ebff4"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SpaceAction_Tap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bb853795-83e9-4f25-9286-4952726bf411"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": ""MultiTap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SpaceAction_DoubleTap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fb2f5b35-8e9a-4e37-9bf7-6488db7f7836"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": ""Hold(duration=0.2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SpaceAction_Hold"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f55b5086-f076-4087-b589-f2fc1cf91762"",
                    ""path"": ""<Keyboard>/f5"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Debug_RestartScene"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7f7d9186-0dea-4638-9b87-d3558d65b1b2"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Extra_Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9e7b7c08-31eb-4657-999d-0d3b07b65ba4"",
                    ""path"": ""<Keyboard>/f6"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Debug_ToggleSong"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""07b5971b-8dac-4a2c-91f7-a104b11d7e75"",
                    ""path"": ""<Keyboard>/f7"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Debug_SwitchSong"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f2f037f4-a4c6-4d99-8cde-074aeda26dc3"",
                    ""path"": ""<Keyboard>/f8"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Debug_ToggleBackgroundGeneration"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_SpaceAction_Tap = m_Player.FindAction("SpaceAction_Tap", throwIfNotFound: true);
        m_Player_SpaceAction_DoubleTap = m_Player.FindAction("SpaceAction_DoubleTap", throwIfNotFound: true);
        m_Player_SpaceAction_Hold = m_Player.FindAction("SpaceAction_Hold", throwIfNotFound: true);
        m_Player_Debug_RestartScene = m_Player.FindAction("Debug_RestartScene", throwIfNotFound: true);
        m_Player_Extra_Pause = m_Player.FindAction("Extra_Pause", throwIfNotFound: true);
        m_Player_Debug_ToggleSong = m_Player.FindAction("Debug_ToggleSong", throwIfNotFound: true);
        m_Player_Debug_SwitchSong = m_Player.FindAction("Debug_SwitchSong", throwIfNotFound: true);
        m_Player_Debug_ToggleBackgroundGeneration = m_Player.FindAction("Debug_ToggleBackgroundGeneration", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
    private readonly InputAction m_Player_SpaceAction_Tap;
    private readonly InputAction m_Player_SpaceAction_DoubleTap;
    private readonly InputAction m_Player_SpaceAction_Hold;
    private readonly InputAction m_Player_Debug_RestartScene;
    private readonly InputAction m_Player_Extra_Pause;
    private readonly InputAction m_Player_Debug_ToggleSong;
    private readonly InputAction m_Player_Debug_SwitchSong;
    private readonly InputAction m_Player_Debug_ToggleBackgroundGeneration;
    public struct PlayerActions
    {
        private @PlayerInputs m_Wrapper;
        public PlayerActions(@PlayerInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @SpaceAction_Tap => m_Wrapper.m_Player_SpaceAction_Tap;
        public InputAction @SpaceAction_DoubleTap => m_Wrapper.m_Player_SpaceAction_DoubleTap;
        public InputAction @SpaceAction_Hold => m_Wrapper.m_Player_SpaceAction_Hold;
        public InputAction @Debug_RestartScene => m_Wrapper.m_Player_Debug_RestartScene;
        public InputAction @Extra_Pause => m_Wrapper.m_Player_Extra_Pause;
        public InputAction @Debug_ToggleSong => m_Wrapper.m_Player_Debug_ToggleSong;
        public InputAction @Debug_SwitchSong => m_Wrapper.m_Player_Debug_SwitchSong;
        public InputAction @Debug_ToggleBackgroundGeneration => m_Wrapper.m_Player_Debug_ToggleBackgroundGeneration;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
            @SpaceAction_Tap.started += instance.OnSpaceAction_Tap;
            @SpaceAction_Tap.performed += instance.OnSpaceAction_Tap;
            @SpaceAction_Tap.canceled += instance.OnSpaceAction_Tap;
            @SpaceAction_DoubleTap.started += instance.OnSpaceAction_DoubleTap;
            @SpaceAction_DoubleTap.performed += instance.OnSpaceAction_DoubleTap;
            @SpaceAction_DoubleTap.canceled += instance.OnSpaceAction_DoubleTap;
            @SpaceAction_Hold.started += instance.OnSpaceAction_Hold;
            @SpaceAction_Hold.performed += instance.OnSpaceAction_Hold;
            @SpaceAction_Hold.canceled += instance.OnSpaceAction_Hold;
            @Debug_RestartScene.started += instance.OnDebug_RestartScene;
            @Debug_RestartScene.performed += instance.OnDebug_RestartScene;
            @Debug_RestartScene.canceled += instance.OnDebug_RestartScene;
            @Extra_Pause.started += instance.OnExtra_Pause;
            @Extra_Pause.performed += instance.OnExtra_Pause;
            @Extra_Pause.canceled += instance.OnExtra_Pause;
            @Debug_ToggleSong.started += instance.OnDebug_ToggleSong;
            @Debug_ToggleSong.performed += instance.OnDebug_ToggleSong;
            @Debug_ToggleSong.canceled += instance.OnDebug_ToggleSong;
            @Debug_SwitchSong.started += instance.OnDebug_SwitchSong;
            @Debug_SwitchSong.performed += instance.OnDebug_SwitchSong;
            @Debug_SwitchSong.canceled += instance.OnDebug_SwitchSong;
            @Debug_ToggleBackgroundGeneration.started += instance.OnDebug_ToggleBackgroundGeneration;
            @Debug_ToggleBackgroundGeneration.performed += instance.OnDebug_ToggleBackgroundGeneration;
            @Debug_ToggleBackgroundGeneration.canceled += instance.OnDebug_ToggleBackgroundGeneration;
        }

        private void UnregisterCallbacks(IPlayerActions instance)
        {
            @SpaceAction_Tap.started -= instance.OnSpaceAction_Tap;
            @SpaceAction_Tap.performed -= instance.OnSpaceAction_Tap;
            @SpaceAction_Tap.canceled -= instance.OnSpaceAction_Tap;
            @SpaceAction_DoubleTap.started -= instance.OnSpaceAction_DoubleTap;
            @SpaceAction_DoubleTap.performed -= instance.OnSpaceAction_DoubleTap;
            @SpaceAction_DoubleTap.canceled -= instance.OnSpaceAction_DoubleTap;
            @SpaceAction_Hold.started -= instance.OnSpaceAction_Hold;
            @SpaceAction_Hold.performed -= instance.OnSpaceAction_Hold;
            @SpaceAction_Hold.canceled -= instance.OnSpaceAction_Hold;
            @Debug_RestartScene.started -= instance.OnDebug_RestartScene;
            @Debug_RestartScene.performed -= instance.OnDebug_RestartScene;
            @Debug_RestartScene.canceled -= instance.OnDebug_RestartScene;
            @Extra_Pause.started -= instance.OnExtra_Pause;
            @Extra_Pause.performed -= instance.OnExtra_Pause;
            @Extra_Pause.canceled -= instance.OnExtra_Pause;
            @Debug_ToggleSong.started -= instance.OnDebug_ToggleSong;
            @Debug_ToggleSong.performed -= instance.OnDebug_ToggleSong;
            @Debug_ToggleSong.canceled -= instance.OnDebug_ToggleSong;
            @Debug_SwitchSong.started -= instance.OnDebug_SwitchSong;
            @Debug_SwitchSong.performed -= instance.OnDebug_SwitchSong;
            @Debug_SwitchSong.canceled -= instance.OnDebug_SwitchSong;
            @Debug_ToggleBackgroundGeneration.started -= instance.OnDebug_ToggleBackgroundGeneration;
            @Debug_ToggleBackgroundGeneration.performed -= instance.OnDebug_ToggleBackgroundGeneration;
            @Debug_ToggleBackgroundGeneration.canceled -= instance.OnDebug_ToggleBackgroundGeneration;
        }

        public void RemoveCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    public interface IPlayerActions
    {
        void OnSpaceAction_Tap(InputAction.CallbackContext context);
        void OnSpaceAction_DoubleTap(InputAction.CallbackContext context);
        void OnSpaceAction_Hold(InputAction.CallbackContext context);
        void OnDebug_RestartScene(InputAction.CallbackContext context);
        void OnExtra_Pause(InputAction.CallbackContext context);
        void OnDebug_ToggleSong(InputAction.CallbackContext context);
        void OnDebug_SwitchSong(InputAction.CallbackContext context);
        void OnDebug_ToggleBackgroundGeneration(InputAction.CallbackContext context);
    }
}
