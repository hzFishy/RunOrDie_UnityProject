using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerInteractions
{
    /// <summary> Stop Player movement </summary>
    /// <param name="elementType"></param>
    /// /// <param name="gameobject">gameobject origin</param>
    void StopPlayer(ElementsType elementType, GameObject gameobject);

    /// <summary> Unlock a "Actions" </summary>
    /// <param name="elementType"></param>
    /// <param name="gameobject">gameobject origin</param>
    void UnlockAction(ElementsType elementType, GameObject gameobject);

    /// <summary> Reset back currentAction to None </summary>
    /// <param name="elementType"></param>
    /// <param name="gameobject">gameobject origin</param>
    void LockAction(ElementsType elementType, GameObject gameobject);
}