using System.Collections;
namespace Assets.Scripts.BT
{
    public class BTIsTargetAlive : BTnode
    {
        private HealthManager targetHealth;

        public BTIsTargetAlive(HealthManager targetHealth)
        {
            this.targetHealth = targetHealth;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if (targetHealth == null)
            {
                status = BTstatus.FAILURE;

                yield break;
            }

            if (targetHealth.IsAlive())
                status = BTstatus.SUCCESS;
            else
                status = BTstatus.FAILURE;

            yield break;
        }
    }
}