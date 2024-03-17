using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.BT
{
    public class BTIsPathCompleteStatus : BTnode
    {
        private Transform target;
        private NavMeshAgent agent;
        private Transform agentTransform;

        public BTIsPathCompleteStatus(Transform target, NavMeshAgent agent)
        {
            this.target = target;
            this.agent = agent;

            agentTransform = agent.transform;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if(agent == null || target == null){
                status = BTstatus.FAILURE;
                yield break;
            }

            Vector3 pointPosition = target.position;

            if (agent.isOnNavMesh == false || agent.enabled == false)
            {
                float placementAttempts = 0;

                do
                {
                    placementAttempts++;

                    agent.enabled = false;

                    if (placementAttempts > 3)
                    {
                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(agentTransform.position, out hit, 100f, NavMesh.AllAreas))
                        {
                            Debug.Log("FAIL AMOUNT greater than three, agent repositioned");
                            agentTransform.position = hit.position;
                        }
                        else
                        {
                            Debug.LogError("Can't place agent in Navmesh (" + agent.gameObject.name + ")");
                            status = BTstatus.FAILURE;
                            yield break;
                        }

                    }

                    yield return null;
                    agent.enabled = true;
                    yield return null;

                } while (agent.isOnNavMesh == false);
            }

            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(pointPosition, path);

            //Debug.Log(path);
            //Debug.Log(path.status);

            if (path.status == NavMeshPathStatus.PathComplete)
                status = BTstatus.SUCCESS;
            else
                status = BTstatus.FAILURE;

            yield break;

        }
    }
}