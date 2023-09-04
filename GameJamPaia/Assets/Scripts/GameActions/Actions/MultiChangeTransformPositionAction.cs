
using UnityEngine;
using Assets.Scripts.GameAction;

public class MultiChangeTransformPositionAction : GameAction
{
    [SerializeField] private TransformToChange[] actions;

    public override void DoAction()
    {    
        for(int i = 0; i< actions.Length; i++)
        {
            if (actions[i].target == null) return;

            if (actions[i].useLocalPosition)
                actions[i]._transform.localPosition = actions[i].target.localPosition + actions[i].offSet;
            else
                actions[i]._transform.position = actions[i].target.position + actions[i].offSet;
        }
    }

    [System.Serializable]
    private struct TransformToChange
    {
        public Transform _transform;
        public Transform target;
        public Vector3 offSet;
        public bool useLocalPosition;
        public bool drawGizmos;
        public Color gizmosColor;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < actions.Length; i++)
        {
            if (actions[i].drawGizmos == false || actions[i].target == null || actions[i]._transform == null)continue;

            float arrowHeadAngle = 20f;
            float arrowHeadLength = 0.5f;
            Vector3 direction = actions[i].target.position - transform.position; 

            Gizmos.color = actions[i].gizmosColor;

            Gizmos.DrawLine(transform.position, actions[i].target.position);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(actions[i].target.position, right * arrowHeadLength);
            Gizmos.DrawRay(actions[i].target.position, left * arrowHeadLength);


        }
    }
}
