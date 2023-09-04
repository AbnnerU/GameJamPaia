using Assets.Scripts.BT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowAI : MonoBehaviour, IAgentMovementState
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private bool playOnStart;
    [Header("BT")]
    [SerializeField] private BehaviorTree behaviorTree;
    [SerializeField] private GameObject spriteObj;
    [SerializeField] private float executionInterval;
    [SerializeField] private Transform target;
    [SerializeField] private float followTargetUpdateTime;
    [SerializeField] private float minDistance;
    [SerializeField] private float passOffMeshLinkDelay;
    [SerializeField] private Transform particlesTransform;
    [SerializeField] private ParticleSystem particlesRef;

    private BTSelector rootSelector;

    private bool spriteActive = true;
 

    private void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        if(playOnStart)
            StartBehaviourTree();
    }

    public void StartBehaviourTree()
    { 
        BTFollowTarget bTFollowTarget = new BTFollowTarget(target, agent, this, minDistance, followTargetUpdateTime);
        BTWaitForSeconds bTWaitForSeconds = new BTWaitForSeconds(1);
        BTSequence btFollowTargetSequence = new BTSequence(new List<BTnode> {
            bTFollowTarget, 
            bTWaitForSeconds 
        });

        //----Root----
        rootSelector = new BTSelector(new List<BTnode>
        {
            btFollowTargetSequence
        });

        behaviorTree.SetActive(true);

        behaviorTree.SetBehaviorRoot(rootSelector);

        behaviorTree.SetExecutionInterval(executionInterval);

        behaviorTree.StartCoroutine(behaviorTree.Begin());

    }

    //private void Update()
    //{
    //    if (particles)
    //    {
    //        if (agent.isOnOffMeshLink)
    //            particles.position = agent.nextOffMeshLinkData.endPos;
    //    }
    //}

    public void AgentIsOnNavMeshLink(bool isOnNavMeshLink)
    {
        if(spriteActive && isOnNavMeshLink)
        {
            spriteObj.SetActive(false);
            spriteActive = false;

            StartCoroutine(PassOffMeshLink());
        }
        else if(!spriteActive && !isOnNavMeshLink)
        {
            spriteObj.SetActive(true);
            spriteActive = true;

            StopAllCoroutines();
            particlesRef.Stop();
        }
    }

    IEnumerator PassOffMeshLink()
    {
        float currentTime = 0;
        Vector3 offMeshEndPosition = agent.currentOffMeshLinkData.endPos;

        particlesTransform.position = offMeshEndPosition;
        particlesRef.Play();

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
}
