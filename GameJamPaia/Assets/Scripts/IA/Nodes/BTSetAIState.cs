using System.Collections;


namespace Assets.Scripts.BT
{
    public class BTSetAIState : BTnode
    {
        private IAIState aiState;
        private AIState newState;

        public BTSetAIState(IAIState aiState, AIState newState)
        {
            this.aiState = aiState;
            this.newState = newState;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if (aiState != null) {
                aiState.SetAIState(newState);
                status = BTstatus.SUCCESS;
            }
            else
                status = BTstatus.FAILURE;

            yield break;
        }
    }
}