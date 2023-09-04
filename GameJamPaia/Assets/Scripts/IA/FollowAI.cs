using Assets.Scripts.BT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowAI : MonoBehaviour, IAgentMovementState
{
    [SerializeField] private NavMeshAgent agent;
    [Header("BT")]
    [SerializeField] private BehaviorTree behaviorTree;
    [SerializeField] private GameObject spriteObj;
    [SerializeField] private float executionInterval;
    [SerializeField] private Transform target;
    [SerializeField] private float followTargetUpdateTime;
    [SerializeField] private float minDistance;

    private BTSelector rootSelector;

    private bool spriteActive = true;
 

    private void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        StartBehaviourTree();
    }

    private void StartBehaviourTree()
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

    public void AgentIsOnNavMeshLink(bool isOnNavMeshLink)
    {
        if(spriteActive && isOnNavMeshLink)
        {
            spriteObj.SetActive(false);
            spriteActive = false;
        }
        else if(!spriteActive && !isOnNavMeshLink)
        {
            spriteObj.SetActive(true);
            spriteActive = true;
        }
    }
}
