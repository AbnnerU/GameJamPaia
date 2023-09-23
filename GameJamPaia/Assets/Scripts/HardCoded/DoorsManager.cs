using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DoorsManager : MonoBehaviour
{
    [SerializeField] private bool setDisabledDoorAsNotAvailableIndex=true;
    [SerializeField] private Door2D[] doors;
    private List<Door2D> disabledDoors;
    private List<int> doorsIndexAvailable;

    private int currentDoorsLockedValue = 0;
    private void Awake()
    {
        doorsIndexAvailable = new List<int>(doors.Length);
        disabledDoors = new List<Door2D>();

        for (int i = 0; i < doors.Length; i++)
        {
            doorsIndexAvailable.Add(i);

            doors[i].OnUnlockDoor += Door_OnUnlockDoor;
        }

       
        if (setDisabledDoorAsNotAvailableIndex)
        {
            List<int> temp = new List<int>();

            for (int i=0; i< doorsIndexAvailable.Count; i++)
            {
                if (doors[doorsIndexAvailable[i]].IsEnabled() == false)
                {
                    temp.Add(i);
                    disabledDoors.Add(doors[doorsIndexAvailable[i]]);
                }
            }

            for(int i=0; i< temp.Count; i++)
            {
                doorsIndexAvailable.Remove(temp[i]);
            }
            
        }

        
    }

    public void TryLockRandomDoor()
    {
        if (doorsIndexAvailable.Count > 0)
        {
            LockDoorAt(Random.Range(0, doorsIndexAvailable.Count));
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
        if (doors[doorsIndexAvailable[index]].IsEnabled() == false) return;

        doors[doorsIndexAvailable[index]].LockDoor();

        doorsIndexAvailable.RemoveAt(index);

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
        int indexRef = GetDoorIndex(doorRef);

        if (indexRef >= 0)
        {
            for (int i = 0; i < doorsIndexAvailable.Count; i++)
            {
                if (doorsIndexAvailable[i] == indexRef)
                    return i;
            }
            return -1;
        }

        return -1;
    }

    public void SendActiveStateEvent()
    {
        for(int i=0; i < doors.Length;i++)
        {
            doors[i].SendUpdateStateEvent();
        }
    }

    public void EnableAllDoors()
    {
        int index = 0;
        for(int i=0; i < disabledDoors.Count;i++)
        {
            index = GetDoorIndex(disabledDoors[i]);

            disabledDoors[i].Enable();
            disabledDoors[i].UnlockDoorWhitoutActions();

            doorsIndexAvailable.Add(index);
        }

        disabledDoors.Clear();
        disabledDoors.Capacity = 0;
    }

    public int DoorsLockedValue()
    {
        return currentDoorsLockedValue;
    }

}
