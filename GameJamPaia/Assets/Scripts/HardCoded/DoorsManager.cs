using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorsManager : MonoBehaviour
{
    [SerializeField] private Door2D[] doors;
    private List<int> doorsIndexAvailable;

    private int currentDoorsLockedValue = 0;
    private void Awake()
    {
        doorsIndexAvailable = new List<int>(doors.Length);

        for (int i = 0; i < doors.Length; i++)
        {
            doorsIndexAvailable.Add(i);

            doors[i].OnUnlockDoor += Door_OnUnlockDoor;
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

    public void LockDoorAt(int index)
    {
        //print("Index:" + doors[doorsIndexAvailable[index]]);
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

    public int DoorsLockedValue()
    {
        return currentDoorsLockedValue;
    }

}
