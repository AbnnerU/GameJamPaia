using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowArea : MonoBehaviour
{
    [SerializeField] private string targetTag;
    [SerializeField] private float slowPercentage;
    private PlayerMovement[] objectsInside;

    private void Awake()
    {
        objectsInside = new PlayerMovement[0];
    }

    private bool ObjectExist(PlayerMovement reference)
    {
        for (int i = 0; i < objectsInside.Length; i++)
        {
            if (objectsInside[i] == reference)
            {
                return true;
            }
        }

        return false;
    }

    private void RegisterObject(PlayerMovement reference)
    {
        int length = objectsInside.Length + 1;
        PlayerMovement[] temp = new PlayerMovement[length];

        for (int i = 0; i < objectsInside.Length; i++)
        {
            temp[i] = objectsInside[i];
        }

        temp[length - 1] = reference;

        objectsInside = temp;
    }

    private void RemoveObject(PlayerMovement reference)
    {
        int length = objectsInside.Length - 1;
        PlayerMovement[] temp = new PlayerMovement[length];

        for (int i = 0; i < objectsInside.Length; i++)
        {
            if (objectsInside[i] == reference) continue;

            temp[i] = objectsInside[i];
        }

        objectsInside = temp;
    }

    private void SetDefaultSpeed(PlayerMovement reference)
    {
        for (int i = 0; i < objectsInside.Length; i++)
        {
            if (objectsInside[i] == reference)
            {
                objectsInside[i].SetDefaultSpeed();
                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            PlayerMovement movement = collision.GetComponent<PlayerMovement>();

            if (movement != null)
            {
                if (!ObjectExist(movement))
                {
                    RegisterObject(movement);

                    ApplySlow(movement);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            PlayerMovement movement = collision.GetComponent<PlayerMovement>();

            if (movement != null)
            {
                if (ObjectExist(movement))
                {

                    SetDefaultSpeed(movement);

                    RemoveObject(movement);

                }
            }
        }
    }

    private void ApplySlow(PlayerMovement reference)
    {
        for (int i = 0; i < objectsInside.Length; i++)
        {
            if (objectsInside[i] == reference)
            {
                float current = objectsInside[i].GetSpeed();
                float slow = current * (1f - (slowPercentage / 100f));

                objectsInside[i].SetNewSpeed(slow);
                return;
            }
        }

    }
}
