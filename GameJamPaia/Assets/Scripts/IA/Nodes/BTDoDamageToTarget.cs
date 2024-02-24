using System.Collections;

namespace Assets.Scripts.BT
{
    public class BTDoDamageToTarget : BTnode
    {
        private IHittable targetHealth;
        private int damageValue;

        public BTDoDamageToTarget(IHittable targetHealth, int damageValue)
        {
            this.targetHealth = targetHealth;
            this.damageValue = damageValue;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if (targetHealth == null )
            {
                status = BTstatus.FAILURE;

                yield break;
            }

            targetHealth.OnHit(damageValue);

            status = BTstatus.SUCCESS;

            yield break;
        }
    }
}