using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class NavigationSelectables : MonoBehaviour
{
    private PlayerInputs playerInputActions;
    [SerializeField] private List<Button> _selectables = new List<Button>();
    private int _currentSelection = -1;
    private void Awake()
    {
        playerInputActions = new PlayerInputs();
    }
    private void OnEnable()
    {
        playerInputActions.Player.SpaceAction_Tap.performed += SpaceAction_Tap; // bind function to "Space tap "
        playerInputActions.Player.SpaceAction_Tap.Enable();
        playerInputActions.Player.SpaceAction_Hold.performed += SpaceAction_Hold;
        playerInputActions.Player.SpaceAction_Hold.Enable();
    }
    private void OnDisable()
    {
        playerInputActions.Player.SpaceAction_Tap.Disable();
        playerInputActions.Player.SpaceAction_Hold.Disable();
    }

    public void DisableInputs()
    {
        if (playerInputActions == null)
        {
            return;
        }
        playerInputActions.Player.SpaceAction_Tap.Disable();
        playerInputActions.Player.SpaceAction_Hold.Disable();
    }

    private void SpaceAction_Tap(InputAction.CallbackContext context)
    {
        NextSelection();
    }
    private void SpaceAction_Hold(InputAction.CallbackContext context)
    {
        SelectSelection();
    }

    private void SelectSelection()
    {
        if (_currentSelection > -1)
        {
            _selectables[_currentSelection].onClick.Invoke();
        }
    }
    private void NextSelection()
    {
        if (_currentSelection < _selectables.Count - 1)
        {
            _currentSelection += 1;
        }
        else
        {
            _currentSelection = 0;
        }
        _selectables[_currentSelection].Select();
    }

    public void RemoveSelectable(int index)
    {
        _selectables.RemoveAt(index);
    }
}
