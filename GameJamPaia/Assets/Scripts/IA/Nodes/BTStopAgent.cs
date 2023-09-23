using System.Collections;
using UnityEngine.AI;

namespace Assets.Scripts.BT
{
    public class BTStopAgent : BTnode
    {
        private NavMeshAgent agent;

        public BTStopAgent(NavMeshAgent agent)
        {
            this.agent = agent;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if (agent == null)
            {
                status = BTstatus.FAILURE;
                yield break;
            }

            agent.isStopped = true;

            yield break;
        }
    }
}