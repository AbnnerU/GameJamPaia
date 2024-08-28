
using System;
using UnityEngine;

public class Door2DSpriteManager : MonoBehaviour
{
    [SerializeField] private Door2D door;
    [SerializeField] private SpriteRenderer doorSpriteRenderer;
    [SerializeField] private Sprite unlockedSprite;
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Vector3 lockedLocalScale = Vector3.one;
    [SerializeField] private Sprite disabledSprite;
    [SerializeField] private Vector3 disabledSpriteLocalScale = Vector3.one;
    [SerializeField] private bool disableIfSpriteIsNull;

    private void Awake()
    {
        door.OnLockDoor += Door_OnLock;
        door.OnUnlockDoor += Door_OnUnlock;
        door.OnChangeDoorActiveState += Door_OnChangeActiveState;
    }

    private void Door_OnChangeActiveState(bool isActive)
    {
        doorSpriteRenderer.enabled = true;

        if (!isActive)
        {       
            if (disabledSprite == null && disableIfSpriteIsNull)
            {
                doorSpriteRenderer.enabled = false;
                return;
            }

            doorSpriteRenderer.sprite = disabledSprite;
            doorSpriteRenderer.transform.localScale = disabledSpriteLocalScale;
        }
        else
        {
            if (door.IsLocked())
                Door_OnLock(door);
            else
                Door_OnUnlock(door);
            
        }
            
    }

    private void Door_OnUnlock(Door2D d)
    {
        doorSpriteRenderer.enabled = true;
        if (unlockedSprite == null && disableIfSpriteIsNull)
        {
            doorSpriteRenderer.enabled = false;
            return;
        }
        
        doorSpriteRenderer.sprite = unlockedSprite;
   
    }

    private void Door_OnLock(Door2D d)
    {
        doorSpriteRenderer.enabled = true;
        if (lockedSprite == null && disableIfSpriteIsNull)
        {
            doorSpriteRenderer.enabled = false;
            return;
        }

        doorSpriteRenderer.sprite = lockedSprite;
        doorSpriteRenderer.transform.localScale = lockedLocalScale;
    }
}
