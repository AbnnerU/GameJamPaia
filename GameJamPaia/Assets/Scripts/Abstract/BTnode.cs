using System.Collections;


namespace Assets.Scripts.BT
{
    public abstract class BTnode
    {
        protected BTstatus status;

        public abstract IEnumerator Run(BehaviorTree behaviorTree);

        public virtual BTstatus GetStatus()
        {
            return status;
        }
    }

    public interface INode
    {

        IEnumerator Run(BehaviorTree behaviorTree);

        BTstatus GetStatus();
    }



    public enum BTstatus
    {
        RUNNING,
        FAILURE,
        SUCCESS
    }
}