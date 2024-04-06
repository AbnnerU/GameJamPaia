using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]public class OnShoot: UnityEvent { }

public class Turret : MonoBehaviour { 

    [SerializeField] private bool drawGizmos;

    [SerializeField] private float turnSpeed;

    [SerializeField] private bool active = false;

    [SerializeField] private int maxAllocation;

    [SerializeField] private Transform verificationStartPoint;

    [SerializeField] private float verificationRadius;

    [SerializeField] private LayerMask layerMask;

    [SerializeField] private string targetTag;

    [Header("Projectile")]

    [SerializeField] private int atackDamage;

    [SerializeField] private float fireRate;

    [SerializeField] private float projectileSpeed;

    [SerializeField] private float gravityValue = Physics.gravity.y;

    [SerializeField] private float projectileRadius;

    [SerializeField] private LayerMask projectileLayerMask;

    [SerializeField] private bool useGroundLayer;

    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Transform atackPoint;

    [SerializeField] private GameObject projectilePrefab;

    public OnShoot onShoot;

    private Collider2D[] hitResults;

    private Transform _transform;

    private HealthManager targetHealth;

    private Transform targetTransform;

    private bool findedTarget = false;

    private bool canAtack=true;

    private bool paused = false;

    private void Awake()
    {
        _transform = GetComponent<Transform>();

        hitResults = new Collider2D[maxAllocation];

        if (active) TurrentActive();

    }

    public void TurrentActive()
    {
        if (gameObject.activeSelf == false) return;

        active = true;

        findedTarget = false;
        targetTransform = null;
        canAtack = true;
        StartCoroutine(StartTurret());
    }

    public void StopTurrent()
    {
        active = false;
        findedTarget = false;
        targetTransform = null;
        canAtack = true;
    }

    IEnumerator StartTurret()
    {
        do
        {
            while (paused)
            {
                yield return null;
            }

            if (findedTarget == false)
            {
                SeachForTarget();
            }
            else
            {
                if (targetTransform.gameObject.activeSelf)
                {
                    Vector3 directionVector = targetTransform.position - _transform.position;

                    if (directionVector.magnitude <= verificationRadius)
                    {
                        Rotate(directionVector);

                        if (canAtack)
                        {
                            if (targetHealth.IsAlive())
                            {
                                Shoot();

                                StartCoroutine(FireRate());
                            }
                            else
                            {
                                findedTarget = false;
                            }
                        }
                    }
                    else
                    {
                        findedTarget = false;
                    }
                }
                else
                {
                    findedTarget = false;
                }
            }

            yield return null;

        } while (active);

        yield break;
    }

    IEnumerator FireRate()
    {
        canAtack = false;

        yield return new WaitForSeconds(fireRate);

        canAtack = true;

        yield break;
    }

    private void SeachForTarget()
    {      
        int hitAmount = Physics2D.OverlapCircleNonAlloc(verificationStartPoint.position, verificationRadius, hitResults, layerMask,0);

        if (hitAmount > 0)
        {
            for (int i = 0; i < hitAmount; i++)
            {
                if (hitResults[i].CompareTag(targetTag))
                {
                    HealthManager healthManager = hitResults[i].gameObject.GetComponent<HealthManager>();

                    if (healthManager)
                    {
                        if (healthManager.IsAlive() && hitResults[i].gameObject.activeSelf)
                        {
                            targetHealth = healthManager;
                            targetTransform = hitResults[i].transform;
                            findedTarget = true;
                            break;
                        }
                    }
                }
            }
        }
    }

    private void Rotate(Vector2 direction)
    {
        Vector2 directionAngle = direction.normalized;

        float angle = Mathf.Atan2(directionAngle.x, directionAngle.y) * Mathf.Rad2Deg;

        Quaternion rotationValue = Quaternion.Euler(Vector3.forward * -angle);

        _transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationValue, turnSpeed * Time.deltaTime);

    }

    private void Shoot()
    {
        onShoot?.Invoke();

        Vector3 speed = (atackPoint.up * projectileSpeed);

        GameObject obj = PoolManager.SpawnObject(projectilePrefab, atackPoint.position, Quaternion.identity);

        Projectile projectile = obj.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.EnableProjectile(atackPoint.position, speed, Vector3.up * gravityValue, projectileLayerMask, useGroundLayer, groundLayer, projectileRadius, atackDamage,targetTag);
        }
        else
        {
            print("Object dont have Projectile script");
        }
    }


    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            if (verificationStartPoint)
            {
                Gizmos.DrawSphere(verificationStartPoint.position, verificationRadius);
            }
        }
    }

    public void SetTurnSpeed(float value)
    {
        turnSpeed = value;
    }
}
