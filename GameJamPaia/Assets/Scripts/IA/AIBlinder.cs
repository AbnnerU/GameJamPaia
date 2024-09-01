using Assets.Scripts.BT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBlinder : MonoBehaviour, IHasBehaviourTree, IAgentMovementState, IAIState
{
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected AIState currentState = AIState.SPAWNING;
    [SerializeField] protected bool playOnStart;
    [SerializeField] protected Transform cameraTransform;
    [SerializeField] protected Collider2D agentCollider;
    [SerializeField] protected MapManager mapManager;
    [SerializeField] private VisionClamp visionClamp;
    [Header("Animation")]
    [SerializeField] private Animator AIAnimator;
    [SerializeField] private string idleAnimation;
    [SerializeField] private string walkAnimation;

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
    [SerializeField] private float speed;
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
    [SerializeField] private float startFollowDelay;
    [SerializeField] protected Vector3 stickOnTargetOffset;
    [SerializeField] private Animator anim;
    [SerializeField] private string sawTheTargetAnimation;


    [Header("Random Point")]
    [SerializeField] private float goToRandomPointRadius;
    [SerializeField] private float randomPointSpeed;
    [SerializeField] private float distanceToTarget;
    [Header("Sound")]
    [SerializeField] private AudioChannel audioChannel;
    [SerializeField] private AudioConfig[] seeTargetSound;
    [SerializeField] private AudioConfig[] runAfterTargetSound;
    [SerializeField] private AudioConfig spawnSound;


    private Transform _transform;
    protected HealthManager targetHealth;
    //protected Transform[] transformsArray;
    //protected Vector3[] offSetArray;

    protected BTSelector rootSelector;

    protected bool passingNavMeshLink = false;


    protected virtual void Awake()
    {
        currentState = AIState.SPAWNING;
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

        _transform = transform;

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        agent.enabled = false;

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (mapManager == null)
            mapManager = FindAnyObjectByType<MapManager>();

        if (visionClamp == null)
            visionClamp = FindAnyObjectByType<VisionClamp>();

        //if (gameManager == null)
        //    gameManager = FindObjectOfType<GameManager>();

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

        targetHealth = target.GetComponent<HealthManager>();

        mapManager.AddNewAgent(agent.transform);

        currentState = AIState.SPAWNING;
    }


    public void StartBehaviourTree()
    {
        BTDoAction btIdleAnimation = new BTDoAction(() => PlayerAIAnimation(idleAnimation));
        BTDoAction btWalkAnimation = new BTDoAction(() => PlayerAIAnimation(walkAnimation));

        BTDoAction btEnableAgentColliderAction = new BTDoAction(() => SetColliderActive(true));
        BTDoAction btDisableAgentColliderAction = new BTDoAction(() => SetColliderActive(false));

        BTIsTargetAlive bTIsTargetAlive = new BTIsTargetAlive(targetHealth);
        BTInverter btIsTargetDead = new BTInverter(bTIsTargetAlive);
        BTStopAgent bTStopAgent = new BTStopAgent(agent);
        BTSetAIState btSetFollowAIState = new BTSetAIState(this, AIState.FOLLOWTARGET);
        BTIsTargetNear bTIsTargetNear = new BTIsTargetNear(_transform, target, distanceToTarget);
        BTInverter btIsTargetFar = new BTInverter(bTIsTargetNear);
        BTDoAction btEnableBlindEffect = new BTDoAction(() => BlindEffect(true));
        BTDoAction btDisableBlindEffect = new BTDoAction(() => BlindEffect(false));

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
             btIdleAnimation,
            btDisableAgentColliderAction,
            btDisableRendersAction,
            btSpawningBehaviourAction,
            btSpawnDelay,
            btEnableRendersAction,
            btEnableAgentColliderAction,
            btSetFollowTargetState,
        });
        #endregion


        #region Stunned
        BTIsOnAIState btIsOnStunnedState = new BTIsOnAIState(this, AIState.STUNNED);
        BTDoAction btStunEffectAction = new BTDoAction(() => StunEffect());
        BTWaitForSeconds btStunnedTime = new BTWaitForSeconds(stunTime);
        BTSequence btStunnedSequence = new BTSequence(new List<BTnode>
        {
            btIsOnStunnedState,
            btIdleAnimation,
            btDisableBlindEffect,
            bTStopAgent,
            btStunEffectAction,
            btDisableAgentColliderAction,
            btStunnedTime,
            btEnableAgentColliderAction,
        });
        #endregion


        #region IsTargetInArea
        BTIsPathCompleteStatus bTIsPathCompleteStatus = new BTIsPathCompleteStatus(target, agent);
        BTIsColliderEnabled bTIsColliderEnabled = new BTIsColliderEnabled(targetCollider);
        BTIsOnAIState btIsOnFollowTargetState = new BTIsOnAIState(this, AIState.FOLLOWTARGET);
        BTDoAction btSawTargetAction = new BTDoAction(() => SawTargetAction());
        BTWaitForSeconds btStartFollowDelay = new BTWaitForSeconds(startFollowDelay);
        BTConditionalSequence btStartFollowCondicionalSequence = new BTConditionalSequence(new List<BTnode> { btIsOnFollowTargetState, bTIsPathCompleteStatus }, btStartFollowDelay);
        BTDoAction btRunAfterTargetAction = new BTDoAction(() => RunAfterTargetAction());
        BTFollowTarget bTFollowTarget = new BTFollowTarget(target, agent, this, minDistance, followTargetUpdateTime, speed);
        BTConditionalSequence bTFollowTargetCondicionalSequence = new BTConditionalSequence(new List<BTnode> { btIsOnFollowTargetState, bTIsColliderEnabled, bTIsPathCompleteStatus }, bTFollowTarget);
        BTSequence btFollowTargetSequence = new BTSequence(new List<BTnode> {
            bTIsPathCompleteStatus,
            bTIsTargetAlive,
            bTIsColliderEnabled,
            btSetFollowTargetState,
            btSawTargetAction,
            btIdleAnimation,
            btStartFollowCondicionalSequence,
            btWalkAnimation,
            btRunAfterTargetAction,
            btEnableAgentColliderAction,
            bTFollowTargetCondicionalSequence
        });

        #endregion


        #region Stick
        BTStickToTheTarget bTStickToTheTarget = new BTStickToTheTarget(target, _transform, stickOnTargetOffset, 0);
        BTIsOnAIState btIsOnHittedTargetState = new BTIsOnAIState(this, AIState.HITTEDTARGET);
        BTConditionalSequence bTStickCondicionalSequence = new BTConditionalSequence(new List<BTnode> { bTIsPathCompleteStatus, bTIsColliderEnabled, bTIsTargetAlive }, bTStickToTheTarget);
        BTSequence btStickSequence = new BTSequence(new List<BTnode>
        {
            bTIsPathCompleteStatus,
            btIsOnHittedTargetState,
            btIdleAnimation,
            btEnableBlindEffect,
            bTStickCondicionalSequence
        });
        #endregion 


        #region TargetOutsideArea
        BTInverter bTIsNotPathCompleteStatus = new BTInverter(bTIsPathCompleteStatus);
        BTIsAIStateDifferentOf bTIsAIStateDifferentOfStopped = new BTIsAIStateDifferentOf(this, AIState.STOPPED);
        BTSetAIState btSetStoppedState = new BTSetAIState(this, AIState.STOPPED);
        BTDodge bTDodge = new BTDodge(_transform, agent, goToRandomPointRadius, randomPointSpeed, speed, 1, 0.1f, this);
        BTConditionalSequence btGoToRandomPointCondicionalSequence = new BTConditionalSequence(new List<BTnode> { bTIsNotPathCompleteStatus }, bTDodge);
        BTSequence btGoToRandomPointSequence = new BTSequence(new List<BTnode>
        {
            bTIsNotPathCompleteStatus,
            bTIsAIStateDifferentOfStopped,
            btDisableBlindEffect,
            btIdleAnimation,
            btGoToRandomPointCondicionalSequence,
            btSetStoppedState,
            bTStopAgent
        });
        #endregion

        rootSelector = new BTSelector(new List<BTnode>
        {
            btSpawnSequence,
            btFollowTargetSequence,
            btStickSequence,
            btGoToRandomPointSequence
        });

        behaviorTree.SetActive(true);

        behaviorTree.SetBehaviorRoot(rootSelector);

        behaviorTree.SetExecutionInterval(executionInterval);

        behaviorTree.StartCoroutine(behaviorTree.Begin());
    }

    private void PlayerAIAnimation(string animationName)
    {
        AIAnimator.Play(animationName, 0, 0);
    }

    protected virtual void ChangeRendersActiveState(bool active)
    {
        for (int i = 0; i < sprites.Length; i++)
            sprites[i].enabled = active;
    }

    private void BlindEffect(bool setActive)
    {
        if (setActive)
            visionClamp.Enable();
        else
            visionClamp.Disable();
    }

    private void SawTargetAction()
    {
        anim.Play(sawTheTargetAnimation, 0, 0);

        int index = Random.Range(0, seeTargetSound.Length);

        audioChannel.AudioRequest(seeTargetSound[index], _transform.position);
    }

    private void RunAfterTargetAction()
    {
        int index = Random.Range(0, runAfterTargetSound.Length);

        audioChannel.AudioRequest(runAfterTargetSound[index], _transform.position);
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
        //
        //if (!passingNavMeshLink && isOnNavMeshLink)
        //{
        //    // spriteObj.SetActive(false);
        //    passingNavMeshLink = true;

        //    StartCoroutine(PassOffMeshLink());
        //}
        //else if (passingNavMeshLink && !isOnNavMeshLink)
        //{
        //    //spriteObj.SetActive(true);
        //    passingNavMeshLink = false;

        //    StopAllCoroutines();
        //    //particlesRef.Stop();
        //}
    }

    //protected IEnumerator PassOffMeshLink()
    //{
    //    SetColliderActive(false);
    //    float currentTime = 0;
    //    Vector3 offMeshEndPosition = agent.currentOffMeshLinkData.endPos;

    //    particlesTransform.position = offMeshEndPosition;
    //    particlesRef.Play();

    //    do
    //    {
    //        currentTime += Time.deltaTime;

    //        if (agent.currentOffMeshLinkData.endPos != offMeshEndPosition && currentState == AIState.FOLLOWTARGET)
    //        {
    //            SetColliderActive(true);
    //            yield break;
    //        }
    //        else if (currentState == AIState.STUNNED)
    //        {
    //            yield break;
    //        }

    //        yield return null;

    //    } while (currentTime < passOffMeshLinkDelay);


    //    agent.CompleteOffMeshLink();
    //    SetColliderActive(true);
    //}

    public AIState GetCurrentAIState()
    {
        return currentState;
    }

    public void SetAIState(AIState newState)
    {
        currentState = newState;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag) && currentState == AIState.FOLLOWTARGET && targetCollider.enabled && !passingNavMeshLink)
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


    protected virtual void StunEffect()
    {
        //print("Pika");
        stunEffect.Play();
    }



    private void OnDrawGizmos()
    {

    }
}