using System.Collections;
using System;

namespace Assets.Scripts.BT
{
    public class BTDoAction : BTnode
    {
        private Action action;

        public BTDoAction(Action action)
        {
            this.action = action;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if (action == null)
            {
                status = BTstatus.FAILURE;
                yield break;
            }

            action?.Invoke();

            status = BTstatus.SUCCESS;

            yield break;
        }
    }
}
