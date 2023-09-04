using System.Collections;
using UnityEngine.AI;

namespace Assets.Scripts.BT
{
    public class BTIsAgentOnNavMeshLink : BTnode
    {
        private NavMeshAgent agent;

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if(agent==null || agent.isOnNavMesh == false || agent.enabled == false || agent.isOnOffMeshLink)            
                status = BTstatus.FAILURE;           
            else if(agent.isOnOffMeshLink)             
                status = BTstatus.SUCCESS;     
            
            yield break;
        }
    }
}