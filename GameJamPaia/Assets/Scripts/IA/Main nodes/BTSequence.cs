using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.BT
{
    public class BTSequence : BTnode
    {
        private List<BTnode> nodes;

        public BTSequence(List<BTnode> nodes)
        {
            this.nodes = nodes;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            for (int i = 0; i < nodes.Count; i++)
            {
                BTnode currentNode = nodes[i];
                yield return behaviorTree.StartCoroutine(currentNode.Run(behaviorTree));
                if (currentNode.GetStatus() == BTstatus.FAILURE)
                {
                    status = BTstatus.FAILURE;
                    break;
                }
            }

            if (status == BTstatus.RUNNING)
                status = BTstatus.SUCCESS;
        }
    }
}
