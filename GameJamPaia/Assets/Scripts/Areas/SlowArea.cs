
using UnityEngine;

public class SlowArea : MonoBehaviour
{
    [SerializeField] private string targetTag;
    [SerializeField] private float slowPercentage;
    private NegativeEffects[] objectsInside;

    private void Awake()
    {
        objectsInside = new NegativeEffects[0];
    }

    private bool ObjectExist(NegativeEffects reference)
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

    private void RegisterObject(NegativeEffects reference)
    {
        int length = objectsInside.Length + 1;
        NegativeEffects[] temp = new NegativeEffects[length];

        for (int i = 0; i < objectsInside.Length; i++)
        {
            temp[i] = objectsInside[i];
        }

        temp[length - 1] = reference;

        objectsInside = temp;
    }

    private void RemoveObject(NegativeEffects reference)
    {
        int length = objectsInside.Length - 1;
        NegativeEffects[] temp = new NegativeEffects[length];

        for (int i = 0; i < objectsInside.Length; i++)
        {
            if (objectsInside[i] == reference) continue;

            temp[i] = objectsInside[i];
        }

        objectsInside = temp;
    }

    private void SetDefaultSpeed(NegativeEffects reference)
    {
        for (int i = 0; i < objectsInside.Length; i++)
        {
            if (objectsInside[i] == reference)
            {
                objectsInside[i].CancelSlow();
                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            NegativeEffects movement = collision.GetComponent<NegativeEffects>();

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
            NegativeEffects movement = collision.GetComponent<NegativeEffects>();

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

    private void ApplySlow(NegativeEffects reference)
    {
        for (int i = 0; i < objectsInside.Length; i++)
        {
            if (objectsInside[i] == reference)
            {
                objectsInside[i].ApllySlow(slowPercentage);
                return;
            }
        }

    }
}
