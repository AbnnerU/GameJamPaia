using System.Collections;
using UnityEngine;

namespace Assets.Scripts.BT
{
    public class BTIsTargetNear : BTnode
    {
        private Transform target;
        private Transform startPoint;
        private float distanceRef;

        public BTIsTargetNear(Transform startPoint ,Transform target, float distanceRef)
        {
            this.startPoint = startPoint;
            this.target = target;
            this.distanceRef = distanceRef;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if(target== null || startPoint == null)
            {
                status = BTstatus.FAILURE;

                yield break;
            }

            float distance = CalculateDistance(startPoint.position, target.position);
            Debug.Log(distance + "| Ref:"+distanceRef);
            if (distance < distanceRef)
            {
                Debug.Log("Alvo Perto");
                status = BTstatus.SUCCESS;
            }
            else
            {
                Debug.Log("Alvo Longe");
                status = BTstatus.FAILURE;
            }
                

            yield break;
        }

        private float CalculateDistance(Vector2 positionA, Vector2 positionB)
        {
            // Calcula a distância euclidiana entre dois pontos em um plano 2D
            float dx = positionB.x - positionA.x;
            float dy = positionB.y - positionA.y;

            return Mathf.Sqrt(dx * dx + dy * dy);
        }
    }

   
}