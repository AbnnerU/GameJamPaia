using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.BT
{

    public class BTDodge : BTnode
    {
        private NavMeshAgent agent;
        private Transform agentTransform;
        private IAgentMovementState agentMovementState;
        private float radius;
        private float normalSpeed;
        private float dodgeSpeed;
        private Vector3 pointPosition;
        private float distance;
        private float updateInterval;

        public BTDodge(Transform agentTransform, NavMeshAgent agent, float radius, float normalSpeed, float dodgeSpeed, float distance, float updateInterval, IAgentMovementState agentMovementState)
        {
            this.agentTransform = agentTransform;
            this.agent = agent;
            this.radius = radius;
            this.normalSpeed = normalSpeed;
            this.dodgeSpeed = dodgeSpeed;
            this.distance = distance;
            this.updateInterval = updateInterval;
            this.agentMovementState = agentMovementState;
        }


        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;
            Debug.Log("DODGE");
            if (agent.isOnNavMesh == false || agent.enabled == false)
            {
                do
                {
                    agent.enabled = false;
    
                    yield return null;
                    agent.enabled = true;
                    yield return null;

                } while (agent.isOnNavMesh == false);
            }

            float currentUpdateInterval = 0;

            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += agentTransform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas);
            agent.SetDestination(hit.position);
            while (agent.pathPending == true)
                yield return null;

            agent.isStopped = false;

            agent.stoppingDistance = distance;

            agent.speed = dodgeSpeed;


            while (agentTransform != null && agent != null && agent.enabled == true && agent.isStopped == false && agentTransform.gameObject.activeSelf == true)
            {

                currentUpdateInterval = 0;
                if (agent.pathPending == false && agent.remainingDistance < distance)
                {
                    status = BTstatus.SUCCESS;

                    agent.speed = normalSpeed;

                    agent.isStopped = true;

                    yield break;
                }
                else
                {

                    //movementDirection = agentTransform.position - lastPosition;

                    //animationDataRecibe.ReceiveMovementDirection(movementDirection);

                    //lastPosition = agentTransform.position;

                    do
                    {
                        currentUpdateInterval += Time.deltaTime;

                        agentMovementState.AgentIsOnNavMeshLink(agent.isOnOffMeshLink);

                        yield return null;
                    } while (currentUpdateInterval < updateInterval);

                    //yield return new WaitForSeconds(updateInterval);
                    //agentMovementState.AgentIsOnNavMeshLink(agent.isOnOffMeshLink);
                }

            }

            status = BTstatus.FAILURE;

            yield break;
        }

    }


}