using Assets.Scripts.BT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIResetScore : MonoBehaviour,IAgentMovementState, IAIState
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private AIState currentState = AIState.FOLLOWTARGET;
    [SerializeField] private bool playOnStart;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform target;
    [SerializeField] private PlayerMovement targetMovement;
    [SerializeField] private SimpleAnimationManager targetAnimator;
    [SerializeField] private string targetTag = "player";
    [Header("BT")]
    [SerializeField] private BehaviorTree behaviorTree;
    [SerializeField] private GameObject spriteObj;
    [SerializeField] private float executionInterval;
    [Header("FollowConfig")]
    [SerializeField] private float followTargetUpdateTime;
    [SerializeField] private float minDistance;
    [SerializeField] private float passOffMeshLinkDelay;
    [Header("Teleport")]
    [SerializeField] private MapManager mapManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private string teleportPlayerAnimation;
    [SerializeField] private float teleportPlayerAnimationDuration;
    [SerializeField] private float releasePlayerAnimationDuration;
    [Header("Score")]
    [SerializeField] private GameScore scoreRef;
    [SerializeField] private string scoreTag = "Coin";
    //[SerializeField] private Transform particlesTransform;
    //[SerializeField] private ParticleSystem particlesRef;

    private Transform[] transformsArray;
    private Vector3[] offSetArray;

    private BTSelector rootSelector;

    private bool spriteActive = true;

    private void Awake()
    {
        transformsArray = new Transform[2];
        offSetArray = new Vector3[2];
    }

    private void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (scoreRef == null)
            scoreRef = GameObject.FindGameObjectWithTag(scoreTag).GetComponent<GameScore>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (mapManager == null)
            mapManager = FindAnyObjectByType<MapManager>();

        if (gameManager==null)
            gameManager = FindObjectOfType<GameManager>();


        if (target == null)
            target = GameObject.FindGameObjectWithTag(targetTag).transform;

        if (targetAnimator == null)
            targetAnimator = target.GetComponent<SimpleAnimationManager>();

        if (targetMovement == null)
            targetMovement = target.GetComponent<PlayerMovement>();

        transformsArray[0] = target;
        offSetArray[0] = new Vector3(0, 0, 0);

        transformsArray[1] = cameraTransform;
        offSetArray[1] = new Vector3(0, 0, -10);

        if (playOnStart)
            StartBehaviourTree();
    }

    public void StartBehaviourTree()
    {
        BTIsOnAIState bTIsOnHittedTargetState = new BTIsOnAIState(this, AIState.HITTEDTARGET);
        BTStopAgent bTStopAgent = new BTStopAgent(agent);
        BTDoAction bTTeleportPlayerAnimation = new BTDoAction(() => TeleportPlayerAnimation());
        BTWaitForSeconds bTWaitTeleportPlayerAnimation = new BTWaitForSeconds(teleportPlayerAnimationDuration);
        BTDoAction bTTeleportTargetAction = new BTDoAction(() => TeleportTarget());
        BTWaitForSeconds bTWaitReleasePlayerAnimation = new BTWaitForSeconds(releasePlayerAnimationDuration);
        BTDoAction bTUnpauseAlarmsAction = new BTDoAction(() => UnpauseAlrams());
        BTSetAIState btSetFollowAIState = new BTSetAIState(this, AIState.FOLLOWTARGET);
        BTSequence bTTeleportPlayerSequence = new BTSequence(new List<BTnode>
        {
            bTIsOnHittedTargetState,
            bTStopAgent,
            bTTeleportPlayerAnimation,
            bTWaitTeleportPlayerAnimation,
            bTTeleportTargetAction,
            bTWaitReleasePlayerAnimation,
            bTUnpauseAlarmsAction,
            btSetFollowAIState
        });

        BTIsOnAIState btIsOnFollowTargetState = new BTIsOnAIState(this, AIState.FOLLOWTARGET);
        BTFollowTarget bTFollowTarget = new BTFollowTarget(target, agent, this, minDistance, followTargetUpdateTime);
        BTConditionalSequence bTFollowTargetCondicionalSequence = new BTConditionalSequence(new List<BTnode> { btIsOnFollowTargetState }, bTFollowTarget);

        BTSequence btFollowTargetSequence = new BTSequence(new List<BTnode> {
            btIsOnFollowTargetState,
            bTFollowTargetCondicionalSequence

        });


        //----Root----
        rootSelector = new BTSelector(new List<BTnode>
        {
            bTTeleportPlayerSequence,
            btFollowTargetSequence
        });

        behaviorTree.SetActive(true);

        behaviorTree.SetBehaviorRoot(rootSelector);

        behaviorTree.SetExecutionInterval(executionInterval);

        behaviorTree.StartCoroutine(behaviorTree.Begin());

    }

    private void TeleportPlayerAnimation()
    {
        if (targetAnimator)
        {
            targetMovement.Disable();
            targetAnimator.SetAnimationManagerActive(false);
            targetAnimator.PlayAnimation(teleportPlayerAnimation);
        }
    }

    private void TeleportTarget()
    {
        if (target && mapManager && gameManager)
        {
            gameManager.PauseAlarmsEnableDelay(true);
            mapManager.SetRandomRoom(transformsArray, offSetArray);

            scoreRef.RemoveAll();
        }
    }

    private void UnpauseAlrams()
    {
        if (gameManager)
        {
            gameManager.PauseAlarmsEnableDelay(false);

            targetMovement.Enable();

            targetAnimator.SetAnimationManagerActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag) && currentState != AIState.HITTEDTARGET)
        {
            currentState = AIState.HITTEDTARGET;
        }
    }


    public void AgentIsOnNavMeshLink(bool isOnNavMeshLink)
    {
        if (spriteActive && isOnNavMeshLink)
        {
            spriteObj.SetActive(false);
            spriteActive = false;

            StartCoroutine(PassOffMeshLink());
        }
        else if (!spriteActive && !isOnNavMeshLink)
        {
            spriteObj.SetActive(true);
            spriteActive = true;

            StopAllCoroutines();
            //particlesRef.Stop();
        }
    }

    IEnumerator PassOffMeshLink()
    {
        float currentTime = 0;
        Vector3 offMeshEndPosition = agent.currentOffMeshLinkData.endPos;

        //particlesTransform.position = offMeshEndPosition;
        //particlesRef.Play();

        do
        {
            currentTime += Time.deltaTime;

            if (agent.currentOffMeshLinkData.endPos != offMeshEndPosition)
            {
                yield break;
            }

            yield return null;

        } while (currentTime < passOffMeshLinkDelay);

        agent.CompleteOffMeshLink();
    }

    public AIState GetCurrentAIState()
    {
        return currentState;
    }

    public void SetAIState(AIState newState)
    {
        currentState = newState;
    }
}
