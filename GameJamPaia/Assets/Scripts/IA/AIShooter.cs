using Assets.Scripts.BT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIShooter : MonoBehaviour, IHasBehaviourTree, IAgentMovementState, IAIState, IAIShooter
{
    [SerializeField] private bool drawGizmos;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected AIState currentState = AIState.SPAWNING;
    [SerializeField] protected bool playOnStart;
    [SerializeField] protected Transform cameraTransform;
    [SerializeField] protected Collider2D agentCollider;
    [SerializeField] protected MapManager mapManager;
    [Header("Player Info")]
    [SerializeField] protected Transform target;
    [SerializeField] protected Collider2D targetCollider;
    [SerializeField] protected PlayerMovement targetMovement;
    [SerializeField] protected SimpleAnimationManager targetAnimator;
    [SerializeField] protected string targetTag = "player";
    [Header("Shoot")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private int projectileDamage;
    [SerializeField] private float reloadTime;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float gravityValue = Physics.gravity.y;
    [SerializeField] private float projectileRadius;
    [SerializeField] private LayerMask projectileLayerMask;
    [SerializeField] private bool useGroundLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float shootDelay;
    [SerializeField] private float timeStoppedAfterShoot;
    [Header("BT")]
    [SerializeField] protected BehaviorTree behaviorTree;
    [SerializeField] protected GameObject spriteObj;
    [SerializeField] protected float executionInterval;
    [SerializeField] private float speed;
    [Header("Spawn")]
    [SerializeField] protected ParticleSystem spawnParticle;
    [SerializeField] protected float spawnDelay = 2;
    [SerializeField] protected SpriteRenderer[] sprites;
    [Header("FollowConfig")]
    [SerializeField] protected float followTargetUpdateTime;
    [SerializeField] protected float minDistance;
    [SerializeField] protected float passOffMeshLinkDelay;
    [SerializeField] protected Transform particlesTransform;
    [SerializeField] protected ParticleSystem particlesRef;
    [SerializeField] private ParticleSystem dodgeParticle;
    [Header("Dodge")]
    [SerializeField] private float distanceToDodge;
    [SerializeField] private float dodgeRadius;
    [SerializeField] private float dodgeSpeed;
    [SerializeField] private float timeStoppedAfterDodge;
    [Header("Sound")]
    [SerializeField] protected AudioChannel audioChannel;
    [SerializeField] protected AudioConfig[] passTheMeshLinkSound;
    [SerializeField] protected AudioConfig shootSound;
    [SerializeField] protected AudioConfig spawnSound;
    [SerializeField] private AudioConfig[] laughSounds;

    private Transform _transform;
    protected HealthManager targetHealth;
    //protected Transform[] transformsArray;
    //protected Vector3[] offSetArray;

    protected BTSelector rootSelector;

    protected bool passingNavMeshLink = false;

    protected bool canShoot = true;

    protected virtual void Awake()
    {
        //transformsArray = new Transform[2];
        // offSetArray = new Vector3[2];
    }

    protected virtual void Start()
    {
        if (playOnStart)
        {
            Setup();
            StartBehaviourTree();
        }
    }

    public virtual void Setup()
    {
        canShoot = true;

        _transform = transform;

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        agent.enabled = false;

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (mapManager == null)
            mapManager = FindAnyObjectByType<MapManager>();

        //if (gameManager == null)
        //    gameManager = FindObjectOfType<GameManager>();

        if (target == null)
            target = GameObject.FindGameObjectWithTag(targetTag).transform;

        if (targetAnimator == null)
            targetAnimator = target.GetComponent<SimpleAnimationManager>();

        if (targetMovement == null)
            targetMovement = target.GetComponent<PlayerMovement>();

        //if (shield == null)
        //    shield = FindObjectOfType<Shield>();

        if (targetCollider == null)
            targetCollider = target.GetComponent<Collider2D>();

        targetHealth = target.GetComponent<HealthManager>();

        mapManager.AddNewAgent(agent.transform);

        currentState = AIState.SPAWNING;
    }


    public void StartBehaviourTree()
    {
        BTDoAction btEnableAgentColliderAction = new BTDoAction(() => SetColliderActive(true));
        BTDoAction btDisableAgentColliderAction = new BTDoAction(() => SetColliderActive(false));

        BTIsTargetAlive bTIsTargetAlive = new BTIsTargetAlive(targetHealth);
        BTInverter btIsTargetDead = new BTInverter(bTIsTargetAlive);
        BTIsTargetNear bTIsTargetNear = new BTIsTargetNear(_transform, target, distanceToDodge);
        BTInverter btIsTargetFar = new BTInverter(bTIsTargetNear);
        BTStopAgent bTStopAgent = new BTStopAgent(agent);

        #region Spawn
        BTIsOnAIState btIsOnSpawningState = new BTIsOnAIState(this, AIState.SPAWNING);
        BTDoAction btDisableRendersAction = new BTDoAction(() => ChangeRendersActiveState(false));
        BTDoAction btSpawningBehaviourAction = new BTDoAction(() => Spawn());
        BTWaitForSeconds btSpawnDelay = new BTWaitForSeconds(spawnDelay);
        BTDoAction btEnableRendersAction = new BTDoAction(() => ChangeRendersActiveState(true));
        BTSetAIState btSetFollowTargetState = new BTSetAIState(this, AIState.FOLLOWTARGET);
        BTSequence btSpawnSequence = new BTSequence(new List<BTnode>
        {
            btIsOnSpawningState,
            btDisableAgentColliderAction,
            btDisableRendersAction,
            btSpawningBehaviourAction,
            btSpawnDelay,
            btEnableRendersAction,
            btEnableAgentColliderAction,
            btSetFollowTargetState
        });
        #endregion


        BTDoAction btLaughAction = new BTDoAction(() => Laugh());
        BTDodge bTDodge = new BTDodge(agent.transform, agent, dodgeRadius, speed, dodgeSpeed, 0.5f, 0.01f, this);
        BTWaitForSeconds btTimeStoppedAfterDodge = new BTWaitForSeconds(timeStoppedAfterDodge);
        BTSetAIState btSetFollowTargetAIState = new BTSetAIState(this, AIState.FOLLOWTARGET);
        BTSequence btDodgeSequence = new BTSequence(new List<BTnode>
        {
            bTIsTargetNear,
            btLaughAction,
            bTDodge,
            btTimeStoppedAfterDodge,
            btSetFollowTargetAIState
        });

        BTIsOnAIState btIsOnShootingState = new BTIsOnAIState(this, AIState.SHOOTING);
        BTAICanShoot bTAICanShoot = new BTAICanShoot(this);
        BTWaitForSeconds btDelayToShoot = new BTWaitForSeconds(shootDelay);
        BTDoAction btShoot = new BTDoAction(() => Shoot());
        BTWaitForSeconds btTimeStoppedAfterShoot = new BTWaitForSeconds(timeStoppedAfterShoot);
        BTSequence btShootingSequence = new BTSequence(new List<BTnode>
        {
            btIsTargetFar,
            btIsOnShootingState,
            bTAICanShoot,
            btDelayToShoot,
            btShoot,
            btTimeStoppedAfterShoot,
            btSetFollowTargetAIState
        });

        BTIsColliderEnabled bTIsColliderEnabled = new BTIsColliderEnabled(targetCollider);
        BTIsOnAIState btIsOnFollowTargetState = new BTIsOnAIState(this, AIState.FOLLOWTARGET);
        BTFollowTarget bTFollowTarget = new BTFollowTarget(target, agent, this, minDistance, followTargetUpdateTime, speed);
        BTConditionalSequence bTFollowTargetCondicionalSequence = new BTConditionalSequence(new List<BTnode> { btIsOnFollowTargetState, bTIsColliderEnabled, btIsTargetFar }, bTFollowTarget);
        BTSetAIState btSetShootingAIState = new BTSetAIState(this, AIState.SHOOTING);
        BTSequence btFollowTargetSequence = new BTSequence(new List<BTnode> {
            bTIsTargetAlive,
            btIsTargetFar,
            btIsOnFollowTargetState,
            bTIsColliderEnabled,
            btEnableAgentColliderAction,
            bTFollowTargetCondicionalSequence,
            btSetShootingAIState,
            bTStopAgent

        });

        rootSelector = new BTSelector(new List<BTnode>
        {
            btSpawnSequence,
             btDodgeSequence,
            btShootingSequence,
            btFollowTargetSequence

        });

        behaviorTree.SetActive(true);

        behaviorTree.SetBehaviorRoot(rootSelector);

        behaviorTree.SetExecutionInterval(executionInterval);

        behaviorTree.StartCoroutine(behaviorTree.Begin());
    }

    private void Laugh()
    {
        audioChannel.AudioRequest(laughSounds[Random.Range(0,laughSounds.Length)], _transform.position);
        dodgeParticle.Play();
    }

    protected virtual void ChangeRendersActiveState(bool active)
    {
        for (int i = 0; i < sprites.Length; i++)
            sprites[i].enabled = active;
    }

    protected virtual void Spawn()
    {
        SetColliderActive(false);
        spawnParticle.Play();
        audioChannel.AudioRequest(spawnSound, _transform.position);
    }

    protected virtual void SetColliderActive(bool active)
    {
        agentCollider.enabled = active;
    }

    //protected virtual void StunEffect()
    //{
    //    //print("Pika");
    //    stunEffect.Play();
    //}

    //protected virtual void TargetWasCaughtAnimation()
    //{
    //    if (targetAnimator)
    //    {
    //        targetMovement.Disable();
    //        targetCollider.enabled = false;
    //        targetAnimator.SetAnimationManagerActive(false);
    //        targetAnimator.PlayAnimation(teleportPlayerAnimation);

    //        gameManager.PauseAlarmsLoop(true);
    //    }
    //}

    //protected virtual void ReleaseTargetAnimations()
    //{
    //    if (target && mapManager && gameManager)
    //    {
    //        int id = 0;
    //        targetAnimator.PlayAnimation(releasePlayerAnimation);

    //        mapManager.RandomRoomNoRepeatAndAlarmOff(target, Vector3.zero, out id);
    //        mapManager.SetObjectInRoom(cameraTransform, new Vector3(0, 0, -10), id);
    //    }
    //}

    //protected virtual void UnpauseAlrams()
    //{
    //    if (gameManager)
    //    {
    //        gameManager.PauseAlarmsLoop(false);

    //        targetMovement.Enable();
    //        targetCollider.enabled = true;
    //        targetAnimator.SetAnimationManagerActive(true);
    //    }
    //}

    public virtual void AgentIsOnNavMeshLink(bool isOnNavMeshLink)
    {
        if (!passingNavMeshLink && isOnNavMeshLink)
        {
            // spriteObj.SetActive(false);
            passingNavMeshLink = true;

            StartCoroutine(PassOffMeshLink());
        }
        else if (passingNavMeshLink && !isOnNavMeshLink)
        {
            //spriteObj.SetActive(true);
            passingNavMeshLink = false;

            StopAllCoroutines();
            //particlesRef.Stop();
        }
    }

    protected IEnumerator PassOffMeshLink()
    {
        SetColliderActive(false);
        float currentTime = 0;
        Vector3 offMeshEndPosition = agent.currentOffMeshLinkData.endPos;

        particlesTransform.position = offMeshEndPosition;
        particlesRef.Play();

        int index = Random.Range(0, passTheMeshLinkSound.Length);
        audioChannel.AudioRequest(passTheMeshLinkSound[index], offMeshEndPosition);


        do
        {
            currentTime += Time.deltaTime;

            if (agent.currentOffMeshLinkData.endPos != offMeshEndPosition && currentState == AIState.FOLLOWTARGET)
            {
                SetColliderActive(true);
                yield break;
            }
            else if (currentState == AIState.STUNNED)
            {
                yield break;
            }

            yield return null;

        } while (currentTime < passOffMeshLinkDelay);


        agent.CompleteOffMeshLink();
        SetColliderActive(true);
    }

    public AIState GetCurrentAIState()
    {
        return currentState;
    }

    public void SetAIState(AIState newState)
    {
        currentState = newState;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag) && currentState != AIState.HITTEDTARGET && currentState != AIState.STUNNED && targetCollider.enabled && !passingNavMeshLink)
        {

        }
    }

    public void Shoot()
    {
        if (canShoot)
        {
            Vector3 direction = (target.position - shootPoint.position);
            direction.Normalize();

            Vector3 speed = direction * projectileSpeed;

            GameObject obj = PoolManager.SpawnObject(projectilePrefab, shootPoint.position, Quaternion.identity);

            Projectile projectile = obj.GetComponent<Projectile>();

            if (projectile != null)
            {
                projectile.EnableProjectile(shootPoint.position, speed, Vector3.up * gravityValue, projectileLayerMask, useGroundLayer, groundLayer, projectileRadius, projectileDamage, targetTag);

                audioChannel.AudioRequest(shootSound, shootPoint.position);
            }
            else
            {
                print("Object dont have Projectile script");
            }

            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        canShoot = false;
        yield return new WaitForSeconds(reloadTime);
        canShoot = true;
    }

    public bool CanShoot()
    {
        return canShoot;
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.DrawSphere(transform.position, distanceToDodge);
        }

    }
}