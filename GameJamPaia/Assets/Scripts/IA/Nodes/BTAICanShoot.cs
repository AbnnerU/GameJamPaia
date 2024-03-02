using System.Collections;


namespace Assets.Scripts.BT
{
    public class BTAICanShoot : BTnode
    {
        private IAIShooter aIShooter;

        public BTAICanShoot( IAIShooter aIShooter)
        {
            this.aIShooter = aIShooter;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if (aIShooter == null)
                status = BTstatus.FAILURE;
            else
            {
                if(aIShooter.CanShoot())
                    status = BTstatus.SUCCESS;
                else
                    status = BTstatus.FAILURE;
            }

            yield break;
        }
    }
}