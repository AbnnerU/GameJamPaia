using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private bool drawGizmos;

    [SerializeField] private bool active;

    [SerializeField] private Vector3 gravityValue = Physics.gravity;

    [SerializeField] private Vector3 projectileVelocity;

    [SerializeField] private int damage;

    [SerializeField] private string targetTag;

    [SerializeField] private LayerMask layerMask;

    [SerializeField] private bool useGroundLayer;

    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private int predictionPerFrame=6;

    [SerializeField] private float projectileRadius;

    [SerializeField] private int maxAllocation;

    public Action<IHittable> OnHitDamageableTarget;

    private Collider2D[] hitResults;

    private Transform _transform;

    private Vector3 point1;

    private bool paused = false;

    private void Awake()
    {
        hitResults = new Collider2D[maxAllocation];

        _transform = GetComponent<Transform>();

    }

    private void OnDisable()
    {
        active = false;
    }

    private void Update()
    {
        if (active == false)
            return;

        if (active)
        {
            if (paused)
                return;

            float currentDeltaTime = Time.deltaTime;

            point1 = transform.position;

            float totalPredictions = 1f / predictionPerFrame;

            for (float i = 0; i < 1f; i += totalPredictions)
            {
                if (paused)
                    return;

                projectileVelocity += gravityValue * (totalPredictions * currentDeltaTime);

                Vector3 point2 = point1 + projectileVelocity * (totalPredictions * currentDeltaTime);

                Vector3 direction = point2 - point1;

                int hits = Physics2D.OverlapCircleNonAlloc(point1, projectileRadius, hitResults,layerMask,0,10);

                if (hits > 0)
                {
                    Collider2D collider = hitResults[0];

                    if (collider != null)
                    {
                        if (useGroundLayer)
                        {
                            if ((groundLayer & 1 << collider.gameObject.layer) == 1 << collider.gameObject.layer)
                            {
                                PoolManager.ReleaseObject(gameObject);

                                break;
                            }
                        }

                        if (collider.CompareTag(targetTag))
                        {
                            OnHit(collider.gameObject);
                            PoolManager.ReleaseObject(gameObject);

                            break;
                        }

                    }
                }

                point1 = point2;

            }

            _transform.position = point1;

        }
    }


    public void EnableProjectile(Vector3 startPoint,Vector3 speedOrietation,Vector3 gravityValue, LayerMask layerMask, bool useGroundLayer, LayerMask groundLayer, float radius,int damage,string targetTag)
    {
        point1 = startPoint;

        projectileVelocity = speedOrietation;

        this.gravityValue = gravityValue;

        projectileRadius = radius;

        this.damage = damage;

        active = true;

        this.targetTag = targetTag;

        this.useGroundLayer = useGroundLayer;

        this.groundLayer = groundLayer;

        this.layerMask = layerMask;
    }

    private void OnHit(GameObject obj)
    {
        IHittable damageable = obj.GetComponent<IHittable>();

        if (damageable != null && obj.CompareTag(targetTag))
        {
            damageable.OnHit(damage);

            OnHitDamageableTarget?.Invoke(damageable);
        }
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.DrawSphere(transform.position, projectileRadius);
        }
        
    }

}
