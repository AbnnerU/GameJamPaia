
using UnityEngine;
using Assets.Scripts.GameAction;

public class ChangeCollider2DActiveStateAction : GameAction
{


    [SerializeField] private Collider2D colliderRef;
    [SerializeField] private bool setActive;


    public override void DoAction()
    {
        if (colliderRef != null)
        {
            colliderRef.enabled = setActive;
        }
    }
}
