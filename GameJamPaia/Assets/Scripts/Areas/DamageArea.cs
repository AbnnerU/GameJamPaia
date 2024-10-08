
using UnityEngine;

public class DamageArea : MonoBehaviour
{
    [SerializeField] private string targetTag;
    [SerializeField] private float damage;
    [SerializeField] private float delayToDoDamageAgain;
    private IHittable[] objectsInside;

    private float currentTime = 0;

    private bool canDoDamage = true;

    private void Awake()
    {
        objectsInside = new IHittable[0];
    }

    private bool ObjectExist(IHittable reference)
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

    private void RegisterObject(IHittable reference)
    {
        int length = objectsInside.Length + 1;
        IHittable[] temp = new IHittable[length];

        for (int i = 0; i < objectsInside.Length; i++)
        {
            temp[i] = objectsInside[i];
        }

        temp[length - 1] = reference;

        objectsInside = temp;
    }

    private void RemoveObject(IHittable reference)
    {
        int length = objectsInside.Length - 1;
        IHittable[] temp = new IHittable[length];

        for (int i = 0; i < objectsInside.Length; i++)
        {
            if (objectsInside[i] == reference) continue;

            temp[i] = objectsInside[i];
        }

        objectsInside = temp;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            IHittable hittable = collision.GetComponent<IHittable>();

            if (hittable != null)
            {
                if (!ObjectExist(hittable))
                {
                    RegisterObject(hittable);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            IHittable hittable = collision.GetComponent<IHittable>();

            if (hittable != null)
            {
                if (ObjectExist(hittable))
                {
                    RemoveObject(hittable);
                }
            }
        }
    }

    private void Update()
    {
        if (!canDoDamage)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= delayToDoDamageAgain)
            {
                canDoDamage = true;
                currentTime = 0;
            }
        }
        else
        {
            if (objectsInside.Length == 0) return;

            for (int i = 0; i < objectsInside.Length; i++)
            {
                objectsInside[i].OnHit(damage);
            }

            canDoDamage = false;
            currentTime = 0;
        }
    }
}
