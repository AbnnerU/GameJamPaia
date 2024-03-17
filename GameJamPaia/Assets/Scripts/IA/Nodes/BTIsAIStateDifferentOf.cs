using System.Collections;


namespace Assets.Scripts.BT
{
    public class BTIsAIStateDifferentOf : BTnode
    {
        private IAIState aiState;
        private AIState stateToCompare;

        public BTIsAIStateDifferentOf(IAIState aiState, AIState stateToCompare)
        {
            this.aiState = aiState;
            this.stateToCompare = stateToCompare;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if (aiState != null && aiState.GetCurrentAIState() != stateToCompare)
                status = BTstatus.SUCCESS;
            else
                status = BTstatus.FAILURE;

            yield break;
        }
    }
}