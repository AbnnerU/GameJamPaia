using System.Collections;
using UnityEngine;

namespace Assets.Scripts.BT
{
    public class BTIsColliderEnabled : BTnode
    {
        private Collider2D colliderRef;

        public BTIsColliderEnabled(Collider2D colliderRef)
        {
            this.colliderRef = colliderRef;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if (colliderRef != null)
            {
                if (colliderRef.enabled)
                    status = BTstatus.SUCCESS;
                else
                    status = BTstatus.FAILURE;
            }
            else
                status = BTstatus.FAILURE;

            yield break;
        }
    }
}    