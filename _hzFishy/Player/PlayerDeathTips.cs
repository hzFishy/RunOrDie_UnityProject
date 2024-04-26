using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;


public class PlayerDeathTips : MonoBehaviour
{
    [System.Serializable] public class DeathTipsData : SerializableDictionaryBase<ElementsType, string> { }

    public DeathTipsData DeathTips = new();

}
