using System.Collections;


namespace Assets.Scripts.BT
{
    public class BTIsBlindEffectActive : BTnode
    {
        private VisionClamp visionClamp;

        public BTIsBlindEffectActive(VisionClamp visionClamp)
        {
            this.visionClamp = visionClamp;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if(visionClamp == null)
            {
                status = BTstatus.FAILURE;
                yield break;
            }

            if (visionClamp.IsActive())
                status = BTstatus.SUCCESS;
            else
                status = BTstatus.FAILURE;


            yield break;            
        }
    }
}