using System.Collections;
using UnityEngine;

namespace Assets.Scripts.BT
{
    public class BTStickToTheTarget : BTnode
    {
        private Transform target;
        private Transform agent;
        private Vector3 offSet;
        private float stickUpdateInterval;

        public BTStickToTheTarget(Transform target, Transform agent, Vector3 offSet, float stickUpdateInterval)
        {
            this.target = target;
            this.agent = agent;
            this.offSet = offSet;
            this.stickUpdateInterval = stickUpdateInterval;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if (target == null || agent == null)
            {
                status = BTstatus.FAILURE;
                yield break;
            }

            do
            {
                if (target == null || agent == null)
                {
                    status = BTstatus.FAILURE;
                    yield break;
                }

                agent.position = target.position + offSet;

                if(stickUpdateInterval > 0)
                    yield return new WaitForSeconds(stickUpdateInterval);
                else
                    yield return null;

            } while (true);
        }
    }
}