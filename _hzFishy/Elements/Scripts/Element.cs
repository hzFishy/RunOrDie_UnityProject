using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{

    private bool _playerHit = false;
    public bool playerHit
    {
        get
        {
            return _playerHit;
        }
    }

    [SerializeField]
    private ElementsType elementType = ElementsType.None;


    public void ParentOnEnter(Collider playerCollider, ElementChild elementChild)
    {
        Debug.Log("enter collision of type: "+elementType);
        
        IPlayerInteractions iPlayerInteractions = playerCollider.transform.parent.GetComponent<IPlayerInteractions>();

        if (elementChild != null && iPlayerInteractions != null)
        {
            //Debug.Log("passed");
            //Debug.Log(overlapTrigger.blockPlayer);
            if (!elementChild.blockPlayer)
            {
                iPlayerInteractions.UnlockAction(elementType, elementChild.gameObject);

            }
            else
            {
                _playerHit = true;
                iPlayerInteractions.StopPlayer(elementType, elementChild.gameObject);
            }
        }
    }

    public void ParentOnExit(Collider playerCollider, ElementChild elementChild)
    {
        Debug.Log("exit collision of type: " + elementType);

        IPlayerInteractions iPlayerInteractions = playerCollider.transform.parent.GetComponent<IPlayerInteractions>();

        if (elementChild != null && iPlayerInteractions != null)
        {
            //Debug.Log("passed");
            //Debug.Log(overlapTrigger.blockPlayer);
            if (!elementChild.blockPlayer)
            {
                iPlayerInteractions.LockAction(elementType, elementChild.gameObject); ;
            }
        }
    }
}
