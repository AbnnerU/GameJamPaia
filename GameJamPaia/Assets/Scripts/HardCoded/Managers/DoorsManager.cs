using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DoorsManager : MonoBehaviour
{
    [SerializeField] private bool setDisabledDoorAsNotAvailable=true;
    [SerializeField]private MapManager mapManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Door2D[] doors;
    private List<Door2D> disabledDoors;
    private List<Door2D> doorsAvailable;

    private int currentDoorsLockedValue = 0;
    private void Awake()
    {
        gameManager.OnSetupCompleted += GameManager_OnMapSetupCompleted;  
    }

    private void GameManager_OnMapSetupCompleted()
    {
        doors = mapManager.GetAllDoors().ToArray();

        doorsAvailable = new List<Door2D>(doors.Length);
        disabledDoors = new List<Door2D>();

        for (int i = 0; i < doors.Length; i++)
        {
            doorsAvailable.Add(doors[i]);

            doors[i].OnUnlockDoor += Door_OnUnlockDoor;
        }


        if (setDisabledDoorAsNotAvailable)
        {
            List<Door2D> temp = new List<Door2D>();

            for (int i = 0; i < doorsAvailable.Count; i++)
            {
                if (doorsAvailable[i].CanBeActive() == false)
                {
                    temp.Add(doorsAvailable[i]);
                    disabledDoors.Add(doorsAvailable[i]);
                }
            }

            for (int i = 0; i < temp.Count; i++)
            {
                doorsAvailable.Remove(temp[i]);
            }

        }
    }

    public void TryLockRandomDoor()
    {
        if (doorsAvailable.Count > 0)
        {
            LockDoorAt(Random.Range(0, doorsAvailable.Count));
        }
    }

    private void Door_OnUnlockDoor(Door2D doorRef)
    {
        currentDoorsLockedValue--;

    }

    public void TryLookDoor(Door2D door2D)
    {
        int id = GetAvailableDoorIndex(door2D);

        if (id >= 0)
        {
            LockDoorAt(id);
        }
    }

    public void LockDoorAt(int index)
    {
        //print("Index:" + doors[doorsIndexAvailable[index]]);
        if (index < 0) return;

        if (doorsAvailable[index].CanBeActive() == false) return;

        doorsAvailable[index].LockDoor();

        doorsAvailable.RemoveAt(index);

        currentDoorsLockedValue++;
    }


    public int GetDoorIndex(Door2D doorRef)
    {
        for (int i = 0; i < doors.Length; i++)
        {
            if (doorRef == doors[i])
                return i;
        }

        return -1;
    }

    public int GetAvailableDoorIndex(Door2D doorRef)
    {
        int indexRef = doorsAvailable.IndexOf(doorRef);

        if (indexRef >= 0)       
            return indexRef;
        else
            return -1;
        
    }

    public void SendActiveStateEvent()
    {
        for(int i=0; i < doors.Length;i++)
        {
            doors[i].SendUpdateStateEvent();
        }
    }

    public void SetNewAvailableDoor(Door2D door)
    {
        if(disabledDoors.Contains(door))
            disabledDoors.Remove(door);

        if (!doorsAvailable.Contains(door))
            doorsAvailable.Add(door);
        else
            Debug.LogWarning("For some reason, the door (" + door + ") is already an available door");

        door.Enable();
        door.UnlockDoorWhitoutActions();

    }

    public void UnlockAllDoors()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].UnlockDoor();
        }
    }

    public int DoorsLockedValue()
    {
        return currentDoorsLockedValue;
    }

}
