using System.Collections;



namespace Assets.Scripts.BT
{
    public class BTInverter : BTnode
    {
        private BTnode node;

        public BTInverter(BTnode node)
        {
            this.node = node;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            behaviorTree.StartCoroutine(node.Run(behaviorTree));

            while (node.GetStatus() == BTstatus.RUNNING)
                yield return null;

            if (node.GetStatus() == BTstatus.SUCCESS)
                status = BTstatus.FAILURE;
            else
                status = BTstatus.SUCCESS;

        }
    }
}