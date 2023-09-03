using System.Collections;
using System.Collections.Generic;



namespace Assets.Scripts.BT
{
    public class BTSelector : BTnode
    {
        private List<BTnode> nodes;

        private BTnode currentNode;

        private BehaviorTree bt;

        public BTSelector(List<BTnode> nodes)
        {
            this.nodes = nodes;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            bt = behaviorTree;

            for (int i = 0; i < nodes.Count; i++)
            {

                currentNode = nodes[i];
                yield return behaviorTree.StartCoroutine(currentNode.Run(behaviorTree));

                if (currentNode.GetStatus() == BTstatus.SUCCESS)
                {
                    status = BTstatus.SUCCESS;
                    break;
                }
            }

            if (status == BTstatus.RUNNING)
                status = BTstatus.FAILURE;

        }

        //public void ForceBreak()
        //{
        //    if (currentNode != null)
        //    {
        //        bt.StopCoroutine(currentNode.Run(bt));
        //    }

        //    forceBreak = true;
        //}
    }
}