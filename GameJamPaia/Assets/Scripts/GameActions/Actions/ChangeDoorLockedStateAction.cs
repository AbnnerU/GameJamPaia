using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.GameAction;

public class ChangeDoorLockedStateAction : GameAction
{
    [SerializeField] private Door2D doorRef;
    [SerializeField] private bool setLocked;
    


    public override void DoAction()
    {
        if(doorRef != null)
        {
            if (setLocked)
                doorRef.LockDoor();
            else
                doorRef.UnlockDoor();
        }
    }
}
