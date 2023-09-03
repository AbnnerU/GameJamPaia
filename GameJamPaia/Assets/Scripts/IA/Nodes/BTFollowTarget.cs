using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.BT 
{
    public class BTFollowTarget : BTnode
    {
        private Transform target;
        private NavMeshAgent agent;
        private Transform agentTransform;
        private Vector3 pointPosition;
        private float distance;
        private float updateInterval;


        public BTFollowTarget(Transform target, NavMeshAgent agent, float distance, float updateInterval)
        {
            this.target = target;
            this.agent = agent;
            this.distance = distance;
            this.updateInterval = updateInterval;

            agentTransform = agent.transform;
        }


        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if (agent == null || target == null)
            {
                status = BTstatus.FAILURE;

                yield break;
            }

            pointPosition = target.position;

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

            if (agent.remainingDistance <= distance && (pointPosition - agentTransform.position).magnitude > distance)
            {
                float value = 5;
                float addValue = 5;
                float maxValue = 20;
                do
                {
                    if (value >= maxValue)
                    {
                        //Debug.LogError("Can't set destination to agent (" + agent.gameObject.name + ")");
                        status = BTstatus.FAILURE;
                        yield break;
                    }

                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(pointPosition, out hit, value, NavMesh.AllAreas))
                    {
                        //Debug.Log("Complete destination ajust");
                        pointPosition = hit.position;
                        break;
                    }

                    //Debug.Log("Trying to ajust destination:" + value + "  (" + agent.gameObject.name + ")");

                    yield return null;
                    value += addValue;
                } while (agent.remainingDistance <= distance && (pointPosition - agentTransform.position).magnitude > distance);
            }
            agent.SetDestination(pointPosition);

            while (agent.pathPending == true)
                yield return null;


            agent.isStopped = false;

            agent.stoppingDistance = distance;


            while (target != null && agent != null && agent.enabled == true && agent.isStopped == false)
            {
                if (agent.pathPending == false && agent.remainingDistance < distance)
                {                 
                    status = BTstatus.SUCCESS;

                    Debug.Log("Arrived"+" | "+agent.remainingDistance);

                    yield break;
                }
                else
                {

                    //movementDirection = agentTransform.position - lastPosition;

                    //animationDataRecibe.ReceiveMovementDirection(movementDirection);

                    //lastPosition = agentTransform.position;

                    yield return new WaitForSeconds(updateInterval);

                }

                if (target != null)
                    agent.SetDestination(target.position);

                yield return null;
            }

            status = BTstatus.FAILURE;

            yield break;
        }

    }
}

