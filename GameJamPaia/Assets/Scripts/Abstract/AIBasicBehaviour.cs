using Assets.Scripts.BT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AIBasicBehaviour : MonoBehaviour, IHasBehaviourTree, IAgentMovementState, IAIState
{
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected AIState currentState = AIState.SPAWNING;
    [SerializeField] protected bool playOnStart;
    [SerializeField] protected Transform cameraTransform;
    [SerializeField] protected Collider2D agentCollider;
    [Header("Player Info")]
    [SerializeField] protected Transform target;
    [SerializeField] protected Collider2D targetCollider;
    [SerializeField] protected PlayerMovement targetMovement;
    [SerializeField] protected SimpleAnimationManager targetAnimator;
    [SerializeField] protected string targetTag = "player";
    [Header("BT")]
    [SerializeField] protected BehaviorTree behaviorTree;
    [SerializeField] protected GameObject spriteObj;
    [SerializeField] protected float executionInterval;
    [Header("Spawn")]
    [SerializeField] protected ParticleSystem spawnParticle;
    [SerializeField] protected float spawnDelay = 2;
    [SerializeField] protected SpriteRenderer[] sprites;
    [Header("Shield")]
    [SerializeField] protected Shield shield;
    [SerializeField] protected float stunTime = 2;
    [SerializeField] protected ParticleSystem stunEffect;
    [Header("FollowConfig")]
    [SerializeField] protected float followTargetUpdateTime;
    [SerializeField] protected float minDistance;
    [SerializeField] protected float passOffMeshLinkDelay;
    [Header("Teleport")]
    [SerializeField] protected MapManager mapManager;
    [SerializeField] protected GameManager gameManager;
    [SerializeField] protected string teleportPlayerAnimation;
    [SerializeField] protected float teleportPlayerAnimationDuration;
    [SerializeField] protected string releasePlayerAnimation;
    [SerializeField] protected float releasePlayerAnimationDuration;
    [SerializeField] protected Transform particlesTransform;
    [SerializeField] protected ParticleSystem particlesRef;

    //protected Transform[] transformsArray;
    //protected Vector3[] offSetArray;

    protected BTSelector rootSelector;

    protected bool passingNavMeshLink = false;

    protected virtual void Awake()
    {
        //transformsArray = new Transform[2];
       // offSetArray = new Vector3[2];
    }

    protected virtual void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (mapManager == null)
            mapManager = FindAnyObjectByType<MapManager>();

        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (target == null)
            target = GameObject.FindGameObjectWithTag(targetTag).transform;

        if (targetAnimator == null)
            targetAnimator = target.GetComponent<SimpleAnimationManager>();

        if (targetMovement == null)
            targetMovement = target.GetComponent<PlayerMovement>();

        if(targetCollider == null)
            targetCollider = target.GetComponent<Collider2D>();

        //transformsArray[0] = target;
        //offSetArray[0] = new Vector3(0, 0, 0);

        //transformsArray[1] = cameraTransform;
        // offSetArray[1] = new Vector3(0, 0, -10);

        if (playOnStart)
            StartBehaviourTree();
    }

    public virtual void Setup()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        agent.enabled = false;

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (mapManager == null)
            mapManager = FindAnyObjectByType<MapManager>();

        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (target == null)
            target = GameObject.FindGameObjectWithTag(targetTag).transform;

        if (targetAnimator == null)
            targetAnimator = target.GetComponent<SimpleAnimationManager>();

        if (targetMovement == null)
            targetMovement = target.GetComponent<PlayerMovement>();

        if (shield == null)
            shield = FindObjectOfType<Shield>();

        if (targetCollider == null)
            targetCollider = target.GetComponent<Collider2D>();

        //transformsArray[0] = target;
        //offSetArray[0] = new Vector3(0, 0, 0);

        //transformsArray[1] = cameraTransform;
        //offSetArray[1] = new Vector3(0, 0, -10);


        mapManager.AddNewAgent(agent.transform);
    }

    public virtual void StartBehaviourTree()
    {
        BTDoAction btEnableAgentColliderAction = new BTDoAction(() => SetColliderActive(true));
        BTDoAction btDisableAgentColliderAction = new BTDoAction(() =>SetColliderActive(false));


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

        BTIsOnAIState btIsOnStunnedState = new BTIsOnAIState(this, AIState.STUNNED);
        BTStopAgent bTStopAgent = new BTStopAgent(agent);
        BTDoAction btStunEffectAction = new BTDoAction(() => StunEffect());
        BTWaitForSeconds btStunnedTime = new BTWaitForSeconds(stunTime);
        BTSetAIState btSetFollowAIState = new BTSetAIState(this, AIState.FOLLOWTARGET);
        BTSequence btStunnedSequence = new BTSequence(new List<BTnode>
        {
            btIsOnStunnedState,
            bTStopAgent,
            btStunEffectAction,
            btDisableAgentColliderAction,
            btStunnedTime,
            btEnableAgentColliderAction,
            btSetFollowAIState
        });

        BTIsOnAIState bTIsOnHittedTargetState = new BTIsOnAIState(this, AIState.HITTEDTARGET);
        BTDoAction bTTeleportPlayerAnimation = new BTDoAction(() => TargetWasCaughtAnimation());
        BTWaitForSeconds bTWaitTeleportPlayerAnimation = new BTWaitForSeconds(teleportPlayerAnimationDuration);
        BTDoAction bTTeleportTargetAction = new BTDoAction(() => ReleaseTargetAnimations());
        BTWaitForSeconds bTWaitReleasePlayerAnimation = new BTWaitForSeconds(releasePlayerAnimationDuration);
        BTDoAction bTUnpauseAlarmsAction = new BTDoAction(() => UnpauseAlrams());
        BTSequence bTTeleportPlayerSequence = new BTSequence(new List<BTnode>
        {
            bTIsOnHittedTargetState,
            bTStopAgent,
            bTTeleportPlayerAnimation,
            //bTIsOnHittedTargetState,
            bTWaitTeleportPlayerAnimation,
            bTTeleportTargetAction,
            bTWaitReleasePlayerAnimation,
            bTUnpauseAlarmsAction,
            btSetFollowAIState
        });

        BTIsColliderEnabled bTIsColliderEnabled = new BTIsColliderEnabled(targetCollider);
        BTIsOnAIState btIsOnFollowTargetState = new BTIsOnAIState(this, AIState.FOLLOWTARGET);
        BTFollowTarget bTFollowTarget = new BTFollowTarget(target, agent, this, minDistance, followTargetUpdateTime);
        BTConditionalSequence bTFollowTargetCondicionalSequence = new BTConditionalSequence(new List<BTnode> { btIsOnFollowTargetState, bTIsColliderEnabled }, bTFollowTarget);

        BTSequence btFollowTargetSequence = new BTSequence(new List<BTnode> {
            btIsOnFollowTargetState,
            bTIsColliderEnabled,
            btEnableAgentColliderAction,
            bTFollowTargetCondicionalSequence
        });


        //----Root----
        rootSelector = new BTSelector(new List<BTnode>
        {
            btSpawnSequence,
            btStunnedSequence,
            bTTeleportPlayerSequence,
            btFollowTargetSequence
        });

        behaviorTree.SetActive(true);

        behaviorTree.SetBehaviorRoot(rootSelector);

        behaviorTree.SetExecutionInterval(executionInterval);

        behaviorTree.StartCoroutine(behaviorTree.Begin());

    }

    protected virtual void ChangeRendersActiveState(bool active)
    {
        for(int i=0; i< sprites.Length; i++)
            sprites[i].enabled = active;
    }

    protected virtual void Spawn()
    {
        SetColliderActive(false);
        spawnParticle.Play();
    }

    protected virtual void SetColliderActive(bool active)
    {
        agentCollider.enabled = active;
    }

    protected virtual void StunEffect()
    {
        //print("Pika");
        stunEffect.Play();
    }

    protected virtual void TargetWasCaughtAnimation()
    {
        if (targetAnimator)
        {
            targetMovement.Disable();
            targetCollider.enabled = false;
            targetAnimator.SetAnimationManagerActive(false);
            targetAnimator.PlayAnimation(teleportPlayerAnimation);

            gameManager.PauseAlarmsLoop(true);
        }
    }

    protected virtual void ReleaseTargetAnimations()
    {
        if (target && mapManager && gameManager)
        {
            int id = 0;
            targetAnimator.PlayAnimation(releasePlayerAnimation);

            mapManager.RandomRoomNoRepeatAndAlarmOff(target, Vector3.zero, out id);
            mapManager.SetObjectInRoom(cameraTransform, new Vector3(0, 0, -10), id);
        }
    }

    protected virtual void UnpauseAlrams()
    {
        if (gameManager)
        {
            gameManager.PauseAlarmsLoop(false);

            targetMovement.Enable();
            targetCollider.enabled = true;
            targetAnimator.SetAnimationManagerActive(true);
        }
    }

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
        if (collision.CompareTag(targetTag) && currentState != AIState.HITTEDTARGET && currentState!=AIState.STUNNED && targetCollider.enabled && !passingNavMeshLink)
        {
            if (shield.IsShieldActive())
            {
                currentState = AIState.STUNNED;             
                shield.HitShield();
                agent.isStopped = true;
                SetColliderActive(false);
            }
            else
            {
                currentState = AIState.HITTEDTARGET;
            }
        }
    }



}
