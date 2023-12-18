using Assets.Scripts.GameAction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door2D : MonoBehaviour, IHasActiveState
{
    [SerializeField] private bool canBeActive = true;
    [SerializeField] private bool locked=false;
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private TransformToChange[] onTriggerDoorUpdatePositions;
    [SerializeField] private Collider2D doorCollider;
    [Header("On Trigger Door")]
    [SerializeField] private GameAction[] onTriggerDoorActions;
    [Header("On Door Locked")]
    [SerializeField] private GameAction[] onDoorLockedActions;
    [Header("On Door Unlocked")]
    [SerializeField] private GameAction[] onDoorUnlockedActions;


    public Action<Door2D> OnLockDoor;
    public Action<Door2D> OnUnlockDoor;
    public Action<bool> OnChangeDoorActiveState;
    public Action OnUpdatePositions;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!canBeActive) return;

        if (locked) return;

        if (collision.CompareTag(targetTag))
        {
            for (int i = 0; i < onTriggerDoorUpdatePositions.Length; i++)
            {
                if (onTriggerDoorUpdatePositions[i].target == null) return;

                if (onTriggerDoorUpdatePositions[i].useLocalPosition)
                    onTriggerDoorUpdatePositions[i]._transform.localPosition = onTriggerDoorUpdatePositions[i].target.localPosition + onTriggerDoorUpdatePositions[i].offSet;
                else
                    onTriggerDoorUpdatePositions[i]._transform.position = onTriggerDoorUpdatePositions[i].target.position + onTriggerDoorUpdatePositions[i].offSet;
            }

            OnUpdatePositions?.Invoke();

            if(onTriggerDoorActions.Length > 0)
            {
                for (int i = 0; i < onTriggerDoorActions.Length; i++)
                    onTriggerDoorActions[i].DoAction();
            }
        }
    }

    public void LockDoor()
    {
        if (!canBeActive) return;

        if (locked) return;

        locked = true;
        doorCollider.enabled = false;

        if (onDoorLockedActions.Length > 0)
        {
            for (int i = 0; i < onDoorLockedActions.Length; i++)
                onDoorLockedActions[i].DoAction();
        }

        OnLockDoor?.Invoke(this);
    }

    public void UnlockDoor()
    {
        if (!canBeActive) return;

        if (!locked) return; 

        locked = false;
        doorCollider.enabled = true;

        if (onDoorUnlockedActions.Length > 0)
        {
            for (int i = 0; i < onDoorUnlockedActions.Length; i++)
                onDoorUnlockedActions[i].DoAction();
        }

        OnUnlockDoor?.Invoke(this);
    }

    public void UnlockDoorWhitoutActions()
    {
        if (!canBeActive) return;

        if (!locked) return;

        locked = false;
        doorCollider.enabled = true;

        OnUnlockDoor?.Invoke(this);
    }

    public bool IsLocked() 
    { 
        return locked; 
    }

    public bool CanBeActive()
    {
        return canBeActive;
    }

    public void Disable()
    {
        canBeActive = false;

        OnChangeDoorActiveState?.Invoke(false);
    }

    public void Enable()
    {
        canBeActive = true;

        OnChangeDoorActiveState?.Invoke(true);
    }

    public void SendUpdateStateEvent()
    {
        OnChangeDoorActiveState?.Invoke(canBeActive);
    }

 

    [Serializable]
    private struct TransformToChange
    {
        public Transform _transform;
        public Transform target;
        public Vector3 offSet;
        public bool useLocalPosition;
        public bool drawGizmos;
        public Color gizmosColor;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < onTriggerDoorUpdatePositions.Length; i++)
        {
            if (onTriggerDoorUpdatePositions[i].drawGizmos == false || onTriggerDoorUpdatePositions[i].target == null || onTriggerDoorUpdatePositions[i]._transform == null) continue;

            float arrowHeadAngle = 20f;
            float arrowHeadLength = 0.5f;
            Vector3 direction = onTriggerDoorUpdatePositions[i].target.position - transform.position;

            Gizmos.color = onTriggerDoorUpdatePositions[i].gizmosColor;

            Gizmos.DrawLine(transform.position, onTriggerDoorUpdatePositions[i].target.position);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(onTriggerDoorUpdatePositions[i].target.position, right * arrowHeadLength);
            Gizmos.DrawRay(onTriggerDoorUpdatePositions[i].target.position, left * arrowHeadLength);


        }
    }

   
}
