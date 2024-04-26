using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YBotScript : MonoBehaviour
{
    public PlayerController PlayerController;

    public void StartRolling()
    {
        PlayerController.UpdateRolling(true);
    }

    public void StopRolling()
    {
        PlayerController.UpdateRolling(false);
    }
}
