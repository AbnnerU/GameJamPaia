using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleArea : MonoBehaviour
{
    [SerializeField] private string targetTag;
    [SerializeField] private Transform blackHoleCenter;
    [SerializeField] private float attractionForce;
    private Rigidbody2D[] objectsInside;

    private void Awake()
    {
        objectsInside = new Rigidbody2D[0];
    }

    private bool ObjectExist(Rigidbody2D reference)
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

    private void RegisterObject(Rigidbody2D reference)
    {
        int length = objectsInside.Length + 1;
        Rigidbody2D[] temp = new Rigidbody2D[length];

        for (int i = 0; i < objectsInside.Length; i++)
        {
            temp[i] = objectsInside[i];
        }

        temp[length - 1] = reference;

        objectsInside = temp;
    }

    private void RemoveObject(Rigidbody2D reference)
    {
        int length = objectsInside.Length - 1;
        Rigidbody2D[] temp = new Rigidbody2D[length];

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
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                if (!ObjectExist(rb))
                {
                    RegisterObject(rb);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                if (ObjectExist(rb))
                {
                    RemoveObject(rb);
                }
            }
        }
    }


    private void FixedUpdate()
    {
        if (objectsInside.Length == 0) return;

        ApplyBlackHoleForce();
    }


    private void ApplyBlackHoleForce()
    {
        for (int i = 0; i < objectsInside.Length; ++i)
        {
            Vector2 directionToCenter = blackHoleCenter.position - objectsInside[i].transform.position;
            float distanceToCenter = directionToCenter.magnitude;

            // A força de atração diminui à medida que o jogador se aproxima do centro do buraco negro
            float force = attractionForce / distanceToCenter;

            objectsInside[i].AddForce(directionToCenter.normalized * attractionForce, ForceMode2D.Force);
        }
    }

}
